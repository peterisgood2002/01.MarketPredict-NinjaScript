using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.Table
{
    public class Figure
    {
        public enum Field
        {
            MARKET_NAME,
            CONTRACT_NAME,
            CONTRACT_DATA_TYPE,
            TIMESTAMP,
            PRICE,
            VOLUME
        }

        public class FigureId
        {
            public string MarketName { get; set; }
            public string ContractName { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime Timestamp { get; set; }

            public int UpdateSeqno { get; set; }
            public FigureId(string market, string contract, DateTime time, int updateSeq)
            {
                
                MarketName = market;
                ContractName = contract;

                Timestamp = time;
                UpdateSeqno = updateSeq;
            }
        }

       

        /*We flat CONTRACT_DATA_TYPE so this is not a key.*/
        public string ContractDataType { get; set; }

        public double Price
        {
            get; set;
        }

        public double Volume
        {
            get; set;
        }

        public void setPrice(string type, double price, double volume)
        {
            ContractDataType = type;
            
            Price = price;
            Volume = volume;
        }

    }
}
