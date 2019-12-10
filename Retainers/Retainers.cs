using System;
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
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;
using static LlamaLibrary.Retainers.HelperFunctions;

namespace LlamaLibrary.Retainers
{
    public class Retainers : BotBase
    {
        private static readonly string botName = "Retainers Test";

        private static bool done;

        private static readonly InventoryBagId[] inventoryBagId_0 = new InventoryBagId[6]
        {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4,
            InventoryBagId.Bag5,
            InventoryBagId.Bag6
        };

        private Composite _root;

        private bool debug;

        private SettingsForm settings;

        public override string Name
        {
            get
            {
#if RB_CN
                return "雇员";
#else
                return "Retainers Test";
#endif
            }
        }

        public override bool WantButton => true;

        public override string EnglishName => "Retainers Test";

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

            //Settings variables
            debug = RetainerSettings.Instance.DebugLogging;

            await UseSummoningBell();
            //await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
/*            while (RetainerList.Instance.IsOpen)
            {
                Log($"{Core.Memory.Read<uint>(RetainerList.Instance.WindowByName.Pointer + 0x180) & 0xF00000u}");
                await Coroutine.Sleep(50);
            }*/
            await Coroutine.Wait(5000, () => RetainerList.Instance.IsVisible());
            //Log("Visible:" + RetainerList.Instance.IsOpen);
            //await Coroutine.Sleep(1000);

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
                    //await Coroutine.Sleep(500);
                }

                if (!RetainerList.Instance.IsOpen) Log("Failed opening retainer list");

