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
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;
using static LlamaLibrary.Retainers.HelperFunctions;

namespace LlamaLibrary.Retainers
{
    public class Retainers : BotBase
    {
        private static readonly string botName = "Retainers Organize";

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

        public Retainers()
        {
            OffsetManager.Init();
            //Task.Factory.StartNew(() =>
           // {
                init();
                _init = true;
                Log("INIT DONE");
          //  });
        }

        public override string Name
        {
            get
            {
#if RB_CN
                return "雇员整理";
#else
                return botName;
#endif
            }
        }

        public override bool WantButton => true;

        public override string EnglishName => "Retainers Organize";

        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        internal static Lazy<List<RetainerTaskData>> VentureData;
        private volatile bool _init;
        private int ventures;

        internal void init()
        {
            Log("Load venture.json");
            VentureData = new Lazy<List<RetainerTaskData>>(() =>loadResource<List<RetainerTaskData>>(Resources.Ventures));
            Log("Loaded venture.json");
        }

        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public override void Initialize()
        {
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
            
            var count = await HelperFunctions.GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);
            
           
            //var retainerIndex = 0;

            //Settings variables
            debug = RetainerSettings.Instance.DebugLogging;
            var bell = await GoToSummoningBell();

            if (bell == false)
            {
                LogCritical("No summoning bell near by");
                TreeRoot.Stop("Done playing with retainers");
                return false;
            }
            await UseSummoningBell();
            await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't Open Bell");
                TreeRoot.Stop("Done playing with retainers");
                return false;
            }

