using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BinanceTester
{
    public static class Logger
    {
        private static object locky = new object();
        static Logger() 
        {
//            File.Create("Log.txt").Close(); // clear log on startup
        }
        public static void Log(string message)
        {
            string msg = DateTime.UtcNow.ToLongTimeString() + " " + message;
            lock (locky)
            {
                using (StreamWriter streamWriter = new StreamWriter("Log.txt", true, System.Text.Encoding.Default))
                {
                    streamWriter.WriteLine(message);
                }
               
            }
        }

        public static void ConsoleOut(string message)
        {
            message = DateTime.Now.ToString() + ": " + message;
            Console.WriteLine("\n" + message);
            Log(message);
           
        }
    }
}
