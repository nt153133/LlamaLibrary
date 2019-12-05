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

    }
}