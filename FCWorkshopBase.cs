using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary
{
    public class FCWorkshopBase : BotBase
    {
        private Composite _root;
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
           //await Shop();

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

        public async Task VendorRepair()
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            var desynthFunc = patternFinder.Find("Search 40 53 48 83 EC ? 8B DA 48 8B 15 ? ? ? ? 48 85 D2 0F 84 ? ? ? ? 48 89 6C 24 ? 4C 8D 0D ? ? ? ? 33 ED 0F B7 CD 0F 1F 80 ? ? ? ? 0F B7 C1 41 39 1C 81 74 ? 66 FF C1 66 83 F9 ? 72 ? 48 8B 6C 24 ? 48 83 C4 ? 5B C3 66 83 F9 ? 0F 83 ? ? ? ? 0F B7 C1 48 8D 0C 40 48 39 2C CA 48 8D 0C CA 0F 84 ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 48 89 74 24 ? 48 89 7C 24 ? 41 0F BF F8 8B D7 E8 ? ? ? ? 48 8B F0 48 85 C0 0F 84 ? ? ? ? 45 33 C0 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? 84 C0 75 ? 45 33 C0 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? E9 ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 75 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 48 8B 10 48 8B C8 FF 52 ? BA ? ? ? ? E9 ? ? ? ? 40 38 2D ? ? ? ? 76 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 48 8B 10 48 8B C8 FF 52 ? BA ? ? ? ? E9 ? ? ? ? 48 8B CE E8 ? ? ? ? 44 8B C8 89 6C 24 ? 44 8B C7 8B D3 B9 ? ? ? ? E8 ? ? ? ? 48 8B 0D ? ? ? ? 48 8B 01 FF 10 89 6C 24 ? 48 8D 0D ? ? ? ? 4C 8B C8 89 6C 24 ? BA ? ? ? ? 89 6C 24 ? 41 B8 ? ? ? ? E8 ? ? ? ? 84 C0 74 ? 41 B0 ? 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? EB ? 48 8B 0D ? ? ? ?");

            var ItemFuncParam = patternFinder.Find("48 8D 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 74 ? 4C 63 78 ? Add 3 TraceRelative");

            var itemsToDesynth = InventoryManager.FilledSlots.Where(i => i.RawItemId == 23203);

            foreach (var item in itemsToDesynth)
            {
                lock (Core.Memory.Executor.AssemblyLock)
                {
                    using (Core.Memory.TemporaryCacheState(false))
                    {
                        Core.Memory.CallInjected64<uint>(desynthFunc, new object[3]
                        {
                            ItemFuncParam,
                            (uint) item.BagId,
                            item.Slot,
                        });
                    }
                }

                await Coroutine.Wait(10000, () => SalvageResult.IsOpen);

                if (SalvageResult.IsOpen)
                {
                    SalvageResult.Close();
                    await Coroutine.Wait(5000, () => !SalvageResult.IsOpen);
                }
            }
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

            AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName(windowName);

            if (windowByName == null)
                return false;

            while (SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.YellowCraftersScrips) > 50 && InventoryManager.FreeSlots > 1)
            {
                if (windowByName != null)
                {
                    windowByName.SendAction(4, 3, 0, 3, 0x19, 3, 1, 0, 0);
                }

                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);

                if (SelectYesno.IsOpen)
                    SelectYesno.Yes();

                await Coroutine.Sleep(700);
            }
            
            windowByName.SendAction(1, 3uL, 4294967295uL);
            
            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            
            SelectString.ClickSlot((uint) (SelectString.LineCount-1));
            
            await Coroutine.Sleep(700);

            return true;
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
            if (GameObjectManager.GetObjectByNPCId(2005238) == null)
            {
                Logging.Write("Can't find Fabrication Station");
                return false;
            }

            var station = GameObjectManager.GetObjectByNPCId(2005238);

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