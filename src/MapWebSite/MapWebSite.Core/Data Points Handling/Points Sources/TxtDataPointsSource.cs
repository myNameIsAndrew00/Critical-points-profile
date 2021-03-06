﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MapWebSite.Model;

namespace MapWebSite.Core.DataPoints
{
    using HeaderData = Tuple<DateTime, decimal, decimal>;

    /// <summary>
    /// Use this object to parse a text file into a Points Dataset object
    /// </summary>
    public class TxtDataPointsSource : IDataPointsSource
    {
        readonly int headerUnusedLinesCount = 10;

        readonly int blobSize = 100000;

        readonly Helper.UTMConverter utmConverter = new Helper.UTMConverter();

        string[] dataBlob;

        /// <summary>
        /// Represents a file which contains metadata about points format
        /// </summary>
        public string HeaderFile { get; set; } = null;

        /// <summary>
        /// Represents a file which contains the points which must be read
        /// </summary>
        public string DisplacementsFile { get; set; } = null;

        public int Zone { get; set; } = int.MinValue;

        public char LatitudeZone { get; set; } = '0';

        public IEnumerable<PointsDataSet> CreateDataSet(string datasetName, CoordinateSystem coordinateSystem)
        {
            if (HeaderFile == null || DisplacementsFile == null) yield return null;
            if (coordinateSystem == CoordinateSystem.UTM && (Zone == int.MinValue || LatitudeZone == '0')) throw new ArgumentException("Zone and Latitude properties must be set");


            HeaderData[] headerData = parseHeaderData();
            StreamReader reader = File.OpenText(this.DisplacementsFile);
            ///The displacements file must be read in chunks (or a OutOfMemoryException could be thrown)

            bool reading = true;
            while (reading)
            {
                ConcurrentBag<Point> points = new ConcurrentBag<Point>();
                PointsDataSet pointsDataSet = null;

                try
                {
                    pointsDataSet = new PointsDataSet() { Name = datasetName, Points = points };

                    reading = readBlob(reader);

                    ConcurrentQueue<Exception> exceptions = new ConcurrentQueue<Exception>();

                    Parallel.ForEach(dataBlob, (dataLine) =>
                    {
                        try
                        {
                            IDictionary<string, decimal> lineInfo = generateDictionary(dataLine, out decimal[] lineDisplacements);
                            points.Add(generatePoint(lineInfo, lineDisplacements, headerData, coordinateSystem));
                        }
                        catch (Exception exception)
                        {

                            CoreContainers.LogsRepository.LogError(exception, Database.Logs.LogTrigger.CoreModule);
                                
                            exceptions.Enqueue(exception);
                        }
                        if (exceptions.Count > 0) throw new Exception("Failed to parse with success the file");
                    });

                    //set the maximum and minimum latitude / longitude
                    pointsDataSet.MinimumLatitude = pointsDataSet.Points.Min(point => point.Latitude);
                    pointsDataSet.MinimumLongitude = pointsDataSet.Points.Min(point => point.Longitude);
                    pointsDataSet.MaximumLatitude = pointsDataSet.Points.Max(point => point.Latitude);
                    pointsDataSet.MaximumLongitude = pointsDataSet.Points.Max(point => point.Longitude);

                    //set the maximum and minimum from other informations
                    pointsDataSet.MinimumHeight = pointsDataSet.Points.Min(point => point.DeformationRate);
                    pointsDataSet.MinimumStdDev = pointsDataSet.Points.Min(point => point.StandardDeviation);
                    pointsDataSet.MinimumDeformationRate = pointsDataSet.Points.Min(point => point.DeformationRate);
                    pointsDataSet.MaximumDeformationRate = pointsDataSet.Points.Max(point => point.DeformationRate);
                    pointsDataSet.MaximumHeight = pointsDataSet.Points.Max(point => point.DeformationRate);
                    pointsDataSet.MaximumStdDev = pointsDataSet.Points.Max(point => point.StandardDeviation);
                }
                catch (Exception exception)
                {
                    CoreContainers.LogsRepository.LogError(exception, Database.Logs.LogTrigger.CoreModule);

                    pointsDataSet = null;
                }
                yield return pointsDataSet;
            }

            reader.Dispose();

        }





        #region Private

