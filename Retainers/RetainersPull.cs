﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using LlamaLibrary.Enums;
using LlamaLibrary.Extensions;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;
using static LlamaLibrary.Retainers.HelperFunctions;

namespace LlamaLibrary.Retainers
{
    public class RetainersPull : BotBase
    {
        private static readonly string botName = "Retainers Pull";

        private static bool done;

        private static readonly InventoryBagId[] PlayerInventoryBags = new InventoryBagId[4]
        {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4
        };

        IEnumerable<Bag> PlayerInventory => InventoryManager.GetBagsByInventoryBagId(PlayerInventoryBagIds);

        private Composite _root;

        private bool debug;

        private SettingsForm settings;

        public override string Name
        {
            get
            {
#if RB_CN
                return "雇员拉";
#else
                return "Retainers Pull";
#endif
            }
        }

        public override bool WantButton => true;

        public override string EnglishName => "Retainers Pull";

        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override void Initialize()
        {
            OffsetManager.Init();
        }

        public override void OnButtonPress()
        {
            if (settings == null || settings.IsDisposed)
                settings = new SettingsForm();
            try
            {
                settings.Show();
                settings.Activate();
            }
            catch (ArgumentOutOfRangeException ee)
            {
            }
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + botName + "] " + text, args);
            Logging.Write(Colors.Green, msg);
        }

        private void LogVerbose(string text, params object[] args)
        {
            if (!debug)
                return;
            var msg = string.Format("[" + botName + "] " + text, args);
            Logging.WriteVerbose(msg);
        }

        private void LogCritical(string text, params object[] args)
        {
            var msg = string.Format("[" + botName + "] " + text, args);
            Logging.Write(Colors.OrangeRed, msg);
        }

        public override void Start()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            _root = new ActionRunCoroutine(r => RetainerTest());
            done = false;
        }

        /*The await sleeps shouldn't be necessary but if they aren't there the game crashes some times since
        it tries to send commands to a window that isn't open even though it reports it as open (guess it didn't load yet)*/

        private async Task<bool> RetainerTest()
        {
            if (done) return true;

            Log(" ");
            Log("==================================================");
            Log("====================Retainers=====================");
            Log("==================================================");
            Log(" ");

            //var retainerIndex = 0;
            Dictionary<MyItemRole, int> itemRoleCount = Enum.GetValues(typeof(MyItemRole)).Cast<MyItemRole>().ToDictionary(eItemUiCRole => eItemUiCRole, eItemUiCategory => 0);

            Dictionary<ItemUiCategory, int> itemUiCount = Enum.GetValues(typeof(ItemUiCategory)).Cast<ItemUiCategory>().ToDictionary(eItemUiCategory => eItemUiCategory, eItemUiCategory => 0);
            int count = 0;

            //Settings variables
            debug = RetainerSettings.Instance.DebugLogging;

            await UseSummoningBell();
            await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            await Coroutine.Sleep(1000);

            var numRetainers = RetainerList.Instance.NumberOfRetainers; //GetNumberOfRetainers();

            var retList = new List<RetainerInventory>();
            var moveToOrder = new List<KeyValuePair<uint, int>>();
            var masterInventory = new Dictionary<uint, List<KeyValuePair<int, uint>>>();

            var retainerNames = new Dictionary<int, string>();

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();

                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return true;
            }

            //Moves
            var moveFrom = new List<uint>[numRetainers];

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                moveFrom[retainerIndex] = new List<uint>();
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                var inventory = new RetainerInventory();

                if (!RetainerList.Instance.IsOpen)
                {
                    await UseSummoningBell();
                    await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
                    await Coroutine.Sleep(500);
                }

                if (!RetainerList.Instance.IsOpen) Log("Failed opening retainer list");

                LogVerbose("Open:" + RetainerList.Instance.IsOpen);

                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);

                await Coroutine.Sleep(500);
                if (!retainerNames.ContainsKey(retainerIndex)) retainerNames.Add(retainerIndex, RetainerList.Instance.RetainerName(retainerIndex));
                await RetainerList.Instance.SelectRetainer(retainerIndex);
                await Coroutine.Sleep(200);
                //await Coroutine.Wait(5000, () => RetainerTasks.IsOpen);

                Log("Selected Retainer: " + retainerNames[retainerIndex]);

                if (RetainerSettings.Instance.GetGil)
                    GetRetainerGil();

                RetainerTasks.OpenInventory();
                await Coroutine.Wait(5000, RetainerTasks.IsInventoryOpen);


