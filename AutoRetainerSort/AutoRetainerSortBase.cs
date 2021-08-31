using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using LlamaLibrary.AutoRetainerSort.Classes;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.RetainerItemFinder;
using LlamaLibrary.Retainers;
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

        public override async void OnButtonPress()
        {
            HelperFunctions.ForceGetRetainerDataSync();
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

        private async Task<bool> Run()
        {
            if (!ItemSortStatus.AnyRulesExist())
            {
                LogCritical("You don't have any sorting rules set up... maybe go hit the Auto-Setup button?");
                TreeRoot.Stop("No sort settings.");
                return false;
            }
            
            LogCritical($"The journey begins! {Strings.AutoSetup_CacheAdvice}");
            await GeneralFunctions.StopBusy(true, true, false);

            var retData = await HelperFunctions.GetOrderedRetainerArray(true);
            if (retData.Length == 0)
            {
                LogCritical("No retainers. Exiting.");
                TreeRoot.Stop("No retainer data found.");
                return false;
            }

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

            await ItemFinder.FlashSaddlebags();

            ItemSortStatus.UpdateFromCache(retData);

            if (AutoRetainerSortSettings.Instance.PrintMoves)
            {
                alreadyPrintedUniques.Clear();
                foreach (CachedInventory cachedInventory in ItemSortStatus.GetAllInventories())
                {
                    PrintMoves(cachedInventory.Index);
                }
            }

            while (ItemSortStatus.AnyUnsorted())
            {
                if (ItemSortStatus.FilledAndSortedInventories.Contains(ItemSortStatus.PlayerInventoryIndex))
                {
                    LogCritical("Everything currently in the player's inventory belongs there, but it's full! Can't move items like this. I quit.");
                    break;
                }

                await DepositFromPlayer();
                await RetrieveFromInventories();

                await Coroutine.Sleep(250);
                ItemSortStatus.UpdateFromCache(retData);
                await Coroutine.Sleep(250);
            }

            foreach (CachedInventory cachedInventory in ItemSortStatus.GetAllInventories())
            {
                foreach (ItemSortInfo sortInfo in cachedInventory.ItemCounts.Select(x => ItemSortStatus.GetSortInfo(x.Key)))
                {
                    int[] localIndexCache = sortInfo.MatchingIndexes.ToArray();
                    if (localIndexCache.Length == 0) continue;
                    if (sortInfo.SortStatus(cachedInventory.Index) == SortStatus.MoveButUnable)
                    {
                        if (cachedInventory.FreeSlots == 0)
                        {
                            LogCritical($"We want to move {sortInfo.Name} to {ItemSortStatus.GetByIndex(localIndexCache[0]).Name} but it's full and everything there belongs. Too bad!");
                        }
                        else if (sortInfo.ItemInfo.Unique)
                        {
                            if (localIndexCache.Length == 1)
                            {
                                LogCritical($"We want to move {sortInfo.Name} to {ItemSortStatus.GetByIndex(localIndexCache[0]).Name} but it's unique and that inventory already has one. Too bad!");
                            }
                            else
                            {
                                LogCritical($"We want to move {sortInfo.Name} but it's unique and all inventories set for it already have one. Too bad!");
                            }
                        }
                        else
                        {
                            LogCritical($"We want to move {sortInfo.Name} to {ItemSortStatus.GetByIndex(localIndexCache[0]).Name} but it can't be moved there. Too bad!");
                        }
                    }
                }
            }

            await GeneralFunctions.ExitRetainer(true);

            if (AutoRetainerSortSettings.Instance.AutoGenLisbeth)
            {
                string lisbethSettingsPath = LisbethRuleGenerator.GetSettingsPath();
                if (!string.IsNullOrEmpty(lisbethSettingsPath))
                {
                    LisbethRuleGenerator.PopulateSettings(lisbethSettingsPath);
                    LogSuccess("Auto-populated Lisbeth's retainer item rules!");
                    MessageBox.Show(
                        Strings.LisbethRules_RestartRB,
                        "Just Letting You Know...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    LogCritical("Couldn't find Lisbeth settings path! We won't auto-generate retainer rules.");
                }
            }

            TreeRoot.Stop("Done sorting inventories.");
            return false;
        }

        private static readonly HashSet<uint> alreadyPrintedUniques = new HashSet<uint>();

        private static void PrintMoves(int index)
        {
            foreach (ItemSortInfo sortInfo in ItemSortStatus.GetByIndex(index).ItemCounts.Select(x => ItemSortStatus.GetSortInfo(x.Key)))
            {
                if (sortInfo.SortStatus(index) != SortStatus.Move) continue;
                if (alreadyPrintedUniques.Contains(sortInfo.TrueItemId)) continue;

                if (sortInfo.ItemInfo.Unique)
                {
                    alreadyPrintedUniques.Add(sortInfo.TrueItemId);
                }

                int[] localIndexCache = sortInfo.MatchingIndexes.ToArray();
                StringBuilder sb = new StringBuilder();
                sb.Append($"We want to move {sortInfo.Name} from {ItemSortStatus.GetByIndex(index).Name}");
                bool isFull = localIndexCache.All(x => ItemSortStatus.FilledAndSortedInventories.Contains(x));
                if (sortInfo.ItemInfo.Unique)
                {
                    var uniqueNoSpace = true;
                    for (int i = 0; i < localIndexCache.Length; i++)
                    {
                        if (ItemSortStatus.GetByIndex(localIndexCache[i]).ItemCounts.ContainsKey(sortInfo.TrueItemId)) continue;
                        sb.Append($" to {ItemSortStatus.GetByIndex(localIndexCache[i]).Name}");
                        uniqueNoSpace = false;
                        break;
                    }
                    isFull = uniqueNoSpace;
                }
                else
                {
                    sb.Append($" to {ItemSortStatus.GetByIndex(localIndexCache[0]).Name}");
                }

                if (sortInfo.ItemInfo.Unique && isFull)
                {
                    sb.Append("... but it's unique and there's no available space.");
                }
                else if (isFull)
                {
                    sb.Append("... but it's full.");
                }
                else
                {
                    sb.Append(".");
                }

                if (isFull)
                {
                    LogCritical(sb.ToString());
                }
                else
                {
                    Log(sb.ToString());
                }
            }
        }

        private static async Task DepositFromPlayer()
        {
            if (ItemSortStatus.PlayerInventory.AllBelong())
            {
                return;
            }

            var orderedSortStatusCounts = ItemSortStatus.PlayerInventory.DestinationCountsByIndex()
                .OrderByDescending(x => x.Value);

            foreach (var indexCountPair in orderedSortStatusCounts)
            {
                if (ItemSortStatus.GetByIndex(indexCountPair.Key).FreeSlots == 0) continue;
                await SortLoop(indexCountPair.Key);
            }
        }

        private static async Task RetrieveFromInventories()
        {
            foreach (CachedInventory cachedInventory in ItemSortStatus.GetAllInventories())
            {
                if (cachedInventory.Index <= ItemSortStatus.PlayerInventoryIndex) continue;
                if (cachedInventory.AllBelong()) continue;
                await SortLoop(cachedInventory.Index);
            }
        }

        private static async Task SortLoop(int index)
        {
            if (index < ItemSortStatus.PlayerInventoryIndex)
            {
                LogCritical($"Tried to sort index of #{index.ToString()} but that's out of range...");
                return;
            }

            if (index < ItemSortStatus.SaddlebagInventoryIndex)
            {
                LogCritical($"Tried to sort the player's inventory, but we can't do anything with that alone...");
                return;
            }

            bool openingSaddlebag = index == ItemSortStatus.SaddlebagInventoryIndex;
            await GeneralFunctions.ExitRetainer(openingSaddlebag);

            if (openingSaddlebag)
            {
                await InventoryBuddy.Instance.Open();

                if (!InventoryBuddy.Instance.IsOpen)
                {
                    LogCritical($"We were unable to open the saddlebag!");
                    return;
                }
            }
            else
            {
                await RetainerRoutine.SelectRetainer(index);
                RetainerTasks.OpenInventory();
                await Coroutine.Wait(3000, RetainerTasks.IsInventoryOpen);

                if (!RetainerTasks.IsInventoryOpen())
                {
                    LogCritical($"We were unable to open Retainer #{index.ToString()}!");
                    return;
                }
            }

            await Coroutine.Sleep(200);

            await CombineStacks(GeneralFunctions.MainBagsFilledSlots());
            await CombineStacks(InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)).SelectMany(x => x.FilledSlots));

            while (ShouldSortLoop(index))
            {
                bool depositResult = await DepositLoop(index);

                UpdatePlayerInventory();
                UpdateOpenedInventory(index);

                bool retrieveResult = await RetrieveLoop(index);

                UpdatePlayerInventory();
                UpdateOpenedInventory(index);

                await Coroutine.Sleep(250);

                if (!depositResult || !retrieveResult) break;
            }

            if (openingSaddlebag)
            {
                InventoryBuddy.Instance.Close();
            }
            else
            {
                await GeneralFunctions.ExitRetainer();
            }

            await Coroutine.Sleep(250);
        }

        private static void UpdatePlayerInventory()
        {
            ItemSortStatus.UpdateIndex(ItemSortStatus.PlayerInventoryIndex, InventoryManager.GetBagsByInventoryBagId(GeneralFunctions.MainBags));
        }

        private static void UpdateOpenedInventory(int index)
        {
            ItemSortStatus.UpdateIndex(index, InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)));
        }

        private static bool ShouldSortLoop(int index)
        {
            if (ItemSortStatus.FilledAndSortedInventories.Contains(index)) return false;
            if (ItemSortStatus.FilledAndSortedInventories.Contains(ItemSortStatus.PlayerInventoryIndex)) return false;
            if (ItemSortStatus.PlayerInventory.DestinationCountsByIndex().ContainsKey(index)) return true;

            return !ItemSortStatus.GetByIndex(index).AllBelong();
        }

        private static async Task<bool> DepositLoop(int index)
        {
            if (ItemSortStatus.GetByIndex(ItemSortStatus.PlayerInventoryIndex).AllBelong()) return true;

            string name = ItemSortStatus.GetByIndex(index).Name;
            if (BagsFreeSlotCount(index) == 0)
            {
                LogCritical($"We tried depositing to {name} but their inventory was full!");
                return false;
            }
            Log($"Depositing items to {name}...");
            foreach (BagSlot bagSlot in GeneralFunctions.MainBagsFilledSlots())
            {
                if (BagsFreeSlotCount(index) == 0) break;
                var sortInfo = ItemSortStatus.GetSortInfo(bagSlot.TrueItemId);
                if (sortInfo.SortStatus(index) == SortStatus.BelongsInIndex)
                {
                    bool moveResult;
                    if (index == ItemSortStatus.SaddlebagInventoryIndex)
                    {
                        moveResult = await bagSlot.TryAddToSaddlebag(bagSlot.Count);
                    }
                    else
                    {
                        moveResult = await bagSlot.TryEntrustToRetainer(bagSlot.Count);
                    }

                    if (moveResult)
                    {
                        LogSuccess($"Deposited {sortInfo.Name}.");
                        if (sortInfo.ItemInfo.Unique)
                        {
                            ItemSortStatus.PlayerInventoryUniques.Remove(sortInfo.TrueItemId);
                            ItemSortStatus.TryingToMoveUniques.Remove(sortInfo.TrueItemId);
                        }
                    }
                    else
                    {
                        LogCritical($"Something went wrong with depositing {sortInfo.Name}, it's still in the same slot!");
                    }
                }
            }

            return true;
        }

        private static async Task<bool> RetrieveLoop(int index)
        {
            if (ItemSortStatus.GetByIndex(index).AllBelong()) return true;

            string name = ItemSortStatus.GetByIndex(index).Name;
            if (InventoryManager.FreeSlots == 0)
            {
                LogCritical($"We tried to retrieve items from {name} but our player inventory is full!");
                return false;
            }

            Log($"Retrieving items from {name}...");
            foreach (BagSlot bagSlot in InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)).SelectMany(x => x.FilledSlots))
            {
                if (InventoryManager.FreeSlots == 0) break;
                var sortInfo = ItemSortStatus.GetSortInfo(bagSlot.TrueItemId);
                if (sortInfo.ItemInfo.Unique && InventoryManager.FilledSlots.Any(x => x.TrueItemId == sortInfo.TrueItemId)) continue;
                if (sortInfo.SortStatus(index) == SortStatus.Move)
                {
                    bool moveResult;
                    if (index == ItemSortStatus.SaddlebagInventoryIndex)
                    {
                        moveResult = await bagSlot.TryRemoveFromSaddlebag(bagSlot.Count);
                    }
                    else
                    {
                        moveResult = await bagSlot.TryRetrieveFromRetainer(bagSlot.Count);
                    }

                    if (moveResult)
                    {
                        string belongsInName = ItemSortStatus.GetByIndex(sortInfo.MatchingIndexes[0]).Name;
                        if (sortInfo.ItemInfo.Unique)
                        {
                            var localIndexCache = sortInfo.MatchingIndexes.ToArray();
                            for (int i = 0; i < localIndexCache.Length; i++)
                            {
                                var cachedInventory = ItemSortStatus.GetByIndex(localIndexCache[i]);
                                if (cachedInventory.ItemCounts.ContainsKey(sortInfo.TrueItemId)) continue;
                                belongsInName = cachedInventory.Name;
                                break;
                            }
                        }
                        LogSuccess($"Retrieved {sortInfo.Name}. It belongs in {belongsInName}.");
                        if (sortInfo.ItemInfo.Unique)
                        {
                            ItemSortStatus.PlayerInventoryUniques.Add(sortInfo.TrueItemId);
                            ItemSortStatus.TryingToMoveUniques.Remove(sortInfo.TrueItemId);
                        }
                    }
                    else
                    {
                        LogCritical($"Something went wrong with retrieving {sortInfo.Name}, it's still in the same slot!");
                    }
                }
            }

            return true;
        }

        private static InventoryBagId[] BagIdsByIndex(int index)
        {
            switch (index)
            {
                case ItemSortStatus.PlayerInventoryIndex:
                    return GeneralFunctions.MainBags;
                case ItemSortStatus.SaddlebagInventoryIndex:
                    return GeneralFunctions.SaddlebagIds;
                default:
                    return HelperFunctions.RetainerBagIds;
            }
        }

        private static int BagsFreeSlotCount(int index) => (int)InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)).Sum(x => x.FreeSlots);

        public static async Task CombineStacks(IEnumerable<BagSlot> bagSlots)
        {
            var groupedSlots = bagSlots
                .Where(x => x.IsValid && x.IsFilled && x.Item.StackSize > 1)
                .GroupBy(x => x.TrueItemId)
                .Where(x => x.Count(slot => slot.Count < slot.Item.StackSize) > 1);

            foreach (var slotGrouping in groupedSlots)
            {
                if (slotGrouping.Key > ItemSortInfo.CollectableOffset) continue;
                LogSuccess($"Combining stacks of {ItemSortStatus.GetSortInfo(slotGrouping.Key).Name}");

                var bagSlotArray = slotGrouping.OrderByDescending(x => x.Count).ToArray();
                int moveToIndex = Array.FindIndex(bagSlotArray, x => x.Count < x.Item.StackSize);
                if (moveToIndex < 0) continue;
                for (int i = bagSlotArray.Length - 1; i > moveToIndex; i--)
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