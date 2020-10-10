using System.Linq;
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
using ff14bot.Objects;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary
{
    public class IshgardHandinBase : BotBase
    {
        private Composite _root;
        public override string Name => "Ishgard Handin";
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public static bool DiscardCollectable = false;
        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        private static uint[] items20 = {28725, 29792, 28726, 29793, 28727, 29794, 28728, 29795, 28729, 29796, 28730, 29797, 28731, 29798, 28732, 29799};
        private static uint[] items40 = {28733, 29800, 28734, 29801, 28735, 29802, 28736, 29803, 28737, 29804, 28738, 29805, 28739, 29806, 28740, 29807};
        private static uint[] items150 = {28741, 29808, 28742, 29809, 28743, 29810, 28744, 29811, 28745, 29812, 28746, 29813, 28747, 29814, 28748, 29815};
        private static uint[] items290 = {28749, 29816, 28750, 29817, 28751, 29818, 28752, 29819, 28753, 29820, 28754, 29821, 28755, 29822, 28756, 29823};
        private static uint[] items430 = {28757, 29824, 28758, 29825, 28759, 29826, 28760, 29827, 28761, 29828, 28762, 29829, 28763, 29830, 28764, 29831};
        private static uint[] items481 = {29832, 29833, 29834, 29835, 29836, 29837, 29838, 29839};
        private static uint[] items511 = {31224, 31225, 31226, 31227, 31228, 31229, 31230, 31231};


        private async Task<bool> Run()
        {
            await Handin();
            //await BuyItem(13630);
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

        public IshgardHandinBase()
        {
            OffsetManager.Init();
        }

        public static async Task<bool> Handin()
        {
            await StopCrafting();
            if (Translator.Language == Language.Chn)
            {
                await HandinOld();
                await GatheringHandin();
            }
            else
            {
                await HandinNew();
                await GatheringHandin();
            }

            return true;
        }

        public static async Task<bool> StopCrafting()
        {
            for (int tryStep = 1; tryStep < 6; tryStep++)
            {
                if (!(DutyManager.InInstance || CraftingLog.IsOpen || FishingManager.State != FishingState.None || MovementManager.IsOccupied || CraftingManager.IsCrafting)) break;

                Log($"We're occupied. Trying to exit out. Attempt #{tryStep}");

                if (FishingManager.State != FishingState.None)
                {
                    var quit = ActionManager.CurrentActions.Values.FirstOrDefault(i => i.Id == 299);
                    if (quit != default(SpellData))
                    {
                        Log($"Exiting Fishing.");
                        if (ActionManager.CanCast(quit, Core.Me))
                        {
                            ActionManager.DoAction(quit, Core.Me);
                            await Coroutine.Wait(6000, () => FishingManager.State == FishingState.None);
                        }
                    }
                }

                if (CraftingLog.IsOpen || CraftingManager.IsCrafting)
                {
                    Log($"Closing Crafting Window.");
                    await Lisbeth.ExitCrafting();
                    CraftingLog.Close();
                    await Coroutine.Wait(6000, () => !CraftingLog.IsOpen);
                    await Coroutine.Wait(6000, () => !CraftingManager.IsCrafting && !MovementManager.IsOccupied);
                }

                if (DutyManager.InInstance)
                {
                    Log($"Leaving Diadem.");
                    DutyManager.LeaveActiveDuty();

                    if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
                    {
                        await Coroutine.Yield();
                        await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                        await Coroutine.Sleep(5000);
                    }
                }

                await Coroutine.Sleep(2500);
            }

            if (DutyManager.InInstance || CraftingLog.IsOpen || FishingManager.State != FishingState.None || MovementManager.IsOccupied || CraftingManager.IsCrafting)
            {
                Log("Something went wrong, we're still occupied.");
                TreeRoot.Stop("Stopping bot.");
                return false;
            }
            return true;
        }

        public static async Task<bool> HandinOld()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();

            if (DiscardCollectable)
            {
                foreach (var item in InventoryManager.FilledSlots.Where(i => items20.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 50))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items40.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 90))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items150.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 300))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items290.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 480))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items430.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 1350))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items481.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 4500))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }
            }

            // Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28725))
                await ishgardHandin.HandInItem(28725, 10, 0);

            // Skybuilders' Wain (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28733))
                await ishgardHandin.HandInItem(28733, 9, 0);

            // Skybuilders' Barrel (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28741))
                await ishgardHandin.HandInItem(28741, 8, 0);

            // Skybuilders' Pedestal (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28749))
                await ishgardHandin.HandInItem(28749, 7, 0);

            // Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28757))
                await ishgardHandin.HandInItem(28757, 6, 0);

            // Grade 2 Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29792))
                await ishgardHandin.HandInItem(29792, 5, 0);

            // Grade 2 Skybuilders' Crate (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29800))
                await ishgardHandin.HandInItem(29800, 4, 0);

            // Grade 2 Skybuilders' Grindstone (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29808))
                await ishgardHandin.HandInItem(29808, 3, 0);

            // Grade 2 Skybuilders' Stepladder (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29816))
                await ishgardHandin.HandInItem(29816, 2, 0);

            // Grade 2 Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29824))
                await ishgardHandin.HandInItem(29824, 1, 0);

            // Grade 2 Artisanal Skybuilders' Wardrobe (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29832))
                await ishgardHandin.HandInItem(29832, 0, 0);

            // Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28726))
                await ishgardHandin.HandInItem(28726, 10, 1);

            // Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28734))
                await ishgardHandin.HandInItem(28734, 9, 1);

            // Skybuilders' Hammer (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28742))
                await ishgardHandin.HandInItem(28742, 8, 1);

            // Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28750))
                await ishgardHandin.HandInItem(28750, 7, 1);

            // Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28758))
                await ishgardHandin.HandInItem(28758, 6, 1);

            // Grade 2 Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29793))
                await ishgardHandin.HandInItem(29793, 5, 1);

            // Grade 2 Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29801))
                await ishgardHandin.HandInItem(29801, 4, 1);

            // Grade 2 Skybuilders' Hammer (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29809))
                await ishgardHandin.HandInItem(29809, 3, 1);

            // Grade 2 Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29817))
                await ishgardHandin.HandInItem(29817, 2, 1);

            // Grade 2 Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29825))
                await ishgardHandin.HandInItem(29825, 1, 1);

            // Grade 2 Artisanal Skybuilders' Chandelier (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29833))
                await ishgardHandin.HandInItem(29833, 0, 1);

            // Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28727))
                await ishgardHandin.HandInItem(28727, 10, 2);

            // Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28735))
                await ishgardHandin.HandInItem(28735, 9, 2);

            // Skybuilders' Cooking Pot (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28743))
                await ishgardHandin.HandInItem(28743, 8, 2);

            // Skybuilders' Counter (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28751))
                await ishgardHandin.HandInItem(28751, 7, 2);

            // Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28759))
                await ishgardHandin.HandInItem(28759, 6, 2);

            // Grade 2 Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29794))
                await ishgardHandin.HandInItem(29794, 5, 2);

            // Grade 2 Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29802))
                await ishgardHandin.HandInItem(29802, 4, 2);

            // Grade 2 Skybuilders' Still (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29810))
                await ishgardHandin.HandInItem(29810, 3, 2);

            // Grade 2 Skybuilders' Mesail (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29818))
                await ishgardHandin.HandInItem(29818, 2, 2);

            // Grade 2 Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29826))
                await ishgardHandin.HandInItem(29826, 1, 2);

            // Grade 2 Artisanal Skybuilders' Fireplace (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29834))
                await ishgardHandin.HandInItem(29834, 0, 2);

            // Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28728))
                await ishgardHandin.HandInItem(28728, 10, 3);

            // Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28736))
                await ishgardHandin.HandInItem(28736, 9, 3);

            // Skybuilders' Candelabra (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28744))
                await ishgardHandin.HandInItem(28744, 8, 3);

            // Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28752))
                await ishgardHandin.HandInItem(28752, 7, 3);

            // Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28760))
                await ishgardHandin.HandInItem(28760, 6, 3);

            // Grade 2 Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29795))
                await ishgardHandin.HandInItem(29795, 5, 3);

            // Grade 2 Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29803))
                await ishgardHandin.HandInItem(29803, 4, 3);

            // Grade 2 Skybuilders' Embroidery Frame (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29811))
                await ishgardHandin.HandInItem(29811, 3, 3);

            // Grade 2 Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29819))
                await ishgardHandin.HandInItem(29819, 2, 3);

            // Grade 2 Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29827))
                await ishgardHandin.HandInItem(29827, 1, 3);

            // Grade 2 Artisanal Skybuilders' Bathtub (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29835))
                await ishgardHandin.HandInItem(29835, 0, 3);

            // Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28729))
                await ishgardHandin.HandInItem(28729, 10, 4);

            // Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28737))
                await ishgardHandin.HandInItem(28737, 9, 4);

            // Skybuilders' Rug (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28745))
                await ishgardHandin.HandInItem(28745, 8, 4);

            // Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28753))
                await ishgardHandin.HandInItem(28753, 7, 4);

            // Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28761))
                await ishgardHandin.HandInItem(28761, 6, 4);

            // Grade 2 Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29796))
                await ishgardHandin.HandInItem(29796, 5, 4);

            // Grade 2 Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29804))
                await ishgardHandin.HandInItem(29804, 4, 4);

            // Grade 2 Skybuilders' Rug (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29812))
                await ishgardHandin.HandInItem(29812, 3, 4);

            // Grade 2 Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29820))
                await ishgardHandin.HandInItem(29820, 2, 4);

            // Grade 2 Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29828))
                await ishgardHandin.HandInItem(29828, 1, 4);

            // Grade 2 Artisanal Skybuilders' Overcoat (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29836))
                await ishgardHandin.HandInItem(29836, 0, 4);

            // Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28730))
                await ishgardHandin.HandInItem(28730, 10, 5);

            // Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28738))
                await ishgardHandin.HandInItem(28738, 9, 5);

            // Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28746))
                await ishgardHandin.HandInItem(28746, 8, 5);

            // Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28754))
                await ishgardHandin.HandInItem(28754, 7, 5);

            // Skybuilders' Waterproof Sheet (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28762))
                await ishgardHandin.HandInItem(28762, 6, 5);

            // Grade 2 Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29797))
                await ishgardHandin.HandInItem(29797, 5, 5);

            // Grade 2 Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29805))
                await ishgardHandin.HandInItem(29805, 4, 5);

            // Grade 2 Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29813))
                await ishgardHandin.HandInItem(29813, 3, 5);

            // Grade 2 Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29821))
                await ishgardHandin.HandInItem(29821, 2, 5);

            // Grade 2 Skybuilders' Gazebo (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29829))
                await ishgardHandin.HandInItem(29829, 1, 5);

            // Grade 2 Artisanal Skybuilders' Wallpaper (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29837))
                await ishgardHandin.HandInItem(29837, 0, 5);

            // Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28731))
                await ishgardHandin.HandInItem(28731, 10, 6);

            // Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28739))
                await ishgardHandin.HandInItem(28739, 9, 6);

            // Skybuilders' Charcoal (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28747))
                await ishgardHandin.HandInItem(28747, 8, 6);

            // Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28755))
                await ishgardHandin.HandInItem(28755, 7, 6);

            // Skybuilders' Alchemic (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28763))
                await ishgardHandin.HandInItem(28763, 6, 6);

            // Grade 2 Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29798))
                await ishgardHandin.HandInItem(29798, 5, 6);

            // Grade 2 Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29806))
                await ishgardHandin.HandInItem(29806, 4, 6);

            // Grade 2 Skybuilders' Dye (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29814))
                await ishgardHandin.HandInItem(29814, 3, 6);

            // Grade 2 Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29822))
                await ishgardHandin.HandInItem(29822, 2, 6);

            // Grade 2 Skybuilders' Alchemic (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29830))
                await ishgardHandin.HandInItem(29830, 1, 6);

            // Grade 2 Artisanal Skybuilders' Remedies (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29838))
                await ishgardHandin.HandInItem(29838, 0, 6);

            // Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28732))
                await ishgardHandin.HandInItem(28732, 10, 7);

            // Skybuilders' Uncooked Pasta (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28740))
                await ishgardHandin.HandInItem(28740, 9, 7);

            // Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28748))
                await ishgardHandin.HandInItem(28748, 8, 7);

            // Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28756))
                await ishgardHandin.HandInItem(28756, 7, 7);

            // Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28764))
                await ishgardHandin.HandInItem(28764, 6, 7);

            // Grade 2 Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29799))
                await ishgardHandin.HandInItem(29799, 5, 7);

            // Grade 2 Skybuilders' Bread (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29807))
                await ishgardHandin.HandInItem(29807, 4, 7);

            // Grade 2 Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29815))
                await ishgardHandin.HandInItem(29815, 3, 7);

            // Grade 2 Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29823))
                await ishgardHandin.HandInItem(29823, 2, 7);

            // Grade 2 Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29831))
                await ishgardHandin.HandInItem(29831, 1, 7);

            // Grade 2 Artisanal Skybuilders' Quiche (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29839))
                await ishgardHandin.HandInItem(29839, 0, 7);


            HWDSupply.Instance.Close();

            //TreeRoot.Stop("Stop Requested");
            return true;
        }

        public static async Task<bool> GatheringHandin()
        {
            Log($"Gathering Started");
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();
            while (ScriptConditions.Helpers.HasIshgardGatheringBotanist())
            {
                await ishgardHandin.HandInGatheringItem(1);
                await Coroutine.Sleep(200);
            }

            while (ScriptConditions.Helpers.HasIshgardGatheringMining())
            {
                await ishgardHandin.HandInGatheringItem(0);
                await Coroutine.Sleep(200);
            }

            while (ScriptConditions.Helpers.HasIshgardGatheringFisher())
            {
                await ishgardHandin.HandInGatheringItem(2);
                await Coroutine.Sleep(200);
            }

            if (HWDGathereInspect.Instance.IsOpen)
                HWDGathereInspect.Instance.Close();

            Log($"Gathering Done");
            return false;
        }

        public static async Task<bool> HandinNew()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();

            Log("Started");

            if (DiscardCollectable)
            {
                foreach (var item in InventoryManager.FilledSlots.Where(i => items20.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 50))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items40.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 90))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items150.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 300))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items290.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 480))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items430.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 1350))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items481.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 4500))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items511.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 5800))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }
            }

            // Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28725))
                await ishgardHandin.HandInItem(28725, 16, 0);

            // Skybuilders' Wain (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28733))
                await ishgardHandin.HandInItem(28733, 15, 0);

            // Skybuilders' Barrel (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28741))
                await ishgardHandin.HandInItem(28741, 14, 0);

            // Skybuilders' Pedestal (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28749))
                await ishgardHandin.HandInItem(28749, 13, 0);

            // Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28757))
                await ishgardHandin.HandInItem(28757, 12, 0);

            // Grade 2 Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29792))
                await ishgardHandin.HandInItem(29792, 11, 0);

            // Grade 2 Skybuilders' Crate (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29800))
                await ishgardHandin.HandInItem(29800, 10, 0);

            // Grade 2 Skybuilders' Grindstone (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29808))
                await ishgardHandin.HandInItem(29808, 9, 0);

            // Grade 2 Skybuilders' Stepladder (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29816))
                await ishgardHandin.HandInItem(29816, 8, 0);

            // Grade 2 Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29824))
                await ishgardHandin.HandInItem(29824, 7, 0);

            // Grade 2 Artisanal Skybuilders' Wardrobe (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29832))
                await ishgardHandin.HandInItem(29832, 6, 0);

            // Grade 3 Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31184))
                await ishgardHandin.HandInItem(31184, 5, 0);

            // Grade 3 Skybuilders' Crate (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31192))
                await ishgardHandin.HandInItem(31192, 4, 0);

            // Grade 3 Skybuilders' Grinding Wheel (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31200))
                await ishgardHandin.HandInItem(31200, 3, 0);

            // Grade 3 Skybuilders' Stepladder (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31208))
                await ishgardHandin.HandInItem(31208, 2, 0);

            // Grade 3 Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31216))
                await ishgardHandin.HandInItem(31216, 1, 0);

            // Grade 3 Artisanal Skybuilders' Goldsmithing Bench (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31224))
                await ishgardHandin.HandInItem(31224, 0, 0);

            // Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28726))
                await ishgardHandin.HandInItem(28726, 16, 1);

            // Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28734))
                await ishgardHandin.HandInItem(28734, 15, 1);

            // Skybuilders' Hammer (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28742))
                await ishgardHandin.HandInItem(28742, 14, 1);

            // Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28750))
                await ishgardHandin.HandInItem(28750, 13, 1);

            // Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28758))
                await ishgardHandin.HandInItem(28758, 12, 1);

            // Grade 2 Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29793))
                await ishgardHandin.HandInItem(29793, 11, 1);

            // Grade 2 Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29801))
                await ishgardHandin.HandInItem(29801, 10, 1);

            // Grade 2 Skybuilders' Hammer (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29809))
                await ishgardHandin.HandInItem(29809, 9, 1);

            // Grade 2 Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29817))
                await ishgardHandin.HandInItem(29817, 8, 1);

            // Grade 2 Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29825))
                await ishgardHandin.HandInItem(29825, 7, 1);

            // Grade 2 Artisanal Skybuilders' Chandelier (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29833))
                await ishgardHandin.HandInItem(29833, 6, 1);

            // Grade 3 Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31185))
                await ishgardHandin.HandInItem(31185, 5, 1);

            // Grade 3 Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31193))
                await ishgardHandin.HandInItem(31193, 4, 1);

            // Grade 3 Skybuilders' Pickaxe (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31201))
                await ishgardHandin.HandInItem(31201, 3, 1);

            // Grade 3 Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31209))
                await ishgardHandin.HandInItem(31209, 2, 1);

            // Grade 3 Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31217))
                await ishgardHandin.HandInItem(31217, 1, 1);

            // Grade 3 Artisanal Skybuilders' Smithing Bench (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31225))
                await ishgardHandin.HandInItem(31225, 0, 1);

            // Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28727))
                await ishgardHandin.HandInItem(28727, 16, 2);

            // Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28735))
                await ishgardHandin.HandInItem(28735, 15, 2);

            // Skybuilders' Cooking Pot (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28743))
                await ishgardHandin.HandInItem(28743, 14, 2);

            // Skybuilders' Counter (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28751))
                await ishgardHandin.HandInItem(28751, 13, 2);

            // Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28759))
                await ishgardHandin.HandInItem(28759, 12, 2);

            // Grade 2 Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29794))
                await ishgardHandin.HandInItem(29794, 11, 2);

            // Grade 2 Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29802))
                await ishgardHandin.HandInItem(29802, 10, 2);

            // Grade 2 Skybuilders' Still (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29810))
                await ishgardHandin.HandInItem(29810, 9, 2);

            // Grade 2 Skybuilders' Mesail (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29818))
                await ishgardHandin.HandInItem(29818, 8, 2);

            // Grade 2 Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29826))
                await ishgardHandin.HandInItem(29826, 7, 2);

            // Grade 2 Artisanal Skybuilders' Fireplace (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29834))
                await ishgardHandin.HandInItem(29834, 6, 2);

            // Grade 3 Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31186))
                await ishgardHandin.HandInItem(31186, 5, 2);

            // Grade 3 Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31194))
                await ishgardHandin.HandInItem(31194, 4, 2);

            // Grade 3 Skybuilders' Cookpot (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31202))
                await ishgardHandin.HandInItem(31202, 3, 2);

            // Grade 3 Skybuilders' Mesail (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31210))
                await ishgardHandin.HandInItem(31210, 2, 2);

            // Grade 3 Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31218))
                await ishgardHandin.HandInItem(31218, 1, 2);

            // Grade 3 Artisanal Skybuilders' Door (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31226))
                await ishgardHandin.HandInItem(31226, 0, 2);

            // Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28728))
                await ishgardHandin.HandInItem(28728, 16, 3);

            // Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28736))
                await ishgardHandin.HandInItem(28736, 15, 3);

            // Skybuilders' Candelabra (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28744))
                await ishgardHandin.HandInItem(28744, 14, 3);

            // Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28752))
                await ishgardHandin.HandInItem(28752, 13, 3);

            // Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28760))
                await ishgardHandin.HandInItem(28760, 12, 3);

            // Grade 2 Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29795))
                await ishgardHandin.HandInItem(29795, 11, 3);

            // Grade 2 Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29803))
                await ishgardHandin.HandInItem(29803, 10, 3);

            // Grade 2 Skybuilders' Embroidery Frame (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29811))
                await ishgardHandin.HandInItem(29811, 9, 3);

            // Grade 2 Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29819))
                await ishgardHandin.HandInItem(29819, 8, 3);

            // Grade 2 Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29827))
                await ishgardHandin.HandInItem(29827, 7, 3);

            // Grade 2 Artisanal Skybuilders' Bathtub (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29835))
                await ishgardHandin.HandInItem(29835, 6, 3);

            // Grade 3 Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31187))
                await ishgardHandin.HandInItem(31187, 5, 3);

            // Grade 3 Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31195))
                await ishgardHandin.HandInItem(31195, 4, 3);

            // Grade 3 Skybuilders' Embroidery Frame (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31203))
                await ishgardHandin.HandInItem(31203, 3, 3);

            // Grade 3 Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31211))
                await ishgardHandin.HandInItem(31211, 2, 3);

            // Grade 3 Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31219))
                await ishgardHandin.HandInItem(31219, 1, 3);

            // Grade 3 Artisanal Skybuilders' Chronometer (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31227))
                await ishgardHandin.HandInItem(31227, 0, 3);

            // Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28729))
                await ishgardHandin.HandInItem(28729, 16, 4);

            // Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28737))
                await ishgardHandin.HandInItem(28737, 15, 4);

            // Skybuilders' Rug (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28745))
                await ishgardHandin.HandInItem(28745, 14, 4);

            // Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28753))
                await ishgardHandin.HandInItem(28753, 13, 4);

            // Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28761))
                await ishgardHandin.HandInItem(28761, 12, 4);

            // Grade 2 Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29796))
                await ishgardHandin.HandInItem(29796, 11, 4);

            // Grade 2 Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29804))
                await ishgardHandin.HandInItem(29804, 10, 4);

            // Grade 2 Skybuilders' Rug (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29812))
                await ishgardHandin.HandInItem(29812, 9, 4);

            // Grade 2 Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29820))
                await ishgardHandin.HandInItem(29820, 8, 4);

            // Grade 2 Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29828))
                await ishgardHandin.HandInItem(29828, 7, 4);

            // Grade 2 Artisanal Skybuilders' Overcoat (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29836))
                await ishgardHandin.HandInItem(29836, 6, 4);

            // Grade 3 Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31188))
                await ishgardHandin.HandInItem(31188, 5, 4);

            // Grade 3 Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31196))
                await ishgardHandin.HandInItem(31196, 4, 4);

            // Grade 3 Skybuilders' Leather Sack (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31204))
                await ishgardHandin.HandInItem(31204, 3, 4);

            // Grade 3 Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31212))
                await ishgardHandin.HandInItem(31212, 2, 4);

            // Grade 3 Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31220))
                await ishgardHandin.HandInItem(31220, 1, 4);

            // Grade 3 Artisanal Skybuilders' Leather Chair (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31228))
                await ishgardHandin.HandInItem(31228, 0, 4);

            // Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28730))
                await ishgardHandin.HandInItem(28730, 16, 5);

            // Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28738))
                await ishgardHandin.HandInItem(28738, 15, 5);

            // Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28746))
                await ishgardHandin.HandInItem(28746, 14, 5);

            // Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28754))
                await ishgardHandin.HandInItem(28754, 13, 5);

            // Skybuilders' Waterproof Sheet (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28762))
                await ishgardHandin.HandInItem(28762, 12, 5);

            // Grade 2 Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29797))
                await ishgardHandin.HandInItem(29797, 11, 5);

            // Grade 2 Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29805))
                await ishgardHandin.HandInItem(29805, 10, 5);

            // Grade 2 Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29813))
                await ishgardHandin.HandInItem(29813, 9, 5);

            // Grade 2 Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29821))
                await ishgardHandin.HandInItem(29821, 8, 5);

            // Grade 2 Skybuilders' Gazebo (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29829))
                await ishgardHandin.HandInItem(29829, 7, 5);

            // Grade 2 Artisanal Skybuilders' Wallpaper (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29837))
                await ishgardHandin.HandInItem(29837, 6, 5);

            // Grade 3 Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31189))
                await ishgardHandin.HandInItem(31189, 5, 5);

            // Grade 3 Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31197))
                await ishgardHandin.HandInItem(31197, 4, 5);

            // Grade 3 Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31205))
                await ishgardHandin.HandInItem(31205, 3, 5);

            // Grade 3 Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31213))
                await ishgardHandin.HandInItem(31213, 2, 5);

            // Grade 3 Skybuilders' Awning (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31221))
                await ishgardHandin.HandInItem(31221, 1, 5);

            // Grade 3 Artisanal Skybuilders' Apron (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31229))
                await ishgardHandin.HandInItem(31229, 0, 5);

            // Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28731))
                await ishgardHandin.HandInItem(28731, 16, 6);

            // Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28739))
                await ishgardHandin.HandInItem(28739, 15, 6);

            // Skybuilders' Charcoal (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28747))
                await ishgardHandin.HandInItem(28747, 14, 6);

            // Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28755))
                await ishgardHandin.HandInItem(28755, 13, 6);

            // Skybuilders' Alchemic (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28763))
                await ishgardHandin.HandInItem(28763, 12, 6);

            // Grade 2 Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29798))
                await ishgardHandin.HandInItem(29798, 11, 6);

            // Grade 2 Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29806))
                await ishgardHandin.HandInItem(29806, 10, 6);

            // Grade 2 Skybuilders' Dye (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29814))
                await ishgardHandin.HandInItem(29814, 9, 6);

            // Grade 2 Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29822))
                await ishgardHandin.HandInItem(29822, 8, 6);

            // Grade 2 Skybuilders' Alchemic (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29830))
                await ishgardHandin.HandInItem(29830, 7, 6);

            // Grade 2 Artisanal Skybuilders' Remedies (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29838))
                await ishgardHandin.HandInItem(29838, 6, 6);

            // Grade 3 Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31190))
                await ishgardHandin.HandInItem(31190, 5, 6);

            // Grade 3 Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31198))
                await ishgardHandin.HandInItem(31198, 4, 6);

            // Grade 3 Skybuilders' Holy Water (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31206))
                await ishgardHandin.HandInItem(31206, 3, 6);

            // Grade 3 Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31214))
                await ishgardHandin.HandInItem(31214, 2, 6);

            // Grade 3 Skybuilders' Growth Formula (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31222))
                await ishgardHandin.HandInItem(31222, 1, 6);

            // Grade 3 Artisanal Skybuilders' Tiled Flooring (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31230))
                await ishgardHandin.HandInItem(31230, 0, 6);

            // Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28732))
                await ishgardHandin.HandInItem(28732, 16, 7);

            // Skybuilders' Uncooked Pasta (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28740))
                await ishgardHandin.HandInItem(28740, 15, 7);

            // Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28748))
                await ishgardHandin.HandInItem(28748, 14, 7);

            // Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28756))
                await ishgardHandin.HandInItem(28756, 13, 7);

            // Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28764))
                await ishgardHandin.HandInItem(28764, 12, 7);

            // Grade 2 Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29799))
                await ishgardHandin.HandInItem(29799, 11, 7);

            // Grade 2 Skybuilders' Bread (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29807))
                await ishgardHandin.HandInItem(29807, 10, 7);

            // Grade 2 Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29815))
                await ishgardHandin.HandInItem(29815, 9, 7);

            // Grade 2 Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29823))
                await ishgardHandin.HandInItem(29823, 8, 7);

            // Grade 2 Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29831))
                await ishgardHandin.HandInItem(29831, 7, 7);

            // Grade 2 Artisanal Skybuilders' Quiche (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29839))
                await ishgardHandin.HandInItem(29839, 6, 7);

            // Grade 3 Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31191))
                await ishgardHandin.HandInItem(31191, 5, 7);

            // Grade 3 Skybuilders' Sesame Cookie (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31199))
                await ishgardHandin.HandInItem(31199, 4, 7);

            // Grade 3 Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31207))
                await ishgardHandin.HandInItem(31207, 3, 7);

            // Grade 3 Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31215))
                await ishgardHandin.HandInItem(31215, 2, 7);

            // Grade 3 Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31223))
                await ishgardHandin.HandInItem(31223, 1, 7);

            // Grade 3 Artisanal Skybuilders' Luncheon (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31231))
                await ishgardHandin.HandInItem(31231, 0, 7);


            HWDSupply.Instance.Close();
            Log($"Done");
            //TreeRoot.Stop("Stop Requested");
            return true;
        }

        public static async Task<bool> BuyItem(uint ItemId, int SelectStringLine = 0)
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();

            await ishgardHandin.BuyItem(ItemId, SelectStringLine);

            return true;
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[Ishgard Handin] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}