            if (SelectString.IsOpen)
            {
                await RetainerRoutine.DeSelectRetainer();
            }
            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i=> i.Active).ToArray();
            var numRetainers = ordered.Count(); //GetNumberOfRetainers();

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

            ventures = RetainerList.Instance.NumberOfVentures;
            
            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                if (!retainerNames.ContainsKey(retainerIndex)) retainerNames.Add(retainerIndex, RetainerList.Instance.RetainerName(retainerIndex));
                bool hasJob = RetainerList.Instance.RetainerHasJob(retainerIndex);
                Log($"Selecting {RetainerList.Instance.RetainerName(retainerIndex)}");
                await RetainerRoutine.SelectRetainer(retainerIndex);

                var inventory = new RetainerInventory();

                if (RetainerSettings.Instance.GetGil)
                    GetRetainerGil();

                LogVerbose("Inventory open");
                foreach (var item in InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Where(FilterStackable))
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

                LogVerbose("Inventory done");

                Log("Checking retainer[{0}] against player inventory", retainerNames[retainerIndex]);

                if (RetainerSettings.Instance.DepositFromPlayer) await RetainerRoutine.DumpItems();

                Log("Done checking against player inventory");

                if (RetainerSettings.Instance.ReassignVentures && (ordered[retainerIndex].Job != ClassJobType.Adventurer) && ventures > 2 && (ordered[retainerIndex].VentureEndTimestamp - UnixTimestamp) <=0)
                {
                    Log("Checking Ventures");
                    await CheckVentures();
                }
                else if ((ordered[retainerIndex].VentureEndTimestamp - UnixTimestamp) > 0)
                {
                    Log($"Venture will be done in {(ordered[retainerIndex].VentureEndTimestamp - UnixTimestamp)/60} minutes");
                }
                else
                {
                    Log("Retainer has no job");
                }

                await RetainerRoutine.DeSelectRetainer();

                LogVerbose("Should be back at retainer list by now");

                // await Coroutine.Sleep(200);
                // }

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
                else if (numOfMoves < InventoryManager.FreeSlots - 1)
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
                        //await Coroutine.Sleep(1000);
                    }

                    if (!RetainerList.Instance.IsOpen) Log("Failed opening retainer list");

                    LogVerbose("Open:" + RetainerList.Instance.IsOpen);

                    await RetainerList.Instance.SelectRetainer(retainerIndex);

                    Log($"Selected Retainer: {retainerNames[retainerIndex]}");

                    await Coroutine.Wait(5000, () => RetainerTasks.IsOpen);

                    RetainerTasks.OpenInventory();
                    //
                    await Coroutine.Wait(5000, RetainerTasks.IsInventoryOpen);

                    if (!RetainerTasks.IsInventoryOpen()) continue;
                    await Coroutine.Sleep(500);

                    Log("Checking retainer[{0}] against move list", retainerNames[retainerIndex]);

                    foreach (var item in moveFrom[retainerIndex])
                    {
                        if (!InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Any(i => i.TrueItemId == item)) continue;

                        Log("Moved: " + InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).First(i => i.TrueItemId == item)
                                .Move(InventoryManager.GetBagsByInventoryBagId(inventoryBagId_0).First(bag => bag.FreeSlots > 0).GetFirstFreeSlot()));
                        await Coroutine.Sleep(200);
                    }

                    Log("Done checking against player inventory");

                    RetainerTasks.CloseInventory();

                    await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);

                    RetainerTasks.CloseTasks();

                    await Coroutine.Wait(3000, () => DialogOpen);

                    if (DialogOpen) Next();

                    await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);

                    LogVerbose("Should be back at retainer list by now");
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
                Log($"Selecting {RetainerList.Instance.RetainerName(retainerIndex)}");
                await RetainerRoutine.SelectRetainer(retainerIndex);

                await RetainerRoutine.DumpItems();

                await RetainerRoutine.DeSelectRetainer();
                Log($"Done with {RetainerList.Instance.RetainerName(retainerIndex)}");
            }
            //   await RetainerRoutine.ReadRetainers(RetainerRoutine.DumpItems());

            LogVerbose("Closing Retainer List");

            RetainerList.Instance.Close();

            TreeRoot.Stop("Done playing with retainers");

            done = true;

            return true;
        }

        public async Task<bool> CheckVentures()
        {
            if (!SelectString.IsOpen)
            {
                return false;
            }

            if (SelectString.LineCount > 9)
            {
                if (SelectString.Lines().Contains(Translator.VentureCompleteText))
                {
                    Log("Venture Done");
                    SelectString.ClickLineEquals(Translator.VentureCompleteText);

                    await Coroutine.Wait(5000, () => RetainerTaskResult.IsOpen);

                    if (!RetainerTaskResult.IsOpen)
                    {
                        Log("RetainerTaskResult didn't open");
                        return false;
                    }

                    var taskId = AgentRetainerVenture.Instance.RetainerTask;

                    var task = VentureData.Value.FirstOrDefault(i => i.Id == taskId);

                    if (task != default(RetainerTaskData))
                    {
                        Log($"Finished Venture {task.Name}");
                        Log($"Reassigning Venture {task.Name}");
                    }
                    else
                    {
                        Log($"Finished Venture");
                        Log($"Reassigning Venture");
                    }

                    RetainerTaskResult.Reassign();

                    await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen);
                    if (!RetainerTaskAsk.IsOpen)
                    {
                        Log("RetainerTaskAsk didn't open");
                        return false;
                    }
                    await Coroutine.Wait(2000,RetainerTaskAskExtensions.CanAssign);
                    if (RetainerTaskAskExtensions.CanAssign())
                    {
                        RetainerTaskAsk.Confirm();
                        ventures -= task.VentureCost;
                        Log($"Should be down to {ventures} venture tokens");
                    }
                    else
                    {
                        Log($"RetainerTaskAsk Error: {RetainerTaskAskExtensions.GetErrorReason()}");
                        RetainerTaskAsk.Close();
                    }

                    await Coroutine.Wait(1500, () => DialogOpen);
                    await Coroutine.Sleep(200);
                    if (DialogOpen) Next();
                    await Coroutine.Sleep(200);
                    await Coroutine.Wait(5000, () => SelectString.IsOpen);
                }
                else
                {
                    Log("Venture Not Done");
                }
            }
            else
            {
                Log("Venture Not Done");
            }

            return true;
        }
    }
}