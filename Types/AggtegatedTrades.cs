using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceTester.Types.AggregatedTrades
{
    public class Data
    {
        public string e { get; set; }
        public long E { get; set; }
        public string s { get; set; }
        public int a { get; set; }
        public string p { get; set; }
        public string q { get; set; }
        public int f { get; set; }
        public int l { get; set; }
        public long T { get; set; }
        public bool m { get; set; }
        public bool M { get; set; }
    }

    public class Root
    {
        public string stream { get; set; }
        public Data data { get; set; }
    }

}
