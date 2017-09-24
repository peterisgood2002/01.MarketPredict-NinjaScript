using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.Thread
{
    public class ThreadMethod :Attribute
    {
        public string methodName { get; set; }

        public ThreadMethod(string name)
        {
            methodName = name;
        }
    }
}
