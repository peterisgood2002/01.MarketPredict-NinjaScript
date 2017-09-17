using MongoDB.Bson;
using MongoDB.Driver;
using NinjaTrader.Cbi;
using NinjaTrader.Custom.MongoDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.TableOperation
{
    class ContractsOperation
    {
        public static Contracts insertContract(IMongoCollection<Contracts> collection, string marketName, Instrument instrument)
        {
            Rollover data = getRollOverInformation(instrument);

            Contracts contract = new Contracts(marketName, instrument.FullName, data.Date, data.Offset);

            insertContract(collection, contract);

            return contract;
        }

        public static Contracts insertContract(IMongoCollection<Contracts> collection, string marketName, Instrument instrument, DateTime beginDate, DateTime expiryDate)
        {
            Rollover data = getRollOverInformation(instrument);

            Contracts contract = new Contracts(marketName, instrument.FullName, data.Date, data.Offset);

            contract.BeginDate = beginDate;
            contract.ExpiryDate = expiryDate;

            insertContract(collection, contract);

            return contract;
        }

        private static void insertContract(IMongoCollection<Contracts> collection, Contracts contract)
        {
            collection.InsertOne(contract);
        }
        public static Rollover getRollOverInformation(Instrument instrument)
        {
            Rollover result = null;

            string[] month = instrument.FullName.Split(' ');
            foreach (Rollover data in instrument.MasterInstrument.RolloverCollection)
            {
                if (data.ToString().Equals(month[1]))
                {
                    result = data;
                }
            }

            
            return result;
        }

        public static UpdateResult updateContract(IMongoCollection<Contracts> collection, FilterDefinition<Contracts> filter, Instrument instrument, Contracts result)
        {
            UpdateOptions options = new UpdateOptions();
            options.IsUpsert = true;

            UpdateDefinition<Contracts> definition = createContractUpdateDefinition(result, instrument);

            return updateContract(collection, filter, options, definition);
        }

        public static UpdateResult updateContract(IMongoCollection<Contracts> collection, FilterDefinition<Contracts> filter, Instrument instrument, Contracts result, DateTime beginDate, DateTime expiryDate)
        {
            UpdateOptions options = new UpdateOptions();
            options.IsUpsert = true;

            UpdateDefinition<Contracts> definition = createContractUpdateDefinition(result, instrument, beginDate, expiryDate);

            return updateContract(collection, filter, options, definition);
        }

        private static UpdateDefinition<Contracts> createContractUpdateDefinition(Contracts contract, Instrument instrument)
        {
            UpdateDefinition<Contracts> definition = null;

            //RollOver information
            Rollover rollover = getRollOverInformation(instrument);
            if (contract.RollDate == null || !contract.RollDate.Equals(rollover.Date))
            {
                definition = Definitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.ROLL_DATE.ToString(), rollover.Date);
            }

            if (contract.RollOffset != rollover.Offset)
            {
                definition = Definitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.ROLL_OFFSET.ToString(), rollover.Offset);
            }

            
            return definition;
        }

        private static UpdateDefinition<Contracts> createContractUpdateDefinition(Contracts contract, Instrument instrument, DateTime beginDate, DateTime expiryDate)
        {
            UpdateDefinition<Contracts> definition = createContractUpdateDefinition(contract, instrument);

            if(contract.BeginDate == null || !contract.BeginDate.Equals( beginDate) )
            {
                definition = Definitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.BEGIN_DATE.ToString(), beginDate);
            }

            if (contract.ExpiryDate == null || contract.ExpiryDate.CompareTo(expiryDate) < 0)
            {
                definition = Definitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.EXPIRY_DATE.ToString(), expiryDate);
            }
            return definition;
        }

        private static UpdateResult updateContract(IMongoCollection<Contracts> collection, FilterDefinition<Contracts> filter, UpdateOptions options, UpdateDefinition<Contracts> definition)
        {
            if (definition != null)
            {
                UpdateResult r = collection.UpdateOne(filter, definition, options);

                return r;
            }
            else
            {
                return null;
            }
        }
    }
}