                LogVerbose("Open:" + RetainerList.Instance.IsOpen);

                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);

                //await Coroutine.Sleep(500);
                if (!retainerNames.ContainsKey(retainerIndex)) retainerNames.Add(retainerIndex, RetainerList.Instance.RetainerName(retainerIndex));
                await RetainerList.Instance.SelectRetainer(retainerIndex);
                //await Coroutine.Sleep(200);
                
                //await Coroutine.Wait(5000, () => RetainerTasks.IsOpen);

                Log("Selected Retainer: " + retainerNames[retainerIndex]);

                if (RetainerSettings.Instance.GetGil)
                    GetRetainerGil();

                RetainerTasks.OpenInventory();
                await Coroutine.Wait(5000, RetainerTasks.IsInventoryOpen);

                if (RetainerTasks.IsInventoryOpen())
                {
                    LogVerbose("Inventory open");
                    foreach (var retbag in InventoryManager.GetBagsByInventoryBagId(RetainerBagIds))
                    {
                        foreach (var item in retbag.FilledSlots.Where(FilterStackable))
                        {
                            try
                            {
                                inventory.AddItem(item);
                                if (masterInventory.ContainsKey(item.TrueItemId))
                                {
                                    masterInventory[item.TrueItemId]
                                        .Add(new KeyValuePair<int, uint>(retainerIndex, item.Count));
                                }
                                else
                                {
                                    masterInventory.Add(item.TrueItemId, new List<KeyValuePair<int, uint>>());
                                    masterInventory[item.TrueItemId]
                                        .Add(new KeyValuePair<int, uint>(retainerIndex, item.Count));
                                }
                            }
                            catch (Exception e)
                            {
                                LogCritical("SHIT:" + e);
                                throw;
                            }
                        }
                    }

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

                    Log("Done checking against player inventory");

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

                    RetainerTasks.CloseInventory();

                    //await Coroutine.Sleep(200);

                    await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);

                    //await Coroutine.Sleep(200);

                    RetainerTasks.CloseTasks();

                    await Coroutine.Wait(1500, () => DialogOpen);

                    await Coroutine.Sleep(200);

                    if (DialogOpen) Next();

                    //await Coroutine.Sleep(200);

                    await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);

                    LogVerbose("Should be back at retainer list by now");

                   // await Coroutine.Sleep(200);
                }

                retList.Add(inventory);
            }

            //await Coroutine.Sleep(1000);

            if (RetainerSettings.Instance.DontOrganizeRetainers || !RetainerSettings.Instance.DepositFromPlayer)
            {
                RetainerList.Instance.Close();

                TreeRoot.Stop("Done playing with retainers (Don't organize or don't deposit items.)");
                return true;
            }

            if (debug)
                foreach (var itemId in masterInventory)
                {
                    var retainers = "";

                    foreach (var retainerId in itemId.Value)
                    {
                        retainers += $"Retainer[{retainerNames[retainerId.Key]}] has {retainerId.Value} ";
                    }

                    Log("Item {0}: {1}", itemId.Key, retainers);
                }

            LogCritical("Duplicate items Found:");

            if (debug)
                foreach (var itemId in masterInventory.Where(r => r.Value.Count > 1))
                {
                    var retainers = "";
                    var retListInv = new List<KeyValuePair<int, uint>>(itemId.Value.OrderByDescending(r => r.Value));

                    foreach (var retainerId in retListInv)
                    {
                        retainers += $"Retainer[{retainerNames[retainerId.Key]}] has {retainerId.Value} ";
                    }

                    Log("Item {0}: {1}", itemId.Key, retainers);
                }

            /*
                 * Same as above but before the second foreach save retainer/count
                 * remove that one since it's where we're going to move stuff to
                 */
            var numOfMoves = 0;

            foreach (var itemId in masterInventory.Where(r => r.Value.Count > 1))
            {
                var retListInv = new List<KeyValuePair<int, uint>>(itemId.Value.OrderByDescending(r => r.Value));

                var retainerTemp = retListInv[0].Key;
                var countTemp = retListInv[0].Value;

                var retainers = "";

                retListInv.RemoveAt(0);

                foreach (var retainerId in retListInv)
                {
                    retainers += $"Retainer[{retainerNames[retainerId.Key]}] has {retainerId.Value} ";
                    countTemp += retainerId.Value;
                }

                Log($"Item: {DataManager.GetItem(NormalRawId(itemId.Key))} ({itemId.Key}) Total:{countTemp} should be in {retainerNames[retainerTemp]} and {retainers}");

                if (countTemp > 999)
                {
                    LogCritical("This item will have a stack size over 999: {0}", itemId.Key);
                }
                else
                {
                    numOfMoves++;
                    foreach (var retainerIdTemp in retListInv)
                    {
                        moveFrom[retainerIdTemp.Key].Add(itemId.Key);
                    }
                }
            }

            LogCritical("Looks like we need to do {0} moves", numOfMoves);

            if (numOfMoves < InventoryManager.FreeSlots && numOfMoves > 0)
            {
                LogCritical($"Looks like we have {InventoryManager.FreeSlots} free spaces in inventory so we can just dump into player inventory");

                //First loop
                for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
                {
                    var inventory = new RetainerInventory();

                    if (!RetainerList.Instance.IsOpen)
                    {
                        await UseSummoningBell();
                        await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
                        await Coroutine.Sleep(1000);
                    }

                    await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);

                    await Coroutine.Sleep(200);

                    if (!RetainerList.Instance.IsOpen) Log("Failed opening retainer list");

                    LogVerbose("Open:" + RetainerList.Instance.IsOpen);

                    await RetainerList.Instance.SelectRetainer(retainerIndex);

                    Log($"Selected Retainer: {retainerNames[retainerIndex]}");

                    await Coroutine.Wait(5000, () => RetainerTasks.IsOpen);

                    RetainerTasks.OpenInventory();

                    await Coroutine.Wait(5000, RetainerTasks.IsInventoryOpen);

                    if (!RetainerTasks.IsInventoryOpen()) continue;

                    LogVerbose("Inventory open");
                    foreach (var retbag in InventoryManager.GetBagsByInventoryBagId(RetainerBagIds))
                    {
                        foreach (var item in retbag.FilledSlots.Where(FilterStackable))
                        {
                            try
                            {
                                inventory.AddItem(item);
                                if (masterInventory.ContainsKey(item.TrueItemId))
                                {
                                    masterInventory[item.TrueItemId].Add(new KeyValuePair<int, uint>(retainerIndex, item.Count));
                                }
                                else
                                {
                                    masterInventory.Add(item.TrueItemId, new List<KeyValuePair<int, uint>>());
                                    masterInventory[item.TrueItemId].Add(new KeyValuePair<int, uint>(retainerIndex, item.Count));
                                }

                                //Logging.Write("Name: {0} Count: {1} BagId: {2} IsHQ: {3}", item.Item.EnglishName, item.Item.StackSize, item.BagId, item.Item.IsHighQuality);
                            }
                            catch (Exception e)
                            {
                                Log("SHIT:" + e);
                                throw;
                            }
                        }
                    }

                    LogVerbose("Inventory done");

                    Log("Checking retainer[{0}] against move list", retainerNames[retainerIndex]);

                    foreach (var item in moveFrom[retainerIndex])
                    {
                        var moved = false;
                        if (inventory.HasItem(item))
                            foreach (var bagId in InventoryManager.GetBagsByInventoryBagId(inventoryBagId_0))
                            {
                                if (moved)
                                    break;

                                foreach (var bagslot in bagId)
                                {
                                    if (!bagslot.IsFilled)
                                    {
                                        Log("Moved: " + inventory.GetItem(item).Move(bagslot));
                                        await Coroutine.Sleep(200);
                                        moved = true;
                                        break;
                                    }
                                }
                            }
                    }

                    Log("Done checking against player inventory");

                    RetainerTasks.CloseInventory();

                    await Coroutine.Sleep(500);

                    await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);

                    RetainerTasks.CloseTasks();

                    await Coroutine.Sleep(500);

                    await Coroutine.Wait(3000, () => DialogOpen);

                    if (DialogOpen) Next();

                    await Coroutine.Sleep(200);

                    await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);

                    LogVerbose("Should be back at retainer list by now");

                    //inventory.PrintList();
                }
            }
            else
            {
                if (numOfMoves <= 0)
                {
                    LogCritical("No duplicate stacks found so no moved needed.");
                    RetainerList.Instance.Close();

                    TreeRoot.Stop("Done playing with retainers");
                    return true;
                }

                LogCritical("Crap, we don't have enough player inventory to dump it all here");
                RetainerList.Instance.Close();

                TreeRoot.Stop("Done playing with retainers");
                return false;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                var inventory = new RetainerInventory();

                if (!RetainerList.Instance.IsOpen)
                {
                    await UseSummoningBell();
                    await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
                    await Coroutine.Sleep(1000);
                }

                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);

                await Coroutine.Sleep(1000);

                if (!RetainerList.Instance.IsOpen) Log("Failed opening retainer list");

                LogVerbose("Open:" + RetainerList.Instance.IsOpen);

                await RetainerList.Instance.SelectRetainer(retainerIndex);

                Log("Selected Retainer: " + retainerNames[retainerIndex]);

                await Coroutine.Wait(5000, () => RetainerTasks.IsOpen);

                RetainerTasks.OpenInventory();

                await Coroutine.Wait(5000, RetainerTasks.IsInventoryOpen);

                if (RetainerTasks.IsInventoryOpen())
                {
                    LogVerbose("Inventory open");
                    foreach (var retbag in InventoryManager.GetBagsByInventoryBagId(RetainerBagIds))
                    {
                        foreach (var item in retbag.FilledSlots.Where(FilterStackable))
                        {
                            try
                            {
                                inventory.AddItem(item);
                                if (masterInventory.ContainsKey(item.TrueItemId))
                                {
                                    masterInventory[item.TrueItemId]
                                        .Add(new KeyValuePair<int, uint>(retainerIndex, item.Count));
                                }
                                else
                                {
                                    masterInventory.Add(item.TrueItemId, new List<KeyValuePair<int, uint>>());
                                    masterInventory[item.TrueItemId]
                                        .Add(new KeyValuePair<int, uint>(retainerIndex, item.Count));
                                }
                            }
                            catch (Exception e)
                            {
                                Log("SHIT:" + e);
                                throw;
                            }
                        }
                    }

                    LogVerbose("Inventory done");

                    Log("Checking retainer[{0}] against player inventory", retainerNames[retainerIndex]);

                    foreach (var item in InventoryManager.FilledSlots.Where(x => x.BagId == InventoryBagId.Bag1 || x.BagId == InventoryBagId.Bag2 || x.BagId == InventoryBagId.Bag3 || x.BagId == InventoryBagId.Bag4)
                        .Where(FilterStackable))
                    {
                        if (inventory.HasItem(item.TrueItemId))
                        {
                            Log("BOTH PLAYER AND RETAINER HAVE Name: " + item.Item.EnglishName +
                                "\tItemCategory: " + item.Item.EquipmentCatagory + "\tId: " + item.Item.Id);

                            if (RetainerSettings.Instance.DepositFromPlayer)
                            {
                                Log("Moved: " + MoveItem(item,
                                        inventory.GetItem(item.TrueItemId)));
                                await Coroutine.Sleep(200);
                            }
                        }
                    }

                    Log("Done checking against player inventory");

                    RetainerTasks.CloseInventory();

                    await Coroutine.Sleep(500);

                    await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);

                    //await Coroutine.Sleep(1000);

                    //Call quit in tasks and get through dialog

                    RetainerTasks.CloseTasks();

                    await Coroutine.Sleep(500);

                    await Coroutine.Wait(3000, () => DialogOpen);

                    if (DialogOpen) Next();

                    await Coroutine.Sleep(200);

                    await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);

                    LogVerbose("Should be back at retainer list by now");

                    //inventory.PrintList();
                }

                retList.Add(inventory);
            }

            LogVerbose("Closing Retainer List");

            RetainerList.Instance.Close();

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