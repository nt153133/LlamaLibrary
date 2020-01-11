using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteWindows
{
    public class SubmarinePartsMenu : RemoteWindow<SubmarinePartsMenu>
    {
        private const string WindowName = "SubmarinePartsMenu";

        public SubmarinePartsMenu() : base(WindowName)
        {
            _name = WindowName;
        }

        public void ClickItem(int index)
        {
            SendAction(3,3,0,4,(ulong) index,4,6);
        }

        public int GetNumberOfTurnins()
        {
            return IsOpen ? ___Elements()[11].TrimmedData : 0;
        }

        public int GetCraftItemID()
        {
            return IsOpen ? ___Elements()[0].TrimmedData : 0;
        }

        public List<Item> GetTurninItemsObjs()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 12, GetNumberOfTurnins());

            return itemElements.Select(item => DataManager.GetItem((uint) item.TrimmedData)).ToList();
        }

        public List<int> GetTurninItemsIds()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 12, GetNumberOfTurnins());

            return itemElements.Select(item => item.TrimmedData).ToList();
        }

        public List<int> GetTurninItemsQty()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 60, GetNumberOfTurnins());

            return itemElements.Select(item => item.TrimmedData).ToList();
        }

        public List<int> GetTurninsRequired()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 120, GetNumberOfTurnins());

            return itemElements.Select(item => item.TrimmedData).ToList();
        }

        public List<int> GetTurninsDone()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 108, GetNumberOfTurnins());

            return itemElements.Select(item => item.TrimmedData).ToList();
        }

        public List<int> GetItemAvailCount()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 72, GetNumberOfTurnins());

            return itemElements.Select(item => item.TrimmedData).ToList();
        }

        public List<FCWorkshopItem> GetCraftingTurninItems()
        {
            var result = new List<FCWorkshopItem>();
            var itemElements = GetTurninItemsIds();
            var requiredElements = GetTurninsRequired();
            var qtyElements = GetTurninItemsQty();

            for (var i = 0; i < GetNumberOfTurnins(); i++)
            {
                result.Add(new FCWorkshopItem(itemElements[i], qtyElements[i], requiredElements[i]));
            }

            return result;
        }
    }
}