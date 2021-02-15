using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using Generate;
using GreyMagic;
using LlamaLibrary.Enums;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;
using Newtonsoft.Json.Linq;

namespace LlamaLibrary
{
    public class FCWorkshopBase : BotBase
    {
        public static List<Item> itemList = new List<Item>();

        private static readonly InventoryBagId[] RetainerBagIds =
        {
            InventoryBagId.Retainer_Page1, InventoryBagId.Retainer_Page2, InventoryBagId.Retainer_Page3,
            InventoryBagId.Retainer_Page4, InventoryBagId.Retainer_Page5, InventoryBagId.Retainer_Page6,
            InventoryBagId.Retainer_Page7
        };
        
        private static readonly ItemUiCategory[] GatheringCategories =
        {
            ItemUiCategory.Lumber, ItemUiCategory.Stone //,ItemUiCategory.Reagent,ItemUiCategory.Metal
        };
        
        private static readonly ItemUiCategory[] CountCategories =
        {
            ItemUiCategory.Miscellany, ItemUiCategory.Minion, ItemUiCategory.Other, ItemUiCategory.Seasonal_Miscellany
        };

        private static Dictionary<ItemUiCategory, List<ItemStored>> categoryCount = new Dictionary<ItemUiCategory, List<ItemStored>>();
        
        private static List<ItemStored> retainerItems = new List<ItemStored>();

        public static List<uint> TimedNodeIds = new List<uint>
        {
            10095, 7588, 5149, 5150, 10335, 10099, 5121, 8030, 7589, 5148, 5147, 5273, 19, 7766, 7610, 7763, 5118, 6199, 18, 14, 16, 15, 17, 6152, 5146, 5151, 7760, 5158, 10098, 5350, 7611, 7591, 7590, 5545, 7733, 6147, 7594, 8031, 7595,
            5365,
            7593, 8024, 7592, 5395, 7734, 4816, 5547, 7724, 6148, 5537, 6209, 5546, 5120, 5394, 9518, 9519, 10093, 10096, 4800, 12884, 13767, 13768, 12877, 12882, 12900, 5108, 13765, 12896, 12941, 5109, 5112, 12889, 12944, 12901, 5159,
            12943,
            12899, 5162, 5161, 12897, 12898, 12942, 5117, 12538, 12536, 14148, 12634, 12904, 5226, 5163, 5160, 12540, 12945, 12587, 12902, 4833, 12903, 14154, 12971, 15948, 12972, 12973, 12968, 12969, 12970, 5218, 15949, 5224, 12967, 5214,
            12966, 5220, 15647, 14957, 14151, 15646, 16721, 16722, 5392, 16723, 16725, 16726, 16724, 19968, 19936, 17944, 19973, 19970, 19972, 19971, 19918, 19860, 19865, 19852, 19991, 21086, 17948, 19857, 19934, 21085, 19959, 19958, 19907,
            19916, 23221, 10, 20012, 11, 19937, 9, 20010, 23220, 8, 20009, 12, 20011, 22417, 23179, 24255, 22418, 22419, 23180, 22420, 24240, 24241, 24242, 24243, 27816, 27726, 27833, 27836, 27822, 27828, 27835, 27688, 28716, 27761, 27727,
            27728, 27729, 27730, 27731, 27705, 28717, 27704, 27805, 27809, 27806, 27810, 27807, 27808
        };
        
        public static List<uint> ItemsToLower = new List<uint>
            {4835,4840,4843,4852,4858,4860,5118,5232,5259,5260,5270,5280,5282,5284,5291,5292,5296,5317,5326,5345,5350,5384,5385,5386,5389,5436,5491,5496,5508,5518,5525,5527,5536,7588,9518,12525,12539,12548,12565,12566,12569,12576,12583,12592,12602,12614,12615,12629,12630,12632,12639,12874,12936,12937,12939,15648,16729,19846,19865,19867,19869,19874,19891,19911,19913,19914,19915,19920,19928,19932,19933,19945,19947,19953,19954,19956,19957,19958,19959,19970,19971,19972,19973,19985,19987,19989,20006,20008,20014,20015,20016,21085,21180,22412,22416,22417,23183,24246,24250,24253,24255,27683,27690,27696,27697,27706,27708,27726,27727,27732,27738,27739,27746,27765,27766,27802,27820,27825,27827,27833,27850};

