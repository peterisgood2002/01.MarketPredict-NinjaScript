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
        public static Contracts insertContract(IMongoCollection<Contracts> collection, ObjectId marketId, Instrument instrument)
        {
            Rollover data = getRollOverInformation(instrument);

            Contracts contract = new Contracts( marketId, instrument.FullName, data.Date, data.Offset);

            collection.InsertOne(contract);
            
            return contract;
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

        public static UpdateResult updateContract(IMongoCollection<Contracts> collection, FilterDefinition<Contracts> filter, Instrument instrument, List<Contracts> result) 
        {
            UpdateOptions options = new UpdateOptions();
            options.IsUpsert = true;

            UpdateDefinition<Contracts> definition = createContractUpdateDefinition(result.First(), instrument);
            
            if( definition != null )
            {
                UpdateResult r = collection.UpdateOne(filter, definition, options);

                return r;
            } else
            {
                return null;
            }
        } 

        private static UpdateDefinition<Contracts> createContractUpdateDefinition(Contracts contract, Instrument instrument)
        {
            UpdateDefinition<Contracts> definition = null;

            if( contract.ContractName == null || !contract.ContractName.Equals( instrument.FullName))
            {
                definition = UpdateDefinitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.CONTRACT_NAME.ToString(), instrument.FullName);
            }


            //RollOver information
            Rollover rollover = getRollOverInformation(instrument);
            if (contract.RollDate == null || !contract.RollDate.Equals( rollover.Date))
            {
                definition = UpdateDefinitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.ROLL_DATE.ToString(), rollover.Date);
            }

            if (contract.RollOffset != rollover.Offset)
            {
                definition = UpdateDefinitions<Contracts>.setUpdateDefinition(definition, Contracts.Field.ROLL_OFFSET.ToString(), rollover.Offset);
            }
            return definition;
        }
    }
}
