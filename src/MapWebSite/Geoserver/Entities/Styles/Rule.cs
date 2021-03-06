﻿using MapWebSite.GeoserverAPI.Entities.Symbolizers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MapWebSite.GeoserverAPI.Entities
{
 
    public sealed class Rule
    {      
        [XmlElement]
        public string Name { get; set; }
        
       
        [XmlElement]
        public string Title { get; set; }

        
        [XmlElement]
        public string Abstract { get; set; }

        
        [XmlElement("Filter", Namespace = "http://www.opengis.net/ogc")]
        public Filter Filter { get; set; }


        [XmlElement]
        public double MinScaleDenominator { get; set; }

        
        [XmlElement]
        public double MaxScaleDenominator { get; set; }

        
        [XmlElement("PointSymbolizer")]
        public List<PointSymbolizer> PointSymbolizers { get; set; }

        public bool ShouldSerializeMinScaleDenominator()
        {
            return MinScaleDenominator != 0;
        }

        public bool ShouldSerializeMaxScaleDenominator()
        {
            return MaxScaleDenominator != 0;
        }
    }
}
