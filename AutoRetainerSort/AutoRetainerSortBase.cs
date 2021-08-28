using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.RetainerItemFinder;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
using TreeSharp;

namespace LlamaLibrary.AutoRetainerSort
{
    public class AutoRetainerSort : BotBase
    {
        private Composite _root;
        public override string Name => "AutoRetainerSort";
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override Composite Root => _root;
        public override bool WantButton { get; } = true;
        private AutoRetainerSortForm _settingsForm;

        public override void Initialize()
        {
            OffsetManager.Init();
        }
        
        public override void OnButtonPress()
        {
            if (_settingsForm == null || _settingsForm.IsDisposed)
                _settingsForm = new AutoRetainerSortForm();
            try
            {
                _settingsForm.Show();
                _settingsForm.Activate();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public override void Start()
        {
            LogSuccess("Sorting retainers...");
            _root = new ActionRunCoroutine(r => Run());
        }

        private static bool StillNeedsSorting() => CachedInventoryByIndex.Any(pair => !IsSorted(pair.Key));

        private static bool IsSorted(int index) => CachedInventoryByIndex[index].ItemCounts.All(pair => !BelongsInDifferentIndex(pair.Key, index));

        private async Task<bool> Run()
        {
            await GeneralFunctions.StopBusy(true, true, false);
            
            BuildItemSortTypeCache();
            
            var retData = await HelperFunctions.GetOrderedRetainerArray(true);
            if (retData.Length == 0)
            {
                LogCritical("No retainers. Exiting.");
                TreeRoot.Stop("No retainer data found.");
                return false;
            }

            await UpdateCachedInventories(retData);

            foreach (var pair in AutoRetainerSortSettings.Instance.InventoryOptions)
            {
                if (pair.Key < 0) continue;
                
                if (pair.Key >= retData.Length)
                {
                    LogCritical($"{pair.Value.Name}'s index of {pair.Key.ToString()} doesn't exist in retainer data.");
                    TreeRoot.Stop("Invalid index.");
                    return false;
                }

                if (!retData[pair.Key].Active)
                {
                    LogCritical($"{pair.Value.Name} isn't an active retainer!");
                    TreeRoot.Stop("Retainer inactive.");
                    return false;
                }
            }

            while (StillNeedsSorting())
            {
                var sortStatus = GetSortStatus();
                await DepositFromPlayer(sortStatus[-2]);
                await Coroutine.Sleep(250);
                foreach (int index in GetSortStatus().Keys.Where(x => x > -2))
                {
                    if (sortStatus[index].Count == 0) continue;

                    if (index == -1) await OrganizeSaddlebag();
                    else await OrganizeRetainer(index);
                    await Coroutine.Sleep(250);
                }

                await Coroutine.Sleep(500);
                await UpdateCachedInventories(retData);
            }

            await GeneralFunctions.ExitRetainer(true);
            
            TreeRoot.Stop("Done sorting inventories.");
            return false;
        }

        private static Dictionary<int, Dictionary<uint, int>> GetSortStatus()
        {
            var sortStatus = new Dictionary<int, Dictionary<uint, int>>();

            foreach (var pair in CachedInventoryByIndex)
            {
                sortStatus[pair.Key] = new Dictionary<uint, int>();
                foreach (uint itemId in pair.Value.ItemCounts.Keys)
                {
                    int belongingIndex = GetBelongingIndex(itemId);
                    if (belongingIndex == pair.Key || belongingIndex < -2) continue;

                    sortStatus[pair.Key][itemId] = belongingIndex;
                }
            }

            return sortStatus;
        }

        private static async Task DepositFromPlayer(Dictionary<uint, int> sortInfo)
        {
            var indexCounts = new Dictionary<int, int>();
            foreach (int index in sortInfo.Values)
            {
                if (indexCounts.ContainsKey(index))
                {
                    indexCounts[index]++;
                }
                else
                {
                    indexCounts.Add(index, 1);
                }
            }

            if (!indexCounts.Any()) return;

            foreach (int index in indexCounts.OrderBy(x => x.Value).Select(x => x.Key))
            {
                if (index <= -2) continue;

                if (index == -1)
                {
                    await OrganizeSaddlebag();
                }
                else
                {
                    await OrganizeRetainer(index);
                }
            }
        }

        private static bool MainInventoryAnyShouldBeDeposited(int index) => MainBagsFilledSlots.Any(x => GetBelongingIndex(x) == index);

        private static async Task OrganizeSaddlebag()
        {
            await GeneralFunctions.ExitRetainer(true);
            Log("Opening Saddlebag...");
            await InventoryBuddy.Instance.Open();

            if (SaddlebagFreeSlots() != 0 && MainInventoryAnyShouldBeDeposited(-1))
            {
                Log("Depositing items...");
                foreach (BagSlot bagSlot in MainBagsFilledSlots)
                {
                    if (SaddlebagFreeSlots() == 0) break;
                    if (BelongsInIndex(bagSlot, -1))
                    {
                        LogSuccess($"Moving {bagSlot.EnglishName} to Saddlebag.");
                        bagSlot.AddToSaddlebagQuantity(bagSlot.Count);
                        await Coroutine.Sleep(500);
                    }
                }
            }


            if (InventoryManager.FreeSlots != 0 && SaddlebagShouldAnyBeSorted())
            {
                Log("Retrieving items...");
                foreach (BagSlot bagSlot in InventoryManager.GetBagsByInventoryBagId(SaddlebagIds).SelectMany(x => x.FilledSlots))
                {
                    if (InventoryManager.FreeSlots == 0) break;
                    if (BelongsInDifferentIndex(bagSlot, -1))
                    {
                        LogSuccess($"Pulling {bagSlot.EnglishName} as it belongs elsewhere.");
                        bagSlot.RemoveFromSaddlebagQuantity(bagSlot.Count);
                        await Coroutine.Sleep(500);
                    }
                }
            }
            

            Log("Exiting Saddlebag...");
            InventoryBuddy.Instance.Close();
            await Coroutine.Wait(3000, () => !InventoryBuddy.Instance.IsOpen);
            await Coroutine.Sleep(250);
        }

        private static long SaddlebagFreeSlots() => InventoryManager.GetBagsByInventoryBagId(SaddlebagIds).Sum(x => x.FreeSlots);

        private static bool SaddlebagShouldAnyBeSorted()
        {
            return InventoryManager
                .GetBagsByInventoryBagId(SaddlebagIds)
                .SelectMany(x => x.FilledSlots)
                .Any(x => BelongsInDifferentIndex(x, -1));
        }

        private static async Task OrganizeRetainer(int index)
        {
            Log($"Opening retainer #{index}...");
            await RetainerRoutine.SelectRetainer(index);

            RetainerTasks.OpenInventory();

            await Coroutine.Wait(3000, RetainerTasks.IsInventoryOpen);

            if (RetainerFreeSlots() != 0 && MainInventoryAnyShouldBeDeposited(index))
            {
                Log("Depositing items...");
                foreach (BagSlot bagSlot in MainBagsFilledSlots)
                {
                    if (RetainerFreeSlots() == 0) break;
                    if (BelongsInIndex(bagSlot, index))
                    {
                        LogSuccess($"Moving {bagSlot.EnglishName} to retainer #{index.ToString()}");
                        bagSlot.RetainerEntrustQuantity(bagSlot.Count);
                        await Coroutine.Sleep(500);
                    }
                }
            }

            if (InventoryManager.FreeSlots != 0 && RetainerShouldAnyBeSorted(index))
            {
                Log("Retrieving items...");
                foreach (BagSlot bagSlot in InventoryManager.GetBagsByInventoryBagId(HelperFunctions.RetainerBagIds).SelectMany(x => x.FilledSlots))
                {
                    if (InventoryManager.FreeSlots == 0) break;
                    if (BelongsInDifferentIndex(bagSlot, index))
                    {
                        LogSuccess($"Pulling {bagSlot.EnglishName} as it belongs elsewhere.");
                        bagSlot.RetainerRetrieveQuantity(bagSlot.Count);
                        await Coroutine.Sleep(500);
                    }
                }
            }

            Log("Exiting retainer...");
            await GeneralFunctions.ExitRetainer();
            await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);
            await Coroutine.Sleep(500);
        }

        private static long RetainerFreeSlots() => InventoryManager.GetBagsByInventoryBagId(HelperFunctions.RetainerBagIds).Sum(x => x.FreeSlots);

        private static bool RetainerShouldAnyBeSorted(int index)
        {
            return InventoryManager
                .GetBagsByInventoryBagId(HelperFunctions.RetainerBagIds)
                .SelectMany(x => x.FilledSlots)
                .Any(x => BelongsInDifferentIndex(x, index));
        }

        private static async Task UpdateCachedInventories(RetainerInfo[] retData)
        {
            CachedInventoryByIndex.Clear();
            var saddleBagInv = await ItemFinder.GetCachedSaddlebagInventories();
            CachedInventoryByIndex[-1] = new CachedInventoryInfo(saddleBagInv);

            CachedInventoryByIndex[-2] = new CachedInventoryInfo(MainBagsFilledSlots);

            var retainerInventories = await ItemFinder.SafelyGetCachedRetainerInventories();

            for (int i = 0; i < retData.Length; i++)
            {
                RetainerInfo retInfo = retData[i];
                if (!retInfo.Active) continue;

                if (retainerInventories.TryGetValue(retInfo.Unique, out StoredRetainerInventory storedInventory))
                {
                    int index = GetRetainerIndexByUnique(retInfo.Unique, retData);
                    CachedInventoryByIndex[index] = new CachedInventoryInfo(storedInventory);
                }
            }
        }

        private static int GetRetainerIndexByUnique(ulong uniqueId, RetainerInfo[] retData)
        {
            for (int i = 0; i < retData.Length; i++)
            {
                if (retData[i].Unique == uniqueId) return i;
            }
            
            LogCritical($"Couldn't find retainer index for {uniqueId.ToString()}");
            TreeRoot.Stop("Unable to find retainer index.");
            return int.MinValue;
        }

        private static readonly Dictionary<int, CachedInventoryInfo> CachedInventoryByIndex = new Dictionary<int, CachedInventoryInfo>();
        
        private static readonly InventoryBagId[] SaddlebagIds =
        {
            (InventoryBagId) 0xFA0,(InventoryBagId) 0xFA1//, (InventoryBagId) 0x1004,(InventoryBagId) 0x1005 
        };

        private static int GetBelongingIndex(BagSlot slot)
        {
            foreach (var pair in AutoRetainerSortSettings.Instance.InventoryOptions)
            {
                if (pair.Value.Contains(slot)) return pair.Key;
            }

            return int.MinValue;
        }
        
        private static int GetBelongingIndex(uint itemId)
        {
            SortType sortType = GetSortTypeFromId(itemId);

            var belongingIndex = int.MinValue;
            foreach (var pair in AutoRetainerSortSettings.Instance.InventoryOptions)
            {
                if (pair.Value.Contains(itemId))
                {
                    belongingIndex = pair.Key;
                    break;
                }

                if (pair.Value.Contains(sortType))
                {
                    belongingIndex = pair.Key;
                }
            }

            return belongingIndex;
        }

        private static bool BelongsInIndex(BagSlot slot, int otherIndex) => GetBelongingIndex(slot) == otherIndex;

        private static bool BelongsInDifferentIndex(BagSlot slot, int currentIndex)
        {
            int belongingIndex = GetBelongingIndex(slot);
            if (belongingIndex == int.MinValue) return false;
            return belongingIndex != currentIndex;
        }
        
        private static bool BelongsInDifferentIndex(uint itemId, int currentIndex)
        {
            int belongingIndex = GetBelongingIndex(itemId);
            if (belongingIndex == int.MinValue) return false;
            return belongingIndex != currentIndex;
        }

        private static SortType GetSortTypeFromId(uint itemId)
        {
            const uint collectableOffset = 5000000;
            const uint highQualityOffset = 1000000;
            
            uint adjustedId = itemId;
            if (adjustedId > collectableOffset) adjustedId -= collectableOffset;
            else if (adjustedId > highQualityOffset) adjustedId -= highQualityOffset;
            
            return cachedSortTypes.ContainsKey(adjustedId) ? cachedSortTypes[adjustedId] : SortType.UNKNOWN;
        }

        private static Dictionary<uint, SortType> cachedSortTypes;

        private static void BuildItemSortTypeCache()
        {
            if (cachedSortTypes != null) return;

            cachedSortTypes = new Dictionary<uint, SortType>();

            foreach (Item itemInfo in DataManager.ItemCache.Values)
            {
                if (itemInfo.Id > 1000000) continue;
                if (cachedSortTypes.ContainsKey(itemInfo.Id)) continue;
                
                cachedSortTypes.Add(itemInfo.Id, itemInfo.EquipmentCatagory.GetSortType());
            }
        }

        private class CachedInventoryInfo
        {
            public readonly Dictionary<uint, int> ItemCounts;

            public int this[uint itemId] => ItemCounts[itemId];

            public CachedInventoryInfo(StoredRetainerInventory retInfo)
            {
                ItemCounts = retInfo.Inventory;
            }

            public CachedInventoryInfo(Dictionary<uint, ushort> saddlebagDic)
            {
                ItemCounts = new Dictionary<uint, int>();
                foreach (var pair in saddlebagDic)
                {
                    if (ItemCounts.ContainsKey(pair.Key))
                    {
                        ItemCounts[pair.Key] += pair.Value;
                    }
                    else
                    {
                        ItemCounts.Add(pair.Key, pair.Value);
                    }
                }
            }

            public CachedInventoryInfo(IEnumerable<BagSlot> bagSlots)
            {
                ItemCounts = new Dictionary<uint, int>();
                foreach (BagSlot slot in bagSlots)
                {
                    if (ItemCounts.ContainsKey(slot.TrueItemId))
                    {
                        ItemCounts[slot.TrueItemId] += (int)slot.Count;
                    }
                    else
                    {
                        ItemCounts.Add(slot.TrueItemId, (int)slot.Count);
                    }
                }
            }
        }
        
        private static readonly InventoryBagId[] MainBags = {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4,
        };
        
        public static IEnumerable<BagSlot> MainBagsFilledSlots => InventoryManager.GetBagsByInventoryBagId(MainBags).SelectMany(x => x.FilledSlots);

        private class ItemSortInfo
        {
            public readonly uint ItemId;

            public int CurrentIndex;

            private int? _belongingIndex;
            
            public int BelongingIndex
            {
                get
                {
                    if (_belongingIndex == null)
                    {
                        _belongingIndex = GetBelongingIndex(ItemId);
                    }
                    return _belongingIndex.Value;
                }
            }

            public ItemSortInfo(uint itemId, int currentIndex)
            {
                ItemId = itemId;
                CurrentIndex = currentIndex;
            }
        }

        public static async Task CombineStacks(IEnumerable<BagSlot> bagSlots)
        {
            var groupedSlots = bagSlots
                .Where(x => x.IsValid && x.IsFilled && x.Item.StackSize > 1)
                .GroupBy(x => x.RawItemId)
                .Where(x => x.Count() > 1);

            foreach (var slotGrouping in groupedSlots)
            {
                var bagSlotArray = slotGrouping.OrderByDescending(x => x.Count).ToArray();
                int moveToIndex = Array.FindIndex(bagSlotArray, x => x.Count < x.Item.StackSize);
                if (moveToIndex < 0) continue;
                for (int i = bagSlotArray.Length; i > moveToIndex; i--)
                {
                    var moveFromSlot = bagSlotArray[i];
                    if (moveFromSlot == null || !moveFromSlot.IsValid || !moveFromSlot.IsFilled)
                    {
                        continue;
                    }

                    uint curCount = bagSlotArray[moveToIndex].Count;
                    bool result = moveFromSlot.Move(bagSlotArray[moveToIndex]);
                    if (result)
                    {
                        await Coroutine.Wait(3000, () => curCount != bagSlotArray[moveToIndex].Count);
                    }

                    await Coroutine.Yield();

                    BagSlot curMoveTo = bagSlotArray[moveToIndex];
                    if (curMoveTo.Count >= curMoveTo.Item.StackSize)
                    {
                        moveToIndex = Array.FindIndex(bagSlotArray, x => x.IsValid && x.IsFilled && x.Count < x.Item.StackSize);
                    }
                }
            }
        }

        public override void Stop()
        {
            _root = null;
        }

        public static void Log(string text)
        {
            Logging.Write(Colors.Orange, "[AutoRetainerSort] " + text);
        }
        
        public static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, "[AutoRetainerSort] " + text);
        }

        public static void LogSuccess(string text)
        {
            Logging.Write(Colors.Green, "[AutoRetainerSort] " + text);
        }
    }
}