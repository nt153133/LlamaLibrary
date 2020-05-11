using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
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
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
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

namespace LlamaLibrary
{
    public class TesterBase : BotBase
    {
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

        public static float PostCombatDelay = 0f;

        internal static bool bool_0;
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

        internal static bool InFight => GameObjectManager.Attackers.Any();

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
            var hadOld =  await GetHuntBills();
            await CompleteHunts();

            if (hadOld)
            {
                await GetHuntBills();
                await CompleteHunts();
            }

            if (WorldManager.CanTeleport())
            {
                WorldManager.TeleportById(Core.Me.HomePoint.Id);
                await Coroutine.Sleep(5000);

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
            }
            
            

            //Log($"START:\n{sb.ToString()}");
            TreeRoot.Stop("Stop Requested");
            // await Coroutine.Sleep(100);
            return false;
        }

        public async Task<bool> GetHuntBills()
        {  
            var statues = HuntHelper.GetDailyStatus();

            foreach ((var orderType, var huntOrderStatus) in statues)
            {
                var orderTypeObj = HuntHelper.GetMobHuntOrderType((int) orderType);
                
                switch (huntOrderStatus)
                {
                    case HuntOrderStatus.OnlyFatesLeft:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Only Fates left for today's dailies so done");
                        break;
                    case HuntOrderStatus.OnlyFatesLeftOld:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Only Fates left for old dailies so should yeet them and get new ones");
                        HuntHelper.DiscardMobHuntType(orderType);
                        await HuntHelper.GetHuntsByOrderType(orderType);
                        break;
                    case HuntOrderStatus.NotAccepted:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Have not accepted today's hunts");
                        await HuntHelper.GetHuntsByOrderType(orderType);
                        break;
                    case HuntOrderStatus.NotAcceptedOld:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Not Accepted and last accepted were old and so should get new ones");
                        await HuntHelper.GetHuntsByOrderType(orderType);
                        break;
                    case HuntOrderStatus.Complete:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Finished today's dailies");
                        break;
                    case HuntOrderStatus.CompleteOld:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Finished  old dailies");
                        await HuntHelper.GetHuntsByOrderType(orderType);
                        break;
                    case HuntOrderStatus.Unfinished:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Unfinished current dailies");
                        break;
                    case HuntOrderStatus.UnFinishedOld:
                        Log($"{orderTypeObj.Item.CurrentLocaleName} - Unfinished old dailies");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return statues.Any(i => i.Item2 == HuntOrderStatus.UnFinishedOld);
        }

        public async Task CompleteHunts()
        {
            int[] dailyOrderTypes = new[] {0, 1, 2, 3, 6, 7, 8, 10, 11, 12};
            int flytoHunt = 418;
            int[] umbra = new[] {107, 247};
            foreach (var orderType in dailyOrderTypes)
            {
                var dailies = HuntHelper.GetAcceptedDailyHunts(orderType);
                if (dailies.Any(i => !i.IsFinished))
                {
                    foreach (var hunt in dailies.Where(i => !i.IsFinished))
                    {
                        Log($"{hunt}");
                        while (Core.Me.InCombat)
                        {
                            var target = GameObjectManager.Attackers.FirstOrDefault();
                            if (target != default(BattleCharacter) && target.IsValid && target.IsAlive)
                            {
                                await Navigation.GetTo(WorldManager.ZoneId, target.Location);
                                await KillMob(target);
                            }
                        }
                        if (hunt.HuntTarget == flytoHunt)
                        {
                            var AE = WorldManager.AetheryteIdsForZone(hunt.MapId).OrderBy(i => i.Item2.DistanceSqr(hunt.Location)).First();
                            //LogCritical("Can teleport to AE");
                            WorldManager.TeleportById(AE.Item1);
                            await Coroutine.Wait(20000, () => WorldManager.ZoneId == AE.Item1);
                            await Coroutine.Sleep(2000);
                            await Navigation.FlightorMove(new Vector3(196.902f, -163.4457f, 113.3596f));
                            
                            //
                            Navigator.PlayerMover.MoveTowards(new Vector3(208.712f, -165.4754f, 128.228f));

                            await Coroutine.Wait(10000, () => CommonBehaviors.IsLoading);
                            Navigator.Stop();
                            await Coroutine.Sleep(1000);

                            if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                            while (!hunt.IsFinished)
                            {
                                if (await FindAndKillMob(hunt.NpcID))
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
                        }
                        else if (umbra.Contains(hunt.HuntTarget))
                        {
                            await Navigation.GetToIslesOfUmbra();
                            if (await Navigation.GetTo(hunt.MapId, hunt.Location))
                            {
                                while (!hunt.IsFinished)
                                {
                                    if (await FindAndKillMob(hunt.NpcID))
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
                            }
                        }
                        else if (await Navigation.GetTo(hunt.MapId, hunt.Location))
                        {
                            while (!hunt.IsFinished)
                            {
                                if (await FindAndKillMob(hunt.NpcID))
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
                        }

                        Log($"Done: {hunt}");
                        await Coroutine.Sleep(1000);
                    }
                }
            }
            
            while (Core.Me.InCombat)
            {
                var target = GameObjectManager.Attackers.FirstOrDefault();
                if (target != default(BattleCharacter) && target.IsValid && target.IsAlive)
                {
                    await Navigation.GetTo(WorldManager.ZoneId, target.Location);
                    await KillMob(target);
                }
            }
        }

        public static async Task<bool> FindAndKillMob(uint NpcId)
        {
            while (Core.Me.InCombat)
            {
                var target = GameObjectManager.Attackers.FirstOrDefault();
                if (target != default(BattleCharacter) && target.IsValid && target.IsAlive)
                {
                    await Navigation.GetTo(WorldManager.ZoneId, target.Location);
                    await KillMob(target);
                }
            }
            var mob = GameObjectManager.GetObjectsOfType<BattleCharacter>(true).Where(i => i.NpcId == NpcId && i.IsValid && i.IsAlive).OrderBy(r => r.Distance()).FirstOrDefault();

            if (mob == default(BattleCharacter))
            {
                LogCritical($"Couldn't find mob with NPCID {NpcId}");
                return false;
            }
            else
            {
                LogSucess($"Found mob {mob}");
                await Navigation.GetTo(WorldManager.ZoneId, mob.Location);
                await KillMob(mob);
                LogSucess($"Did we kill it? {!(mob.IsValid && mob.IsAlive)}");
                return true;
            }
        }

        public static async Task KillMob(BattleCharacter mob)
        {
            if (!mob.IsValid) return;
            var test = new Poi(mob, PoiType.Kill);
            Poi.Current = test;


           // var combat = CombatCoroutine();

            while (mob.IsValid && mob.IsAlive && Poi.Current != null && Poi.Current.Unit != null)
            {
                LogCritical("Looping combat");
                await CombatCoroutine().ExecuteCoroutine();
                await Coroutine.Yield();
            }
        }

        private static Composite CombatCoroutine()
        {
            return new PrioritySelector(new Decorator(object_0 => !InFight && !Core.Me.IsDead, new PrioritySelector(new HookExecutor("Rest", "", new ActionAlwaysFail()), new HookExecutor("PreCombatBuff", "", new ActionAlwaysFail()))),
                                        new Decorator(object_0 => Core.Me.IsDead || Poi.Current == null || Poi.Current.BattleCharacter == null, new Action(delegate { Poi.Clear("Invalid Combat Poi"); })),
                                        new Decorator(object_0 => !Poi.Current.Unit.IsValid || Poi.Current.BattleCharacter.IsDead || Poi.Current.Unit.IsFateGone,
                                                      new Action(delegate
                                                      {
                                                          Poi.Clear("Targeted unit is dead, clearing Poi and carrying on!");
                                                          return RunStatus.Failure;
                                                      })),
                                        new Decorator(object_0 => Poi.Current.Unit.Pointer != Core.Me.PrimaryTargetPtr && Poi.Current.Unit.Distance() < 30f, new Action(delegate { Poi.Current.Unit.Target(); })),
                                        new Decorator(object_0 => Core.Me.PrimaryTargetPtr == IntPtr.Zero , new Action(r => Navigation.OffMeshMoveInteract(Poi.Current.Unit).Wait())),
                                        new HookExecutor("PreCombatLogic"),
                                        new Decorator(object_0 => Core.Me.PrimaryTargetPtr != IntPtr.Zero,
                                                      new PrioritySelector(new Decorator(object_0 => Core.Player.IsMounted &&
                                                                                                     Core.Target.Location.Distance2D(Core.Player.Location) < Core.Me.CombatReach + RoutineManager.Current.PullRange,
                                                                                         new Action(delegate
                                                                                         {
                                                                                             ActionManager.Dismount();
                                                                                             Navigator.Stop();
                                                                                         })),
                                                                           new Decorator(object_0 => !Core.Me.InCombat || (!Core.Me.InCombat && !Poi.Current.BattleCharacter.InCombat), new HookExecutor("Pull", "Run when pulling a mob to kill.", RoutineManager.Current.PullBehavior)),
                                                                           new Decorator(object_0 => Core.Me.InCombat,
                                                                                         new PrioritySelector(new Decorator(object_0 => PostCombatDelay > 0f && !bool_0,
                                                                                                                            new Action(delegate
                                                                                                                            {
                                                                                                                                bool_0 = true;
                                                                                                                                return RunStatus.Failure;
                                                                                                                            })),
                                                                                                              new Decorator(object_0 => !Poi.Current.BattleCharacter.InCombat &&
                                                                                                                                        Poi.Current.TimeSet + TimeSpan.FromSeconds(10.0) < DateTime.Now,
                                                                                                                            new Action(delegate
                                                                                                                            {
                                                                                                                                Poi
                                                                                                                                    .Clear("I'm in combat, but POI isn't and it has been atleast 10 seconds. Clearing POI and picking up a new target.");
                                                                                                                            })),
                                                                                                              new Decorator(object_0 => Poi.Current.BattleCharacter.IsDead,
                                                                                                                            new Action(delegate { Poi.Clear("I'm in combat, but POI is dead. Clearing POI and picking up a new target."); })),
                                                                                                              new HookExecutor("RoutineCombat", "Executes the current routine's Combat behavior", BrainBehavior.CombatLogic))))),
                                        new Action(object_0 => RunStatus.Success));
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