using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public class FreeShop: RemoteWindow<FreeShop>
    {
        private const string WindowName = "FreeShop";

        public FreeShop() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public int NumberOfItems =>  ___Elements()[3].TrimmedData;
        
        public List<Item> GetAvailItems()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 65, NumberOfItems);

            return itemElements.Select(item => DataManager.GetItem((uint) (item.TrimmedData))).ToList();
        }

        public async Task<bool> BuyItem(uint itemId)
        {
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == itemId))
                return true;

            if (IsOpen)
            {
                ClickItem(GetItemIndex(itemId));
                await Coroutine.Wait(5000, () => InventoryManager.FilledSlots.Any(i => i.RawItemId == itemId));
            }

            return (InventoryManager.FilledSlots.Any(i => i.RawItemId == itemId));
        }

        public int GetItemIndex(uint itemId)
        {
            var items = GetAvailItems().ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Id == itemId)
                    return i;
            }

            return -1;
        }

        public void ClickItem(int index)
        {
            if (index >=0)
                SendAction(2,3,0,3,(ulong) index);
        }
    }
}