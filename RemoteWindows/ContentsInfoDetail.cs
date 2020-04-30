using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;

namespace LlamaLibrary.RemoteWindows
{
    public class ContentsInfoDetail : RemoteWindow<ContentsInfoDetail>
    {
        private const string WindowName = "ContentsInfoDetail";

        public ContentsInfoDetail() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public int GetNumberOfCraftingTurnins()
        {
            return IsOpen ? ___Elements()[55].TrimmedData : 0;
        }
        
        public int GetNumberOfGatheringTurnins()
        {
            return IsOpen ? ___Elements()[56].TrimmedData : 0;
        }
        
        public List<Item> GetCraftingTurninItemsIds()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 209, GetNumberOfCraftingTurnins());

            return itemElements.Select(item => DataManager.GetItem((uint) (item.TrimmedData))).ToList();
        }
        
        public List<Item> GetGatheringTurninItemsIds()
        {
            var currentElements = ___Elements();

            var itemElements = new ArraySegment<TwoInt>(currentElements, 268, GetNumberOfGatheringTurnins());

            return itemElements.Select(item => DataManager.GetItem((uint) (item.TrimmedData))).ToList();
        }
        
        public Dictionary<Item, KeyValuePair<int,string>> GetCraftingTurninItems()
        {
            var result = new Dictionary<Item, KeyValuePair<int,string>>();
            var currentElements = ___Elements();
            var itemElements = new ArraySegment<TwoInt>(currentElements, 209, GetNumberOfCraftingTurnins()).ToArray();
            var jobElements = new ArraySegment<TwoInt>(currentElements, 217, GetNumberOfCraftingTurnins()).ToArray();
            var qtyElements = new ArraySegment<TwoInt>(currentElements, 225, GetNumberOfCraftingTurnins()).ToArray();
            
            for (var i = 0; i < GetNumberOfCraftingTurnins(); i++)
            {
                result.Add(DataManager.GetItem((uint) (itemElements[i].TrimmedData)), new KeyValuePair<int, string>(qtyElements[i].TrimmedData,((RetainerRole)jobElements[i].TrimmedData ).ToString()));
            }

            return result;
        }
        
        public Dictionary<Item, KeyValuePair<int,string>> GetGatheringTurninItems()
        {
            var result = new Dictionary<Item, KeyValuePair<int,string>>();
            var currentElements = ___Elements();
            var itemElements = new ArraySegment<TwoInt>(currentElements, 268, GetNumberOfGatheringTurnins()).ToArray();
            var jobElements = new ArraySegment<TwoInt>(currentElements, 271, GetNumberOfGatheringTurnins()).ToArray();
            var qtyElements = new ArraySegment<TwoInt>(currentElements, 274, GetNumberOfGatheringTurnins()).ToArray();
            
            for (var i = 0; i < GetNumberOfGatheringTurnins(); i++)
            {
                result.Add(DataManager.GetItem((uint) (itemElements[i].TrimmedData)), new KeyValuePair<int, string>(qtyElements[i].TrimmedData,((RetainerRole)jobElements[i].TrimmedData ).ToString()));
            }

            return result;
        }
    }
}