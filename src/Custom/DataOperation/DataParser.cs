using MongoDB.Bson;
using NinjaTrader.Cbi;
using NinjaTrader.Custom.MongoDB.Table;
using NinjaTrader.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NinjaTrader.Custom.DataOperation
{
    class DataParser
    {
        public static DateTime parseDate(string file, bool endDate)
        {
            using (StreamReader stream = new StreamReader(file))
            {
                if (!endDate)
                {
                    string[] line = stream.ReadLine().Split(';');

                    return parseDate(line[2], line[3]);
                } else
                {
                    string line = null;
                    do
                    {
                        line = stream.ReadLine();
                    } while (stream.Peek() != -1);

                 
                    string[] split = line.Split(';');
                    return parseDate(split[2], split[3]);
                    
                }
            }
        }
        public class ListComparater<T> : IEqualityComparer<List<T>>
        {
            public bool Equals(List<T> x, List<T> y)
            {
                
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<T> obj)
            {
                int hashcode = 0;
                foreach (T t in obj)
                {
                    hashcode ^= t.GetHashCode();
                }
                return hashcode;
            }
        }
        public static List<Figure> parse(string market, string contract, string file, IDictionary<DateTime, List<Figure>> dataInDB)
        {
            IDictionary<List<Object>, Int32> seq = new Dictionary<List<Object>, Int32>( new ListComparater<Object>() );
            using (StreamReader stream = new StreamReader(file) )
            {
                
                List<Figure> data = new List<Figure>();
                while( !stream.EndOfStream )
                {
                    string[] line = stream.ReadLine().Split(';');
                    switch( line[0] )
                    {
                        case "L1":
                            parseL1(data, market, contract, line, seq, dataInDB);
                            break;
                        case "L2":
                            parseL2(data, market, contract, line, seq, dataInDB);
                            break;
                    }
                    
                }

                return data;
            }
        }

        private static void parseL2(List<Figure> data, string market, string contract, string[] line, IDictionary<List<Object>, Int32> seq, IDictionary<DateTime, List<Figure>> dataInDB)
        {
            try
            {
                if (line.Length == 9)
                {
                    DateTime time = parseDate(line[2], line[3]);

                    if (!dataInDB.ContainsKey(time))
                    {
                        int type = Int32.Parse(line[1]);


                        int op = Int32.Parse(line[4]);
                        int level = Int32.Parse(line[5]);
                        double price = Double.Parse(line[7]);
                        int volume = Int32.Parse(line[8]);

                        int seqNo = getSeq(seq, "L2", type, time, op, level, price);
                        L2Price amount = new L2Price(market, contract, time, seqNo, type, op, level, price, volume);
                        data.Add(amount);
                    }
                   
                } 

            } catch (FormatException)
            {
                NinjaTrader.Code.Output.Process("[parseL2] Result:" + line, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            }


        }

        private static void parseL1(List<Figure> data, string market, string contract, string[] line, IDictionary<List<Object>, Int32> seq, IDictionary<DateTime, List<Figure>> dataInDB)
        {
            try
            {
                if (line.Length == 6)
                {
                    DateTime time = parseDate(line[2], line[3]);
                    if (!dataInDB.ContainsKey(time))
                    {
                        int type = Int32.Parse(line[1]);

                        double price = Double.Parse(line[4]);
                        int volume = Int32.Parse(line[5]);
                        int seqNo = getSeq(seq, "L1", type, time, price);
                        L1Price amount = new L1Price(market, contract, time, seqNo, type, price, volume);

                        data.Add(amount);
                    }
                     
                }

            }
            catch (FormatException)
            {
                NinjaTrader.Code.Output.Process("[parseL2] Result:" + line, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            }
        }

        private static int getSeq(IDictionary<List<Object>, Int32> seq, params Object[] parameter)
        {
            List<Object> key = new List<Object>();
            foreach( Object p in parameter )
            {
                key.Add(p);
            }

            int seqNo = 0;

            if( seq.TryGetValue(key, out seqNo))
            {
                return ++seq[key];
            }

            seq.Add(key, seqNo);

            return seqNo;
        }

        public static string parseFileName( string line)
        {
            int length = line.LastIndexOf('.') - line.LastIndexOf("\\") - 1;
            return line.Substring(line.LastIndexOf("\\") + 1, length);

        }
        public static  DateTime parseDate(string line)
        {
            string firstFile = parseFileName(line);
            DateTime date = DateTime.ParseExact(firstFile, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            return date;

        }

        public static DateTime parseDate(string dateTime, string millisecond)
        {
            DateTime time = DateTime.ParseExact(dateTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            double mill = 0;
            if(millisecond.Length >= 3)
            {
                mill = Double.Parse(millisecond.Substring(0, 3));
            }
           

            return time.AddMilliseconds(mill);
        }
    }
}
