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
            [BsonElementAttribute("L")]
            public int priceLevel { get; set; }
            [BsonElementAttribute("O")]
            public int Operation { get; set; }

            public L2PriceId(string market, string contract, DateTime time, int updateSeq, int type, int op, int level, double price) :base(market, contract, time, updateSeq, type, price)
            {
                priceLevel = level;
                Operation = op;
            }

            public override string ToString()
            {
                return base.ToString() + " PriceLevel = " + priceLevel + " Operation = " + Operation;
            }
        }
        public L2Price(string market, string contract, DateTime time, int updateSeq, int type, int op, int level, double price, int volume)
        {
            Id = new L2PriceId(market, contract, time, updateSeq, type, op, level, price);

            Volume = volume;
       
        }
        [BsonId]
        public L2PriceId Id { get; set; }

        public override string ToString()
        {
            return Id.ToString() + " Volume = " + Volume;
        }
    }
}
