using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using Generate;
using LlamaLibrary.Enums;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;

//using UI_Checker;

namespace MasterPieceSupplyTest
{
    public class MasterPieceSupplyTester : BotBase
    {
        private Composite _root;
        public override string Name => "GCDailyLisbeth";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        private async Task<bool> Run()
        {
            //await PrintMasterPieceList();
            await DoGCDailyTurnins();

            TreeRoot.Stop("Stop Requested");
            return true;
        }

        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
        }

        public async Task DoGCDailyTurnins()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

            var items = Core.Memory.ReadArray<GCTurninItem>(Offsets.GCTurnin, Offsets.GCTurninCount);

            if (!items.Any(i => i.CanHandin))
            {
                Log("All done.");
                return;
            }

            string lisbethOrder = await GetGCSupplyList();
            
            if (lisbethOrder == "")
            {
                Log("All done.");
                return;
            }
            //Log(lisbethOrder);
            Log("Calling lisbeth");
            await Lisbeth.ExecuteOrders(lisbethOrder);
            Log("Lisbeth order should be done");

            if (!GrandCompanySupplyList.Instance.IsOpen)
            {
                await GrandCompanyHelper.InteractWithNpc(GCNpc.Personnel_Officer);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                if (!SelectString.IsOpen)
                {
                    Log("Window is not open...maybe it didn't get to npc?");
                    return;
                }

                SelectString.ClickSlot(0);
                await Coroutine.Wait(5000, () => GrandCompanySupplyList.Instance.IsOpen);
                if (!GrandCompanySupplyList.Instance.IsOpen)
                {
                    Log("Window is not open...maybe it didn't get to npc?");
                    return;
                }
            }

