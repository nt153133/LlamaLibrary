using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.Utilities.Helpers;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using ff14bot.RemoteWindows.GoldSaucer;
using GreyMagic;
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
using Action = TreeSharp.Action;
using ActionType = ff14bot.Enums.ActionType;
using static LlamaLibrary.Retainers.HelperFunctions;

namespace LlamaLibrary
{
    public class TesterBase : BotBase
    {
        
        bool IsJumping => Core.Memory.NoCacheRead<byte>(Offsets.Conditions + Offsets.JumpingCondition) != 0;
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

        internal static List<RetainerTaskData> VentureData;


        private static readonly List<(uint, Vector3)> SummoningBells = new List<(uint, Vector3)>
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


        private volatile bool _init;
        private Composite _root;

        private readonly SortedDictionary<string, List<string>> luaFunctions = new SortedDictionary<string, List<string>>();


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

        public override bool WantButton { get; } = true;

        public Dictionary<string, List<Composite>> hooks;


        public override void OnButtonPress()
        {
            StringBuilder sb1 = new StringBuilder();
            foreach (var obj in luaFunctions.Keys.Where(obj => luaFunctions[obj].Count >= 1))
            {
                sb1.AppendLine(obj);
                foreach (var funcName in luaFunctions[obj])
                {
                    sb1.AppendLine($"\t{funcName}");
                }
            }

            Log($"\n {sb1}");
        }

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
            //hooks = TreeHooks.Instance.Hooks;
           // TreeHooks.Instance.ClearAll();
           
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
        }

        private async Task<bool> Run()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();


            //await BuyHouse();
            //TreeRoot.Stop("Stop Requested");
            //await LeveWindow(1018997);
            //await HousingWards();
            //await testVentures();


            //DutyManager.AvailableContent
            // RoutineManager.Current.PullBehavior.Start();

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

            //await GoToSummoningBell();
            //string fun3 = $"return _G['CmnDefRetainerBell']:GetVentureFinishedRetainerName();";


            //Log($"{await VerifiedRowenaData()}");
            // var resultBool = WorldManager.Raycast(Core.Me.Location, GameObjectManager.Target.Location, out var result);
            // HuntHelper.Test();

            // await Navigation.GetTo(155, new Vector3(-279.682159f, 256.4128f, 339.207031f));

            //var composite_0 = BrainBehavior.CreateBrain();

            //DumpLuaFunctions();
            /*
            if (mob.Distance() > (RoutineManager.Current.PullRange - 1))
            {
                Log($"Moving");
                await Navigation.GetTo(WorldManager.ZoneId, mob.Location);
            }

            if (Core.Me.IsMounted)
                await CommonTasks.StopAndDismount();
           
            mob.Target();
            await Coroutine.Sleep(300);
            await RoutineManager.Current.PreCombatBuffBehavior.ExecuteCoroutine();
            if (Core.Me.HasTarget && Core.Me.CurrentTarget.Distance() > RoutineManager.Current.PullRange - 1)
                await Navigation.OffMeshMove(Core.Me.CurrentTarget.Location);
            await RoutineManager.Current.PullBehavior.ExecuteCoroutine();
            await Coroutine.Sleep(300);
            while (Core.Me.InCombat && Core.Me.HasTarget && mob.IsAlive)
            {
                if (Core.Me.CurrentTarget.Distance() > RoutineManager.Current.PullRange - 1)
                    await Navigation.OffMeshMove(Core.Me.CurrentTarget.Location);
                await RoutineManager.Current.CombatBehavior.ExecuteCoroutine();
                Log($"is it alive ? {mob.IsAlive}");
                await Coroutine.Yield();
            }
            */

            //   await FindAndKillMob(8609);
            //  Log("Current Daily Hunts");
            //  HuntHelper.Test();


            //  Log("\nAccepted Hunts");
            // HuntHelper.PrintAcceptedHunts();

