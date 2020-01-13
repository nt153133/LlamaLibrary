using System.Windows.Media;
using ff14bot.Helpers;

namespace LlamaLibrary.Helpers
{
    public static class Logger
    {
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

    }
}