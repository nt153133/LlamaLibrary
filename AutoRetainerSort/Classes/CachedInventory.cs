using System.Collections.Generic;
using System.Linq;
using ff14bot.Managers;
using LlamaLibrary.RetainerItemFinder;

namespace LlamaLibrary.AutoRetainerSort.Classes
{
    public class CachedInventory
    {
        public string Name;

        public int Index;

        public Dictionary<uint, int> ItemCounts;

        public Dictionary<uint, int> ItemSlotsTakenCounts;

        public int FreeSlots;

        public bool AllBelong()
        {
            foreach (var idCountPair in ItemCounts.Where(x => x.Key > 0 && x.Value > 0))
            {
                ItemSortInfo sortInfo = ItemSortStatus.GetSortInfo(idCountPair.Key);
                if (Index != ItemSortStatus.PlayerInventoryIndex
                    && sortInfo.ItemInfo.Unique
                    && ItemSortStatus.PlayerInventoryUniques.Contains(sortInfo.TrueItemId)) continue;
                if (sortInfo.SortStatus(Index) == SortStatus.Move) return false;
            }
            return true;
        }

        public Dictionary<int, int> DestinationCountsByIndex()
        {
            ItemSortStatus.TryingToMoveUniques.Clear();
            var sortStatus = new Dictionary<int, int>();

            foreach (var idCountPair in ItemSlotsTakenCounts.Where(x => x.Value > 0 && x.Key > 0))
            {
                var sortInfo = ItemSortStatus.GetSortInfo(idCountPair.Key);
                var desiredIndexes = sortInfo.MatchingIndexes;
                if (desiredIndexes.Length == 0) continue;
                for (int i = 0; i < desiredIndexes.Length; i++)
                {
                    if (desiredIndexes[i] == Index) continue;
                    if (desiredIndexes[i] < ItemSortStatus.PlayerInventoryIndex) continue;
                    if (desiredIndexes[i] == int.MinValue || desiredIndexes[i] == int.MaxValue) continue;
                    if (ItemSortStatus.FilledAndSortedInventories.Contains(desiredIndexes[i])) continue;
                    if (sortInfo.ItemInfo.Unique)
                    {
                        if (ItemSortStatus.GetByIndex(desiredIndexes[i]).ItemCounts.ContainsKey(sortInfo.TrueItemId)) continue;
                        if (ItemSortStatus.TryingToMoveUniques.Contains(sortInfo.TrueItemId)) continue;
                        if (Index != ItemSortStatus.PlayerInventoryIndex
                            && ItemSortStatus.PlayerInventoryUniques.Contains(sortInfo.TrueItemId)) continue;
                    }

                    ItemSortStatus.TryingToMoveUniques.Add(sortInfo.TrueItemId);

                    if (sortStatus.ContainsKey(desiredIndexes[i]))
                    {
                        sortStatus[desiredIndexes[i]] += idCountPair.Value;
                        break;
                    }
                    else
                    {
                        sortStatus.Add(desiredIndexes[i], idCountPair.Value);
                        break;
                    }
                }
            }

            return sortStatus;
        }

        public void Clear()
        {
            ItemCounts.Clear();
            ItemSlotsTakenCounts.Clear();
        }

        public void Update(IStoredInventory storedInventory)
        {
            Clear();
            foreach (var idCountPair in storedInventory.Inventory)
            {
                ItemCounts[idCountPair.Key] = idCountPair.Value;
            }
            foreach (var idCountPair in storedInventory.SlotCount)
            {
                ItemSlotsTakenCounts[idCountPair.Key] = idCountPair.Value;
            }

            FreeSlots = storedInventory.FreeSlots;
        }

        public void Update(Bag[] bags)
        {
            Clear();
            foreach (BagSlot bagSlot in bags.SelectMany(x => x.FilledSlots))
            {
                if (bagSlot == null || !bagSlot.IsValid || !bagSlot.IsFilled) continue;
                if (ItemCounts.ContainsKey(bagSlot.TrueItemId))
                {
                    ItemCounts[bagSlot.TrueItemId] += (int)bagSlot.Count;
                    ItemSlotsTakenCounts[bagSlot.TrueItemId]++;
                }
                else
                {
                    ItemCounts.Add(bagSlot.TrueItemId, (int)bagSlot.Count);
                    ItemSlotsTakenCounts.Add(bagSlot.TrueItemId, (int)bagSlot.Count);
                }
            }
            FreeSlots = (int)bags.Sum(x => x.FreeSlots);
        }

        public CachedInventory(string name, int index)
        {
            Name = name;
            Index = index;
            ItemCounts = new Dictionary<uint, int>();
            ItemSlotsTakenCounts = new Dictionary<uint, int>();
        }
    }
}