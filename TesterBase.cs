using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using ff14bot.RemoteWindows.GoldSaucer;
using Generate;
using GreyMagic;
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
using static LlamaLibrary.Retainers.HelperFunctions;
using Action = System.Action;

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

        private static List<FcActionShopItem> FcShopActions = new List<FcActionShopItem>()
        {
            new FcActionShopItem(1, 5, 3010, 0, "The Heat of Battle"),
            new FcActionShopItem(2, 5, 3010, 1, "Earth and Water"),
            new FcActionShopItem(3, 5, 3010, 2, "Helping Hand"),
            new FcActionShopItem(4, 5, 2408, 3, "A Man's Best Friend"),
            new FcActionShopItem(5, 5, 2408, 4, "Mark Up"),
            new FcActionShopItem(6, 5, 2408, 5, "Seal Sweetener"),
            new FcActionShopItem(17, 5, 2408, 6, "Jackpot"),
            new FcActionShopItem(7, 5, 1505, 7, "Brave New World"),
            new FcActionShopItem(8, 5, 2408, 8, "Live off the Land"),
            new FcActionShopItem(9, 5, 2408, 9, "What You See"),
            new FcActionShopItem(10, 5, 2408, 10, "Eat from the Hand"),
            new FcActionShopItem(11, 5, 2408, 11, "In Control"),
            new FcActionShopItem(12, 5, 3010, 12, "That Which Binds Us"),
            new FcActionShopItem(13, 5, 2408, 13, "Meat and Mead"),
            new FcActionShopItem(14, 5, 3010, 14, "Proper Care"),
            new FcActionShopItem(15, 5, 2408, 15, "Back on Your Feet"),
            new FcActionShopItem(16, 5, 3010, 16, "Reduced Rates"),
            new FcActionShopItem(31, 8, 6020, 17, "The Heat of Battle II"),
            new FcActionShopItem(32, 8, 6020, 18, "Earth and Water II"),
            new FcActionShopItem(33, 8, 6020, 19, "Helping Hand II"),
            new FcActionShopItem(34, 8, 6020, 20, "A Man's Best Friend II"),
            new FcActionShopItem(35, 8, 6020, 21, "Mark Up II"),
            new FcActionShopItem(36, 8, 6020, 22, "Seal Sweetener II"),
            new FcActionShopItem(47, 8, 6020, 23, "Jackpot II"),
            new FcActionShopItem(37, 8, 3010, 24, "Brave New World II"),
            new FcActionShopItem(38, 8, 6020, 25, "Live off the Land II"),
            new FcActionShopItem(39, 8, 6020, 26, "What You See II"),
            new FcActionShopItem(40, 8, 6020, 27, "Eat from the Hand II"),
            new FcActionShopItem(41, 8, 6020, 28, "In Control II"),
            new FcActionShopItem(42, 8, 6020, 29, "That Which Binds Us II"),
            new FcActionShopItem(43, 8, 6020, 30, "Meat and Mead II"),
            new FcActionShopItem(44, 8, 6020, 31, "Proper Care II"),
            new FcActionShopItem(45, 8, 4816, 32, "Back on Your Feet II"),
            new FcActionShopItem(46, 8, 6020, 33, "Reduced Rates II")
        };

        private static Dictionary<uint, string> FcActionList = new Dictionary<uint, string>()
        {
            {1, "The Heat of Battle"},
            {2, "Earth and Water"},
            {3, "Helping Hand"},
            {4, "A Man's Best Friend"},
            {5, "Mark Up"},
            {6, "Seal Sweetener"},
            {7, "Brave New World"},
            {8, "Live off the Land"},
            {9, "What You See"},
            {10, "Eat from the Hand"},
            {11, "In Control"},
            {12, "That Which Binds Us"},
            {13, "Meat and Mead"},
            {14, "Proper Care"},
            {15, "Back on Your Feet"},
            {16, "Reduced Rates"},
            {17, "Jackpot"},
            {31, "The Heat of Battle II"},
            {32, "Earth and Water II"},
            {33, "Helping Hand II"},
            {34, "A Man's Best Friend II"},
            {35, "Mark Up II"},
            {36, "Seal Sweetener II"},
            {37, "Brave New World II"},
            {38, "Live off the Land II"},
            {39, "What You See II"},
            {40, "Eat from the Hand II"},
            {41, "In Control II"},
            {42, "That Which Binds Us II"},
            {43, "Meat and Mead II"},
            {44, "Proper Care II"},
            {45, "Back on Your Feet II"},
            {46, "Reduced Rates II"},
            {47, "Jackpot II"},
            {61, "The Heat of Battle III"},
            {62, "Earth and Water III"},
            {63, "Helping Hand III"},
            {64, "A Man's Best Friend III"},
            {65, "Mark Up III"},
            {66, "Seal Sweetener III"},
            {68, "Live off the Land III"},
            {69, "What You See III"},
            {70, "Eat from the Hand III"},
            {71, "In Control III"},
            {72, "That Which Binds Us III"},
            {73, "Meat and Mead III"},
            {74, "Proper Care III"},
            {76, "Reduced Rates III"},
            {77, "Jackpot III"},
        };

        private readonly SortedDictionary<string, List<string>> luaFunctions = new SortedDictionary<string, List<string>>();


        private volatile bool _init;
        private Composite _root;

        public Dictionary<string, List<Composite>> hooks;


        public TesterBase()
        {
            Task.Factory.StartNew(() =>
            {
                init();
                _init = true;
                Log("INIT DONE");
            });
        }

        private bool IsJumping => Core.Memory.NoCacheRead<byte>(Offsets.Conditions + Offsets.JumpingCondition) != 0;

        public override string Name => "Tester";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = true;


        public override void OnButtonPress()
        {
            DumpLuaFunctions();
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

            /*List<(int gardenIndex, int plantIndex, string plant)> plants = new List<(int gardenIndex, int plantIndex, string plant)>();
            foreach (var plant in GardenManager.Plants.Where(i=> i.Distance(Core.Me.Location)< 10))
            {
               var GardenIndex = Lua.GetReturnVal<int>($"return _G['{plant.LuaString}']:GetHousingGardeningIndex();");
               var Plant = DataManager.GetItem(Lua.GetReturnVal<uint>($"return _G['{plant.LuaString}']:GetHousingGardeningPlantCrop();"));
               var PlantIndex = Lua.GetReturnVal<int>($"return _G['{plant.LuaString}']:GetHousingGardeningPlantIndex();");
               plants.Add((GardenIndex,PlantIndex,Plant.CurrentLocaleName));
               
            }
            
            foreach (var plantgroup in plants.GroupBy(i=> i.gardenIndex))
            {
                foreach (var plant in plantgroup.OrderBy(j=> j.plantIndex))
                {
                    Log($"Garden {plant.gardenIndex} Plant {plant.plantIndex}, {plant.plant}");
                }
            }*/

            _root = new ActionRunCoroutine(r => Run());
        }

        private static async Task Plant()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            BagSlot soil = ff14bot.Managers.InventoryManager.FilledInventoryAndArmory.First(x => x.RawItemId == 16026);
            BagSlot seeds = ff14bot.Managers.InventoryManager.FilledInventoryAndArmory.First(x => x.RawItemId == 13765);

            //EventObject plant = null;
            List<(int gardenIndex, int plantIndex, string plant, EventObject obj)> plants = new List<(int gardenIndex, int plantIndex, string plant, EventObject obj)>();
            foreach (var plant in GardenManager.Plants.Where(i => i.Distance(Core.Me.Location) < 10))
            {
                var GardenIndex = Lua.GetReturnVal<int>($"return _G['{plant.LuaString}']:GetHousingGardeningIndex();");
                var Plant = DataManager.GetItem(Lua.GetReturnVal<uint>($"return _G['{plant.LuaString}']:GetHousingGardeningPlantCrop();"));
                var PlantIndex = Lua.GetReturnVal<int>($"return _G['{plant.LuaString}']:GetHousingGardeningPlantIndex();");
                plants.Add((GardenIndex, PlantIndex, Plant.CurrentLocaleName, plant));
            }

            foreach (var plant in plants.Where(i => i.gardenIndex == 0))
            {
                if (plant.gardenIndex == 0 && plant.plant == "")
                {
                    Log($"Garden {plant.gardenIndex} Plant {plant.plantIndex}, {plant.plant}");
                    await GardenHelper.Plant(plant.obj, seeds, soil);
                    await Coroutine.Sleep(5000);
                }
            }

            TreeRoot.Stop("Stop Requested");
        }

        public override void Stop()
        {
            _root = null;
        }

        public static async Task InteractWithDenys(int selectString)
        {
            var npc = GameObjectManager.GetObjectByNPCId(1032900);
            if (npc == null)
            {
                await Navigation.GetTo(418, new Vector3(-160.28f, 17.00897f, -55.8437f));
                npc = GameObjectManager.GetObjectByNPCId(1032900);
            }

            if (npc != null && !npc.IsWithinInteractRange)
            {
                await Navigation.GetTo(418, new Vector3(-160.28f, 17.00897f, -55.8437f));
            }

            if (npc != null && npc.IsWithinInteractRange)
            {
                npc.Interact();
                await Coroutine.Wait(10000, () => Conversation.IsOpen);
                if (Conversation.IsOpen)
                {
                    Conversation.SelectLine((uint) selectString);
                }
            }
        }

        public static async Task TurninSkySteelGathering()
        {
            var GatheringItems = new Dictionary<uint, (uint Reward, uint Cost)>
            {
                {31125, (30331, 10)},
                {31130, (30333, 10)},
                {31127, (30335, 10)},
                {31132, (30337, 10)},
                {31129, (30339, 10)},
                {31134, (30340, 10)}
            };

            var turninItems = InventoryManager.FilledSlots.Where(i => i.IsHighQuality && GatheringItems.Keys.Contains(i.RawItemId));

            if (turninItems.Any())
            {
                await InteractWithDenys(3);
                await Coroutine.Wait(10000, () => ShopExchangeItem.Instance.IsOpen);
                if (ShopExchangeItem.Instance.IsOpen)
                {
                    Log($"Window Open");
                    foreach (var turnin in turninItems)
                    {
                        var reward = GatheringItems[turnin.RawItemId].Reward;
                        var amt = turnin.Count / GatheringItems[turnin.RawItemId].Cost;
                        Log($"Buying {amt}x{DataManager.GetItem(reward).CurrentLocaleName}");
                        await ShopExchangeItem.Instance.Purchase(reward, amt);
                        await Coroutine.Sleep(500);
                    }

                    ShopExchangeItem.Instance.Close();
                    await Coroutine.Wait(10000, () => !ShopExchangeItem.Instance.IsOpen);
                }
            }
        }

        public static async Task TurninSkySteelCrafting()
        {
            Dictionary<uint, CraftingRelicTurnin> TurnItemList = new Dictionary<uint, CraftingRelicTurnin>
            {
                {31101, new CraftingRelicTurnin(31101, 0, 1, 2000, 30315)},
                {31109, new CraftingRelicTurnin(31109, 0, 0, 3000, 30316)},
                {31102, new CraftingRelicTurnin(31102, 1, 1, 2000, 30317)},
                {31110, new CraftingRelicTurnin(31110, 1, 0, 3000, 30318)},
                {31103, new CraftingRelicTurnin(31103, 2, 1, 2000, 30319)},
                {31111, new CraftingRelicTurnin(31111, 2, 0, 3000, 30320)},
                {31104, new CraftingRelicTurnin(31104, 3, 1, 2000, 30321)},
                {31112, new CraftingRelicTurnin(31112, 3, 0, 3000, 30322)},
                {31105, new CraftingRelicTurnin(31105, 4, 1, 2000, 30323)},
                {31113, new CraftingRelicTurnin(31113, 4, 0, 3000, 30324)},
                {31106, new CraftingRelicTurnin(31106, 5, 1, 2000, 30325)},
                {31114, new CraftingRelicTurnin(31114, 5, 0, 3000, 30326)},
                {31107, new CraftingRelicTurnin(31107, 6, 1, 2000, 30327)},
                {31115, new CraftingRelicTurnin(31115, 6, 0, 3000, 30328)},
                {31108, new CraftingRelicTurnin(31108, 7, 1, 2000, 30329)},
                {31116, new CraftingRelicTurnin(31116, 7, 0, 3000, 30330)}
            };

            var collectables = InventoryManager.FilledSlots.Where(i => i.IsCollectable).Select(x => x.RawItemId).Distinct();
            var collectablesAll = InventoryManager.FilledSlots.Where(i => i.IsCollectable);

            if (collectables.Any(i => TurnItemList.Keys.Contains(i)))
            {
                Log("Have collectables");
                foreach (var collectable in collectablesAll)
                {
                    if (TurnItemList.Keys.Contains(collectable.RawItemId))
                    {
                        var turnin = TurnItemList[collectable.RawItemId];
                        if (collectable.Collectability < turnin.MinCollectability)
                        {
                            Log($"Discarding {collectable.Name} is at {collectable.Collectability} which is under {turnin.MinCollectability}");
                            collectable.Discard();
                        }
                    }
                }

                collectables = InventoryManager.FilledSlots.Where(i => i.IsCollectable).Select(x => x.RawItemId).Distinct();

                await InteractWithDenys(2);
                await Coroutine.Wait(10000, () => CollectablesShop.Instance.IsOpen);


                if (CollectablesShop.Instance.IsOpen)
                {
                    // Log("Window open");
                    foreach (var item in collectables)
                    {
                        Log($"Turning in {DataManager.GetItem(item).CurrentLocaleName}");
                        var turnin = TurnItemList[item];

                        // Log($"Pressing job {turnin.Job}");
                        CollectablesShop.Instance.SelectJob(turnin.Job);
                        await Coroutine.Sleep(500);
                        //  Log($"Pressing position {turnin.Position}");
                        CollectablesShop.Instance.SelectItem(turnin.Position);
                        await Coroutine.Sleep(1000);
                        int i = 0;
                        while (CollectablesShop.Instance.TurninCount > 0)
                        {
                            // Log($"Pressing trade {i}");
                            i++;
                            CollectablesShop.Instance.Trade();
                            await Coroutine.Sleep(100);
                        }
                    }

                    CollectablesShop.Instance.Close();
                    await Coroutine.Wait(10000, () => !CollectablesShop.Instance.IsOpen);
                }
            }
        }

        private async Task<bool> Run()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            var DeliveryNpcs = new Dictionary<uint, (uint Zone, Vector3 location, string name, int requiredQuest, uint index)>
            {
                {1019615, (478, new Vector3(-71.68203f, 206.5714f, 29.38501f), "Zhloe Aliapoh", 67087, 1)}, //(Zhloe Aliapoh) Idyllshire(Dravania) 
                {1020337, (635, new Vector3(171.312988f, 13.02367f, -89.951965f), "M'naago", 68541, 2)}, //(M'naago) Rhalgr's Reach(Gyr Abania) 
                {1025878, (613, new Vector3(343.984009f, -120.329468f, -306.019714f), "Kurenai", 68675, 3)}, //(Kurenai) The Ruby Sea(Othard) 
                {1018393, (478, new Vector3(-62.3016f, 206.6002f, 23.893f), "Adkiragh", 68713, 4)}, //(Adkiragh) Idyllshire(Dravania) 
                {1031801, (820, new Vector3(52.811401f, 82.993774f, -65.384949f), "Kai-Shirr", 69265, 5)}, //(Kai-Shirr) Eulmore(Eulmore) 
                {1033543, (886, new Vector3(113.389771f, -20.004639f, -0.961365f), "Ehll Tou", 69425, 6)} //(Ehll Tou) The Firmament(Ishgard) 
            };

            foreach (var npc in DeliveryNpcs.Where(i => ConditionParser.IsQuestCompleted(i.Value.requiredQuest)).OrderByDescending(i=> i.Value.index))
            {
                await AgentSatisfactionSupply.Instance.LoadWindow(npc.Value.index);
                List<uint> items = new List<uint>();
                Log($"{DeliveryNpcs[AgentSatisfactionSupply.Instance.NpcId].name}");
                Log($"\tHeartLevel:{AgentSatisfactionSupply.Instance.HeartLevel}");
                Log($"\tRep:{AgentSatisfactionSupply.Instance.CurrentRep}/{AgentSatisfactionSupply.Instance.MaxRep}");
                Log($"\tDeliveries Remaining:{AgentSatisfactionSupply.Instance.DeliveriesRemaining}");
                Log($"\tDoH: {DataManager.GetItem(AgentSatisfactionSupply.Instance.DoHItemId)}");
                items.Add(AgentSatisfactionSupply.Instance.DoHItemId);
                Log($"\tDoL: {DataManager.GetItem(AgentSatisfactionSupply.Instance.DoLItemId)}");
                items.Add(AgentSatisfactionSupply.Instance.DoLItemId);
                Log($"\tFsh: {DataManager.GetItem(AgentSatisfactionSupply.Instance.FshItemId)}");
                items.Add(AgentSatisfactionSupply.Instance.FshItemId);

                if (AgentSatisfactionSupply.Instance.HeartLevel == 5 || AgentSatisfactionSupply.Instance.DeliveriesRemaining == 0)
                {
                    Log($"{DeliveryNpcs[AgentSatisfactionSupply.Instance.NpcId].name} Satisfaction Level is Maxed or out of deliveries, skipping");
                    continue;
                }

                List<LisbethOrder> outList = new List<LisbethOrder>();

                if (npc.Key == 1025878)
                {
                    outList.Add(new LisbethOrder(0,1,(int) AgentSatisfactionSupply.Instance.DoLItemId, Math.Min(3,(int)AgentSatisfactionSupply.Instance.DeliveriesRemaining),"Gather", true));
                }
                else
                {
                    outList.Add(new LisbethOrder(0,1,(int) AgentSatisfactionSupply.Instance.DoHItemId,Math.Min(3,(int)AgentSatisfactionSupply.Instance.DeliveriesRemaining),"Carpenter", true));
                }

                var order = JsonConvert.SerializeObject(outList, Formatting.None).Replace("Hq","Collectable");

                if (order != "")
                {
                    Log($"Calling Lisbeth with {order}");
                    await Lisbeth.ExecuteOrdersIgnoreHome(order);
                }
                
                if (InventoryManager.FilledSlots.Any(i => items.Contains(i.RawItemId)) && AgentSatisfactionSupply.Instance.DeliveriesRemaining > 0)
                {
                    Log("Have items to turn in");
                    await HandInCustomNpc(npc.Key, (npc.Value.Zone, npc.Value.location));
                }

                /*if (AgentSatisfactionSupply.Instance.DeliveriesRemaining == 0)
                {
                    Log("Out of delivery allowances");
                    break;
                }*/
            }

            TreeRoot.Stop("Stop Requested");
            return true;
        }

       private async Task<bool> HandInCustomNpc(uint npcID, (uint Zone, Vector3 location) npcLocation)
        {
            var npc = GameObjectManager.GetObjectByNPCId(npcID);

            if (npc == default(GameObject) || !npc.IsWithinInteractRange)
            {
                await Navigation.GetTo(npcLocation.Zone, npcLocation.location);
                npc = GameObjectManager.GetObjectByNPCId(npcID);
            }

            if (npc == default(GameObject)) return false;

            npc.Interact();

            await Coroutine.Wait(10000, () => Talk.DialogOpen);

            if (!Talk.DialogOpen)
            {
                npc.Interact();

                await Coroutine.Wait(10000, () => Talk.DialogOpen);
            }

            while (Talk.DialogOpen)
            {
                Talk.Next();
                await Buddy.Coroutines.Coroutine.Sleep(200);
                await Coroutine.Yield();
            }

            await Coroutine.Wait(10000, () => Conversation.IsOpen);
            await Buddy.Coroutines.Coroutine.Sleep(500);

            Logging.WriteDiagnostic("Choosing 'Make a delivery.'");
            Conversation.SelectLine(0);
            await Buddy.Coroutines.Coroutine.Wait(1000, () => Talk.DialogOpen);

            if (Talk.DialogOpen)
                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Buddy.Coroutines.Coroutine.Sleep(200);
                    await Coroutine.Yield();
                }

            await Coroutine.Wait(10000, () => SatisfactionSupply.Instance.IsOpen);

            if (SatisfactionSupply.Instance.IsOpen)
            {
                do
                {
                    Logging.WriteDiagnostic("Turning in items");

                    if (AgentSatisfactionSupply.Instance.DeliveriesRemaining < 1) break;

                    if (AgentSatisfactionSupply.Instance.HasDoHTurnin)
                        SatisfactionSupply.Instance.ClickItem(0);
                    else if (AgentSatisfactionSupply.Instance.HasDoLTurnin)
                        SatisfactionSupply.Instance.ClickItem(1);
                    else if (AgentSatisfactionSupply.Instance.HasFshTurnin)
                        SatisfactionSupply.Instance.ClickItem(2);

                    await Coroutine.Wait(10000, () => Request.IsOpen);

                    Logging.WriteDiagnostic("Selecting items.");
                    await CommonTasks.HandOverRequestedItems();
                    
                    while (!SatisfactionSupply.Instance.IsOpen && !QuestLogManager.InCutscene)
                    {
                        if (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                        }
                        await Buddy.Coroutines.Coroutine.Sleep(500);
                    }

                    if (QuestLogManager.InCutscene)
                    {
                        while (!SatisfactionSupplyResult.Instance.IsOpen && QuestLogManager.InCutscene)
                        {
                            Logging.WriteDiagnostic("Dealing with cutscene.");
                            if (QuestLogManager.InCutscene && AgentCutScene.Instance.CanSkip)
                            {
                                AgentCutScene.Instance.PromptSkip();
                                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                                if (SelectString.IsOpen) SelectString.ClickSlot(0);
                            }
                            if (Talk.DialogOpen)
                            {
                                Talk.Next();
                                await Buddy.Coroutines.Coroutine.Sleep(200);
                            }
                            await Buddy.Coroutines.Coroutine.Sleep(500);
                        }

                        if (SatisfactionSupplyResult.Instance.IsOpen)
                        {
                            Logging.WriteDiagnostic("Clicking Accept.");
                            SatisfactionSupplyResult.Instance.Confirm();
                        }

                        await Coroutine.Wait(3000, () => Talk.DialogOpen);
                        while (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Coroutine.Wait(200, () => !Talk.DialogOpen);
                            await Coroutine.Wait(500, () => Talk.DialogOpen);
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                            await Coroutine.Yield();
                        } 
                        await Buddy.Coroutines.Coroutine.Sleep(500);
                        await Coroutine.Wait(3000, () => Talk.DialogOpen);
                        while (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Coroutine.Wait(200, () => !Talk.DialogOpen);
                            await Coroutine.Wait(500, () => Talk.DialogOpen);
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                            await Coroutine.Yield();
                        }
                        await Buddy.Coroutines.Coroutine.Sleep(500);
                        await Coroutine.Wait(3000, () => Talk.DialogOpen);
                        while (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Coroutine.Wait(200, () => !Talk.DialogOpen);
                            await Coroutine.Wait(500, () => Talk.DialogOpen);
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                            await Coroutine.Yield();
                        }

                        break;
                    }
                    
                    await Coroutine.Wait(5000, () => SatisfactionSupply.Instance.IsOpen);
                    if (!SatisfactionSupply.Instance.IsOpen) break;
                }
                while (AgentSatisfactionSupply.Instance.DeliveriesRemaining > 0 && AgentSatisfactionSupply.Instance.HasAnyTurnin);
            }

            if (SatisfactionSupply.Instance.IsOpen)
            {
                SatisfactionSupply.Instance.Close();
                await Coroutine.Wait(10000, () => !SatisfactionSupply.Instance.IsOpen);
            }
            await Coroutine.Wait(1000, () => Conversation.IsOpen);
            if (Conversation.IsOpen)
            {
                Conversation.SelectLine((uint) (Conversation.GetConversationList.Count -1));
            }
            
            return true;
        }


        public static async Task<bool> GoGarden(uint AE)
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            var house = WorldManager.AvailableLocations.FirstOrDefault(i => i.AetheryteId == AE);

            Log($"Teleporting to housing: (ZID: {house.ZoneId}, AID: {house.AetheryteId}) {house.Name}");
            await CommonTasks.Teleport(house.AetheryteId);

            Log("Waiting for zone to change");
            await Coroutine.Wait(20000, () => WorldManager.ZoneId == house.ZoneId);

            Log("Getting closest gardening plot");

            var gardenPlot = GardenManager.Plants.FirstOrDefault();
            if (gardenPlot != null)
            {
                Log("Found nearby gardening plot, approaching");
                await Navigation.FlightorMove(gardenPlot.Location);
                //await GardenHelper.Main(); 
            }

            return true;
        }

        public static async Task HandInExpert()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

            if (!GrandCompanySupplyList.Instance.IsOpen)
            {
                await GrandCompanyHelper.InteractWithNpc(GCNpc.Personnel_Officer);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                if (!SelectString.IsOpen)
                {
                    Log("Window is not open...maybe it didn't get to npc?");
                }

                SelectString.ClickSlot(0);
                await Coroutine.Wait(5000, () => GrandCompanySupplyList.Instance.IsOpen);
                if (!GrandCompanySupplyList.Instance.IsOpen)
                {
                    Log("Window is not open...maybe it didn't get to npc?");
                }
            }

            if (GrandCompanySupplyList.Instance.IsOpen)
            {
                await GrandCompanySupplyList.Instance.SwitchToExpertDelivery();
                await Coroutine.Sleep(3000);
                //await HandleCurrentGCWindow();

                /*
                var bools = GrandCompanySupplyList.Instance.GetTurninBools();
                var windowItemIds = GrandCompanySupplyList.Instance.GetTurninItemsIds();
                var required = GrandCompanySupplyList.Instance.GetTurninRequired();
                var maxSeals = Core.Me.MaxGCSeals();*/
                //var items = Core.Memory.ReadArray<GCTurninItem>(Offsets.GCTurnin, Offsets.GCTurninCount);
                int i = 0;
                int count = ConditionParser.ItemCount(2049);
                if (count > 0)
                    for (var index = 0; index < count; index++)
                    {
                        //var item = windowItemIds[index];
                        Log($"{index}");
                        GrandCompanySupplyList.Instance.ClickItem(0);
                        await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                        if (SelectYesno.IsOpen)
                        {
                            SelectYesno.Yes();
                        }

                        await Coroutine.Wait(5000, () => GrandCompanySupplyReward.Instance.IsOpen);
                        GrandCompanySupplyReward.Instance.Confirm();
                        await Coroutine.Wait(5000, () => GrandCompanySupplyList.Instance.IsOpen);
                        i += 1;
                        await Coroutine.Sleep(500);
                    }

                if (GrandCompanySupplyList.Instance.IsOpen)
                {
                    GrandCompanySupplyList.Instance.Close();
                    await Coroutine.Wait(5000, () => SelectString.IsOpen);
                    if (SelectString.IsOpen)
                    {
                        SelectString.ClickSlot((uint) (SelectString.LineCount - 1));
                    }
                }

                if (Core.Me.GCSeals() > 200)
                {
                    await GrandCompanyShop.BuyKnownItem(21072, (int) (Core.Me.GCSeals() / 200));
                }
            }
        }

        public static async Task UpdateWebPage()
        {
            string path = @"U:\www\template.php";
            string path1 = @"U:\www\index.html";

            // Open the file to read from.
            string readText = File.ReadAllText(path);
            //Console.WriteLine(readText);
            int gil = (int) InventoryManager.GetBagByInventoryBagId(InventoryBagId.Currency).FilledSlots.First(i => i.RawItemId == 1).Count;
            int tomes = (int) InventoryManager.GetBagByInventoryBagId(InventoryBagId.Currency).First(i => i.RawItemId == 40).Count;
            var zone = WorldManager.CurrentZoneName;
            readText = readText.Replace("{Gil}", $"{gil:N0}");
            readText = readText.Replace("{Character_Name}", Core.Player.Name);
            readText = readText.Replace("{class}", Core.Player.CurrentJob.ToString());
            readText = readText.Replace("{Zone}", zone);
            readText = readText.Replace("{Tomes}", tomes.ToString("N0"));
            string ventureTr = @"				<tr>
					<td style=""width: 17.2083%;""><span style=""font-size: 24px;"">Ret</span></td>
                <td style=""width: 40.7417%; text-align: center;""><span style=""font-size: 24px;"">item</span></td>
                <td style=""width: 41.525%;""><span style=""font-size: 24px;"">time</span></td>
                </tr>";

            string ventureTr1 = @"				<tr>
				<td>Ret</td>
                <td>item</td>
                <td>time</td>
                </tr>";
            string retainerTable = "";
            var count = await GetNumberOfRetainers();
            var retainers = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);
            var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            foreach (var ret in retainers)
            {
                var ventureTimeLeft = ret.VentureEndTimestamp - now;
                var ventureDone = ventureTimeLeft <= 0;
                var ventureName = VentureData.First(i => i.Id == ret.VentureTask).Name;
                if (ventureDone)
                {
                    if (ret.VentureTask != 0)
                    {
                        Log($"{ret.Name} - {ventureName} - Finished\n");
                        retainerTable += ventureTr.Replace("Ret", ret.Name).Replace("item", ventureName).Replace("time", "Finished") + "\n";
                    }
                    else
                    {
                        Log($"{ret.Name}\n");
                    }
                }
                else
                {
                    Log($"{ret.Name} - {ventureName} - {(ret.VentureEndTimestamp - UnixTimestamp) / 60} minutes\n");
                    retainerTable += ventureTr.Replace("Ret", ret.Name).Replace("item", ventureName).Replace("time", $"{(ret.VentureEndTimestamp - UnixTimestamp) / 60} minutes") + "\n";
                }
            }

            readText = readText.Replace("{Retainers}", retainerTable);
            //ActorController.Player
            File.WriteAllText(path1, readText);
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

            var entranceIds = new uint[] {houseEntranceId, aptEntranceId};

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


        private async Task TestMathma()
        {
            await Coroutine.Wait(5000, () => SelectIconString.IsOpen);
            await Coroutine.Sleep(500);

            if (SelectIconString.IsOpen)
            {
                Logging.WriteDiagnostic("SelectIconString Open. Clicking Mahatma Exchange.");
                SelectIconString.ClickSlot(0);
            }
            else
            {
                Logging.WriteDiagnostic("SelectIconString Failed to open.");
            }


            uint Slot = (uint) ScriptConditions.Helpers.ZodiacCompletedMahatma();
            await Coroutine.Sleep(1000);

            GameObjectManager.GetObjectByNPCId(1011791).Interact();

            await Coroutine.Wait(5000, () => SelectIconString.IsOpen);
            await Coroutine.Sleep(500);

            if (SelectIconString.IsOpen)
            {
                Logging.WriteDiagnostic("SelectIconString Open. Clicking Mahatma Exchange.");
                SelectIconString.ClickSlot(0);
            }

            await Coroutine.Wait(5000, () => DialogOpen);
            await Coroutine.Sleep(500);
            if (await Coroutine.Wait(1000, () => DialogOpen))
            {
                Next();
            }

            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            await Coroutine.Sleep(500);
            if (SelectString.IsOpen)
            {
                Logging.WriteDiagnostic("SelectString Open. Choosing weapon.");
                SelectString.ClickSlot(0);
                await Coroutine.Wait(5000, () => !SelectString.IsOpen);
            }

            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            await Coroutine.Sleep(500);
            if (SelectString.IsOpen)
            {
                Logging.WriteDiagnostic("Choosing next Mahatma.");
                SelectString.ClickSlot(Slot + 1);
            }

            await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
            await Coroutine.Sleep(500);
            if (SelectYesno.IsOpen)
            {
                Logging.WriteDiagnostic("Selecting Yes.");
                SelectYesno.ClickYes();
            }

            await Coroutine.Wait(5000, () => DialogOpen);
            await Coroutine.Sleep(500);
            while (DialogOpen)
            {
                Next();
                await Coroutine.Wait(2000, () => !DialogOpen);
                await Coroutine.Sleep(1000);
            }
        }


        private async Task BuyHouse()
        {
            Random _rnd = new Random();

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
                        IntPtr ptr = (IntPtr) t.GetValue(tp);
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
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                Log("Can't find open bell either you have none or not near a bell");
                return;
            }

            var count = await GetNumberOfRetainers();
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

            var result = patternFinder.Find("44 89 BF ?? ?? ?? ?? 83 BF ?? ?? ?? ?? ?? Add 3 Read32").ToInt32();
            //Log(result);
            uint[] npcs = {1029028, 1033777};

            var units = GameObjectManager.GameObjects;
            foreach (var unit in units.Where(i => i.Type == GameObjectType.EventNpc))
            {
                Log("Name:{0}, IconID: {1}", unit.Name, Core.Memory.Read<uint>(unit.Pointer + result));
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

        public static void OldRun()
        {
            /*await testKupoTickets();

   
   foreach (var item in InventoryManager.FilledSlots.Where(i=> i.EnglishName.ToLowerInvariant().Contains("magicked prism")))
   {
       Log($"Discarding {item.Name}");
       item.Discard();
       await Coroutine.Sleep(2000);
   }*/


            //await Helpers.Lisbeth.SelfRepair();
            //await Helpers.Lisbeth.SelfRepairWithMenderFallback();
            //ActionHelper.Test();
            //await testKupoTickets();

            /*var windows = new Dictionary<string, int>();
            if (File.Exists("windows.json"))
            {
                using (var file = new StreamReader("windows.json"))
                    windows = JsonConvert.DeserializeObject<Dictionary<string, int>>(file.ReadToEnd());
            }


            foreach (var control in RaptureAtkUnitManager.Controls)
            {
                if (!windows.ContainsKey(control.Name))
                {
                    AgentInterface agentInterface = null;
                    
                    try
                    {
                        agentInterface = control.TryFindAgentInterface();
                    }
                    catch
                    {
                        
                    }
                    int agentid = agentInterface?.Id ?? -1;
                    windows.Add(control.Name, agentid);
                    if (agentid != -1)
                        Log($"{control.Name} - Agent {agentid}");
                }
            }
            
            using (var outputFile = new StreamWriter($"agents.csv", false))
            {
                foreach (var window in windows.Where(i=> i.Value >0))
                {
                    outputFile.WriteLine($"{window.Key},{window.Value},{new IntPtr(AgentModule.AgentVtables[window.Value].ToInt64() - Core.Memory.ImageBase.ToInt64()).ToString("X")}");
                }
                
            }
            
            using (var outputFile = new StreamWriter($"windows.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(windows));
            }

            DumpOffsets();

            Lua.DoString("return _G['EventHandler'].NpcRepair();");*/


            /*
            InventoryBagId[] PlayerInventoryBagIds = new InventoryBagId[6]
            {
                InventoryBagId.Bag1,
                InventoryBagId.Bag2,
                InventoryBagId.Bag3,
                InventoryBagId.Bag4,
                InventoryBagId.Crystals,
                InventoryBagId.Currency
            };

            var targetPlayer = GameObjectManager.Target as BattleCharacter;

            var result = targetPlayer.OpenTradeWindow();

            uint itemid = 2;
            uint qty = 5;
            Log($"{result}");
            if (result == 0)
            {
                await Coroutine.Wait(5000, () => Trade.IsOpen);
                if (Trade.IsOpen)
                {
                    var item = InventoryManager.GetBagsByInventoryBagId(PlayerInventoryBagIds).SelectMany(i => i.FilledSlots).FirstOrDefault(i => i.RawItemId == itemid);
                    if (item != null)
                    {
                        item.TradeItem();
                        await Coroutine.Wait(5000, () => InputNumeric.IsOpen);
                        if (InputNumeric.IsOpen)
                        {
                            InputNumeric.Ok(qty); //pass nothing for full stack
                        }

                        RaptureAtkUnitManager.GetWindowByName("Trade").SendAction(1, 3uL, 0);
                        await Coroutine.Wait(-1, () => Trade.TradeStage == 5);
                        await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                        if (SelectYesno.IsOpen)
                            SelectYesno.Yes();
                    }
                }
            }
            */

            //await TurninSkySteelGathering();
            //await TurninSkySteelCrafting();


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
*/
            // var newHunts = JsonConvert.DeserializeObject<SortedDictionary<int, StoredHuntLocationLisbeth>>((new StreamReader("hunts.json")).ReadToEnd());


            /*
            var failed = new Dictionary<int, StoredHuntLocation>();
            
            if (File.Exists("hunts_failed.json"))
                failed = JsonConvert.DeserializeObject<Dictionary<int, StoredHuntLocation>>((new StreamReader("hunts_failed.json")).ReadToEnd());
            var start = 76; 
            foreach (var hunt in HuntHelper.DailyHunts.Where(i=> i.Key >= start))
            {
                await Lisbeth.TravelToZones(hunt.Value.Map, hunt.Value.Location);
                if (WorldManager.ZoneId != hunt.Value.Map || Core.Me.Location.DistanceSqr(hunt.Value.Location) > 30)
                {
                    Log($"Map: {WorldManager.ZoneId} ({hunt.Value.Map}) Dist: {Core.Me.Location.DistanceSqr(hunt.Value.Location)}");
                    if (!failed.ContainsKey(hunt.Key))
                    {
                        failed.Add(hunt.Key, hunt.Value);
                        using (var outputFile = new StreamWriter($"hunts_failed.json", false))
                        {
                            outputFile.Write(JsonConvert.SerializeObject(failed));
                        }
                    }
                }
                Log($"Finished {start}");
                start++;
            }
            using (var outputFile = new StreamWriter($"hunts_failed.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(failed));
            }
            */


            //Log($"{Application.ProductVersion} - {Assembly.GetEntryAssembly().GetName().Version.Revision} - {Assembly.GetEntryAssembly().GetName().Version.MinorRevision} - {Assembly.GetEntryAssembly().GetName().Version.Build}");

            // Log($"\n {sb}");
            //DumpLLOffsets();
            //ActionManager.DoAction("Scour", Core.Me);
            /*if (GatheringMasterpieceLL.Instance.IsOpen)
            {
                Log($"Collectability: {GatheringMasterpieceLL.Instance.Collectability}/{GatheringMasterpieceLL.Instance.MaxCollectability}");
                Log($"Integrity: {GatheringMasterpieceLL.Instance.Integrity}/{GatheringMasterpieceLL.Instance.MaxIntegrity}");
                Log($"IntuitionRate: {GatheringMasterpieceLL.Instance.IntuitionRate} Item: {DataManager.GetItem((uint) GatheringMasterpieceLL.Instance.ItemID).CurrentLocaleName}");
            }*/

            /*InventoryBagId[] FCChest = new InventoryBagId[] {InventoryBagId.GrandCompany_Page1, InventoryBagId.GrandCompany_Page2, InventoryBagId.GrandCompany_Page3, (InventoryBagId) 20003, (InventoryBagId) 20004};

            var slots = InventoryManager.GetBagsByInventoryBagId(FCChest).SelectMany(x=> x.FilledSlots);
            foreach (var slot in slots)
            {
               // Log(slot);
            }*/

            /*
            var windowItemIds = LlamaLibrary.RemoteWindows.GrandCompanySupplyList.Instance.GetTurninItemsIds();

            for (int i = 0; i < LlamaLibrary.RemoteWindows.GrandCompanySupplyList.Instance.GetNumberOfTurnins(); i++)
            {
                Log($"Can turn in {DataManager.GetItem(windowItemIds[i])}");
                //bool shouldTurnin = 
            }



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
            */
            /*if (Core.Me.GCSeals() > 200)
            {
                await GrandCompanyShop.BuyKnownItem(21072, (int) (Core.Me.GCSeals() / 200));
            }*/
            //DumpOffsets();
            //await BuyHouse();
            // await testKupoTickets();

            /*var newHunts = HuntHelper.DailyHunts;
            var failed = new Dictionary<int, StoredHuntLocation>();
            var start = 0; 
            foreach (var hunt in newHunts)
            {
                await Lisbeth.TravelToZones(hunt.Value.Map, hunt.Value.Location);
                if ((Core.Me.Location.Distance2DSqr(hunt.Value.Location) > 10f))
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
            // await OutOnALimbBase.RunHomeMGP();


            /*AgentFreeCompany.Instance.Toggle();
            await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
            
            foreach (var buff in buffs)
            {
                Log($"Current Buffs: {buff.Name}");
            }
            
            var FCActionListCur = await AgentFreeCompany.Instance.GetAvailableActions();
            int cnt = 0;
            foreach (var action in FCActionListCur)
            {
                Log($"{cnt} - {FreeCompanyExchange.FcShopActions.First(i=> i.ActionId == action.id).Name}");
                cnt++;
            }
            
            var curActions = await AgentFreeCompany.Instance.GetCurrentActions();
            Log($"# Currently Active Actions: {curActions.Length}");
            if (curActions.Length < 2)
            {
                await FreeCompanyActions.ActivateBuffs(31, 41, GrandCompany.Maelstrom);
            }
            
            if (FreeCompany.Instance.IsOpen)
                FreeCompany.Instance.Close();*/
            /*
            await GrandCompanyHelper.InteractWithNpc(GCNpc.OIC_Quartermaster, GrandCompany.Maelstrom);

            await Coroutine.Wait(5000, () => Talk.DialogOpen);
            if (Talk.DialogOpen)
            {
                Talk.Next();
                await Coroutine.Wait(5000, () => Conversation.IsOpen);
                if (Conversation.IsOpen)
                {
                    Conversation.SelectLine(0);
                    await Coroutine.Wait(10000, () => FreeCompanyExchange.Instance.IsOpen);
                    if (FreeCompanyExchange.Instance.IsOpen)
                    {
                        await Coroutine.Sleep(500);
                        await FreeCompanyExchange.Instance.BuyAction(31);
                        await FreeCompanyExchange.Instance.BuyAction(41);
                        FreeCompanyExchange.Instance.Close();
                    }
                }
            }
            */

            /*var address1 = new IntPtr(0x273E68848A0);
            var count = Core.Memory.Read<int>(address1 + 4);
            var shop = Core.Memory.ReadArray<FcActionShop>(address1 + 0x8, count);
            int x = 0;
            using (var outputFile = new StreamWriter(@"G:\ShopItems.csv", false))
                foreach (var item in shop)
                {
                    var name = FcActionList[item.id];
                    outputFile.WriteLine($"new FcActionShop({item.id}, {item.rank}, {item.cost}, {x}, \"{name}\"),");
                    Log($"{x}, {item.rank}, {item.cost}, {name}");
                    x++;
                }*/


            //Core.Me.Stats
            /*
            Log($"{AgentMinionNoteBook.Instance.MinionListAddress}");
            var minions = AgentMinionNoteBook.Instance.GetCurrentMinions();
            foreach (var minion in minions)
            {
                Log($"{minion.MinionId} - {AgentMinionNoteBook.GetMinionName(minion.MinionId)}");
            }
            */
        }

        [StructLayout(LayoutKind.Explicit, Size = 0xB0)]
        public struct SearchResult
        {
            [FieldOffset(0x0)]
            public readonly ulong listingId;

            [FieldOffset(0x8)]
            public readonly ulong retainerId;

            [FieldOffset(0x10)]
            public readonly ulong retainerOwnerId;

            [FieldOffset(0x18)]
            public readonly uint pricePerUnit;

            [FieldOffset(0x1C)]
            public readonly uint totalTax;

            [FieldOffset(0x20)]
            public readonly uint qty;

            [FieldOffset(0x38)]
            public readonly byte HQ;

            [FieldOffset(0x39)]
            public readonly byte meldNum;

            [FieldOffset(0x40)]
            public readonly byte retainerCityId;
        }


        [StructLayout(LayoutKind.Sequential, Size = 0x10)]
        private struct FcActionShop
        {
            public readonly uint id;
            public readonly uint iconId;
            public readonly uint rank;
            public readonly uint cost;
        }
    }
}