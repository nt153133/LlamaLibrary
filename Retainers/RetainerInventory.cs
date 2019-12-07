using System.Collections.Generic;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace LlamaLibrary.Retainers
{
    internal class RetainerInventory
    {
        private IDictionary<uint, BagSlot> dict = new Dictionary<uint, BagSlot>();

        public void AddItem(BagSlot slot)
        {
            if (HasItem(slot.TrueItemId))
            {
                Logging.Write(
                $"ERROR: Trying to add item twice \t Name: {slot.Item.CurrentLocaleName} Count: {slot.Count} BagId: {slot.BagId} IsHQ: {slot.Item.IsHighQuality}");
                return;
            }

            dict.Add(slot.TrueItemId, slot);
        }

        public BagSlot GetItem(uint trueItemId)
        {
            return dict.TryGetValue(trueItemId, out var returnBagSlot) ? returnBagSlot : null;
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
                Logging.Write($"Name: {item.Item.CurrentLocaleName} Count: {item.Count} RawId: {item.RawItemId} IsHQ: {item.Item.IsHighQuality} TrueID: {item.TrueItemId}");
            }
        }
    }
}