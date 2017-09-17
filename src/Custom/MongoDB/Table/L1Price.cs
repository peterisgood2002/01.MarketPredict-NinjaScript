using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.Table
{
    class L1Price : Figure
    {
     
        public L1Price(string market, string contract, DateTime time, int updateseq, string type, double price, double volume)
        {
            Id = new FigureId(market, contract, time, updateseq);

            setPrice(type, price, volume);

        }
        [BsonId]
        public FigureId Id { get; set; }
        
    }
}
