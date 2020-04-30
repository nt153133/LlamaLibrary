using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
        static List<(uint, Vector3)> SummoningBells = new List<(uint, Vector3)>
            {
                (129, new Vector3(-223.743042f, 16.006714f, 41.306152f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
                (129, new Vector3(-266.376831f, 16.006714f, 41.275635f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
                (129, new Vector3(-149.279053f, 18.203979f, 20.553894f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
                (129, new Vector3(-123.888062f, 17.990356f, 21.469421f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
                (131, new Vector3(148.91272f, 3.982544f, -44.205383f)), //Ul'dah - Steps of Thal(Ul'dah) 
                (131, new Vector3(111.161987f, 4.104675f, -72.343079f)), //Ul'dah - Steps of Thal(Ul'dah) 
                (131, new Vector3(153.185303f, 3.982544f, 13.229492f)), //Ul'dah - Steps of Thal(Ul'dah) 
                (131, new Vector3(118.547363f, 4.013123f, -93.003784f)), //Ul'dah - Steps of Thal(Ul'dah) 
                (133, new Vector3(160.234863f, 15.671021f, -55.649719f)), //Old Gridania(Gridania) 
                (133, new Vector3(169.726074f, 15.487854f, -81.895203f)), //Old Gridania(Gridania) 
                (133, new Vector3(171.007812f, 15.487854f, -101.487854f)), //Old Gridania(Gridania) 
                (133, new Vector3(160.234863f, 15.671021f, -136.369934f)), //Old Gridania(Gridania) 
                (156, new Vector3(34.50061f, 28.976807f, -762.233948f)), //Mor Dhona(Mor Dhona) 
                (156, new Vector3(11.001709f, 28.976807f, -734.554077f)), //Mor Dhona(Mor Dhona) 
                (419, new Vector3(-151.171204f, -12.64978f, -11.764771f)), //The Pillars(Ishgard) 
                (478, new Vector3(34.775269f, 208.148193f, -50.858398f)), //Idyllshire(Dravania) 
                (478, new Vector3(0.38147f, 206.469727f, 51.407593f)), //Idyllshire(Dravania) 
                (628, new Vector3(19.394226f, 4.043579f, 53.025024f)), //Kugane(Kugane) 
                (635, new Vector3(-57.633362f, -0.01532f, 49.30188f)), //Rhalgr's Reach(Gyr Abania) 
                (819, new Vector3(-69.840576f, -7.705872f, 123.491211f)), //The Crystarium(The Crystarium) 
                (819, new Vector3(-64.255798f, 19.97406f, -144.274109f)), //The Crystarium(The Crystarium) 
                (820, new Vector3(7.186951f, 83.17688f, 31.448853f)) //Eulmore(Eulmore) 
            };
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
            /*
            Log($"{await GrandCompanyShop.BuyKnownItem(6141, 5)}"); //Cordial
            await Coroutine.Sleep(1000);
            Log($"{await GrandCompanyShop.BuyKnownItem(21072, 2)}"); //ventures
            await Coroutine.Sleep(1000);
            Log($"{await GrandCompanyShop.BuyKnownItem(21072, 3)}"); //ventures
            await Coroutine.Sleep(1000);
            
            TreeRoot.Stop("Stop Requested");
            return true;
            */

            //      var newList = JsonConvert.DeserializeObject<List<GatheringNodeData>>(File.ReadAllText(Path.Combine("H:\\", $"TimedNodes.json")));
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
            196630
                            await Coroutine.Sleep(200);
                        }
            */

            //Map, Location


            ;

           // var resultBool = WorldManager.Raycast(Core.Me.Location, GameObjectManager.Target.Location, out var result);

           if (await GoToSummoningBell()) 
               LogSucess("\n****************\n MADE IT BELL\n****************");
           else
           {
               LogCritical("\n****************\n FAILED TO MAKE IT TO BELL \n****************");
           }

            //await DoGCDailyTurnins();


            TreeRoot.Stop("Stop Requested");
            return true;
        }
        
        public static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, text);
        }
        
        public static void LogSucess(string text)
        {
            Logging.Write(Colors.Green, text);
        }

        public async Task<bool> GoToSummoningBell()
        {
            var searchBell = FindSummoningBell();
            if (searchBell != null)
            {
                if (searchBell.IsWithinInteractRange)
                {
                    Log($"Found bell in Interact Range");
                    return true;
                }
                if (await Navigation.GetTo(WorldManager.ZoneId, searchBell.Location))
                {
                    Log($"Used Navgraph to get there");
                    if (searchBell.IsWithinInteractRange)
                        return true;
                }
                else if (searchBell.Distance() < 8 && Math.Abs(Core.Me.Location.Y - searchBell.Location.Y) < 0.5f)
                {
                    Log($"Should be able to offmesh move to bell");
                    if (await Navigation.OffMeshMoveInteract(searchBell))
                    {
                        if (searchBell.IsWithinInteractRange)
                            return true;
                        else
                        {
                            Log($"OffMesh nav failed going to new bell");
                        }
                    }
                }
            }
            
            (uint, Vector3) bellLocation;
            int tries = 0;
            if (SummoningBells.Any(i => i.Item1 == WorldManager.ZoneId))
            {
                Log($"Found a bell in our zone");
                bellLocation = SummoningBells.Where(i => i.Item1 == WorldManager.ZoneId).OrderBy(r => Core.Me.Location.DistanceSqr(r.Item2)).First();
            }
            else
            {
                bool foundBell = false;
                Random rand = new Random();
                do
                {
                    tries++;
                    var index = rand.Next(0, SummoningBells.Count);
                    bellLocation = SummoningBells[index];
                    var ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.ZoneId == bellLocation.Item1 && i.IsAetheryte);

                    if (ae == default(AetheryteResult))
                    {
                        switch (bellLocation.Item1)
                        {
                            case 131:
                                ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == 9);
                                break;
                            case 133:
                                ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == 2);
                                break;
                            case 419:
                                ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == 70);
                                break;
                        }
                    }

                    if (ae != default(AetheryteResult))
                    {
                        if (ConditionParser.HasAetheryte(ae.Id))
                        {
                            Log($"{bellLocation.Item1} can get to ConditionParser.HasAetheryte({ae.Id}) = {ConditionParser.HasAetheryte(ae.Id)} {ae.EnglishName}");
                            foundBell = true;
                        }
                    }
                    else
                    {
                        Log($"{bellLocation.Item1} can't find AE");
                    }
                }
                while (!foundBell && tries < 5);
            }
                            
            Log($"Going to bell {bellLocation.Item1} {bellLocation.Item2}");
            if (await Navigation.GetTo(bellLocation.Item1, bellLocation.Item2))
            {
                var bell = FindSummoningBell();
                Log(bell != null ? $"{bell.Name} {bell.Location} {WorldManager.CurrentZoneName} {bell.IsWithinInteractRange}" : $"Couldn't find bell at {bellLocation.Item2} {bellLocation.Item1}");
                return bell != null;
            }
            else
            {
                return false;
            }
            
        }

    //Log("Name:{0}, Location:{1} {2}", unit, unit.Location,WorldManager.CurrentZoneName);
        public GameObject FindSummoningBell()
        {
            uint[] bellIds = {2000072, 2000401, 2000403, 2000439, 2000441, 2000661, 2001271, 2001358, 2006565, 2010284};
            var units = GameObjectManager.GameObjects;

            foreach (var unit in units.Where(i => i.IsVisible).OrderBy(r => r.DistanceSqr()))
            {
                if (unit.VTable == Offsets.HousingObjectVTable && Core.Memory.Read<uint>(unit.Pointer + 0x80) == 196630)
                {
                    return unit;
                }

                if (!bellIds.Contains(unit.NpcId))
                {
                    continue;
                }

                return unit;
            }

            return null;
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

        private void Log(string text)
        {
            var msg = "[" + Name + "] " + text;
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