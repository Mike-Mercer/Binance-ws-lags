using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceTester.Types.Rest
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MyArray
    {
        public int a { get; set; }
        public string p { get; set; }
        public string q { get; set; }
        public int f { get; set; }
        public int l { get; set; }
        public object T { get; set; }
        public bool m { get; set; }
        public bool M { get; set; }
    }

    public class Root
    {
        public List<MyArray> MyArray { get; set; }
    }
}
