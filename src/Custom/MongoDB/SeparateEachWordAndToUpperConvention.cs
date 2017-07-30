using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using System.Text.RegularExpressions;

namespace NinjaTrader.Custom.MongoDB
{
    /*
     * all data member of our class in NinjaTrader.Custom.MongoDB.Table should be the upper of the first character.
     * This convention class will map all data memeber to MongoDB schema of documents.
     */
    class SeparateEachWordAndToUpperConvention : IMemberMapConvention
    {
        private static readonly Regex s_seperateWordRegex =
                    new Regex(@"
                            (?<=[A-Z])(?=[A-Z][a-z]) |
                            (?<=[^A-Z])(?=[A-Z]) |
                            (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        public string Name
        {
            get
            {
                return "SeparateEachWordAndToUpperConvention";
            }
        }

        public void Apply(BsonMemberMap memberMap)
        {
            string replace = s_seperateWordRegex.Replace(memberMap.MemberName, "_");
            memberMap.SetElementName(replace.ToUpper());
        }
    }
}
