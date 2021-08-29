using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot.Managers;
using LlamaLibrary.AutoRetainerSort.Classes;
using LlamaLibrary.Helpers;
using LlamaLibrary.RetainerItemFinder;
using LlamaLibrary.Structs;

// ReSharper disable once CheckNamespace
namespace LlamaLibrary.AutoRetainerSort
{
    public static class ItemSortStatus
    {
        public const int PlayerInventoryIndex = -2;
        public static readonly CachedInventory PlayerInventory = new CachedInventory("Player Inventory", PlayerInventoryIndex);

        public const int SaddlebagInventoryIndex = -1;
        public static readonly CachedInventory SaddlebagInventory = new CachedInventory("Chocobo Saddlebag", SaddlebagInventoryIndex);

        public static readonly Dictionary<int, CachedInventory> RetainerInventories = new Dictionary<int, CachedInventory>();

        private static readonly Dictionary<uint, ItemSortInfo> ItemMatchingIndexCache = new Dictionary<uint, ItemSortInfo>();

        public static readonly HashSet<int> FilledAndSortedInventories = new HashSet<int>();

        public static IEnumerable<CachedInventory> GetAllInventories()
        {
            yield return PlayerInventory;
            yield return SaddlebagInventory;
            foreach (CachedInventory retainerInventory in RetainerInventories.Values)
            {
                yield return retainerInventory;
            }
        }

        public static CachedInventory GetByIndex(int index)
        {
            switch (index)
            {
                case PlayerInventoryIndex:
                    return PlayerInventory;
                case SaddlebagInventoryIndex:
                    return SaddlebagInventory;
                default:
                    return RetainerInventories[index];
            }
        }

        public static ItemSortInfo GetSortInfo(uint trueItemId)
        {
            if (ItemMatchingIndexCache.TryGetValue(trueItemId, out ItemSortInfo sortInfo))
            {
                return sortInfo;
            }
            ItemMatchingIndexCache.Add(trueItemId, new ItemSortInfo(trueItemId));
            return ItemMatchingIndexCache[trueItemId];
        }

        public static bool AnyUnsorted()
        {
            if (!PlayerInventory.IsSorted() && !FilledAndSortedInventories.Contains(PlayerInventoryIndex)) return true;
            if (!SaddlebagInventory.IsSorted() && !FilledAndSortedInventories.Contains(SaddlebagInventoryIndex)) return true;
            return RetainerInventories
                .Where(retInv => !FilledAndSortedInventories.Contains(retInv.Key))
                .Any(retInv => !retInv.Value.IsSorted());
        }

        public static void UpdateIndex(int index, IEnumerable<BagSlot> bagSlots)
        {
            CachedInventory cachedInventory = GetByIndex(index);
            cachedInventory.Update(bagSlots);
            if (cachedInventory.IsSorted() && cachedInventory.FreeSlotCount() == 0) FilledAndSortedInventories.Add(index);
        }

        public static async Task UpdateFromCache(RetainerInfo[] retData)
        {
            ClearAll();

            foreach (BagSlot bagSlot in GeneralFunctions.MainBagsFilledSlots())
            {
                uint trueId = bagSlot.TrueItemId;
                var count = (int)bagSlot.Count;

                if (PlayerInventory.ContainsKey(trueId))
                {
                    PlayerInventory[trueId] += count;
                }
                else
                {
                    PlayerInventory.Add(trueId, count);
                }
            }

            SaddlebagInventory.Update(await ItemFinder.GetCachedSaddlebagInventories());
            
            var cachedRetInventories = await ItemFinder.SafelyGetCachedRetainerInventories();

            for (var i = 0; i < retData.Length; i++)
            {
                RetainerInfo retInfo = retData[i];
                if (!retInfo.Active) continue;

                if (cachedRetInventories.TryGetValue(retInfo.Unique, out StoredRetainerInventory storedInventory))
                {
                    if (RetainerInventories.ContainsKey(i))
                    {
                        RetainerInventories[i].Update(storedInventory.Inventory);
                    }
                    else
                    {
                        RetainerInventories.Add(i, new CachedInventory(retInfo.Name, i));
                        RetainerInventories[i].Update(storedInventory.Inventory);
                    }
                }
            }

            if (PlayerInventory.IsSorted() && PlayerInventory.FreeSlotCount() == 0)
            {
                FilledAndSortedInventories.Add(PlayerInventoryIndex);
            }

            if (SaddlebagInventory.IsSorted() && SaddlebagInventory.FreeSlotCount() == 0)
            {
                FilledAndSortedInventories.Add(SaddlebagInventoryIndex);
            }

            foreach (var indexInventoryPair in RetainerInventories.Where(indexInventoryPair => indexInventoryPair.Value.IsSorted() && indexInventoryPair.Value.FreeSlotCount() == 0))
            {
                FilledAndSortedInventories.Add(indexInventoryPair.Key);
            }
        }

        public static void ClearAll()
        {
            PlayerInventory.Clear();
            SaddlebagInventory.Clear();
            foreach (CachedInventory retInv in RetainerInventories.Values)
            {
                retInv.Clear();
            }
        }
    }
}