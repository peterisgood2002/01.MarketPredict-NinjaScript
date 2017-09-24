using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace NinjaTrader.Custom.Thread
{

    class ThreadManager
    {

        public const int THREADCOUNT = 10;
        public static void createThread(Object obj, string methodName, BindingFlags flag, bool wait, int begin, int end, params Object[] parameter)
        {
            NinjaTrader.Code.Output.Process("[createThread][BEGIN]", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            Type className = obj.GetType();
            MethodInfo method = getMethodWithAttribute(className, methodName, flag, begin, end, parameter);

            int size = (end - begin) / THREADCOUNT;
            List<Task> tasks = new List<Task>();
            for( int beginIdx = 0; beginIdx < end; beginIdx+=size )
            {
                int endIdx = beginIdx + size;
                if (endIdx > end)
                {
                    endIdx = end;
                }

                tasks.Add(invoke(method, obj, beginIdx, endIdx, parameter));

            }

            if (wait)
            {
                NinjaTrader.Code.Output.Process("[createThread]Wait thread to complete", NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                Task.WaitAll(tasks.ToArray());

                NinjaTrader.Code.Output.Process("[createThread]Finish thread to complete", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            }

            NinjaTrader.Code.Output.Process("[createThread][END]", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }

        private static Task invoke(MethodInfo method, object obj, Object beginIdx, Object endIdx,  object[] parameter)
        {
            
            
            Object[] p = new Object[parameter.Length + 2];
            p[0] = beginIdx;
            p[1] = endIdx;
            for (int i = 2; i < p.Length; i++)
            {
                p[i] = parameter[i - 2];
            }

            return Task.Run(() => method.Invoke(obj, p));
            
            
        }

        public static MethodInfo getMethodWithAttribute(Type className, string methodName, BindingFlags flag, Object begin, Object end, Object[] parameter)
        {
            MethodInfo result = null;
            foreach ( MethodInfo method in className.GetMethods(flag)) {
                if( method.IsDefined( typeof(ThreadMethod)))
                {
                    ThreadMethod attribute = method.GetCustomAttribute< ThreadMethod>();
                    if( attribute.methodName.Equals( methodName))
                    {          
                        result = method;
                    }

                }
                
            }

            ParameterInfo[] arguments = result.GetParameters();
            
            if (arguments[0].ParameterType != begin.GetType() && arguments[1].ParameterType != end.GetType() && parameter.Length != arguments.Length - 2 )
            {
                throw new MissingMethodException("Can not find method = " + methodName + "(" + getParameterType(begin, end, parameter) + ")");
            } else
            {
                bool canFound = true;
                for (int i = 0; i < parameter.Length; i++)
                {
                    if( parameter[i].GetType() != arguments[i + 2].ParameterType)
                    {
                        canFound = false;
                    }
                }

                if (canFound == false)
                {
                    throw new MissingMethodException("Can not find method = " + methodName + "(" + getParameterType(begin, end, parameter) + ")");
                }
            }
            return result;
        }

        private static string getParameterType(object begin, object end, object[] parameter)
        {
            StringBuilder result = new StringBuilder(begin.GetType() + ", " + end.GetType() + ", ");

            for( int i = 0; i < parameter.Length; i++ )
            {
                if( i == parameter.Length - 1 )
                {
                    result.Append(parameter[i].GetType() );
                } else
                {
                    result.Append(parameter[i].GetType() + ", ");
                }
                

            }

            return result.ToString();
        }

        public static void createThread(Object obj, string methodName, BindingFlags flag, bool wait, DateTime begin, DateTime end, params Object[] parameter)
        {
            NinjaTrader.Code.Output.Process("[createThread][BEGIN]", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            Type className = obj.GetType();
            MethodInfo method = getMethodWithAttribute(className, methodName, flag, begin, end, parameter);

            int size = end.Subtract( begin).Days / THREADCOUNT;
            List<Task> tasks = new List<Task>();
            for (DateTime beginIdx = begin; beginIdx < end; )
            {
                DateTime endIdx = beginIdx.AddDays(size);
                if (endIdx > end)
                {
                    endIdx = end;
                }

                tasks.Add(invoke(method, obj, beginIdx, endIdx, parameter));

                beginIdx = endIdx;
            }

            if (wait)
            {
                NinjaTrader.Code.Output.Process("[createThread]Wait thread to complete", NinjaTrader.NinjaScript.PrintTo.OutputTab1);

                Task.WaitAll(tasks.ToArray());

                NinjaTrader.Code.Output.Process("[createThread]Finish thread to complete", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            }

            NinjaTrader.Code.Output.Process("[createThread][END]", NinjaTrader.NinjaScript.PrintTo.OutputTab1);
        }
    }
}
