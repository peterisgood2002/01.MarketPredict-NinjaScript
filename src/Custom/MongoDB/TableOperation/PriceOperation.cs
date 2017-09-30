using MongoDB.Driver;
using NinjaTrader.Custom.MongoDB.Table;
using NinjaTrader.Custom.Thread;
using System;
using System.Collections.Generic;

namespace NinjaTrader.Custom.MongoDB.TableOperation
{
    class PriceOperation
    {
        public static void insertPrice(IMongoCollection<L1Price> l1Collection, IMongoCollection<L2Price> l2Collection, List<Figure> data)
        {
            List<L1Price> l1 = new List<L1Price>();
            List<L2Price> l2 = new List<L2Price>();

            foreach(Figure price in data)
            {
                if( price is L1Price )
                {
                    l1.Add((L1Price) price);
                }else if(price is L2Price)
                {
                    l2.Add((L2Price)price);     
                }
            }

            NinjaTrader.Code.Output.Process("[PriceOperation.insertPrice] Level 1 :" + l1.Count, NinjaTrader.NinjaScript.PrintTo.OutputTab1);

            if ( l1.Count > 0 )
            {
                SaveManager.save<L1Price>(l1Collection, l1);  
            }

            NinjaTrader.Code.Output.Process("[PriceOperation.insertPrice] Level 2 :" + l2.Count, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            if ( l2.Count > 0 )
            {
                SaveManager.save<L2Price>(l2Collection, l2);
               
            }
           
        }

        public static void updateL1Price()
        {
            UpdateOptions options = new UpdateOptions();
            options.IsUpsert = true;

        }
    }
}
