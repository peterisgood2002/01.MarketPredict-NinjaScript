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
            NinjaTrader.Code.Output.Process("Connection:" + connection + "DB = " + dbName, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            IMongoDatabase db = connection.GetDatabase(dbName);
            IMongoCollection<Market> collection = db.GetCollection<Market>(MongoDBTable.MARKET.ToString());

            List<Market> result = collection.Find<Market>(Builders<Market>.Filter.Eq(Market.Field.MARKET_ID.ToString(), instrument.MasterInstrument.Name)).ToList();

            if( result.Count == 0 )
            {
                Market market = new Market(
                                     instrument.MasterInstrument.Name, 
                                     instrument.MasterInstrument.Currency.ToString(),
                                     instrument.MasterInstrument.Description,
                                     instrument.MasterInstrument.InstrumentType.ToString(),
                                     instrument.MasterInstrument.TickSize,
                                     instrument.MasterInstrument.PointValue,
                                     instrument.MasterInstrument.Url.ToString());
                collection.InsertOne(market);
                NinjaTrader.Code.Output.Process("Result:" + market, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                return market.Id;
               
            } else
            {
                UpdateOptions options = new UpdateOptions();
                options.IsUpsert = true;
                FilterDefinition<Market> filter = Builders<Market>.Filter.Eq(Market.Field.MARKET_ID.ToString(), instrument.MasterInstrument.Name);
                UpdateDefinition<Market> definition = createMarketUpdateDefinition(result.First(), instrument.MasterInstrument);
                UpdateResult r = collection.UpdateOne(filter, definition, options);
                NinjaTrader.Code.Output.Process(" Update Result:" + r.ModifiedCount, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                return result.First().Id;
            }
        }



        private static UpdateDefinition<Market> createMarketUpdateDefinition(Market market, MasterInstrument instrument)
        {
            UpdateDefinition<Market> definition = null;

            if (!market.MarketId.Equals(instrument.Name))
            {
                definition = UpdateDefinitions<Market>.setUpdateDefinition(definition, Market.Field.MARKET_ID.ToString(), instrument.Name);

            }

            string currency = instrument.Currency.ToString();
            if (!market.Currency.Equals(currency))
            {
                definition = UpdateDefinitions<Market>.setUpdateDefinition(definition, Market.Field.CURRENCY.ToString(), currency);
            }

            if (!market.Description.Equals(instrument.Description))
            {
                definition = UpdateDefinitions<Market>.setUpdateDefinition(definition, Market.Field.DESCRIPTION.ToString(), instrument.Description);
            }

            string type = instrument.InstrumentType.ToString();
            if (!market.Type.Equals(type))
            {
                definition = UpdateDefinitions<Market>.setUpdateDefinition(definition, Market.Field.TYPE.ToString(), type);
            }

            double ticksize = instrument.TickSize;
            if (market.TickSize != ticksize)
            {
                definition = UpdateDefinitions<Market>.setUpdateDefinition(definition, Market.Field.TICK_SIZE.ToString(), ticksize);
            }

            if (market.PointValue != instrument.PointValue)
            {
                UpdateDefinitions<Market>.setUpdateDefinition(definition, Market.Field.POINT_VALUE.ToString(), instrument.PointValue);
            }

            return definition;
        }

        
    }
}
