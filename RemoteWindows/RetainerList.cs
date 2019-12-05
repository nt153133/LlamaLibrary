using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Helpers;
using ff14bot.RemoteWindows;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary.RemoteWindows
{
    internal class RetainerList : RemoteWindow<RetainerList>
    {
        private const string WindowName = "RetainerList";

        public RetainerList() : base(WindowName)
        {
        }

        public async Task<bool> SelectRetainer(int index)
        {
            if (IsOpen)
            {
                Logging.Write("Retainer selection window not open");
                return false;
            }

            Logging.Write("Selecting retainer: {0}", index);

            try
            {
                SendAction(2, 3UL, 2UL, 3UL, (ulong) index);

                await Coroutine.Sleep(500);

                await Coroutine.Wait(9000, () => DialogOpen);

                if (DialogOpen) Next();

                await Coroutine.Sleep(500);

                if (SelectString.IsOpen)
                    return true;
            }
            catch (Exception ex)
            {
                Logging.Write("Error selecting retainer: {0}", ex);
            }

            return false;
        }

    }
}