using MongoDB.Bson.Serialization.Attributes;
using System;

namespace NinjaTrader.Custom.MongoDB.Table
{
    class L1Price : Figure
    {
     
        public L1Price(string market, string contract, DateTime time, int updateseq, string type, double price, double volume)
        {
            Id = new FigureId(market, contract, time, updateseq, type, price);

            Volume = volume;

        }
        [BsonId]
        public FigureId Id { get; set; }
        
        public override string ToString()
        {
            return Id.ToString();
        }
        
    }
}
