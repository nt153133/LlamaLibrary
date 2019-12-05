﻿using System.Collections.Generic;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace Retainers
{
    internal class RetainerInventory
    {
        internal IDictionary<uint, BagSlot> dict = new Dictionary<uint, BagSlot>();

        public void AddItem(BagSlot slot)
        {
            if (HasItem(slot.TrueItemId))
            {
                Logging.Write($"ERROR: Trying to add item twice \t Name: {0} Count: {1} BagId: {2} IsHQ: {3}",
                    slot.Item.EnglishName, slot.Count, slot.BagId, slot.Item.IsHighQuality);
                return;
            }

            dict.Add(slot.TrueItemId, slot);
        }

        public BagSlot GetItem(uint trueItemId)
        {
            if (dict.TryGetValue(trueItemId, out var returnBagSlot))
                return returnBagSlot;
            return null;
        }

        public bool HasItem(uint trueItemId)
        {
            return dict.ContainsKey(trueItemId);
        }

        public void PrintList()
        {
            foreach (var slot in dict)
            {
                var item = slot.Value;
                Logging.Write("Name: {0} Count: {1} RawId: {2} IsHQ: {3} TrueID: {4} ", item.Item.EnglishName,
                    item.Count, item.RawItemId, item.Item.IsHighQuality, item.TrueItemId);
            }
        }
    }
}