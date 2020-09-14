using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceTester.Types.Trades
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Data
    {
        public string e { get; set; }
        public long E { get; set; }
        public long T { get; set; }
        public string s { get; set; }
        public int t { get; set; }
        public string p { get; set; }
        public string q { get; set; }
        public string X { get; set; }
        public bool m { get; set; }
    }

    public class Root
    {
        public string stream { get; set; }
        public Data data { get; set; }
    }



}
