using MongoDB.Driver;
using NinjaTrader.Custom.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.Thread
{
    class SaveManager : ThreadManager
    {
        public static void save<T>(IMongoCollection<T> collection, List<T> lprice)
        {

            createThread<T>(new SaveManager(), "save", BindingFlags.Static | BindingFlags.Public, true, 0, lprice.Count, collection, lprice);

        }
        
        [ThreadMethod("save")]
        public static void save<T>(int begin, int end, IMongoCollection<T> collection, List<T> lprice)
        {
            Logger.Log("SaveManager", "save", "Begin = " + begin, " End = " + end, "BEGIN");
            for (int i = begin; i < end; i++)
            {
                T price = lprice[i];
                try
                {
                    collection.InsertOne(price);
                }
                catch (Exception ex)
                {
                    NinjaTrader.Code.Output.Process("[PriceOperation.insertPrice][Error] Level 1 :" + price.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                    throw ex;
                    // we do not throw any exception because we treat this execption as normal while we insert price to our database due to the duplication our Ninjascript will create
                }

            }

            Logger.Log("SaveManager", "save", "Begin = " + begin, " End = " + end, "END");
        }
    }
}
