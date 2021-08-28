using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.Helpers;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
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

        public RetainerInfo[] OrderedRetainerList => HelperFunctions.GetOrderedRetainerArray(HelperFunctions.ReadRetainerArray());

        public int NumberOfRetainers => OrderedRetainerList.Length;

        public int NumberOfVentures => ___Elements()[1].TrimmedData;

        public string RetainerName(int index)
        {
            return OrderedRetainerList[index].Name;
        }

        public int GetRetainerJobLevel(int index)
        {
            return OrderedRetainerList[index].Level;
        }

        public bool RetainerHasJob(int index)
        {
            return OrderedRetainerList[index].Job != (ClassJobType) 0;
        }

        public RetainerRole RetainerRole(int index)
        {
            return (RetainerRole) (___Elements()[(index * 9) + 4].TrimmedData);
        }

        /*public RetainerInfo[] OrderedRetainerList(RetainerInfo[] retainers)
        {
            return HelperFunctions.GetOrderedRetainerArray(retainers);
        }*/

        public async Task<bool> SelectRetainer(int index)
        {
            if (!IsOpen)
            {
                Logging.Write($"Retainer selection window not open");
                return false;
            }

            //Logging.Write($"Selecting {RetainerName(index)}: {index}");

            try
            {
                SendAction(2, 3UL, 2UL, 3UL, (ulong) index);

                //await Coroutine.Sleep(300);

                await Coroutine.Wait(9000, () => DialogOpen || SelectString.IsOpen);

                if (DialogOpen)
                {
                    while (!SelectString.IsOpen)
                    {
                        await Coroutine.Sleep(200);
                        if (DialogOpen) Next();
                        await Coroutine.Wait(1500, () => DialogOpen || SelectString.IsOpen);
                    }
                }
                //if (DialogOpen) Next();

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