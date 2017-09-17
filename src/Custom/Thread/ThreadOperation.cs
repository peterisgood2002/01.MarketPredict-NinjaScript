using MongoDB.Driver;
using NinjaTrader.Cbi;
using NinjaTrader.Custom.DataOperation;
using NinjaTrader.Custom.MongoDB;
using NinjaTrader.Custom.MongoDB.Table;
using NinjaTrader.Data;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
namespace NinjaTrader.Custom.Thread
{

    class ThreadOperation
    {
        public static void createThread(MongoClient connection, string dbName, Instrument Instrument, Contracts contract, DateTime beginDate, DateTime endDate, string tmpFolder)
        {

            beginDate = new DateTime(2017, 8, 31);
            DateTime afterBeginDate = beginDate.AddDays(1);
            Core.Globals.RandomDispatcher.BeginInvoke(new Action(() =>
            {
                string file = tmpFolder;
                NinjaTrader.Code.Output.Process(String.Format("Dump Data: {0} Time={1} millisecond = {2} ", file, DateTime.Now, DateTime.Now.Ticks / 10000000), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                MarketReplay.DumpMarketDepth(Instrument, beginDate, afterBeginDate, file);
                DateTime firstDate = DataParser.parseDate(file, false);
                DateTime lastDate = DataParser.parseDate(file, true);
                NinjaTrader.Code.Output.Process("First Date of File" + file + " = " + firstDate, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                NinjaTrader.Code.Output.Process("Last Date of File" + file + " = "+ lastDate , NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                NinjaTrader.Code.Output.Process(String.Format("Load Data: {0} Time={1} millisecond = {2}", file, DateTime.Now, DateTime.Now.Ticks / 10000000), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                //MongoDBMethod.searchPrice(connection, dbName, firstDate, lastDate);
                List<Figure> data = DataParser.parse(contract.Id.MarketName, contract.Id.ContractName, file);
                NinjaTrader.Code.Output.Process(String.Format("Finish Data: {0} Time={1} millisecond = {2}", file, DateTime.Now, DateTime.Now.Ticks / 10000000), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                //MongoDBMethod.insertPrice(connection, dbName, data);


            }), DispatcherPriority.SystemIdle);
            NinjaTrader.Code.Output.Process("Date=" + new DateTime(2017, 3, 9), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }
    }
}
