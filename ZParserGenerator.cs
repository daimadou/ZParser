using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZParser
{
    class Production
    {        
        public int ProductionID;
        public string ProductionName;
        public List<string> ProductionItems;
        public HashSet<string> FirstSymbols;
    }
    struct Item
    {
        public @Production Production;
        public string DotRightSymbol 
        { 
            get 
            {
                if (DotOffset < Production.ProductionItems.Count)
                {
                    return Production.ProductionItems[DotOffset];
                }
                else
                {
                    return "";
                }
            } 
        }
        public int DotOffset;
        public HashSet<string> LookAhead;
        private string innerExpr;
        public Item(Production p, int dotOffset, HashSet<String> lookahead)
        {
            this.Production = p;
            this.DotOffset = dotOffset;
            this.LookAhead = lookahead;
            this.innerExpr = null;
        }
        public override string ToString()
        {
            if (innerExpr != null)
            {
                return innerExpr;
            }
            
            StringBuilder sb = new StringBuilder();
            List<string> productionItems = Production.ProductionItems;
            for (int i = 0; i < productionItems.Count; i++)
            {
                if(i==DotOffset)
                {
                    sb.Append(".");
                }
                sb.Append(productionItems[i]);
            }
            if (DotOffset == productionItems.Count)
            {
                sb.Append(".");
            }
            innerExpr = Production.ProductionName + "=" + sb.ToString();
            return innerExpr;
        }
    }

    class State
    {
        public int ID;
        public HashSet<String> KernelItems;
        public List<Item> Contents;
    }
    class ZParserGenerator
    {
        Dictionary<string, List<Production>> ProductionsTable;
        Dictionary<string, HashSet<string>> FirstTable;
        HashSet<string> First(string ProductionName)
        {
            HashSet<string> track = new HashSet<string>();
            return First(ProductionName, track);
        }

        HashSet<string> First(string TokenName, HashSet<string> Track)
        {
            if (FirstTable.ContainsKey(TokenName))
            {
                return FirstTable[TokenName];
            }
            
            HashSet<String> ret = new HashSet<string>();
            if (ProductionsTable.ContainsKey(TokenName) && !Track.Contains(TokenName))
            {
                Track.Add(TokenName);
                List<Production> productions = ProductionsTable[TokenName];
                foreach (var product in productions)
                {
                    string key = product.ProductionItems[0];
                    ret.UnionWith(First(key, Track));
                }
                FirstTable.Add(TokenName, ret);
            }
            else
            {
                ret.Add(TokenName);
            }
            return ret;
        }
        public void Clousre(State s)
        {
            List<Item> items = s.Contents;
            HashSet<string> tracks = new HashSet<string>();
            for (int i = 0; i < items.Count; i++)
            {
                tracks.Add(tracks.ToString());
                Production product = items[i].Production;
                int dotOffset = items[i].DotOffset;
                string symbol = items[i].DotRightSymbol;
                if (ProductionsTable.ContainsKey(symbol))
                {
                    HashSet<string> terminals = null;
                    if (dotOffset < product.ProductionItems.Count - 1)
                    {
                        terminals = First(product.ProductionItems[dotOffset + 1]);
                    }
                    else
                    {
                        terminals = new HashSet<string>();
                        terminals.Add("$");
                    }
                    List<Production> productions = ProductionsTable[symbol];
                    foreach (var p in productions)
                    {
                        Item it = new Item(p, 0, terminals);
                        if (!tracks.Contains(it.ToString()))
                        {
                            items.Add(it);
                        }
                    }
                }
            }
        }
    }
}