        private static Dictionary<int,List<ItemUiCategory>> retainerCategoryStorage = new Dictionary<int, List<ItemUiCategory>>
        {
            {0, new List<ItemUiCategory>{ItemUiCategory.Dye, ItemUiCategory.Medicine, ItemUiCategory.Miscellany, ItemUiCategory.Other, ItemUiCategory.Part}}, //Crick-t
            {1, new List<ItemUiCategory>{ItemUiCategory.Lumber, ItemUiCategory.Reagent}}, //Pricksworth
            {2, new List<ItemUiCategory>{ItemUiCategory.Metal,ItemUiCategory.Stone, ItemUiCategory.Catalyst}}, //RawMats
            {3, new List<ItemUiCategory>{ItemUiCategory.Cloth, ItemUiCategory.Leather, ItemUiCategory.Bone}}, //Keshaa
            {4, new List<ItemUiCategory>{ItemUiCategory.Demimateria,ItemUiCategory.Gardening, ItemUiCategory.Body, ItemUiCategory.Bracelets,ItemUiCategory.Earrings,ItemUiCategory.Feet,ItemUiCategory.Hands,ItemUiCategory.Head,ItemUiCategory.Legs,ItemUiCategory.Necklace,ItemUiCategory.Ring,ItemUiCategory.Shield}}, //Paige
            {5, new List<ItemUiCategory>{ItemUiCategory.Ingredient, ItemUiCategory.Meal, ItemUiCategory.Seafood, ItemUiCategory.Furnishing}}, //Verit-y
        };

        private static uint[] npcids = new uint[] {2005236, 2005238, 2005240, 2007821};
        private bool _init;
        private Composite _root;

        private List<LisbethOrder> orderList;

        private readonly bool readOrder = false;

        public FCWorkshopBase()
        {
            if (readOrder)
                Task.Factory.StartNew(() =>
                {
                    init();
                    _init = true;
                });
        }