/*                ItemUiCategory[] HousingItems = new[]
                {
                    ItemUiCategory.Roof, ItemUiCategory.Exterior_Wall, ItemUiCategory.Exterior_Wall, ItemUiCategory.Window, ItemUiCategory.Door, ItemUiCategory.Roof_Decoration, ItemUiCategory.Exterior_Wall_Decoration,
                    ItemUiCategory.Placard, ItemUiCategory.Fence, ItemUiCategory.Interior_Wall, ItemUiCategory.Flooring, ItemUiCategory.Ceiling_Light, ItemUiCategory.Outdoor_Furnishing, ItemUiCategory.Tabletop, ItemUiCategory.Wall_mounted,
                    ItemUiCategory.Rug, (ItemUiCategory) 95
                };
                ItemUiCategory[] categories = new[] {ItemUiCategory.Roof};*/
               

                if (RetainerTasks.IsInventoryOpen())
                {
                    LogVerbose("Inventory open");
                    foreach (var retbag in InventoryManager.GetBagsByInventoryBagId(RetainerBagIds))
                    {
                        foreach (var item in retbag.FilledSlots)
                        {
                            if (itemRoleCount.ContainsKey(item.Item.MyItemRole()))
                                itemRoleCount[item.Item.MyItemRole()]++;
                            else
                            {
                                itemRoleCount.Add(item.Item.MyItemRole(), 1);
                            }
                            
                            if (itemUiCount.ContainsKey(item.Item.EquipmentCatagory))
                                itemUiCount[item.Item.EquipmentCatagory]++;
                            else
                            {
                                itemUiCount.Add(item.Item.EquipmentCatagory, 1);
                            }

                            if (item.Item.MyItemRole() == MyItemRole.CraftingMaterial && item.Item.EquipmentCatagory == ItemUiCategory.Stone && item.Item.EnglishName.Contains("Ore"))
                            {
                                count++;
                                Log($"Match {item.Item.CurrentLocaleName}");
                            }
                        }

/*
                        foreach (var bagSlot in InventoryManager.FilledSlots.Where(i => i.EnglishName.Contains("Koppranickel Ore")))
                        {
                            Log(string.Format("Match {0} {2} {1:X}", bagSlot.Count, bagSlot.Pointer.ToInt64(), bagSlot.Slot));
                        }

                        foreach (var ptr in AgentModule.AgentPointers)
                        {
                            Log(string.Format("Agent {0:X}",  ptr.ToInt64()));
                        }
                        */
                        //foreach (var item in retbag.FilledSlots.Where(i => HousingItems.Contains(i.Item.EquipmentCatagory)))
                        if (RetainerSettings.Instance.RoleCheck)
                            foreach (var item in retbag.FilledSlots.Where(i => i.Item.MyItemRole() == RetainerSettings.Instance.ItemRoleToPull))
                            {
                                if (InventoryManager.FreeSlots <= 1) break;
                                Log($"Moving {item.Item.CurrentLocaleName}\t To Player");
                                item.Move(PlayerInventory.First(bag => bag.FreeSlots > 0).GetFirstFreeSlot());
                            }
                        
                        if (RetainerSettings.Instance.CategoryCheck)
                            foreach (var item in retbag.FilledSlots.Where(i => i.Item.EquipmentCatagory == RetainerSettings.Instance.ItemCategoryToPull))
                            {
                                if (InventoryManager.FreeSlots <= 1) break;
                                Log($"Moving {item.Item.CurrentLocaleName}\t To Player");
                                item.Move(PlayerInventory.First(bag => bag.FreeSlots > 0).GetFirstFreeSlot());
                            }
                    }

/*
                    LogVerbose("Inventory done");

                    Log("Checking retainer[{0}] against player inventory", retainerNames[retainerIndex]);

                    foreach (var item in InventoryManager.FilledSlots.Where(x => x.BagId == InventoryBagId.Bag1 || x.BagId == InventoryBagId.Bag2 || x.BagId == InventoryBagId.Bag3 || x.BagId == InventoryBagId.Bag4)
                        .Where(FilterStackable).Where(item => inventory.HasItem(item.TrueItemId)))
                    {
                        Log($"PLAYER AND RETAINER both have Name: {item.Item.CurrentLocaleName}\tId: {item.Item.Id}");

                        if (RetainerSettings.Instance.DepositFromPlayer)
                        {
                            Log("Moved: " + MoveItem(item, inventory.GetItem(item.TrueItemId)));
                            await Coroutine.Sleep(100);
                        }
                    }
                    */

                    Log("Done checking against player inventory");

                    /*
                    AgentModule.ToggleAgentInterfaceById(274);
                    await Coroutine.Sleep(200);
                    var cho1 = InventoryManager.GetBagByInventoryBagId((InventoryBagId)4000);
                    var cho2 = InventoryManager.GetBagByInventoryBagId((InventoryBagId)4001);
                    if (cho1 != null && cho2 != null)
                    {
                        var chocobags = (cho1.FilledSlots).Concat(cho2.FilledSlots);
                        foreach (var item in chocobags.Where(FilterStackable).Where(item => inventory.HasItem(item.TrueItemId)))
                        {
                            Log($"Chocobo AND RETAINER both have Name: {item.Item.CurrentLocaleName}\tId: {item.Item.Id}");
                            Log("Moved: " + MoveItem(item, inventory.GetItem(item.TrueItemId)));
                            await Coroutine.Sleep(100);
                        }
                    }
                    */

                    RetainerTasks.CloseInventory();

                    await Coroutine.Sleep(200);

                    await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);

                    await Coroutine.Sleep(200);

                    RetainerTasks.CloseTasks();

                    await Coroutine.Wait(1500, () => DialogOpen);

                    await Coroutine.Sleep(200);

                    if (DialogOpen) Next();

                    await Coroutine.Sleep(200);

                    await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);

                    LogVerbose("Should be back at retainer list by now");

                    await Coroutine.Sleep(200);
                }

                retList.Add(inventory);
            }

            //await Coroutine.Sleep(1000);


            LogVerbose("Closing Retainer List");
            
            RetainerList.Instance.Close();
            
            Log("\nItem Roles");

            foreach (var role in itemRoleCount)
            {
                Log($"{role.Key.ToString()} - {role.Value}");
            }
            
            Log("\nItem UiCategory");

            foreach (var role in itemUiCount)
            {
                Log($"{role.Key.ToString()} - {role.Value}");
            }
            
            Log($"Ore Count: \t{count}");

            TreeRoot.Stop("Done playing with retainers");

            done = true;

            return true;
        }

        private async Task<bool> UseSummoningBell()
        {
            var bell = NearestSummoningBell();

            if (bell == null)
            {
                LogCritical("No summoning bell near by");
                return false;
            }

            if (bell.Distance2D(Core.Me.Location) >= 3)
            {
                await MoveSummoningBell(bell.Location);
                if (bell.Distance2D(Core.Me.Location) >= 3) return false;
            }

            bell.Interact();
            // No need to wait on IsOpen when we already do it in the main task.
            await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            LogVerbose("Summoning Bell Used");

            return true;
        }

        private static async Task<bool> MoveSummoningBell(Vector3 loc)
        {
            var moving = MoveResult.GeneratingPath;
            while (!(moving == MoveResult.Done ||
                     moving == MoveResult.ReachedDestination ||
                     moving == MoveResult.Failed ||
                     moving == MoveResult.Failure ||
                     moving == MoveResult.PathGenerationFailed))
            {
                moving = Flightor.MoveTo(new FlyToParameters(loc));

                await Coroutine.Yield();
            }

            return true;
        }
    }
}