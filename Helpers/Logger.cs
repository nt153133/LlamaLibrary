using System;
using System.Collections.Generic;
using System.Windows.Media;
using ff14bot.Helpers;

namespace LlamaLibrary.Helpers
{
    public static class Logger
    {
        private static Random rng = new Random();  
        public static void External(string caller, string message, Color color)
        {
            Logging.Write(color, $"[{caller}]" + message);
        }
        
        public static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, text);
        }
        
        public static void Info(string text)
        {
            Logging.Write(Colors.Aqua, text);
        }
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

    }
}