        public override string Name => "FCWorkshopBase";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        private async Task<bool> Run()
        {
            //await PrintMasterPieceList();
            await FCWorkshop();
            
            //await PullExcludeId();
            
            //await GenerateList();

            //await LowerItemsById();

            //await GenerateList();
            
            //await Pullorder();

            //await MoveItemsAroundMain();

           // await Facet();
            
           // await LeveWindow(1018997);

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

        internal void init()
        {
            OffsetManager.Init();
           // var text = File.ReadAllText(@"G:\Order.json", Encoding.UTF8);
           // orderList = loadResource<List<LisbethOrder>>(text);
           // Logger.Info("Loaded Order.json");
        }

        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public async Task Pullorder()
        {
            foreach (var item in orderList)
            {
                Logger.Info($"{DataManager.GetItem((uint) item.Item)}");
            }

            await RetainerRoutine.ReadRetainers(CheckForItems);

            foreach (var item in orderList.ToList())
            {
                var itemCount = DataManager.GetItem((uint) item.Item).ItemCount();

                if (itemCount > 0)
                {
                    if (itemCount >= item.Amount)
                    {
                        Logger.Info($"Removing {item}");
                        orderList.Remove(item);
                    }
                    else
                    {
                        Logger.Info($"Changing amount of {item} to {item.Amount - (int) itemCount}");
                        orderList.First(i => i.Item == item.Item).Amount = item.Amount - (int) itemCount;
                    }
                }
                else
                {
                    Logger.Info($"Have no {item}");
                }
            }

            using (var outputFile = new StreamWriter("Order.json", false))
            {
                outputFile.Write(JsonConvert.SerializeObject(orderList, Formatting.None));
            }
        }
        
        public async Task PullExcludeId()
        {
            await RetainerRoutine.ReadRetainers(CheckForItemsById);
        }
        
        public async Task GenerateList()
        {
            foreach (var category in CountCategories)
            {
                categoryCount.Add(category, new List<ItemStored>());
            }
            
            await RetainerRoutine.ReadRetainers(CountCategoryItems);

            foreach (var category in CountCategories)
            {
                using (var outputFile = new StreamWriter($"{category.ToString()}.csv", false))
                {
                    outputFile.WriteLine($"ItemId,IsHq,Count,Ilvl,ItemUICategory,Name");
                    foreach (var itemStored in categoryCount[category])
                    {
                        outputFile.WriteLine($"{itemStored}");
                    }
                
                }
            }
        }

        public async Task VendorRepair()
        {
            var DoH = Enumerable.Range(8, 8);
            var gearSets = GearsetManager.GearSets.Where(i => i.InUse && i.Class == ClassJobType.Botanist);

            if (gearSets.Any())
            {
                gearSets.First().Activate();
                await Coroutine.Sleep(1000);
            }
            
            
        }

        public async Task<bool> Facet()
        {
            var npcId = GameObjectManager.GetObjectByNPCId(1027236);

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

            await Coroutine.Wait(5000, () => HugeCraftworksSupply.Instance.IsOpen);

            if (HugeCraftworksSupply.Instance.IsOpen)
            {
                Logger.Info($"Checking against {HugeCraftworksSupply.Instance.TurnInItemId}");
                if (InventoryManager.FilledSlots.Count(i => i.RawItemId == HugeCraftworksSupply.Instance.TurnInItemId) > 0)
                {
                    Logger.Info($"Found {HugeCraftworksSupply.Instance.TurnInItemId}");
                    
                    
                    await HugeCraftworksSupply.Instance.HandOverItems();
                    return true;
                }
                else
                {
                    HugeCraftworksSupply.Instance.Close();
                }
            }

            return false;
        }

        

        public async Task<bool> Shop()
        {
            var npcId = GameObjectManager.GetObjectByNPCId(1012301);

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

            await Coroutine.Wait(5000, () => SelectIconString.IsOpen);

            SelectIconString.ClickSlot(0);

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot(5);

            await Coroutine.Sleep(700);

            var windowName = "ShopExchangeCurrency";
            await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName(windowName) != null);

            var windowByName = RaptureAtkUnitManager.GetWindowByName(windowName);

            if (windowByName == null)
                return false;

            while (SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.YellowCraftersScrips) > 50 && InventoryManager.FreeSlots > 1)
            {
                if (windowByName != null) windowByName.SendAction(4, 3, 0, 3, 0x19, 3, 1, 0, 0);

                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);

                if (SelectYesno.IsOpen)
                    SelectYesno.Yes();

                await Coroutine.Sleep(700);
            }

            windowByName.SendAction(1, 3uL, 4294967295uL);

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot((uint) (SelectString.LineCount - 1));

            await Coroutine.Sleep(700);