            // Log("\nKill Counts");
            //Log($"is it alive ? {mob.IsAlive}");
            // HuntHelper.PrintKillCounts();
            //305 374
            /*       
                   int[] badLocations = new[] {457};
                   List<int> cantGetTo = new List<int>();
                   foreach (var huntLocation1 in badLocations)
                   {
                       var huntLocation = HuntHelper.DailyHunts[huntLocation1];
                       // LogCritical($"Can't get to {huntLocation1} {huntLocation.BNpcNameKey} {huntLocation.Map} {huntLocation.Location} {DataManager.ZoneNameResults[huntLocation.Map].CurrentLocaleName}");
                       LogSucess($"Going to {huntLocation1}");
       
                       if (huntLocation1 == 107 || huntLocation1 == 247)
                           await Navigation.GetToIslesOfUmbra();
       
       
                       var path = await Navigation.GetTo(huntLocation.Map, huntLocation.Location);
       
                       if (MovementManager.IsFlying)
                       {
                           await CommonTasks.Land();
                       }
       
                       if (Core.Me.Location.DistanceSqr(huntLocation.Location) > 40 && GameObjectManager.GameObjects.All(i => i.NpcId != huntLocation.BNpcNameKey))
                       {
                           cantGetTo.Add(huntLocation1);
                           LogCritical($"Can't get to {huntLocation} {huntLocation.BNpcNameKey} {huntLocation.Map} {huntLocation.Location}");
                       }
                       else
                       {
                           while (true)
                           {
                               if (await FindAndKillMob((uint) huntLocation.BNpcNameKey))
                               {
                                   Log("Killed one");
                                   await Coroutine.Sleep(1000);
                                   if (!Core.Me.InCombat) await Coroutine.Sleep(3000);
                               }
                               else
                               {
                                   Log("None found, sleeping 10 sec.");
                                   await Coroutine.Sleep(10000);
                               }
                           }
                           LogSucess($"Can get to {huntLocation1}");
                       }
       
                       //await Coroutine.Sleep(2000);
                   }
       
                   LogCritical($"\n {string.Join(",", cantGetTo)}\n");
       
       */


            //ActionRunCoroutine test = new ActionRunCoroutine(() => composite_0);
            /*
                        if (await GoToSummoningBell()) 
                            LogSucess("\n****************\n MADE IT BELL\n****************");
                        else 
                        {
                            LogCritical("\n****************\n FAILED TO MAKE IT TO BELL \n****************");
                        }
            */
            //await DoGCDailyTurnins();

            //    bool AgentCharacter = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.AgentCharacter), typeof(AgentCharacter));


            //   Log($"Added Venture Agent: {retaineragent}");

            /* Rowena
            var ItemList = Core.Memory.ReadArray<RowenaItem>(Offsets.RowenaItemList, Offsets.RowenaItemCount);
            StringBuilder sb = new StringBuilder();
            foreach (var itemGroup in ItemList.GroupBy(i=> i.ClassJob))
            {
                foreach (var item in itemGroup)
                {
                    sb.AppendLine(item.ToString().Trim().TrimEnd(','));
                    Log(item.ToString());
                }

            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(@"h:\", $"rowena.csv"), false))
            {
                outputFile.Write(sb.ToString());
            }
            
            */
            //var pat = "48 89 0D ? ? ? ? 0F B7 89 ? ? ? ? Add 3 TraceRelative";


            /*
            var hunts = HuntHelper.DailyHunts;
            var newHunts = new SortedDictionary<int, StoredHuntLocationLisbeth>();
            newHunts = JsonConvert.DeserializeObject<SortedDictionary<int, StoredHuntLocationLisbeth>>((new StreamReader("hunts.json")).ReadToEnd());
            foreach (var hunt in hunts.Where(i => !newHunts.ContainsKey(i.Key)))
            {
                if (hunt.Key == 399)
                {
                    await Navigation.GetToMap399();
                    await Navigation.GetTo(hunt.Value.Map, hunt.Value.Location);
                }
                else
                {
                    await Navigation.GetTo(hunt.Value.Map, hunt.Value.Location);
                }

                newHunts.Add(hunt.Key, new StoredHuntLocationLisbeth(hunt.Value.BNpcNameKey, Lisbeth.GetCurrentAreaName, hunt.Value.Location));
                Log($"{hunt.Key}");
                using (var outputFile = new StreamWriter($"hunts.json", false))
                {
                    outputFile.Write(JsonConvert.SerializeObject(newHunts));
                }
            }
            
            

            using (var outputFile = new StreamWriter($"hunts1.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(newHunts));
            }
            */


