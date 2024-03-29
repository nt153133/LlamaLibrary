﻿using System.Collections.Generic;
using System.Linq;
using Clio.Utilities;
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

        internal static readonly Dictionary<uint, ItemSortInfo> ItemSortInfoCache = new Dictionary<uint, ItemSortInfo>();

        public static readonly HashSet<int> FilledAndSortedInventories = new HashSet<int>();

        public static readonly HashSet<uint> TryingToMoveUniques = new HashSet<uint>();

        public static readonly HashSet<uint> PlayerInventoryUniques = new HashSet<uint>();

        public static IEnumerable<CachedInventory> GetAllInventories()
        {
            if (AutoRetainerSortSettings.Instance.InventoryOptions.ContainsKey(PlayerInventoryIndex))
            {
                yield return PlayerInventory;
            }
            if (AutoRetainerSortSettings.Instance.InventoryOptions.ContainsKey(SaddlebagInventoryIndex))
            {
                yield return SaddlebagInventory;
            }
            foreach (CachedInventory retainerInventory in RetainerInventories.Values)
            {
                yield return retainerInventory;
            }
        }

        public static bool AnyRulesExist() => AutoRetainerSortSettings.Instance.InventoryOptions.Values.Any(invOptions => invOptions.Any());

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
            if (ItemSortInfoCache.TryGetValue(trueItemId, out ItemSortInfo sortInfo))
            {
                return sortInfo;
            }
            ItemSortInfoCache.Add(trueItemId, new ItemSortInfo(trueItemId));
            return ItemSortInfoCache[trueItemId];
        }

        public static bool AnyUnsorted()
        {
            foreach (CachedInventory cachedInventory in GetAllInventories())
            {
                if (FilledAndSortedInventories.Contains(cachedInventory.Index))
                {
                    continue;
                }

                if (!cachedInventory.AllBelong())
                {
                    return true;
                }
            }

            return false;
        }

        public static void UpdateIndex(int index, IEnumerable<Bag> bags)
        {
            var bagsArray = bags.ToArray();
            CachedInventory cachedInventory = GetByIndex(index);
            cachedInventory.Update(bagsArray);
            if (cachedInventory.AllBelong() && cachedInventory.FreeSlots == 0) FilledAndSortedInventories.Add(index);
        }

        public static void UpdateFromCache(RetainerInfo[] retData)
        {
            ClearAll();

            var mainBagsArray = InventoryManager.GetBagsByInventoryBagId(GeneralFunctions.MainBags).ToArray();
            PlayerInventory.Update(mainBagsArray);

            StoredSaddleBagInventory storedSaddlebagInventory = ItemFinder.GetCachedSaddlebagInventoryComplete();
            SaddlebagInventory.Update(storedSaddlebagInventory);

            var cachedRetInventories = ItemFinder.GetCachedRetainerInventories();

            for (var i = 0; i < retData.Length; i++)
            {
                RetainerInfo retInfo = retData[i];
                if (!retInfo.Active) continue;

                if (cachedRetInventories.TryGetValue(retInfo.Unique, out StoredRetainerInventory storedInventory))
                {
                    if (RetainerInventories.ContainsKey(i))
                    {
                        RetainerInventories[i].Update(storedInventory);
                    }
                    else
                    {
                        RetainerInventories.Add(i, new CachedInventory(retInfo.Name, i));
                        RetainerInventories[i].Update(storedInventory);
                    }
                }
            }

            foreach (CachedInventory cachedInventory in GetAllInventories())
            {
                if (cachedInventory.AllBelong() && cachedInventory.FreeSlots == 0)
                {
                    FilledAndSortedInventories.Add(cachedInventory.Index);
                }
            }

            foreach (ItemSortInfo sortInfo in PlayerInventory.ItemCounts.Select(x => GetSortInfo(x.Key)))
            {
                if (sortInfo.ItemInfo.Unique)
                {
                    PlayerInventoryUniques.Add(sortInfo.TrueItemId);
                }
            }
        }

        public static void ClearAll()
        {
            FilledAndSortedInventories.Clear();
            PlayerInventoryUniques.Clear();
            TryingToMoveUniques.Clear();
            GetAllInventories().ForEach(x => x.Clear());
        }
    }
}