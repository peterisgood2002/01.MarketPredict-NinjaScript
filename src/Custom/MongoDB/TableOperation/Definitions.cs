using MongoDB.Driver;
using NinjaTrader.Custom.MongoDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.MongoDB.TableOperation
{
    class Definitions<Type>
    {
        public static UpdateDefinition<Type> setUpdateDefinition(UpdateDefinition<Type> definition, string field, object value)
        {
            if (definition == null)
            {
                definition = Builders<Type>.Update.Set(field, value);
            }
            else
            {
                definition = definition.Set(field, value);
            }

            return definition;
        }

        public static FilterDefinition<Type> generateIdFilter<Field>(Field id)
        {
            return Builders<Type>.Filter.Eq("_id", id);
        }
    }
}
