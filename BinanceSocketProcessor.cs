using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using EncryptionLibrary;


namespace BinanceTester
{
    public class BinanceSocketProcessor
    {
        public long TimeDiff = 0; // Time offset got via NTP, milliseconds
        public long avgLatency = 0;

        private const string ConfigFileName = "config.txt";
        private const string SPOT_URL = "wss://stream.binance.com:9443";
        private const string FUTURES_URL = "wss://fstream.binance.com";
        private long MaxDelta = 200; // start logging if latency more then MaxDelta
        private static BinanceSettings Settings;
        private BinanceDataProcessor DataProcessor;
        private DateTimeGenerator myDTG = DateTimeGenerator.Instance;
        

        public BinanceSocketProcessor() 
        {
            TimeDiff = myDTG.GetNTPTimeDiff();
            Thread TimeSyncWorker = new Thread(new ThreadStart(TimeSyncRun));
            TimeSyncWorker.Start();
            DataProcessor = new BinanceDataProcessor();
            ReloadSettings();
        }

        public void ConnentAllSockets()
        {
            List<string[]> symbolsArrays = DataProcessor.GetArraysOfSymbols(Settings.Futures, Settings.N);

            foreach (string[] syms in symbolsArrays)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Run));
                t.Start(syms);
                if (syms.Count() < 5)
                {
                    Logger.ConsoleOut("Threaded connection subscribed to " + DataProcessor.SubscribePath(syms));
                }
                else 
                {
                    Logger.ConsoleOut("Threaded connection subscribed to " + syms.Count().ToString() + " symbols");
                }
            }
        }

        public void Run(object obj)
        {
            string[] symbols = (string[])obj;
            while (true)
            {
                try
                {
                    CancellationTokenSource cts;
                    ClientWebSocket socket = CreateAndConnectSocket(symbols, out cts);
                    ProcessSocket(socket, cts);
                }
                catch (Exception exx)
                {
                    string msg = "Error: " + exx.Message;
                    Console.WriteLine(msg);
                    Logger.Log(msg);
                }
            }
        }

        public ClientWebSocket CreateAndConnectSocket(string[] symbols,
            out CancellationTokenSource cts)
        {
            bool futures = Settings.Futures;
            string baseurl = SPOT_URL;
            if (futures)
                baseurl = FUTURES_URL;
            string streams = "/stream?streams=";
            ClientWebSocket socket = new ClientWebSocket();
            socket.Options.KeepAliveInterval = TimeSpan.FromMilliseconds(Settings.SocketKeepAlive);
            string symbolsStr = DataProcessor.SubscribePath(symbols);
            string s = baseurl + streams + symbolsStr;

            Uri uri = new Uri(s);
            cts = new CancellationTokenSource();
            Task t = socket.ConnectAsync(uri, cts.Token);
            t.Wait();
            return socket;
        }

        public void ProcessSocket(ClientWebSocket socket, CancellationTokenSource cts)
        {
            while (socket.State == WebSocketState.Open)
            {
                ReadAndProcessData(socket, cts);
            }
            Logger.Log("State=" + socket.State.ToString());
        }

        public void ReadAndProcessData(ClientWebSocket socket, CancellationTokenSource cts)
        {
            WebSocketReceiveResult result;
            try
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                Task<WebSocketReceiveResult> t = socket.ReceiveAsync(bytesReceived, cts.Token);
                result = t.Result;

                string recived = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                ProcessNewMessage(recived);
            }
            catch (Exception exx)
            {
                string msg = "Exception: " + exx.Message;
                Console.WriteLine(msg);
                Logger.Log(msg);
            }
        }

        public void ProcessNewMessage(string recived)
        {
            long dtNow = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
            try
            {
                var root = JsonConvert.DeserializeObject<Types.Trades.Root>(recived);
                long delta = Math.Abs(dtNow - root.data.T + TimeDiff);

                if (avgLatency == 0) { 
                    avgLatency = delta; 
                } else { 
                    avgLatency = Convert.ToInt64((avgLatency * 9 + delta) / 10);
                }

                if (delta > MaxDelta + 10)
                {
                    MaxDelta = delta;
                    Logger.ConsoleOut("Latency = " + delta.ToString() + " ms. \n  data: " + recived);
                }
            }
            catch (Exception ex)
            {
                string msg = "Parsing error: " + ex.Message;
                Console.WriteLine(msg);
                Logger.Log(msg);
            }
        }

        #region TimeSync
        private void TimeSyncRun() {
            while (true)
            {
                Thread.Sleep(20000);

                {
                    TimeDiff = myDTG.GetNTPTimeDiff(); // check time once per 20 seconds
                }

            }
        }

        #endregion

        #region Helpers
        private void ReloadSettings()
        {
            try
            {
                string s = ReadFromFile(ConfigFileName);
                Settings = JsonConvert.DeserializeObject<BinanceSettings>(s);
            }
            catch
            {
                Settings = new BinanceSettings() { Futures = false, N = 1 };
                string s2 = JsonConvert.SerializeObject(Settings);
                Logger.ConsoleOut("error reading settings: " + s2);
            }
        }

        public static string ReadFromFile(string fileName)
        {
            using (FileStream fstream = File.OpenRead(fileName))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.UTF8.GetString(array);
                return textFromFile;
            }
        }

        #endregion    
    }
}
