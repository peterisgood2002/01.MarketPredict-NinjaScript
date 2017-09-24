using MongoDB.Driver;
using NinjaTrader.Custom.MongoDB.Table;
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

                foreach (L1Price price in l1)
                {
                    try
                    {
                        l1Collection.InsertOne(price);
                    } catch ( Exception ex)
                    {
                        NinjaTrader.Code.Output.Process("[PriceOperation.insertPrice][Error] Level 1 :" + price.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                        throw ex;
                        // we do not throw any exception because we treat this execption as normal while we insert price to our database due to the duplication our Ninjascript will create
                    }
                    
                }
                
            }

            NinjaTrader.Code.Output.Process("[PriceOperation.insertPrice] Level 2 :" + l2.Count, NinjaTrader.NinjaScript.PrintTo.OutputTab1);
            if ( l2.Count > 0 )
            {
                foreach (L2Price price in l2)
                {
                    try
                    {
                        l2Collection.InsertOne(price);
                    }
                    catch (Exception ex)
                    {
                        NinjaTrader.Code.Output.Process("[PriceOperation.insertPrice][Error] Level 1 :" + price.ToString(), NinjaTrader.NinjaScript.PrintTo.OutputTab1);
                        throw ex;
                        // we do not throw any exception because we treat this execption as normal while we insert price to our database due to the duplication our Ninjascript will create
                    }

                }
            }
           
        }

        public static void updateL1Price()
        {
            UpdateOptions options = new UpdateOptions();
            options.IsUpsert = true;

        }
    }
}
