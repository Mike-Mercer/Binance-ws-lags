using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace BinanceTester
{

	public class BinanceDataProcessor
	{
		public BinanceDataProcessor()
		{
		}

        private async Task<string> GetRestResponse(string url)
        {
            using (HttpClient cl = new HttpClient())
            {
                HttpResponseMessage prm = await cl.GetAsync(url);
                string result = await prm.Content.ReadAsStringAsync();
                return result;
            }
        }

        public string GetAggrSymbolTemlate(string symbol)
        {
            return symbol + "usdt@aggTrade";
        }
        public string GetSymbolTemlate(string symbol)
        {
            return symbol + "usdt@trade";
        }

        public string SubscribePath(string[] symbols)
        {
            string[] symbolTemplates = new string[symbols.Length];
            for (int i = 0; i < symbols.Length; i++)
            {
                symbolTemplates[i] = GetSymbolTemlate(symbols[i]);
            }
            string rez = string.Join('/', symbolTemplates);
            return rez;
        }

        private string[] GetAllSymbols(bool futures)
        {
            Logger.ConsoleOut("Getting USDT markets list");
            List<string> symbols = new List<string>();
            if (futures)
            {
                string ur = "https://fapi.binance.com/fapi/v1/exchangeInfo";
                Task<string> t = GetRestResponse(ur);
                string s = t.Result;
                Types.FuturesExchangeInfo.Root futureRoot =
                    JsonConvert.DeserializeObject<Types.FuturesExchangeInfo.Root>(s);
                List<Types.FuturesExchangeInfo.Symbol> futureSymbols =
                    futureRoot.symbols;
                List<Types.FuturesExchangeInfo.Symbol> quoteSymbols =
                    futureRoot.symbols.Where(sy => sy.quoteAsset == "USDT").ToList();
                for (int i = 0; i < quoteSymbols.Count; i++)
                {
                    symbols.Add(quoteSymbols[i].baseAsset.ToLower());
                }
            }
            else
            {
                string url = "https://api.binance.com/api/v3/exchangeInfo";
                Task<string> t = GetRestResponse(url);
                string s = t.Result;
                Types.SpotExchangeInfo.Root spotroot =
                    JsonConvert.DeserializeObject<Types.SpotExchangeInfo.Root>(s);

                List<Types.SpotExchangeInfo.Symbol> quoteSymbols =
                    spotroot.symbols.Where(sy => sy.quoteAsset == "USDT").ToList();
                for (int i = 0; i < quoteSymbols.Count; i++)
                {
                    symbols.Add(quoteSymbols[i].baseAsset.ToLower());
                }

            }
            Logger.ConsoleOut("Total symbols: " + symbols.Count);
            return symbols.ToArray();
        }

        private List<string[]> SplitSymbols(string[] symbols, int N)
        {
            List<string> allSymblos = symbols.ToList();

            // BTC will always run in a sepatare thread
            allSymblos.Remove("btc");
            List<string[]> symbolsArrays = new List<string[]>();
            symbolsArrays.Add(new string[] { "btc" });

            // Other markets will run in N threads, N specified in the config file
            double d = (double)((double)allSymblos.Count / (double)N);
            int symbolsInOneSocket = Convert.ToInt32(Math.Round(d));
            if (symbolsInOneSocket < 1)
                symbolsInOneSocket = 1;

            int i = 0;
            while (i < allSymblos.Count)
            {
                int count = Math.Min(symbolsInOneSocket, allSymblos.Count - i);
                symbolsArrays.Add(allSymblos.GetRange(i, count).ToArray());
                i = i + symbolsInOneSocket;
            }
            return symbolsArrays;
        }

        public List<string[]> GetArraysOfSymbols(bool IsFutures, int N)
        {
            string[] symbols = GetAllSymbols(IsFutures);
            return SplitSymbols(symbols, N);
        }

    }
}
