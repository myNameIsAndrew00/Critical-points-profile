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

        public long ZoomLevel { get; set; }

        public IEnumerable<Point> Points { get; set; }

    }

    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    public class BasicPoint
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

    }
    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    public class Point : BasicPoint
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
    }



}
