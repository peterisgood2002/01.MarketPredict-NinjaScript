using MongoDB.Bson.Serialization.Attributes;
using System;

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

            public string Operation { get; set; }

            public L2PriceId(string market, string contract, DateTime time, int updateSeq, string type, string op, int level, double price) :base(market, contract, time, updateSeq, type, price)
            {
                priceLevel = level;
                Operation = op;
            }
        }
        public L2Price(string market, string contract, DateTime time, int updateSeq, string type, string op, int level, double price, double volume)
        {
            Id = new L2PriceId(market, contract, time, updateSeq, type, op, level, price);

            Volume = volume;
       
        }
        [BsonId]
        public L2PriceId Id { get; set; }

    }
}