            if (GrandCompanySupplyList.Instance.IsOpen)
            {
                await GrandCompanySupplyList.Instance.SwitchToSupply();

                await HandleCurrentGCWindow();

                await GrandCompanySupplyList.Instance.SwitchToProvisioning();

                await HandleCurrentGCWindow();

                GrandCompanySupplyList.Instance.Close();
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                if (SelectString.IsOpen)
                {
                    SelectString.ClickSlot((uint) (SelectString.LineCount - 1));
                }
            }
        }

        private async Task HandleCurrentGCWindow()
        {
            var bools = GrandCompanySupplyList.Instance.GetTurninBools();
            var windowItemIds = GrandCompanySupplyList.Instance.GetTurninItemsIds();
            var required = GrandCompanySupplyList.Instance.GetTurninRequired();
            var maxSeals = Core.Me.MaxGCSeals();
            var items = Core.Memory.ReadArray<GCTurninItem>(Offsets.GCTurnin, Offsets.GCTurninCount);
            for (var index = 0; index < bools.Length; index++)
            {
                if (!bools[index])
                {
                    continue;
                }

                var item = items.FirstOrDefault(j => j.ItemID == windowItemIds[index]);
                var index1 = index;
                var handover = InventoryManager.FilledSlots.Where(k => k.RawItemId == item.ItemID && k.Count >= required[index1]).OrderByDescending(k => k.HqFlag).FirstOrDefault();
                if (handover == default(BagSlot)) continue;
                Log($"{handover.Name} {handover.IsHighQuality}");
                if (handover.IsHighQuality)
                {
                    if (Core.Me.GCSeals() + (item.Seals * 2) < maxSeals)
                    {
                        GrandCompanySupplyList.Instance.ClickItem(index);
                        await Coroutine.Wait(5000, () => Request.IsOpen);
                        if (Request.IsOpen)
                        {
                            handover.Handover();
                            await Coroutine.Wait(5000, () => Request.HandOverButtonClickable);
                            Request.HandOver();
                            await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                            if (SelectYesno.IsOpen)
                            {
                                SelectYesno.Yes();
                            }

                            await Coroutine.Wait(5000, () => GrandCompanySupplyReward.Instance.IsOpen);
                            GrandCompanySupplyReward.Instance.Confirm();
                            await Coroutine.Wait(5000, () => GrandCompanySupplyList.Instance.IsOpen);
                            await HandleCurrentGCWindow();
                            break;
                        }
                    }
                    else
                    {
                        Log($"Would get {item.Seals * 2} and we have {Core.Me.GCSeals()} out of {maxSeals}...too many");
                    }
                }
                else
                {
                    if (Core.Me.GCSeals() + (item.Seals) < maxSeals)
                    {
                        GrandCompanySupplyList.Instance.ClickItem(index);
                        await Coroutine.Wait(5000, () => Request.IsOpen);
                        if (Request.IsOpen)
                        {
                            handover.Handover();
                            await Coroutine.Wait(5000, () => Request.HandOverButtonClickable);
                            Request.HandOver();
                            await Coroutine.Wait(5000, () => GrandCompanySupplyReward.Instance.IsOpen);
                            GrandCompanySupplyReward.Instance.Confirm();
                            await Coroutine.Wait(5000, () => GrandCompanySupplyList.Instance.IsOpen);
                            await HandleCurrentGCWindow();
                            break;
                        }
                    }
                    else
                    {
                        Log($"Would get {item.Seals * 2} and we have {Core.Me.GCSeals()} out of {maxSeals}...too many");
                    }
                }
            }
        }

        public async Task<string> GetGCSupplyList()
        {
            if (!ContentsInfoDetail.Instance.IsOpen)
            {
                Logging.Write($"Trying to open window");

                if (!ContentsInfo.Instance.IsOpen)
                {
                    if (await ContentsInfo.Instance.Open())
                        ContentsInfo.Instance.OpenGCSupplyWindow();
                }

                await Coroutine.Wait(5000, () => ContentsInfoDetail.Instance.IsOpen);

                if (!ContentsInfoDetail.Instance.IsOpen)
                {
                    Logging.Write($"Nope failed opening GC Supply window");
                    return "";
                }
            }

            if (!ContentsInfoDetail.Instance.IsOpen)
            {
                Logging.Write($"Nope failed");
                return "";
            }

            List<LisbethOrder> outList = new List<LisbethOrder>();
            int id = 0;
            foreach (var item in ContentsInfoDetail.Instance.GetCraftingTurninItems().Where(item => !InventoryManager.FilledSlots.Any(i => i.RawItemId == item.Key.Id && i.Count >= item.Value.Key)))
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                var order = new LisbethOrder(id, 1, (int) item.Key.Id, item.Value.Key, item.Value.Value);
                outList.Add(order);

                id++;
            }

            foreach (var item in ContentsInfoDetail.Instance.GetGatheringTurninItems().Where(item => !InventoryManager.FilledSlots.Any(i => i.RawItemId == item.Key.Id && i.Count >= item.Value.Key)))
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                string type = "Gather";
                if (item.Value.Value.Equals("Fisher"))
                    continue; //type = "Fisher";
                var order = new LisbethOrder(id, 2, (int) item.Key.Id, item.Value.Key, type, true);

                outList.Add(order);
                id++;
            }

            ContentsInfoDetail.Instance.Close();
            ContentsInfo.Instance.Close();

            /*foreach (var order in outList)
            {
                Logging.Write($"{order}");
            }*/
            if (outList.Count == 0)
                return "";
            return JsonConvert.SerializeObject(outList, Formatting.None);
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Gold, msg);
        }

        public async Task<bool> PrintGCSupplyList()
        {
            if (!ContentsInfoDetail.Instance.IsOpen)
            {
                Logging.Write($"Trying to open window");

                if (!ContentsInfo.Instance.IsOpen)
                {
                    if (await ContentsInfo.Instance.Open())
                        ContentsInfo.Instance.OpenGCSupplyWindow();
                }

                await Coroutine.Wait(5000, () => ContentsInfoDetail.Instance.IsOpen);

                if (!ContentsInfoDetail.Instance.IsOpen)
                {
                    Logging.Write($"Nope failed opening GC Supply window");
                    return false;
                }
            }

            if (!ContentsInfoDetail.Instance.IsOpen)
            {
                Logging.Write($"Nope failed");
                return false;
            }

            List<LisbethOrder> outList = new List<LisbethOrder>();
            int id = 0;
            foreach (var item in ContentsInfoDetail.Instance.GetCraftingTurninItems())
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                var order = new LisbethOrder(id, 1, (int) item.Key.Id, item.Value.Key, item.Value.Value);
                outList.Add(order);

                id++;
            }

            foreach (var item in ContentsInfoDetail.Instance.GetGatheringTurninItems())
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                var type = "Gather";
                if (item.Value.Value.Equals("Fisher"))
                    type = "Fisher";
                var order = new LisbethOrder(id, 1, (int) item.Key.Id, item.Value.Key, type);

                outList.Add(order);
                id++;
            }

            ContentsInfoDetail.Instance.Close();
            ContentsInfo.Instance.Close();

            foreach (var order in outList)
            {
                Logging.Write($"{order}");
            }


            using (StreamWriter outputFile = new StreamWriter("GCSupply.json", false))
            {
                await outputFile.WriteAsync(JsonConvert.SerializeObject(outList, Formatting.None));
            }

            return true;
        }

        public async Task<bool> PrintMasterPieceList()
        {
            Dictionary<ClassJobType, int> Classes = new Dictionary<ClassJobType, int>
            {
                {ClassJobType.Carpenter, 0},
                {ClassJobType.Blacksmith, 1},
                {ClassJobType.Armorer, 2},
                {ClassJobType.Goldsmith, 3},
                {ClassJobType.Leatherworker, 4},
                {ClassJobType.Weaver, 5},
                {ClassJobType.Alchemist, 6},
                {ClassJobType.Culinarian, 7},
                {ClassJobType.Miner, 8},
                {ClassJobType.Botanist, 9},
                {ClassJobType.Fisher, 10},
            };

            if (!MasterPieceSupply.Instance.IsOpen)
            {
                Logging.Write($"Trying to open window");

                AgentModule.ToggleAgentInterfaceById(95);
                await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName("ContentsInfo") != null);
                await Coroutine.Sleep(500);

                if (RaptureAtkUnitManager.GetWindowByName("ContentsInfo") == null)
                {
                    Logging.Write($"Nope failed opening timer window");
                    return false;
                }

                await Coroutine.Sleep(500);
                RaptureAtkUnitManager.GetWindowByName("ContentsInfo").SendAction(2, 3, 0xC, 3, 6);
                await Coroutine.Wait(5000, () => MasterPieceSupply.Instance.IsOpen);
                await Coroutine.Sleep(500);
            }

            if (!MasterPieceSupply.Instance.IsOpen)
            {
                Logging.Write($"Nope failed");
                return false;
            }

            foreach (var job in Classes)
            {
                Logging.Write($"{job.Key}:");

                MasterPieceSupply.Instance.ClassSelected = job.Value;
                await Coroutine.Sleep(1000);

                //Can also use MasterPieceSupply.GetTurninItems() if you don't wanted starred info
                foreach (var item in MasterPieceSupply.Instance.GetTurninItemsStarred())
                {
                    Logging.Write($"{item.Key} Starred: {item.Value}");
                }
            }

            MasterPieceSupply.Instance.Close();

            return true;
        }
    }
}