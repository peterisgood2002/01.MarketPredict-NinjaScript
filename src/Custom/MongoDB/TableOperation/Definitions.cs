using MongoDB.Driver;
using NinjaTrader.Custom.MongoDB.Table;
using System;

namespace NinjaTrader.Custom.MongoDB.TableOperation
{
    class Definitions<Type>
    {
        public const string ID = "_id";
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
            return Builders<Type>.Filter.Eq(ID, id);
        }

        public static FilterDefinition<Type> between(DateTime beginTime, DateTime endTime)
        {
            FilterDefinition<Type> filterbegin = Builders<Type>.Filter.Gte<DateTime>(ID+ "." + Figure.Field.TIMESTAMP.ToString(), beginTime.ToUniversalTime());
            FilterDefinition<Type> filterend = Builders<Type>.Filter.Lte<DateTime>(ID + "." + Figure.Field.TIMESTAMP.ToString(), endTime.ToUniversalTime());
            FilterDefinition<Type> filter = Builders<Type>.Filter.And(filterbegin, filterend);

            return filter;
        }
    }
}
