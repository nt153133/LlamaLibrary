using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.RemoteWindows
{
    public class CollectablesShop: RemoteWindow<CollectablesShop>
    {
        private const string WindowName = "CollectablesShop";

        public int count => ___Elements()[20].TrimmedData -1;
        public CollectablesShop() : base(WindowName)
        {
            _name = WindowName;
        }

        public List<string> ListItems()
        {
            int count = ___Elements()[20].TrimmedData -1;
            var currentElements = ___Elements();
            var result = new List<string>();
            for (int j = 0; j < count; j++)
            {
                if (currentElements[32 + (j * 11)].TrimmedData == ___Elements()[21].TrimmedData) continue; //IconID
                var itemID = currentElements[34 + (j * 11)].TrimmedData;
                //if (itemID == 0 || itemID > 1500000 || itemID < 500000)  continue;
                result.Add($"{j}: {DataManager.GetItem((uint) (itemID - 500000))} {itemID}");
                //Logger.Info($"{itemID}");
            }

            return result;
        }

    }
}