            return true;
        }


        public async Task<bool> CheckForItems()
        {
            await PullItems(orderList);
            return true;
        }
        
        public async Task<bool> PullListItems()
        {
            await PullItemsByID(ItemsToLower);
            return true;
        }
        
        public async Task<bool> CountCategoryItems()
        {
            await ListItemsByCategories(CountCategories);
            return true;
        }

        public async Task LowerItemsById()
        {
            await RetainerRoutine.ReadRetainers(PullListItems);
            await Coroutine.Sleep(1000);
            foreach (var slot in InventoryManager.FilledSlots.Where(i => i.IsHighQuality && ItemsToLower.Contains(i.RawItemId)))
            {
                RetainerRoutine.LogLoud($"Lowering {slot}");
                slot.LowerQuality();
                await Coroutine.Sleep(1000);
            }
            await Coroutine.Sleep(1000);
            
            await RetainerRoutine.ReadRetainers(RetainerRoutine.DumpItems);
        }
        
        public async Task DumpItemsByCategoryMain(int index)
        {
            await DumpItemsByCategories(retainerCategoryStorage[index].ToArray());
            await Coroutine.Sleep(1000);
        }
        
        public async Task PullItemsByCategoryMain(int index)
        {
            var cats = retainerCategoryStorage.Where(i => i.Key != index).SelectMany(x => x.Value).ToArray();
            foreach (var cat in cats)
            {
                Logger.Info($"Pulling {cat}");
            }
            await PullItemsByCategories(retainerCategoryStorage.Where(i => i.Key != index).SelectMany(x => x.Value).ToArray());
            
            await Coroutine.Sleep(1000);
        }
        
        public async Task MoveItemsAroundMain()
        {
            //await RetainerRoutine.ReadRetainers(DumpItemsByCategoryMain);
            //await Coroutine.Sleep(1000);
            await RetainerRoutine.ReadRetainers(PullItemsByCategoryMain);
            await Coroutine.Sleep(1000);
            await RetainerRoutine.ReadRetainers(DumpItemsByCategoryMain);
        }
        
        public async Task<bool> CheckForItemsById()
        {
            await PullItems(TimedNodeIds);
            return true;
        }
        
        internal static async Task PullItems(List<LisbethOrder> ids)
        {
            var itemIds = ids.Select(x => x.Item).ToList(); // SelectMany(x => x.Id);
            Logger.Info($"Checking against {itemIds.Count()}");
            var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Where(i => itemIds.Contains((int) i.RawItemId));

            foreach (var slot in retItems)
            {
                RetainerRoutine.LogLoud($"Want to move {slot}");
                slot.RetainerRetrieveQuantity((int) Math.Min(ids.First(i => i.Item == slot.RawItemId).Amount, slot.Count));
                await Coroutine.Sleep(200);
            }
        }
        
        internal static async Task PullItems(List<uint> ids)
        {
            //var itemIds = ids.Select(x => x.Item).ToList(); // SelectMany(x => x.Id);
            Logger.Info($"Checking against {ids.Count()}");
            var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).AsParallel().Where(i => i.Count < 99 && i.Item.ItemLevel < 300 && i.Item.StackSize == 999).Where(k=> k.Item.EngName.Contains("Ore") || k.Item.EngName.Contains("Log")).Where(j => GatheringCategories.Contains(j.Item.EquipmentCatagory) && !ids.Contains(j.RawItemId));

            foreach (var slot in retItems)
            {
                RetainerRoutine.LogLoud($"Want to move {slot} - {slot.Count}");
                //slot.RetainerRetrieveQuantity((int) Math.Min(ids.First(i => i.Item == slot.RawItemId).Amount, slot.Count));
                await Coroutine.Sleep(200);
            }
        }
        
        internal static async Task PullItemsByID(List<uint> ids)
        {
            //var itemIds = ids.Select(x => x.Item).ToList(); // SelectMany(x => x.Id);
            Logger.Info($"Checking against {ids.Count()}");
            var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Where(i => i.IsHighQuality && ids.Contains(i.RawItemId)).AsParallel();

            foreach (var slot in retItems)
            {
                RetainerRoutine.LogLoud($"Want to move {slot} - {slot.Count}");
                slot.RetainerRetrieveQuantity( (int) slot.Count);
                await Coroutine.Sleep(200);
            }
        }
        
        internal static async Task ListItems()
        {
            var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Where(i=> i.Item.ItemLevel < 400 && i.Item.MyItemRole() == MyItemRole.CraftingMaterial).Select(y => new ItemStored(y.RawItemId, y.IsHighQuality, y.Count,y.Item.ItemLevel, y.Item.EquipmentCatagory.ToString(), y.EnglishName));
            retainerItems.AddRange(retItems);
        }
        
        internal static async Task ListItemsByCategories(ItemUiCategory[] categories)
        {
            foreach (var cat in categories)
            {
                var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Where(i=> i.Item.EquipmentCatagory == cat).Select(y => new ItemStored(y.RawItemId, y.IsHighQuality, y.Count,y.Item.ItemLevel, y.Item.EquipmentCatagory.ToString(), y.EnglishName));
                categoryCount[cat].AddRange(retItems);
                await Coroutine.Sleep(200);
            }

        }
        
        internal static async Task PullItemsByCategories(ItemUiCategory[] categories)
        {
            foreach (var cat in categories)
            {
                RetainerRoutine.LogLoud($"Category {cat}");
                var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).Where(i => i.Item.EquipmentCatagory == cat && i.Item.MyItemRole() != MyItemRole.Map);
                RetainerRoutine.LogLoud($"Category {retItems.Count()}");
                
                foreach (var slot in retItems)
                {
                    RetainerRoutine.LogLoud($"Want to move PULL {slot} - {slot.Count}");
                    slot.RetainerRetrieveQuantity( (int) slot.Count);
                    await Coroutine.Sleep(700);
                }
                await Coroutine.Sleep(200);
            }

        }
        
        internal static async Task DumpItemsByCategories(ItemUiCategory[] categories)
        {
                var myItems = InventoryManager.FilledSlots.Where(i => categories.Contains(i.Item.EquipmentCatagory) && i.Item.MyItemRole() != MyItemRole.Map);

                foreach (var slot in myItems)
                {
                    if (InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).FirstOrDefault(i => i.FreeSlots >= 1) != null)
                    {
                        RetainerRoutine.LogLoud($"Want to move DUMP {slot} - {slot.Count}");
                        slot.Move(InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).First(i => i.FreeSlots >= 1).GetFirstFreeSlot());
                        //slot.RetainerEntrustQuantity((int) slot.Count);
                        await Coroutine.Sleep(700);
                    }
                    else
                    {
                        RetainerRoutine.LogLoud($"Not Enough room in retainer");
                    }
                }

                await RetainerRoutine.DumpItems();
        }

        public async Task<bool> FCWorkshop()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();

            if (!SubmarinePartsMenu.Instance.IsOpen)
            {
                Logging.Write("Trying to open window");

                if (!await OpenFCCraftingStation())
                {
                    Logging.Write("Nope failed opening FC Workshop window");
                    return false;
                }
            }

            if (!SubmarinePartsMenu.Instance.IsOpen)
            {
                Logging.Write("Nope failed");
                return false;
            }


            //    List<LisbethOrder> outList = new List<LisbethOrder>();
            var id = 0;
            var counts = SubmarinePartsMenu.Instance.GetItemAvailCount();
            var done = SubmarinePartsMenu.Instance.GetTurninsDone();
            foreach (var item in SubmarinePartsMenu.Instance.GetCraftingTurninItems())
            {
                var needed = item.Qty * item.TurnInsRequired - item.Qty * done[id];
                var itemCount = (int) DataManager.GetItem((uint) item.ItemId).ItemCount();

                var turnInsAvail = itemCount / item.Qty;

                Logging.Write($"{item}");
                Logging.Write($"Player has {itemCount} and {needed} are still needed and can do {turnInsAvail} turnins");
                var turnInsNeeded = item.TurnInsRequired - done[id];

                if (turnInsNeeded >= 1)
                {
                    if (turnInsAvail >= 1)
                        for (var i = 0; i < Math.Min(turnInsAvail, turnInsNeeded); i++)
                        {
                            BagSlot bagSlot = null;

                            if (HqItemCount(item.ItemId) >= item.Qty)
                            {
                                bagSlot = InventoryManager.FilledSlots.First(slot => slot.RawItemId == item.ItemId && slot.IsHighQuality && slot.Count >= item.Qty);
                                Logging.Write($"Have HQ {bagSlot.Name}");
//                                continue;
                            }
                            else if (ItemCount(item.ItemId) >= item.Qty)
                            {
                                bagSlot = InventoryManager.FilledSlots.FirstOrDefault(slot => slot.RawItemId == item.ItemId && !slot.IsHighQuality && slot.Count >= item.Qty);

                                if (bagSlot == null)
                                {
                                    await CloseFCCraftingStation();

                                    await LowerQualityAndCombine(item.ItemId);

                                    // var nqSlot = InventoryManager.FilledSlots.FirstOrDefault(slot => slot.RawItemId == item.ItemId && slot.IsHighQuality && slot.Count < item.Qty);

                                    await OpenFCCraftingStation();
                                    bagSlot = InventoryManager.FilledSlots.FirstOrDefault(slot => slot.RawItemId == item.ItemId && !slot.IsHighQuality && slot.Count >= item.Qty);
                                    Logging.Write($"Need To Lower Quality {bagSlot.Name}");
                                }
                                else
                                {
                                    Logging.Write($"Have NQ {bagSlot.Name}");
                                }
                            }
                            else
                            {
                                Logging.Write($"Something went wrong {ItemCount(item.ItemId)}");
                            }

                            if (bagSlot != null)
                            {
                                Logging.Write($"Turn in {bagSlot.Name} HQ({bagSlot.IsHighQuality})");
                                await Coroutine.Sleep(500);
                                SubmarinePartsMenu.Instance.ClickItem(id);

                                await Coroutine.Wait(5000, () => Request.IsOpen);
                                var isHQ = bagSlot.IsHighQuality;
                                bagSlot.Handover();

                                await Coroutine.Wait(5000, () => Request.HandOverButtonClickable);

                                if (Request.HandOverButtonClickable)
                                {
                                    Request.HandOver();
                                    await Coroutine.Sleep(500);
                                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);

                                    if (SelectYesno.IsOpen)
                                        SelectYesno.Yes();

                                    await Coroutine.Sleep(700);

                                    if (!isHQ) continue;

                                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);

                                    if (SelectYesno.IsOpen)
                                        SelectYesno.Yes();
                                    await Coroutine.Sleep(700);
                                }
                                else
                                {
                                    Logging.Write("HandOver Stuck");
                                    return false;
                                }
                            }
                            else
                            {
                                Logging.Write("Bagslot is null");
                            }
                        }
                    else
                        Logging.Write($"No Turn ins available {turnInsAvail}");
                }
                else
                {
                    Logging.Write($"turnInsNeeded {turnInsNeeded}");
                }

                Logging.Write("--------------");
                id++;
            }

            await CloseFCCraftingStation();

            return true;
        }

        public static async Task<bool> OpenFCCraftingStation()
        {
            if (GameObjectManager.GetObjectsByNPCIds<GameObject>(npcids).Any())
            {
                Logging.Write("Can't find Fabrication Station");
                return false;
            }

            var station = GameObjectManager.GetObjectsByNPCIds<GameObject>(npcids).First();

            if (!station.IsWithinInteractRange)
            {
                var _target = station.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            station.Interact();

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot(0);

            await Coroutine.Wait(5000, () => SubmarinePartsMenu.Instance.IsOpen);

            return SubmarinePartsMenu.Instance.IsOpen;
        }

        public static async Task<bool> CloseFCCraftingStation()
        {
            if (!SubmarinePartsMenu.Instance.IsOpen)
                return true;

            SubmarinePartsMenu.Instance.Close();

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot(3);

            await Coroutine.Sleep(500);

            return SelectString.IsOpen;
        }

        public static async Task LowerQualityAndCombine(int itemId)
        {
            var HQslots = InventoryManager.FilledSlots.Where(slot => slot.RawItemId == itemId && slot.IsHighQuality);

            if (HQslots.Any())
            {
                HQslots.First().LowerQuality();
                await Coroutine.Sleep(1000);
            }

            var NQslots = InventoryManager.FilledSlots.Where(slot => slot.RawItemId == itemId && !slot.IsHighQuality);

            if (NQslots.Count() > 1)
            {
                var firstSlot = NQslots.First();
                foreach (var slot in NQslots.Skip(1))
                {
                    slot.Move(firstSlot);
                    await Coroutine.Sleep(500);
                }
            }
        }

        public static int HqItemCount(int itemId)
        {
            return Lua.GetReturnVal<int>($"return _G['{Core.Player.LuaString}']:GetNumOfHqItems({itemId});");
        }

        public static int ItemCount(int itemId)
        {
            return Lua.GetReturnVal<int>($"return _G['{Core.Player.LuaString}']:GetNumOfItems({itemId});");
        }
    }
}