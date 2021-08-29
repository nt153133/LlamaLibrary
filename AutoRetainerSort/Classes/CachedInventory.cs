using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ff14bot.Managers;
using LlamaLibrary.RetainerItemFinder;

namespace LlamaLibrary.AutoRetainerSort.Classes
{
    public class CachedInventory
    {
        public string Name;

        public int Index;

        public Dictionary<uint, int> ItemSlotCounts;

        public int FreeSlots;

        public static int FreeSlotsByIndex(int index)
        {
            switch (index)
            {
                case ItemSortStatus.PlayerInventoryIndex:
                    return 140;
                case ItemSortStatus.SaddlebagInventoryIndex:
                    return 70;
                default:
                    return 175;
            }
        }

        public int this[uint itemId]
        {
            get => ItemSlotCounts[itemId];
            set => ItemSlotCounts[itemId] = value;
        }

        public bool AllBelong() => ItemSlotCounts
            .Where(x => x.Value > 0)
            .Select(x => ItemSortStatus.GetSortInfo(x.Key))
            .All(x => (x.IndexStatus(Index) & ItemSortInfo.ItemIndexStatus.DontMove) != 0);

        public int UnsortedCount() => ItemSlotCounts
            .Where(x => x.Value > 0)
            .Select(x => ItemSortStatus.GetSortInfo(x.Key))
            .Count(x => (x.IndexStatus(Index) & ItemSortInfo.ItemIndexStatus.DontMove) != 0);

        public Dictionary<int, int> SortStatusCounts()
        {
            var sortStatus = new Dictionary<int, int>();
            foreach (int index in ItemSlotCounts.Where(x => x.Value > 0).Select(x => ItemSortStatus.GetSortInfo(x.Key).MatchingIndex))
            {
                if (index == int.MinValue) continue;
                if (index == Index) continue;
                if (ItemSortStatus.FilledAndSortedInventories.Contains(index)) continue;

                if (sortStatus.ContainsKey(index))
                {
                    sortStatus[index]++;
                }
                else
                {
                    sortStatus.Add(index, 1);
                }
            }

            return sortStatus;
        }

        public bool ContainsKey(uint key) => ItemSlotCounts.ContainsKey(key);

        public void Add(uint key, int value) => ItemSlotCounts.Add(key, value);

        public void Clear()
        {
            ItemSlotCounts.Clear();
        }

        public void Update(IStoredInventory storedInventory)
        {
            Clear();
            foreach (var idCountPair in storedInventory.Inventory)
            {
                ItemSlotCounts[idCountPair.Key] = idCountPair.Value;
            }

            FreeSlots = storedInventory.FreeSlots;
        }

        public void Update(Bag[] bags)
        {
            Clear();
            foreach (BagSlot bagSlot in bags.SelectMany(x => x.FilledSlots))
            {
                if (bagSlot == null || !bagSlot.IsValid || !bagSlot.IsFilled) continue;
                if (ItemSlotCounts.ContainsKey(bagSlot.TrueItemId))
                {
                    ItemSlotCounts[bagSlot.TrueItemId] += (int)bagSlot.Count;
                }
                else
                {
                    ItemSlotCounts.Add(bagSlot.TrueItemId, (int)bagSlot.Count);
                }
            }
            FreeSlots = (int)bags.Sum(x => x.FreeSlots);
        }

        public CachedInventory(string name, int index)
        {
            Name = name;
            Index = index;
            ItemSlotCounts = new Dictionary<uint, int>();
        }
    }
}