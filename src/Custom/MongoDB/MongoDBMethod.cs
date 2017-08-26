using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NinjaTrader.Cbi;
using NinjaTrader.Custom.MongoDB.Table;
using NinjaTrader.Custom.MongoDB.TableOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB
{
    enum MongoDBTable
    {
        MARKET,
        CONTRACTS
    }
    class MongoDBMethod
    {
        public static void registerClass()
        {
            //BsonClassMap.RegisterClassMap<Market>();

            ConventionPack cp = new ConventionPack();
            cp.Add(new SeparateEachWordAndToUpperConvention());
            ConventionRegistry.Register("FirstCharacterToUpperConvention", cp, t => t.FullName.StartsWith("NinjaTrader.Custom.MongoDB.Table."));

        }
        public static ObjectId readMarketId(MongoClient connection, String dbName, Instrument instrument)
        {
            NinjaTrader.Code.Output.Process("[readMarketId] Connection:" + connection + "DB = " + dbName, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<Market> collection = db.GetCollection<Market>(MongoDBTable.MARKET.ToString());
            /*Filter*/
            FilterDefinition<Market> filter = Builders<Market>.Filter.Eq(Market.Field.MARKET_NAME.ToString(), instrument.MasterInstrument.Name);
            List<Market> result = collection.Find<Market>( filter).ToList();

            if( result.Count == 0 )
            {
                Market market = MarketOperartion.insertMarket(collection, instrument);
                NinjaTrader.Code.Output.Process("[readMarketId] Result:" + market, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                return market.Id;
               
            } else
            {
                UpdateResult r = MarketOperartion.updateMarket(collection, filter, instrument, result);

                if( r != null )
                {
                    NinjaTrader.Code.Output.Process("[readMarketId] Update Result:" + r.ModifiedCount, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                }
                    
                return result.First().Id;
            }
        }

        


        public static ObjectId readContractId(MongoClient connection, string dbName, ObjectId marketId, Instrument instrument)
        {
            NinjaTrader.Code.Output.Process("[readContractId] Connection:" + connection + "DB = " + dbName + "marketId = " + marketId.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<Contracts> collection = db.GetCollection<Contracts>(MongoDBTable.CONTRACTS.ToString());
            /*Filter*/
            FilterDefinition<Contracts> marketIdFilter = Builders<Contracts>.Filter.Eq(Contracts.Field.MARKET_ID.ToString(), marketId);
            FilterDefinition<Contracts> contractNameFilter = Builders<Contracts>.Filter.Eq(Contracts.Field.CONTRACT_NAME.ToString(), instrument.FullName);
            FilterDefinition<Contracts> filter = Builders<Contracts>.Filter.And(marketIdFilter, contractNameFilter);

            List <Contracts> result = collection.Find<Contracts>(filter).ToList();

            if (result.Count == 0)
            {


                Contracts contract = ContractsOperation.insertContract(collection, marketId, instrument);
                NinjaTrader.Code.Output.Process("[readContractId] Result:" + contract, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                return contract.Id;

            } else
            {
                UpdateResult r = ContractsOperation.updateContract(collection, filter, instrument, result);

                if (r != null)
                {
                    NinjaTrader.Code.Output.Process("[readMarketId] Update Result:" + r.ModifiedCount, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                }

                return result.First().Id;
            }
            
        }
    }
}
