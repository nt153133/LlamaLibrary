using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Buddy.Coroutines;
using Buddy.Service.Core;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Forms.ugh;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using Generate;
using LlamaLibrary;
using LlamaLibrary.Enums;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary
{
    public class TesterBase : BotBase
    {
        private Composite _root;

        private static Dictionary<byte, string> FishingState = new Dictionary<byte, string>
        {
            {0, "Unknown"},
            {1, "In Craft"},
            {2, "Not Fishing"},
            {26, "In Craft - Using synthesis (element) action"},
            {30, "In Craft - Using touch action"},
            {29, "In Craft - Using synthesis action"},
            {35, "Fishing Stance"},
            {37, "Packing Up"},
            {38, "Casting"},
            {39, "Mooch Casting"},
            {40, "Casting"},
            {41, "Fishing"},
            {42, "Fishing"},
            {43, "Fishing"},
            {44, "Fishing"},
            {45, "Fishing"},
            {46, "Fishing"},
            {47, "Unsuccessful"},
            {48, "Successful"},
            {49, "Light Reeling"},
            {50, "Light HQ Reeling"},
            {51, "Medium Reeling"},
            {52, "Medium HQ Reeling"},
            {53, "Big Reeling"},
            {54, "Big HQ Reeling"},
            {56, "Light Bite"},
            {57, "Medium Bite"},
            {58, "Heavy Bite"}
        };

        private static Dictionary<byte, string> CraftingState = new Dictionary<byte, string>
        {
            {0, "Unknown"},
            {1, "In Craft"},
            {2, "Not Crafting"},
            {5, "Synthesizing"},
            {26, "In Craft - Using synthesis (element) action"},
            {30, "In Craft - Using touch action"},
            {29, "In Craft - Using synthesis action"}
        };

        public TesterBase()
        {
            Task.Factory.StartNew(() =>
            {
                // init();
                _init = true;
                Log("INIT DONE");
            });
        }

        public override string Name => "Tester";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;
        internal static List<RetainerTaskData> VentureData;
        private volatile bool _init;

        internal void init()
        {
            OffsetManager.Init();

            Log("Load venture.json");
            VentureData = loadResource<List<RetainerTaskData>>(Resources.Ventures);
            Log("Loaded venture.json");
        }

        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
        }


        
        private async Task<bool> Run()
        {
            //await LeveWindow(1018997);
            //await HousingWards();
            //await testVentures();
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

            //var newList = JsonConvert.DeserializeObject<List<GatheringNodeData>>(File.ReadAllText(Path.Combine("H:\\", $"TimedNodes.json")));
            //    foreach (var nodeData in newList)
            //    {
            //        Log($"\n{nodeData}");
            //     }
            /*
                        byte lastChecked = Core.Me.GatheringStatus();
                        while (_root != null)
                        {
                            if (lastChecked != Core.Me.GatheringStatus())
                            {
                                Log(FishingState.ContainsKey(Core.Me.GatheringStatus()) ? $"{FishingState[Core.Me.GatheringStatus()]}" : $"{Core.Me.GatheringStatus()} - {Core.Me.CastingSpellId} {ActionManager.LastSpell}");
                                lastChecked = Core.Me.GatheringStatus();
                            }
            
                            await Coroutine.Sleep(200);
                        }
            */

            await DoGCDailyTurnins();

            
            TreeRoot.Stop("Stop Requested");
            return true;
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
            foreach (var item in ContentsInfoDetail.Instance.GetCraftingTurninItems().Where(item => !InventoryManager.FilledSlots.Any(i=> i.RawItemId == item.Key.Id && i.Count >= item.Value.Key)))
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                var order = new LisbethOrder(id, 1, (int) item.Key.Id, item.Value.Key, item.Value.Value);
                outList.Add(order);
                
                id++;
            }

            foreach (var item in ContentsInfoDetail.Instance.GetGatheringTurninItems().Where(item => !InventoryManager.FilledSlots.Any(i=> i.RawItemId == item.Key.Id && i.Count >= item.Value.Key)))
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                string type = "Gather";
                if (item.Value.Value.Equals("Fisher"))
                    continue;//type = "Fisher";
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
            
            return JsonConvert.SerializeObject(outList, Formatting.None);
        }


        public async Task testVentures()
        {
            //var ishgard = new IshgardHandin();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();

            if (!RetainerList.Instance.IsOpen)
            {
                await HelperFunctions.UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                Log("Can't find open bell either you have none or not near a bell");
                return;
            }

            var count = await HelperFunctions.GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            int index = 0;
            foreach (var ret in rets)
            {
                Log($"{index} {ret.Name}");
                index++;
            }

            index = 0;

            for (int i = 0; i < count; i++)
            {
                Log($"{i} {RetainerList.Instance.RetainerName(i)}");
            }

            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i => i.Active).ToArray();


            foreach (var ret in ordered)
            {
                Log($"{index} {ret.Name}");
                index++;
            }

            RetainerList.Instance.Close();
        }

        public async Task<bool> RetainerCheck(RetainerInfo retainer)
        {
            if (retainer.Job != ClassJobType.Adventurer)
            {
                if (retainer.VentureTask != 0)
                {
                    var now = (Int32) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    var timeLeft = retainer.VentureEndTimestamp - now;

                    if (timeLeft <= 0 && SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.Venture) > 2)
                    {
                        await CheckVentures();
                    }
                    else
                    {
                        Log($"Venture will be done at {RetainerInfo.UnixTimeStampToDateTime(retainer.VentureEndTimestamp)}");
                    }
                }
            }

            if (RetainerSettings.Instance.DepositFromPlayer) await RetainerRoutine.DumpItems();

            Log("Done checking against player inventory");


            //Log($"{RetainerInfo.UnixTimeStampToDateTime(retainer.VentureEndTimestamp)}");

            return true;
        }

        public async Task<bool> CheckVentures()
        {
            if (!SelectString.IsOpen)
            {
                return false;
            }

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

                var task = VentureData.First(i => i.Id == taskId);

                Log($"Finished Venture {task.Name}");
                Log($"Reassigning Venture {task.Name}");

                RetainerTaskResult.Reassign();

                await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen);
                if (!RetainerTaskAsk.IsOpen)
                {
                    Log("RetainerTaskAsk didn't open");
                    return false;
                }

                await Coroutine.Wait(2000, RetainerTaskAskExtensions.CanAssign);
                if (RetainerTaskAskExtensions.CanAssign())
                {
                    RetainerTaskAsk.Confirm();
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


            return true;
        }

        public async Task testKupoTickets()
        {
            var ishgard = new IshgardHandin();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();

            await ishgard.HandInKupoTicket(1);
        }

        public async Task<bool> testFacetCheck()
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);

            var result = patternFinder.Find("44 89 BF ?? ?? ?? ?? 83 BF ?? ?? ?? ?? ?? Add 3 Read32");
            //Log(result);
            uint[] npcs = {1027233, 1027234, 1027235, 1027236, 1027237};

            var units = GameObjectManager.GameObjects;
            foreach (var unit in units.Where(i => npcs.Contains(i.NpcId)))
            {
                Log("Name:{0}, Type:{3}, ID:{1}, Obj:{2}", unit, unit.NpcId, unit.ObjectId, unit.GetType());
            }

            return false;
        }

        public async Task testGather()
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            IntPtr AnimationLocked = patternFinder.Find("48 8D 0D ?? ?? ?? ?? BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 8B ?? ?? ?? ?? ?? 45 33 C9 44 8B C7 89 BB ?? ?? ?? ?? Add 3 TraceRelative");

            var GatherLock = Core.Memory.Read<uint>(AnimationLocked + 0x2A);

            if (GatheringManager.WindowOpen)
            {
                GatheringItem items = GatheringManager.GatheringWindowItems.FirstOrDefault(i => i.IsFilled && !i.IsUnknown && !i.ItemData.Unique && i.CanGather);

                Log($"Gathering: {items}");

                while (GatheringManager.SwingsRemaining > 0)
                {
                    items.GatherItem();
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(AnimationLocked + 0x2A) != 0);
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(AnimationLocked + 0x2A) == 0);
                }
            }
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Pink, msg);
        }

        public async Task<bool> testExtract()
        {
            // var item = InventoryManager.FilledInventoryAndArmory.Where(i => i.Item.EngName.Contains("Voeburtite Ring of Slaying")).FirstOrDefault();


            //  if (item != null)
            //      item.ExtractMateria();
            var a = InventoryManager.FilledSlots.First(i => i.RawItemId == 27712);

            Log(($"{a} {a.BagId} {a.Slot}"));
            Log($"Inventory Pointer: {Offsets.ItemFuncParam.ToInt64():X}  Function: {Offsets.ItemSplitFunc.ToInt64():X}");
            a.Split(1);

            AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("SelectString");

            if (windowByName != null)
            {
                List<string> list = new List<string>();
                IntPtr pointer = Core.Memory.Read<IntPtr>(windowByName.Pointer + 0x238 + 0x38);

                if (pointer != IntPtr.Zero)
                {
                    int count = Core.Memory.Read<int>(pointer + 0x118);
                    for (int i = 0; i < count; i++)
                    {
                        IntPtr addr = Core.Memory.Read<IntPtr>(pointer + 0xF0) + 24 * i + 8;
                        IntPtr pointer2 = Core.Memory.Read<IntPtr>(addr) + 8;
                        var short1 = Core.Memory.Read<ushort>(pointer2 + 0x42);
                        IntPtr addr2 = Core.Memory.Read<IntPtr>(pointer2 + 0x50) + 8 * (short1 - 1);
                        IntPtr pointer3 = Core.Memory.Read<IntPtr>(addr2);
                        string item = Core.Memory.ReadString(Core.Memory.Read<IntPtr>(pointer3 + 0xB8), Encoding.UTF8);
                        list.Add(item);
                    }
                }
            }

            return true;
        }

        public async Task<bool> LeveWindow(uint NpcId)
        {
            var npcId = GameObjectManager.GetObjectByNPCId(NpcId);

            if (!npcId.IsWithinInteractRange)
            {
                var _target = npcId.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            npcId.Interact();

            await Coroutine.Wait(5000, () => Talk.DialogOpen);

            while (Talk.DialogOpen)
            {
                Talk.Next();
                await Coroutine.Sleep(1000);
            }

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot(0);

            await Coroutine.Wait(5000, () => GuildLeve.Instance.IsOpen);

            if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Battle)
            {
                Logger.Info("Battle");
                Logger.Info(GuildLeve.Instance.PrintWindow());

                GuildLeve.Instance.SwitchType(1);
                await Coroutine.Sleep(500);
                Logger.Info("Gathering");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Gathering)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }

                GuildLeve.Instance.SwitchType(2);
                await Coroutine.Sleep(500);
                Logger.Info("Crafting");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Crafting)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }
            }
            else if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Gathering)
            {
                Logger.Info("Gathering");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Gathering)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }

                GuildLeve.Instance.SwitchType(1);
                await Coroutine.Sleep(500);
                Logger.Info("Crafting");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Crafting)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }
            }

            var output = GuildLeve.Instance.PrintWindow();

            await Coroutine.Sleep(500);

            GuildLeve.Instance.Close();

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot((uint) (SelectString.LineCount - 1));

            Logger.Info(output);

            return true;
        }
    }
}