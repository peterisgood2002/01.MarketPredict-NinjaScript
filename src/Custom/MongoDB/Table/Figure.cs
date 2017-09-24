using MongoDB.Bson.Serialization.Attributes;
using System;

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

            public string ContractDataType { get; set; }

            public double Price
            {
                get; set;
            }

            public FigureId(string market, string contract, DateTime time, int updateSeq, string type, double price)
            {
                
                MarketName = market;
                ContractName = contract;
                ContractDataType = type;
                Timestamp = time;
                UpdateSeqno = updateSeq;
                Price = price;
            }

            public override string ToString()
            {
                return "MarketName = " + MarketName + " ContractName = " + ContractName + " ContractDataType = " + ContractDataType + 
                    " Timestamp = " + Timestamp + "UpdateSeqno = " + UpdateSeqno;
            }
        }

        public double Volume
        {
            get; set;
        }

    }
}
