﻿using MapWebSite.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapWebSite.Core.Database
{

    public enum DataPointsFilters
    {
        None = -1
    }

    public interface IDataPointsRepository
    {      
        Task<bool> InsertPointsDatasets(PointsDataSet originalDataSet, PointsDataSet[] zoomedDatasets);

        /// <summary>
        /// Get data points from database
        /// </summary>
        /// <param name="dataSetID">Dataset ID containing the points</param>
        /// <param name="zoomLevel">The zoom required</param>
        /// <param name="from">Latitude and longitude from which points must be start</param>
        /// <param name="to">Latitude and longitude from which points must be end</param>
        /// <returns></returns>
        IEnumerable<BasicPoint> GetDataPointsBasicInfo(int dataSetID, 
                                                        int zoomLevel, 
                                                        Tuple<decimal, decimal> from, 
                                                        Tuple<decimal, decimal> to,
                                                        BasicPoint.BasicInfoOptionalField optionalField);

        Point GetPointDetails(int dataSetID, int zoomLevel, BasicPoint basicPoint);
    }
}