            //Log($"{Lisbeth.GetCurrentAreaName}");

            //  DumpLuaFunctions();


            //var line = LlamaLibrary.RemoteWindows.ContentsInfo.Instance.GetElementString(50);
            //int.Parse(line.Split(':')[1].Trim());
            //Log($"START:\n{sb.ToString()}");

            /*var row = FoodBuff.GetRow(420);

            for (int i = 0; i < 3; i++)
            {
                Log($"Stat: {(ItemAttribute)row.BaseParam[i]} Max: {row.Max[i]}({row.MaxHQ[i]}) Value: {row.Value[i]}%({row.ValueHQ[i]}%) IsRelative: {(row.IsRelative[i]==1 ? "True":"False")}");
            }*/
            /*IntPtr[] array = Core.Memory.ReadArray<IntPtr>(SpecialShopManager.ActiveShopPtr + 0x178, 2);
            ulong num = (ulong)((long)array[1] - (long)array[0]) / (ulong)(uint)0x1a0;

            var list = Core.Memory.ReadArray<SpecialShopItemLL>(array[0], (int)num);

            foreach (var item in list)
            {try t
                Log(item.ToString());
            }*/

            // Log(AgentWorldTravelSelect.Instance.CurrentWorld.ToString());


            //Lisbeth.AddHook("Llama",LlamaLibrary.Retainers.RetainersPull.CheckVentureTask);

            //Log($"{Achievements.HasAchievement(2199)}");
            // Log($"{BlueMageSpellBook.SpellLocation.ToString("X")}");

            //await Lisbeth.SelfRepair();
            /*Lisbeth.AddHook("Llama",TestHook);
            await Lisbeth.ExecuteOrders((new StreamReader("HookTest.json")).ReadToEnd());
            Lisbeth.RemoveHook("Llama");

            var newHunts = JsonConvert.DeserializeObject<SortedDictionary<int, StoredHuntLocationLisbeth>>((new StreamReader("hunts.json")).ReadToEnd());
            var failed = new Dictionary<int, StoredHuntLocationLisbeth>();
            var start = 0; 
            foreach (var hunt in newHunts.Where(i=> i.Key >= start))
            {
                if (!await Lisbeth.TravelTo(hunt.Value.Area, hunt.Value.Location));
                {
                    failed.Add(hunt.Key, hunt.Value);
                    using (var outputFile = new StreamWriter($"hunts_failed.json", false))
                    {
                        outputFile.Write(JsonConvert.SerializeObject(failed));
                    }
                }
                Log($"Finished {start}");
                start++;
            }
            using (var outputFile = new StreamWriter($"hunts_failed.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(failed));
            }*/

            //Log($"{Application.ProductVersion} - {Assembly.GetEntryAssembly().GetName().Version.Revision} - {Assembly.GetEntryAssembly().GetName().Version.MinorRevision} - {Assembly.GetEntryAssembly().GetName().Version.Build}");

            // Log($"\n {sb}");
            //DumpLLOffsets();
            
            /*InventoryBagId[] FCChest = new InventoryBagId[] {InventoryBagId.GrandCompany_Page1, InventoryBagId.GrandCompany_Page2, InventoryBagId.GrandCompany_Page3, (InventoryBagId) 20003, (InventoryBagId) 20004};

            var slots = InventoryManager.GetBagsByInventoryBagId(FCChest).SelectMany(x=> x.FilledSlots);
            foreach (var slot in slots)
            {
               // Log(slot);
            }*/

            uint[] privateHousing = new uint[] {59, 60, 61, 97};
            uint[] FCHousing = new uint[] {56,57,58,96};

            var AE = WorldManager.AvailableLocations;

            var PrivateHouses = AE.Where(x => privateHousing.Contains(x.AetheryteId)).OrderBy(x => x.GilCost);
            var FCHouses = AE.Where(x => FCHousing.Contains(x.AetheryteId)).OrderBy(x => x.GilCost);
            
            bool HavePrivateHousing = PrivateHouses.Any();
            bool HaveFCHousing = FCHouses.Any();