        private Point generatePoint(IDictionary<string, decimal> lineInfo, decimal[] lineDisplacements, HeaderData[] headerData, CoordinateSystem coordinateSystem)
        {
            Point point = new Point()
            {
                Displacements = new List<Point.Displacement>(),
                Number = Convert.ToInt32(lineInfo["PointNumber"]),
                DeformationRate = lineInfo["DeformationRate"],
                EstimatedDeformationRate = lineInfo["EstimatedDeformation"],
                EstimatedHeight = lineInfo["EstimatedHeight"],
                Height = lineInfo["Height"],
                ReferenceImageX = lineInfo["ReferenceImageX"],
                ReferenceImageY = lineInfo["ReferenceImageY"],
                StandardDeviation = lineInfo["StandardDeviation"],
                Observations = "Generated from file" // TODO:add information to observations                
            };

            //todo: create switch if there will be more
            if (coordinateSystem == CoordinateSystem.UTM)
            {
                Tuple<decimal, decimal> coordinatesPair = utmConverter.ToLatLong(this.Zone, this.LatitudeZone,
                                                                                lineInfo["ProjectionX"],
                                                                                lineInfo["ProjectionY"]);
                point.Latitude = coordinatesPair.Item1;
                point.Longitude = coordinatesPair.Item2;
            }
            else
            {
                point.Longitude = lineInfo["ProjectionX"];
                point.Latitude = lineInfo["ProjectionY"];
            }

            //todo: check why here is a difference
            int length = Math.Min(headerData.Length, lineDisplacements.Length);

            for (int index = 0; index < length; index++)
                point.Displacements.Add(new Point.Displacement()
                {
                    Date = headerData[index].Item1,
                    JD = headerData[index].Item2,
                    DaysFromReference = headerData[index].Item3,
                    Value = lineDisplacements[index]
                });


            return point;
        }

        private IDictionary<string, decimal> generateDictionary(string dataLine, out decimal[] displacements)
        {

            IDictionary<string, decimal> result = new ConcurrentDictionary<string, decimal>();

            var tokens = dataLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            decimal[] localDisplacements = new decimal[tokens.Length - headerUnusedLinesCount];

            Task[] tasks = new Task[11];
            //define assignments tasks to be realised in parallel
            tasks[0] = Task.Run(() => result["PointNumber"] = decimal.Parse(tokens[0]));
            tasks[1] = Task.Run(() => result["ReferenceImageX"] = decimal.Parse(tokens[1]));
            tasks[2] = Task.Run(() => result["ReferenceImageY"] = decimal.Parse(tokens[2]));
            tasks[3] = Task.Run(() => result["ProjectionX"] = decimal.Parse(tokens[3]));
            tasks[4] = Task.Run(() => result["ProjectionY"] = decimal.Parse(tokens[4]));
            tasks[5] = Task.Run(() => result["Height"] = decimal.Parse(tokens[5]));
            tasks[6] = Task.Run(() => result["DeformationRate"] = decimal.Parse(tokens[6]));
            tasks[7] = Task.Run(() => result["StandardDeviation"] = decimal.Parse(tokens[7]));
            tasks[8] = Task.Run(() => result["EstimatedHeight"] = decimal.Parse(tokens[8]));
            tasks[9] = Task.Run(() => result["EstimatedDeformation"] = decimal.Parse(tokens[9]));

            tasks[10] = Task.Run(() =>
            {
                for (int index = headerUnusedLinesCount; index < tokens.Length; index++)
                {
                    decimal number = 0;
                    decimal.TryParse(tokens[index].Replace(" ", ""), out number);
                    localDisplacements[index - headerUnusedLinesCount] = number;
                }
            });
            Task.WaitAll(tasks);

            displacements = localDisplacements;
            return result;
        }

        private HeaderData[] parseHeaderData()
        {
            List<HeaderData> result = new List<HeaderData>();

            using (StreamReader streamReader = new StreamReader(HeaderFile))
            {
                //first 10 lines of the file are useles
                for (int i = 0; i < headerUnusedLinesCount; i++)
                    streamReader.ReadLine();

                string dataLine = streamReader.ReadLine();

                while (!string.IsNullOrEmpty(dataLine))
                {
                    /*parse the header file*/
                    var tokens = dataLine.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    var dateInfo = tokens[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var jDInfo = tokens[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var dayReferenceInfo = tokens[3].Trim();

                    result.Add(new HeaderData(
                        new DateTime(Convert.ToInt32(dateInfo[0]), Convert.ToInt32(dateInfo[1]), Convert.ToInt32(dateInfo[2])),
                        decimal.Parse(jDInfo[0]),
                        decimal.Parse(dayReferenceInfo)
                        ));

                    dataLine = streamReader.ReadLine();
                }
            }

            return result.ToArray();
        }



        /// <summary>
        /// Read a blob of text from a reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>Returns false  if the end of file was reached</returns>
        private bool readBlob(StreamReader reader)
        {
            List<string> result = new List<string>();
            string line;

            int count = 0;

            while ((line = reader.ReadLine()) != null && count < blobSize)
            {
                count++;
                result.Add(line);
            }

            this.dataBlob = result.ToArray();

            if (line == null) return false;
            return true;
        }


        #endregion
    }
}
