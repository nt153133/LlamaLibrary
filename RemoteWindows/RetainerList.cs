using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary.RemoteWindows
{
    internal class RetainerList : RemoteWindow<RetainerList>
    {
        private const string WindowName = "RetainerList";

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
            return OrderedRetainerList[index].Job != 0;
        }

        public RetainerRole RetainerRole(int index)
        {
            return (RetainerRole)___Elements()[index * 9 + 4].TrimmedData;
        }

        public async Task<bool> SelectRetainer(ulong retainerContentId)
        {
            var list = OrderedRetainerList;
            if (!list.Any(i => i.Unique == retainerContentId))
            {
                return false;
            }

            return await SelectRetainer(OrderedRetainerList.IndexOf(list.First(i => i.Unique == retainerContentId)));
        }

        public async Task<bool> SelectRetainer(int index)
        {
            if (!IsOpen)
            {
                Logging.Write("Retainer selection window not open");
                return false;
            }

            try
            {
                SendAction(2, 3UL, 2UL, 3UL, (ulong)index);

                await Coroutine.Wait(9000, () => DialogOpen || SelectString.IsOpen);

                if (DialogOpen)
                {
                    while (!SelectString.IsOpen)
                    {
                        if (DialogOpen)
                        {
                            Next();
                            await Coroutine.Sleep(100);
                        }
                        await Coroutine.Wait(1500, () => DialogOpen || SelectString.IsOpen);
                    }
                }

                await Coroutine.Wait(9000, () => SelectString.IsOpen);
            }
            catch (Exception ex)
            {
                Logging.Write("Error selecting retainer: {0}", ex);
            }

            return SelectString.IsOpen;
        }
    }
}