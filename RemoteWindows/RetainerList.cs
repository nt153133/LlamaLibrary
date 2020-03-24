using System;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary.RemoteWindows
{
    internal class RetainerList : RemoteWindow<RetainerList>
    {
        private const string WindowName = "RetainerList";

        //public override bool IsOpen => IsOpen;

        public RetainerList() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public int NumberOfRetainers => ___Elements()[2].TrimmedData;
        
        public int NumberOfVentures => ___Elements()[1].TrimmedData;

        public string RetainerName(int index)
        {
            return Core.Memory.ReadString((IntPtr) ___Elements()[(index * 9) + 3].Data, Encoding.UTF8);
        }

        public int GetRetainerJobLevel(int index)
        {
            return ___Elements()[(index * 9) + 5].TrimmedData;
        }

        public bool RetainerHasJob(int index)
        {
            return ___Elements()[(index * 9) + 4].TrimmedData != 0;
        }

        public RetainerRole RetainerRole(int index)
        {
            return (RetainerRole) (___Elements()[(index * 9) + 4].TrimmedData);
        }
        
        public async Task<bool> SelectRetainer(int index)
        {
            if ( !IsOpen)
            {
                Logging.Write($"Retainer selection window not open");
                return false;
            }

            Logging.Write($"Selecting {RetainerName(index)}: {index}");

            try
            {
                SendAction(2, 3UL, 2UL, 3UL, (ulong) index);

                await Coroutine.Sleep(300);

                await Coroutine.Wait(9000, () => DialogOpen);

                if (DialogOpen) Next();

                //await Coroutine.Sleep(300);
                
                await Coroutine.Wait(9000, () => SelectString.IsOpen);

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