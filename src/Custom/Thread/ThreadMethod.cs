using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.Thread
{
    /*
       a ThreadMethod need to follow the argument constraint. 
       This method has two arguments which mean beginIndex and endIndex following the other arguments
    */
    public class ThreadMethod :Attribute
    {
        public string methodName { get; set; }

        public ThreadMethod(string name)
        {
            methodName = name;
        }
    }
}
