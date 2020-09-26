using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.RemoteWindows
{
    public class ContentsInfoDetail : RemoteWindow<ContentsInfoDetail>
    {
        private const string WindowName = "ContentsInfoDetail";
        private int eleNumCrafting = 51;
        private int eleNumGathering = 52;
        private int eleCraftingItem = 205;
        private int eleCraftingJob = 213;
        private int eleCraftingQty = 221;
        private int eleGatheringItem = 264;
        private int eleGatheringJob = 267;
        private int eleGatheringQty = 270;

        public ContentsInfoDetail() : base(WindowName)
        {
            _name = WindowName;
            if (Translator.Language == Language.Chn)
            {
                eleNumCrafting = 55;
                eleNumGathering = 56;
                eleCraftingItem = 206;
                eleCraftingJob = 217;
                eleCraftingQty = 225;
                eleGatheringItem = 268;
                eleGatheringJob = 271;
                eleGatheringQty = 274;
            }
        }

        public int GetNumberOfCraftingTurnins()
        {
            return IsOpen ? ___Elements()[eleNumCrafting].TrimmedData : 0;
        }

        public int GetNumberOfGatheringTurnins()
        {
            return IsOpen ? ___Elements()[eleNumGathering].TrimmedData : 0;
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

        public Dictionary<Item, KeyValuePair<int, string>> GetCraftingTurninItems()
        {
            var result = new Dictionary<Item, KeyValuePair<int, string>>();
            var currentElements = ___Elements();
            var itemElements = new ArraySegment<TwoInt>(currentElements, eleCraftingItem, GetNumberOfCraftingTurnins()).ToArray();
            var jobElements = new ArraySegment<TwoInt>(currentElements, eleCraftingJob, GetNumberOfCraftingTurnins()).ToArray();
            var qtyElements = new ArraySegment<TwoInt>(currentElements, eleCraftingQty, GetNumberOfCraftingTurnins()).ToArray();

            for (var i = 0; i < GetNumberOfCraftingTurnins(); i++)
            {
                result.Add(DataManager.GetItem((uint) (itemElements[i].TrimmedData)), new KeyValuePair<int, string>(qtyElements[i].TrimmedData, ((RetainerRole) jobElements[i].TrimmedData).ToString()));
            }

            return result;
        }

        public Dictionary<Item, KeyValuePair<int, string>> GetGatheringTurninItems()
        {
            var result = new Dictionary<Item, KeyValuePair<int, string>>();
            var currentElements = ___Elements();
            var itemElements = new ArraySegment<TwoInt>(currentElements, eleGatheringItem, GetNumberOfGatheringTurnins()).ToArray();
            var jobElements = new ArraySegment<TwoInt>(currentElements, eleGatheringJob, GetNumberOfGatheringTurnins()).ToArray();
            var qtyElements = new ArraySegment<TwoInt>(currentElements, eleGatheringQty, GetNumberOfGatheringTurnins()).ToArray();

            for (var i = 0; i < GetNumberOfGatheringTurnins(); i++)
            {
                result.Add(DataManager.GetItem((uint) (itemElements[i].TrimmedData)), new KeyValuePair<int, string>(qtyElements[i].TrimmedData, ((RetainerRole) jobElements[i].TrimmedData).ToString()));
            }

            return result;
        }
    }
}