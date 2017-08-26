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
    
    class Market 
    {
        public enum Field
        {
            MARKET_NAME,
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

            MarketName = name;
            Currency = currency;
            Description = desc;
            Type = type;
            TickSize = tickSize;
            PointValue = pointValue;
            Url = url;
           
        }
        public ObjectId Id { get; set; }

       
        public string MarketName
        {
            get; set;
        }

       
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
