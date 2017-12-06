using MongoDB.Driver;
using NinjaTrader.Custom.Log;
using NinjaTrader.Custom.MongoDB.Table;
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

        public static void createThread<T>(Object obj, string methodName, BindingFlags flag, bool wait, int begin, int end, params Object[] arguments)
        {

            Logger.Log("ThreadManager", "createThread", "BEGIN");
            Type className = obj.GetType();
            MethodInfo method = getMethodWithAttribute<T>(className, methodName, flag, begin, end, arguments);

            List<Task> tasks = invokeThread(obj, begin, end, arguments, method);

            waitMe(wait, tasks);

            Logger.Log("ThreadManager", "createThread", "End");

        }

        protected static MethodInfo getMethodWithAttribute<T>(Type className, string methodName, BindingFlags flag, Object begin, Object end, Object[] arguments)
        {
            MethodInfo result = getMethodInfo(className, methodName, flag);
            Type type = typeof(T);

            result = result.MakeGenericMethod(type);
            Type[] atype = result.GetGenericArguments();
            ParameterInfo[] parameter = result.GetParameters();

            checkExists(methodName, begin, end, arguments, parameter);

            return result;
        }
        public static void createThread(Object obj, string methodName, BindingFlags flag, bool wait, int begin, int end, params Object[] arguments)
        {

            Logger.Log("ThreadManager", "createThread", "BEGIN");
            Type className = obj.GetType();
            MethodInfo method = getMethodWithAttribute(className, methodName, flag, begin, end, arguments);

            List<Task> tasks = invokeThread(obj, begin, end, arguments, method);

            waitMe(wait, tasks);

            Logger.Log("ThreadManager", "createThread", "End");

        }

        private static List<Task> invokeThread(object obj, int begin, int end, object[] arguments, MethodInfo method)
        {
            int size = (end - begin) / THREADCOUNT;
            if( size == 0 )
            {
                size = 1;
            }
            List<Task> tasks = new List<Task>();
            for (int beginIdx = 0; beginIdx < end; beginIdx += size)
            {
                int endIdx = beginIdx + size;
                if (endIdx > end)
                {
                    endIdx = end;
                }

                tasks.Add(invoke(method, obj, beginIdx, endIdx, arguments));

            }

            return tasks;
        }

        private static void waitMe(bool wait, List<Task> tasks)
        {
            if (wait)
            {
                Logger.Log("ThreadManager", "createThread", "Wait thread to complete");

                Task.WaitAll(tasks.ToArray());

                Logger.Log("ThreadManager", "createThread", "Finish thread to complete");

            }
        }

        private static Task invoke(MethodInfo method, object obj, Object beginIdx, Object endIdx, object[] parameter)
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

        protected static MethodInfo getMethodWithAttribute(Type className, string methodName, BindingFlags flag, Object begin, Object end, Object[] arguments)
        {
            MethodInfo result = getMethodInfo(className, methodName, flag);

            ParameterInfo[] parameter = result.GetParameters();

            checkExists(methodName, begin, end, arguments, parameter);

            return result;
        }
        protected static MethodInfo getMethodInfo(Type className, string methodName, BindingFlags flag)
        {
            MethodInfo result = null;
            foreach (MethodInfo method in className.GetMethods(flag))
            {
                if (method.IsDefined(typeof(ThreadMethod)))
                {
                    ThreadMethod attribute = method.GetCustomAttribute<ThreadMethod>();
                    if (attribute.methodName.Equals(methodName))
                    {
                        result = method;
                    }

                }

            }

            return result;
        }
        private static void checkExists(string methodName, object begin, object end, object[] arguments, ParameterInfo[] parameter)
        {
            if (!parameter[0].ParameterType.IsInstanceOfType(begin) &&
                 !parameter[1].ParameterType.IsInstanceOfType(end) &&
                arguments.Length != parameter.Length - 2)
            {
                throw new MissingMethodException("Can not find method = " + methodName + "(" + getParameterType(begin, end, arguments) + ")");
            }
            else
            {

                for (int i = 0; i < arguments.Length; i++)
                {

                    if (!parameter[i + 2].ParameterType.IsInstanceOfType(arguments[i]))
                    {
                        throw new MissingMethodException("Can not find method = " + methodName + "(" + getParameterType(begin, end, arguments) + ")");

                    }
                }

            }
        }

        private static string getParameterType(object begin, object end, object[] parameter)
        {
            StringBuilder result = new StringBuilder(begin.GetType() + ", " + end.GetType() + ", ");

            for (int i = 0; i < parameter.Length; i++)
            {
                if (i == parameter.Length - 1)
                {
                    result.Append(parameter[i].GetType());
                }
                else
                {
                    result.Append(parameter[i].GetType() + ", ");
                }


            }

            return result.ToString();
        }

        public static void createThread(Object obj, string methodName, BindingFlags flag, bool wait, DateTime begin, DateTime end, params Object[] parameter)
        {
            Logger.Log("ThreadManager", "createThread", "BEGIN");

            Type className = obj.GetType();
            MethodInfo method = getMethodWithAttribute(className, methodName, flag, begin, end, parameter);

            int size = end.Subtract(begin).Days / THREADCOUNT;
            if( size == 0 )
            {
                size = 1;
            }
            List<Task> tasks = new List<Task>();
            for (DateTime beginIdx = begin; beginIdx < end;)
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
                Logger.Log("ThreadManager", "createThread", "Wait thread to complete");


                Task.WaitAll(tasks.ToArray());

                Logger.Log("ThreadManager", "createThread", "Finish thread to complete");

            }

            Logger.Log("ThreadManager", "createThread", "End");

        }
    }
}
