using System;
using System.Threading;

namespace BinanceTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.ConsoleOut("\n ==================== Start \n");
            BinanceSocketProcessor SocketProcessor = new BinanceSocketProcessor();

            try
            {
                SocketProcessor.ConnentAllSockets();
            }
            catch (Exception ex)
            {
                string msg = "Disconnected with error " + ex.Message;
                Logger.ConsoleOut(msg);
            }
            while (true)
            {
                Console.Write("\r Working ...  Avg. Latency: {0}   Time Accuracy: {1}    ",
                    SocketProcessor.avgLatency, SocketProcessor.TimeDiff);

                Thread.Sleep(200);
            }
        }
    }
}
