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
            [BsonElementAttribute("M")]
            public string MarketName { get; set; }
            [BsonElementAttribute("C")]
            public string ContractName { get; set; }

            [BsonElementAttribute("T")]
            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime Timestamp { get; set; }
            [BsonElementAttribute("S")]
            public int UpdateSeqno { get; set; }
            [BsonElementAttribute("D")]
            public int ContractDataType { get; set; }
            [BsonElementAttribute("P")]
            public double Price
            {
                get; set;
            }

            public FigureId(string market, string contract, DateTime time, int updateSeq, int type, double price)
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
                    " Timestamp = " + Timestamp + " UpdateSeqno = " + UpdateSeqno + " Price = " + Price ;
            }
        }
        [BsonElementAttribute("V")]
        public int Volume
        {
            get; set;
        }

    }
}