            Log($"Private House Access: {HavePrivateHousing} FC House Access: {HaveFCHousing}");
            
            //await GoToHousingBell(FCHouses.First());
            
            
            if (HavePrivateHousing)
            {
                await GoToHousingBell(PrivateHouses.First());
            }
            else if (HaveFCHousing)
            {
                await GoToHousingBell(FCHouses.First());
            }
            
            
            //DumpOffsets();
            //await BuyHouse();
            //await testKupoTickets();
            TreeRoot.Stop("Stop Requested");
            //Core.Me.Stats
            
            //AtkAddonControl windowByName = RaptureAtkUnitManager.Update()
            // await Coroutine.Sleep(100);
            
            //Log(Core.Me.IsFate);

            return false;


            BeastTribeHelper.PrintDailies();
            BeastTribeHelper.PrintBeastTribes();
            Timers.PrintTimers();
            AgentGoldSaucerInfo.Instance.Toggle();
            await Coroutine.Wait(5000, () => GSInfoGeneral.IsOpen);
            if (GSInfoGeneral.IsOpen)
            {
                await Coroutine.Sleep(500);
                Log($"Mini Cactpot tickets left: {GSInfoGeneral.DailyAllowancesLeft}");
                AgentGoldSaucerInfo.Instance.Toggle();
            }

            TimersSettings testTimers = TimersSettings.Instance;
            Log($"Cycle 1: {testTimers.GetTimer(1)}");
            Log($"Cycle 2: {testTimers.GetTimer(2)}");
            Log($"Cycle 4: {testTimers.GetTimer(4)}");


            //            await Reduce.Reduce.Extract();

            //InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems).Last().IsFilled
            // await BuyHouse();
            TreeRoot.Stop("Stop Requested");
            return false;
            //await BuyHouse();

