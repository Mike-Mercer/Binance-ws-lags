using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceTester
{
    public class BinanceSettings
    {
        public bool Futures { get; set; } = false;
        public int N { get; set; } = 1;
        public string[] Symbols { get; set; } = new string[] { "btc", "eth","ltc","bnb","xrp" };
        public int SocketKeepAlive { get; set; } = 30000;
    }
}
