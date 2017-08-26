using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.Table
{
    class Contracts
    {
        public enum Field
        {
            MARKET_ID,
            CONTRACT_NAME,
            BEGIN_DATE,
            EXPIRE_DATE,
            ROLL_DATE,
            ROLL_OFFSET
        }

        public Contracts()
        {

        }
        
        public Contracts(ObjectId marketId, string contractname, DateTime rolldate, double rolloffset)
        {
            MarketId = marketId;
            ContractName = contractname;
            RollDate = rolldate;
            RollOffset = rolloffset;
            
        }
        public ObjectId Id { get; set; }

        public ObjectId MarketId
        {
            get; set;
        }

        public string ContractName
        {
            get; set;
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BeginDate
        {
            get; set;
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiryDate
        {
            get; set;
        }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RollDate
        {
            get; set;
        }

        public double RollOffset
        {
            get; set;
        }
    }
}
