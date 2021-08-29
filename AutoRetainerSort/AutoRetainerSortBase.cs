using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            await ItemSortStatus.UpdateFromCache(retData);

            var loopCount = 1;
            while (ItemSortStatus.AnyUnsorted())
            {
                if (ItemSortStatus.FilledAndSortedInventories.Contains(ItemSortStatus.PlayerInventoryIndex))
                {
                    LogCritical("Everything currently in the player's inventory belongs there, but it's full! Can't move items like this. I quit.");
                    break;
                }

                await DepositFromPlayer();
                await RetrieveFromInventories();

                await Coroutine.Sleep(500);
                await ItemSortStatus.UpdateFromCache(retData);

                if (ItemSortStatus.AnyUnsorted())
                {
                    LogSuccess($"Loop #{loopCount.ToString()} done but we still have more to sort.");
                    loopCount++;
                }
            }

            await GeneralFunctions.ExitRetainer(true);
            
            TreeRoot.Stop("Done sorting inventories.");
            return false;
        }

        private static void PrintMoves(int index)
        {
            foreach (ItemSortInfo sortInfo in ItemSortStatus.GetByIndex(index).ItemSlotCounts.Select(x => ItemSortStatus.GetSortInfo(x.Key)))
            {
                if (sortInfo.BelongsInIndex(index)) continue;

                StringBuilder sb = new StringBuilder();
                sb.Append($"We want to move {sortInfo.Name} to {ItemSortStatus.GetByIndex(sortInfo.MatchingIndex).Name}");
                bool isFull = ItemSortStatus.FilledAndSortedInventories.Contains(sortInfo.MatchingIndex);
                sb.Append(isFull ? "... but it's full." : ".");
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
            if (ItemSortStatus.PlayerInventory.IsSorted()) return;

            foreach (var indexCountPair in ItemSortStatus.PlayerInventory.SortStatusCounts().OrderByDescending(x => x.Value))
            {
                if (ItemSortStatus.GetByIndex(indexCountPair.Key).FreeSlotCount() == 0) continue;
                await SortLoop(indexCountPair.Key);
            }
        }

        private static async Task RetrieveFromInventories()
        {
            foreach (CachedInventory cachedInventory in ItemSortStatus.GetAllInventories())
            {
                if (cachedInventory.Index <= ItemSortStatus.PlayerInventoryIndex) continue;
                if (cachedInventory.IsSorted()) continue;
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
            
            PrintMoves(index);

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
            
            while (!ItemSortStatus.GetByIndex(index).IsSorted() && !ItemSortStatus.FilledAndSortedInventories.Contains(index) && InventoryManager.FreeSlots > 0)
            {
                await CombineStacks(GeneralFunctions.MainBagsFilledSlots());
                await CombineStacks(InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)).SelectMany(x => x.FilledSlots));
                
                await DepositLoop(index);
                await RetrieveLoop(index);
                
                ItemSortStatus.UpdateIndex(ItemSortStatus.PlayerInventoryIndex, GeneralFunctions.MainBagsFilledSlots());
                ItemSortStatus.UpdateIndex(index, InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)).SelectMany(x => x.FilledSlots));
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

        private static async Task DepositLoop(int index)
        {
            if (ItemSortStatus.GetByIndex(ItemSortStatus.PlayerInventoryIndex).IsSorted()) return;

            string name = ItemSortStatus.GetByIndex(index).Name;
            if (BagsFreeSlotCount(index) == 0)
            {
                LogCritical($"We tried depositing to {name} but their inventory was full!");
                return;
            }
            Log($"Depositing items to {name}...");
            foreach (BagSlot bagSlot in GeneralFunctions.MainBagsFilledSlots())
            {
                if (BagsFreeSlotCount(index) == 0) break;
                if (ItemSortStatus.GetSortInfo(bagSlot.TrueItemId).BelongsInIndex(index))
                {
                    LogSuccess($"Moving {bagSlot.Name} to {name}.");
                    if (index == ItemSortStatus.SaddlebagInventoryIndex)
                    {
                        bagSlot.AddToSaddlebagQuantity(bagSlot.Count);
                    }
                    else
                    {
                        bagSlot.RetainerEntrustQuantity(bagSlot.Count);
                    }
                    await Coroutine.Sleep(500);
                }
            }
        }

        private static async Task RetrieveLoop(int index)
        {
            if (ItemSortStatus.GetByIndex(index).IsSorted()) return;
            
            string name = ItemSortStatus.GetByIndex(index).Name;
            if (InventoryManager.FreeSlots == 0)
            {
                LogCritical($"We tried to retrieve items from {name} but our player inventory is full!");
                return;
            }

            Log($"Retrieving items from {name}...");
            foreach (BagSlot bagSlot in InventoryManager.GetBagsByInventoryBagId(BagIdsByIndex(index)).SelectMany(x => x.FilledSlots))
            {
                if (InventoryManager.FreeSlots == 0) break;
                if (ItemSortStatus.GetSortInfo(bagSlot.TrueItemId).BelongsInIndex(index))
                {
                    LogSuccess($"Pulling {bagSlot.Name} as it belongs in {name}.");
                    if (index == ItemSortStatus.SaddlebagInventoryIndex)
                    {
                        bagSlot.RemoveFromSaddlebagQuantity(bagSlot.Count);
                    }
                    else
                    {
                        bagSlot.RetainerRetrieveQuantity(bagSlot.Count);
                    }
                    await Coroutine.Sleep(500);
                }
            }
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
                .Where(x => x.Count() > 1);

            foreach (var slotGrouping in groupedSlots)
            {
                if (slotGrouping.Key > ItemSortInfo.CollectableOffset) continue;
                LogSuccess($"Combining stacks of {ItemSortStatus.GetSortInfo(slotGrouping.Key).Name}");
                
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