using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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

            if (System.Threading.Thread.CurrentThread.Name != null)
            {
                log.Append("[" + System.Threading.Thread.CurrentThread.Name + "]");
            } else
            {
                log.Append("[Thread Id = " + System.Threading.Thread.CurrentThread.ManagedThreadId + "]");
            }
            foreach (string s in str)
            {
                log.Append("[" + s + "]");
            }

            return log;
        }
        private static void print(StringBuilder log)
        {
            Core.Globals.RandomDispatcher.BeginInvoke(new Action(() =>
            {
                NinjaTrader.Code.Output.Process(log.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            }), DispatcherPriority.SystemIdle);
        }
    }
}
