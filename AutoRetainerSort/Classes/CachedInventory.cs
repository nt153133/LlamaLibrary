using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Managers;

namespace LlamaLibrary.AutoRetainerSort.Classes
{
    public class CachedInventory
    {
        public string Name;

        public int Index;

        public Dictionary<uint, int> ItemSlotCounts;

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

        public int FreeSlotCount()
        {
            int freeSlots = FreeSlotsByIndex(Index);
            foreach (var idCountPair in ItemSlotCounts)
            {
                Item itemInfo = ItemSortStatus.GetSortInfo(idCountPair.Key).ItemInfo;
                if (itemInfo == null) continue;
                var stackSize = (int)itemInfo.StackSize;
                int count = idCountPair.Value;

                int slotsTaken = stackSize / count;
                freeSlots -= slotsTaken;
            }

            return freeSlots;
        }

        public int this[uint itemId]
        {
            get => ItemSlotCounts[itemId];
            set => ItemSlotCounts[itemId] = value;
        }

        public bool IsSorted() => ItemSlotCounts
            .Where(x => x.Value > 0)
            .Select(x => ItemSortStatus.GetSortInfo(x.Key))
            .All(x => x.BelongsInIndex(Index));

        public int UnsortedCount() => ItemSlotCounts
            .Where(x => x.Value > 0)
            .Select(x => ItemSortStatus.GetSortInfo(x.Key))
            .Count(x => !x.BelongsInIndex(Index));

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

        public void Update(Dictionary<uint, int> itemFinderDic)
        {
            Clear();
            foreach (var idCountPair in itemFinderDic)
            {
                ItemSlotCounts[idCountPair.Key] = idCountPair.Value;
            }
        }

        public void Update(IEnumerable<BagSlot> bagSlots)
        {
            Clear();
            foreach (BagSlot bagSlot in bagSlots)
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
        }

        public CachedInventory(string name, int index)
        {
            Name = name;
            Index = index;
            ItemSlotCounts = new Dictionary<uint, int>();
        }
    }
}