using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace NinjaTrader.Custom.MongoDB.Table
{
    
    public class Market 
    {
        public enum Field
        {
            CURRENCY,
            DESCRIPTION,
            TICK_SIZE,
            POINT_VALUE,
            TYPE,
            URL
        }

        public Market()
        {

        }

        public Market(string name, string currency, string desc, string type, double tickSize, double pointValue, string url)
        {
            Id = name;
            Currency = currency;
            Description = desc;
            Type = type;
            TickSize = tickSize;
            PointValue = pointValue;
            Url = url;
           
        }

        [BsonId]
        public string Id { get; set; }

        public string Currency
        {
            get; set;
        }

        
        public string Description
        {
            get; set;
        }

        
        public string Type
        {
            get; set;
        }

        
        public double TickSize
        {
            get; set;
        }
        
        public double PointValue
        {
            get; set;
        }

        public string Url
        {
            get; set;
        }

        
    }
}
