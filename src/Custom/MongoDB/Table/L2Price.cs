using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.Table
{
    class L2Price : Figure
    {
        public enum L2Field
        {
            MARKET_NAME,
            CONTRACT_NAME,
            PRICE_LEVEL,
            CONTRACT_DATA_TYPE,
            OPERATION,
            TIMESTAMP,
            PRICE,
            VOLUME
        }

        public class L2PriceId : FigureId
        {
            public int priceLevel { get; set; }
            public L2PriceId(string market, string contract, DateTime time, int updateSeq, int level) :base(market, contract, time, updateSeq)
            {
                priceLevel = level;
            }
        }
        public L2Price(string market, string contract, DateTime time, int updateSeq, int level, string type, double price, double volume, string op)
        {
            Id = new L2PriceId(market, contract, time, updateSeq, level);

            setL2Price(type, price, volume, level, op);
       
        }
        [BsonId]
        public L2PriceId Id { get; set; }

        
        public string Operation { get; set; }

        public void setL2Price( string type, double price, double volume, int level, string op)
        {
            setPrice(type,  price, volume);

            
            Operation = op;
        }
    }
}
