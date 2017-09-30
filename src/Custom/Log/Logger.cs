using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.Log
{
    class Logger
    {
        public static void Log(Object obj, params string[] str)
        {
            Type type = obj.GetType();
            Log(type.Name, str);
        }

        public static void Log(params string[] str)
        {
            StringBuilder log = compositeLog(str);

            print(log);
            
        }

        private static StringBuilder compositeLog( params string[] str)
        {
            StringBuilder log = new StringBuilder();

            log.Append("[" + System.Threading.Thread.CurrentThread.Name + "]");
            foreach (string s in str)
            {
                log.Append("[" + s + "]");
            }

            return log;
        }
        private static void print(StringBuilder log)
        {
            NinjaTrader.Code.Output.Process(log.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }
    }
}
