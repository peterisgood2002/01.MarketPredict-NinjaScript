using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.Table
{
    public class Contracts
    {
        public enum Field
        {
            
            BEGIN_DATE,
            EXPIRY_DATE,
            ROLL_DATE,
            ROLL_OFFSET
        }

        public Contracts()
        {

        }
        
        public Contracts(string marketName, string contractname, DateTime rolldate, double rolloffset)
        {

            Id = new ContractsId(marketName, contractname);
            
            RollDate = rolldate;
            RollOffset = rolloffset;
            
        }

        public class ContractsId
        {
            public string MarketName { get; set; }
            public string ContractName { get; set; }

            public ContractsId(string market, string contract)
            {
                MarketName = market;
                ContractName = contract;
            }
        }

        [BsonId]
        public ContractsId Id { get; set; }

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
