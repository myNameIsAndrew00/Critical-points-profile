﻿using MapWebSite.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MapWebSite.Model
{
    /// <summary>
    /// Model used for basic data of points data set
    /// </summary>
    public class PointsDataSetBase
    {
        public int ID { get; set; }

        public string Username { get; set; }

        public string DatasetName { get; set; }

        public DatasetStatus Status { get; set; }

        public bool IsValid => Status == DatasetStatus.Generated;
    }

    /// <summary>
    /// Model used for points data set
    /// </summary>
    public class PointsDataSet : PointsDataSetBase
    {

        public string Name { get; set; }


        public IEnumerable<Point> Points { get; set; }

        public IEnumerable<PointsRegion> PointsRegions { get; set; }

    }

    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    public class PointBase : ICloneable
    {
        /// <summary>
        /// An enum which describes the meaning of OptionalField
        /// </summary>
        public enum BasicInfoOptionalField
        {
            [EnumString("Height")]
            Height,

            [EnumString("DeformationRate")]
            DeformationRate,

            [EnumString("StandardDeviation")]
            StandardDeviation
        }

        [DataMember]
        [JsonProperty]
        public int Number { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Longitude { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Latitude { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal Height { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal DeformationRate { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal StandardDeviation { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal EstimatedHeight { get; set; }

        [DataMember]
        [JsonProperty]
        public decimal EstimatedDeformationRate { get; set; }

        [DataMember]
        [JsonProperty]
        public string Observations { get; set; }

        public object Clone()
        {
            return new PointBase
            {
                DeformationRate = this.DeformationRate,
                EstimatedDeformationRate = this.EstimatedDeformationRate,
                EstimatedHeight = this.EstimatedHeight,
                Height = this.Height,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Number = this.Number,
                Observations = this.Observations,
                StandardDeviation = this.StandardDeviation
            };
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    public class Point : PointBase, ICloneable
    {
        [JsonProperty]
        public decimal ReferenceImageX { get; set; }

        [JsonProperty]
        public decimal ReferenceImageY { get; set; }

        [JsonProperty]
        public List<Displacement> Displacements { get; set; }


        [JsonObject(MemberSerialization.OptIn)]
        public class Displacement
        {
            [JsonProperty]
            public DateTime Date { get; set; }

            [JsonProperty]
            public decimal JD { get; set; }

            [JsonProperty]
            public decimal DaysFromReference { get; set; }

            [JsonProperty]
            public decimal Value { get; set; }
        }

        public new object Clone()
        {
            return new Point
            {
                DeformationRate = this.DeformationRate,
                EstimatedDeformationRate = this.EstimatedDeformationRate,
                EstimatedHeight = this.EstimatedHeight,
                Height = this.Height,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Number = this.Number,
                Observations = this.Observations,
                StandardDeviation = this.StandardDeviation,
                Displacements = this.Displacements,
                ReferenceImageX = this.ReferenceImageX,
                ReferenceImageY = this.ReferenceImageY
            };
        }
    }



}