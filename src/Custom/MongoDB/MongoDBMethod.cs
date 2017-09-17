using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NinjaTrader.Cbi;
using NinjaTrader.Custom.MongoDB.Table;
using NinjaTrader.Custom.MongoDB.TableOperation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.Custom.MongoDB
{
    enum MongoDBTable
    {
        MARKET,
        CONTRACTS,
        L1_PRICE,
        L2_PRICE
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
        public static string readMarketId(MongoClient connection, String dbName, Instrument instrument)
        {
            NinjaTrader.Code.Output.Process("[readMarketId] Connection:" + connection + "DB = " + dbName, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<Market> collection = db.GetCollection<Market>(MongoDBTable.MARKET.ToString());
            /*Filter*/
            FilterDefinition<Market> filter = Definitions<Market>.generateIdFilter<string>(instrument.MasterInstrument.Name);
            List <Market> result = collection.Find<Market>( filter).ToList();

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

        public static Contracts readContract(MongoClient connection, string dbName, string marketName, Instrument instrument)
        {
            NinjaTrader.Code.Output.Process("[readContractId] Connection:" + connection + "DB = " + dbName + "marketName = " + marketName.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<Contracts> collection = db.GetCollection<Contracts>(MongoDBTable.CONTRACTS.ToString());
            /*Filter*/
            Contracts.ContractsId id = new Contracts.ContractsId(marketName, instrument.FullName);
            //FilterDefinition<Contracts> filter = Builders<Contracts>.Filter.Eq(Contracts.Field._id.ToString(), id);
            FilterDefinition<Contracts> filter = Definitions<Contracts>.generateIdFilter<Contracts.ContractsId>(id);
            List<Contracts> result = collection.Find<Contracts>(filter).ToList();

            if (result.Count == 0)
            {


                Contracts contract = ContractsOperation.insertContract(collection, marketName, instrument);
                NinjaTrader.Code.Output.Process("[readContractId] Result:" + contract, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                return contract;

            }
            else
            {
                UpdateResult r = ContractsOperation.updateContract(collection, filter, instrument, result.First());

                if (r != null)
                {
                    NinjaTrader.Code.Output.Process("[readMarketId] Update Result:" + r.ModifiedCount, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                }

                return result.First();
            }

        }

        public static Contracts readContract(MongoClient connection, string dbName, string marketName, Instrument instrument, DateTime beginDate, DateTime expiryDate)
        {
            NinjaTrader.Code.Output.Process("[readContract] Connection:" + connection + "DB = " + dbName + " marketName = " + marketName.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<Contracts> collection = db.GetCollection<Contracts>(MongoDBTable.CONTRACTS.ToString());
            /*Filter*/
            Contracts.ContractsId id = new Contracts.ContractsId(marketName, instrument.FullName);
            //FilterDefinition<Contracts> filter = Builders<Contracts>.Filter.Eq(Contracts.Field._id.ToString(), id);
            FilterDefinition<Contracts> filter = Definitions<Contracts>.generateIdFilter<Contracts.ContractsId>(id);
            List <Contracts> result = collection.Find<Contracts>(filter).ToList();

            if (result.Count == 0)
            {


                Contracts contract = ContractsOperation.insertContract(collection, marketName, instrument, beginDate, expiryDate);
                NinjaTrader.Code.Output.Process("[readContract] Result:" + contract, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                return contract;

            } else
            {
                UpdateResult r = ContractsOperation.updateContract(collection, filter, instrument, result.First(), beginDate, expiryDate);

                if (r != null)
                {
                    NinjaTrader.Code.Output.Process("[readContract] Update Result:" + r.ModifiedCount, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                }

                return result.First();
            }
            
        }

        public static IDictionary<DateTime, List<Figure>> searchPrice(MongoClient connection, string dbName, DateTime beginTime, DateTime endTime)
        {
            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<L1Price> collection = db.GetCollection<L1Price>(MongoDBTable.L1_PRICE.ToString());
            
            FilterDefinition<L1Price> filter = Definitions<L1Price>.between(beginTime, endTime);
            List<L1Price> data = collection.Find<L1Price>(filter).ToList();
            IDictionary<DateTime, List<Figure>> result = new Dictionary<DateTime, List<Figure>>();
            foreach(L1Price price in data)
            {
                List<Figure> val = new List<Figure>();
                if( !result.TryGetValue(price.Id.Timestamp, out val))
                {
                    result.Add(price.Id.Timestamp, val);
                }

                val.Add(price);
               
            }

            IMongoCollection<L2Price> collection2 = db.GetCollection<L2Price>(MongoDBTable.L2_PRICE.ToString());
            FilterDefinition<L2Price> filter2 = Definitions<L2Price>.between(beginTime, endTime);
            List<L2Price> data2 = collection2.Find<L2Price>(filter2).ToList();
            foreach (L2Price price in data2)
            {
                List<Figure> val = new List<Figure>();
                if (!result.TryGetValue(price.Id.Timestamp, out val))
                {
                    result.Add(price.Id.Timestamp, val);
                }

                val.Add(price);
            }

            return result;
        }

        public static void insertPrice(MongoClient connection, string dbName, List<Figure> data)
        {
            NinjaTrader.Code.Output.Process("[insertPrice] Connection:" + connection + "DB = " + dbName + " Data Length = " + data.Count, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            IMongoDatabase db = connection.GetDatabase(dbName);

            IMongoCollection<L1Price> l1Collection = db.GetCollection<L1Price>(MongoDBTable.L1_PRICE.ToString());

            IMongoCollection<L2Price> l2Collection = db.GetCollection<L2Price>(MongoDBTable.L2_PRICE.ToString());

            PriceOperation.insertPrice(l1Collection, l2Collection, data);
        }
    }
}