            // await Coroutine.Sleep(100);
        }

        private async Task<bool> GoToHousingBell(WorldManager.TeleportLocation house)
        {
            Log($"Teleporting to housing: (ZID: {house.ZoneId}, AID: {house.AetheryteId}) {house.Name}");
            await CommonTasks.Teleport(house.AetheryteId);

            Log("Waiting for zone to change");
            await Coroutine.Wait(20000, () => WorldManager.ZoneId == house.ZoneId);

            Log("Getting closest housing entrance");
            uint houseEntranceId = 2002737;
            uint aptEntranceId = 2007402;

            var entranceIds = new uint[] { houseEntranceId, aptEntranceId };

            var entrance = GameObjectManager.GetObjectsByNPCIds<GameObject>(entranceIds).OrderBy(x => x.Distance2D()).FirstOrDefault();
            if (entrance != null)
            {
                Log("Found housing entrance, approaching");
                await Navigation.FlightorMove(entrance.Location);

                if (entrance.IsWithinInteractRange)
                {
                    Navigator.NavigationProvider.ClearStuckInfo();
                    Navigator.Stop();
                    await Coroutine.Wait(5000, () => !IsJumping);

                    entrance.Interact();

                    // Handle different housing entrance menus
                    if (entrance.NpcId == houseEntranceId)
                    {
                        Log("Entering house");

                        await Coroutine.Wait(10000, () => SelectYesno.IsOpen);
                        if (SelectYesno.IsOpen)
                        {
                            SelectYesno.Yes();
                        }
                    }
                    else if (entrance.NpcId == aptEntranceId)
                    {
                        Log("Entering apartment");

                        await Coroutine.Wait(10000, () => SelectString.IsOpen);
                        if (SelectString.IsOpen)
                        {
                            SelectString.ClickSlot(0);
                        }
                    }

                    await CommonTasks.HandleLoading();

                    Log("Getting best summoning bell");
                    var bell = HelperFunctions.FindSummoningBell();
                    if (bell != null)
                    {
                        Log("Found summoning bell, approaching");
                        await HelperFunctions.GoToSummoningBell();
                        return true;
                    }
                    else
                    {
                        Log("Couldn't find any summoning bells");
                    }
                }
            }
            else
            {
                Log($"Couldn't find any housing entrances.  Are we in the right zone?  Current: ({WorldManager.ZoneId}) {WorldManager.CurrentZoneName}");
            }

            return false;
        }

        private void LogPtr(IntPtr pointer)
        {
            Log(pointer.ToString("X"));
        }

        private async Task TestHook()
        {
            Log("LL hook");
            //await Navigation.GetToMap399();
        }


        
        /*
        private void DumpLLOffsets()
        {
            var sb = new StringBuilder();
            var sb1 = new StringBuilder();
            foreach (var patternItem in OffsetManager.patterns)
            {
                var name = patternItem.Key;
                var pattern = patternItem.Value.Replace("Search ", "");

                if (name.ToLowerInvariant().Contains("vtable") && name.ToLowerInvariant().Contains("agent"))
                {
                    Log($"Agent_{name}, {pattern}");
                    sb1.AppendLine($"{name.Replace("Vtable", "").Replace("vtable", "").Replace("VTable", "").Replace("_", "")}, {pattern}");
                }
                else if (!name.ToLowerInvariant().Contains("exd"))
                {
                    Log($"{name}, {pattern}");
                    sb.AppendLine($"{name}, {pattern}");
                }
            }

            using (var outputFile = new StreamWriter(@"G:\AgentLL.csv", false))
            {
                outputFile.Write(sb1.ToString());
            }

            using (var outputFile = new StreamWriter(@"G:\LL.csv", false))
            {
                outputFile.Write(sb.ToString());
            }

            sb = new StringBuilder();
            int i = 0;
            foreach (var vtable in AgentModule.AgentVtables)
            {
                sb.AppendLine($"Model_{i},{Core.Memory.GetRelative(vtable).ToString("X")}");
                i++;
            }

            using (var outputFile = new StreamWriter(@"G:\AgentOffsets.csv", false))
            {
                outputFile.Write(sb.ToString());
            }
        }
        */
        

       

        private async Task BuyHouse()
        {
            Random _rnd = new Random();
            ;
            var placard = GameObjectManager.GetObjectsByNPCId(2002736).OrderBy(i => i.Distance()).FirstOrDefault();
            if (placard != null)
            {
                do
                {
                    if (!HousingSignBoard.Instance.IsOpen)
                    {
                        placard.Interact();
                        await Coroutine.Wait(3000, () => HousingSignBoard.Instance.IsOpen);
                    }

                    if (HousingSignBoard.Instance.IsOpen)
                    {
                        if (HousingSignBoard.Instance.IsForSale)
                        {
                            await Coroutine.Sleep(_rnd.Next(200, 400));
                            HousingSignBoard.Instance.ClickBuy();
                            await Coroutine.Wait(3000, () => Conversation.IsOpen);
                            if (Conversation.IsOpen)
                            {
                                await Coroutine.Sleep(_rnd.Next(50, 300));
                                Conversation.SelectLine(0);
                                await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
                                SelectYesno.Yes();
                                await Coroutine.Sleep(_rnd.Next(23, 600));
                            }
                        }
                    }

                    await Coroutine.Sleep(_rnd.Next(1500, 3000));
                    placard.Interact();
                    await Coroutine.Wait(3000, () => HousingSignBoard.Instance.IsOpen);
                }
                while (HousingSignBoard.Instance.IsForSale);

                await Coroutine.Wait(3000, () => HousingSignBoard.Instance.IsOpen);
                HousingSignBoard.Instance.Close();
                await Coroutine.Wait(3000, () => !HousingSignBoard.Instance.IsOpen);
                Lua.DoString("return _G['EventHandler']:Shutdown();");
            }
        }

        private void DumpOffsets()
        {
            var off = typeof(Core).GetProperty("Offsets", BindingFlags.NonPublic | BindingFlags.Static);
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            int j = 0;
            int p1 = 0;
            int p2 = 0;
            foreach (var p in off.PropertyType.GetFields())
            {
                var tp = p.GetValue(off.GetValue(null));
                //stringBuilder.Append($"\nOffset Struct_{i + 88} {i + 1} ({p.FieldType.GetFields().Length})");
                j = 0;
                p1 = 0;
                p2 = 0;
                foreach (var t in p.FieldType.GetFields())
                {
                    //stringBuilder.Append(string.Format("\nField: {0} \t", p2));

                    if (t.FieldType == typeof(IntPtr))
                    {
                        //IntPtr ptr = new IntPtr(((IntPtr) t.GetValue(tp)).ToInt64() - Core.Memory.ImageBase.ToInt64());
                        IntPtr ptr = (((IntPtr) t.GetValue(tp)));
                        stringBuilder.Append($"Struct{i + 88}_IntPtr{p1}, {Core.Memory.GetRelative(ptr).ToInt64()}\n");
                        //stringBuilder.Append(string.Format("\tPtr Offset_{0}: 0x{1:x}", p1, ptr.ToInt64()));

                        p1++;
                    }

                    p2++;
                }

                //stringBuilder.Append("\n");
                i++;
            }

            using (var outputFile = new StreamWriter($"RB{Assembly.GetEntryAssembly().GetName().Version.Build}.csv", false))
            {
                outputFile.Write(stringBuilder.ToString());
            }
        }

        private void DumpLuaFunctions()
        {
            string func = "local values = {} for key,value in pairs(_G) do table.insert(values, key); end return unpack(values);";

            var retValues = Lua.GetReturnValues(func);
            foreach (var ret in retValues.Where(ret => !ret.StartsWith("_") && !ret.StartsWith("Luc") && !ret.StartsWith("Stm") && !char.IsDigit(ret[ret.Length - 1]) && !char.IsLower(ret[0])))
            {
                if (ret.Contains(":"))
                {
                    var name = ret.Split(':')[0];
                    if (luaFunctions.ContainsKey(name))
                        continue;
                    luaFunctions.Add(name, GetSubFunctions(name));
                }
                else
                {
                    if (luaFunctions.ContainsKey(ret))
                        continue;
                    luaFunctions.Add(ret, GetSubFunctions(ret));
                }
            }
        }

        private static List<string> GetSubFunctions(string luaObject)
        {
            string func = $"local values = {{}} for key,value in pairs(_G['{luaObject}']) do table.insert(values, key); end return unpack(values);";
            List<string> functions = new List<string>();
            try
            {
                var retValues = Lua.GetReturnValues(func);
                functions.AddRange(retValues.Where(ret => !ret.Contains("_") && !ret.Contains("OnSequence") && !ret.StartsWith("On") && !ret.Contains("className") && !ret.Contains("referenceCount") && !ret.Contains("ACTOR")));
            }
            catch
            {
            }

            functions.Sort();
            return functions;
        }


        public static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, text);
        }

        public static void LogSucess(string text)
        {
            Logging.Write(Colors.Green, text);
        }

        private async Task<bool> MoveSummoningBell(Vector3 loc)
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

            return moving == MoveResult.ReachedDestination;
        }

        public async Task<bool> GoToSummoningBell()
        {
            var searchBell = FindSummoningBell();
            if (searchBell != null)
            {
                if (searchBell.IsWithinInteractRange)
                {
                    Log("Found bell in Interact Range");
                    return true;
                }

                if (await Navigation.GetTo(WorldManager.ZoneId, searchBell.Location))
                {
                    Log("Used Navgraph/Flightor to get there");
                    if (searchBell.IsWithinInteractRange)
                        return true;
                }
            }

            (uint, Vector3) bellLocation;
            int tries = 0;
            if (SummoningBells.Any(i => i.Item1 == WorldManager.ZoneId))
            {
                Log("Found a bell in our zone");
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

            return false;
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
                    var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
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
            var patternFinder = new PatternFinder(Core.Memory);

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
            var patternFinder = new PatternFinder(Core.Memory);
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

        private static void Log(string text)
        {
            var msg = "[Tester] " + text;
            Logging.Write(Colors.Pink, msg);
        }

        public async Task<bool> testExtract()
        {
            // var item = InventoryManager.FilledInventoryAndArmory.Where(i => i.Item.EngName.Contains("Voeburtite Ring of Slaying")).FirstOrDefault();


            //  if (item != null)
            //      item.ExtractMateria();
            var a = InventoryManager.FilledSlots.First(i => i.RawItemId == 27712);

            Log($"{a} {a.BagId} {a.Slot}");
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

            await Coroutine.Wait(5000, () => DialogOpen);

            while (DialogOpen)
            {
                Next();
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
