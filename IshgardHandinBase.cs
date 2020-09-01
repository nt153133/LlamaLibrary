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
using ff14bot.Pathing.Service_Navigation;
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
           // if (Translator.Language == Language.Chn)
           //     await HandinOld();
          //  else
          //  {
                await HandinNew();
                await GatheringHandin();
          //  }
            return true;
        }

        public static async Task<bool> HandinOld()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();

            // Skybuilders' Plywood (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28725))
                await ishgardHandin.HandInItem(28725, 4, 0);

            // Skybuilders' Wain (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28733))
                await ishgardHandin.HandInItem(28733, 3, 0);

            // Skybuilders' Barrel (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28741))
                await ishgardHandin.HandInItem(28741, 2, 0);

            // Skybuilders' Pedestal (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28749))
                await ishgardHandin.HandInItem(28749, 1, 0);

            // Skybuilders' Bed (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28757))
                await ishgardHandin.HandInItem(28757, 0, 0);

            // Skybuilders' Alloy (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28726))
                await ishgardHandin.HandInItem(28726, 4, 1);

            // Skybuilders' Nails (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28734))
                await ishgardHandin.HandInItem(28734, 3, 1);

            // Skybuilders' Hammer (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28742))
                await ishgardHandin.HandInItem(28742, 2, 1);

            // Skybuilders' Crosscut Saw (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28750))
                await ishgardHandin.HandInItem(28750, 1, 1);

            // Skybuilders' Oven (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28758))
                await ishgardHandin.HandInItem(28758, 0, 1);

            // Skybuilders' Steel Plate (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28727))
                await ishgardHandin.HandInItem(28727, 4, 2);

            // Skybuilders' Rivets (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28735))
                await ishgardHandin.HandInItem(28735, 3, 2);

            // Skybuilders' Cooking Pot (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28743))
                await ishgardHandin.HandInItem(28743, 2, 2);

            // Skybuilders' Counter (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28751))
                await ishgardHandin.HandInItem(28751, 1, 2);

            // Skybuilders' Lamppost (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28759))
                await ishgardHandin.HandInItem(28759, 0, 2);

            // Skybuilders' Ingot (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28728))
                await ishgardHandin.HandInItem(28728, 4, 3);

            // Skybuilders' Rings (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28736))
                await ishgardHandin.HandInItem(28736, 3, 3);

            // Skybuilders' Candelabra (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28744))
                await ishgardHandin.HandInItem(28744, 2, 3);

            // Skybuilders' Stone (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28752))
                await ishgardHandin.HandInItem(28752, 1, 3);

            // Skybuilders' Brazier (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28760))
                await ishgardHandin.HandInItem(28760, 0, 3);

            // Skybuilders' Leather (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28729))
                await ishgardHandin.HandInItem(28729, 4, 4);

            // Skybuilders' Leather Straps (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28737))
                await ishgardHandin.HandInItem(28737, 3, 4);

            // Skybuilders' Rug (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28745))
                await ishgardHandin.HandInItem(28745, 2, 4);

            // Skybuilders' Longboots (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28753))
                await ishgardHandin.HandInItem(28753, 1, 4);

            // Skybuilders' Overalls (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28761))
                await ishgardHandin.HandInItem(28761, 0, 4);

            // Skybuilders' Rope (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28730))
                await ishgardHandin.HandInItem(28730, 4, 5);

            // Skybuilders' Cloth (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28738))
                await ishgardHandin.HandInItem(28738, 3, 5);

            // Skybuilders' Broom (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28746))
                await ishgardHandin.HandInItem(28746, 2, 5);

            // Skybuilders' Gloves (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28754))
                await ishgardHandin.HandInItem(28754, 1, 5);

            // Skybuilders' Waterproof Sheet (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28762))
                await ishgardHandin.HandInItem(28762, 0, 5);

            // Skybuilders' Ink (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28731))
                await ishgardHandin.HandInItem(28731, 4, 6);

            // Skybuilders' Plant Oil (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28739))
                await ishgardHandin.HandInItem(28739, 3, 6);

            // Skybuilders' Charcoal (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28747))
                await ishgardHandin.HandInItem(28747, 2, 6);

            // Skybuilders' Soap (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28755))
                await ishgardHandin.HandInItem(28755, 1, 6);

            // Skybuilders' Alchemic (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28763))
                await ishgardHandin.HandInItem(28763, 0, 6);

            // Skybuilders' Hemp Milk (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28732))
                await ishgardHandin.HandInItem(28732, 4, 7);

            // Skybuilders' Uncooked Pasta (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28740))
                await ishgardHandin.HandInItem(28740, 3, 7);

            // Skybuilders' Tea (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28748))
                await ishgardHandin.HandInItem(28748, 2, 7);

            // Skybuilders' All-purpose Infusion (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28756))
                await ishgardHandin.HandInItem(28756, 1, 7);

            // Skybuilders' Stew (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28764))
                await ishgardHandin.HandInItem(28764, 0, 7);

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
            }

            // Skybuilders' Plywood (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28725))
                await ishgardHandin.HandInItem(28725, 10, 0);

            // Skybuilders' Wain (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28733))
                await ishgardHandin.HandInItem(28733, 9, 0);

            // Skybuilders' Barrel (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28741))
                await ishgardHandin.HandInItem(28741, 8, 0);

            // Skybuilders' Pedestal (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28749))
                await ishgardHandin.HandInItem(28749, 7, 0);

            // Skybuilders' Bed (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28757))
                await ishgardHandin.HandInItem(28757, 6, 0);

            // Grade 2 Skybuilders' Plywood (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29792))
                await ishgardHandin.HandInItem(29792, 5, 0);

            // Grade 2 Skybuilders' Crate (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29800))
                await ishgardHandin.HandInItem(29800, 4, 0);

            // Grade 2 Skybuilders' Grindstone (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29808))
                await ishgardHandin.HandInItem(29808, 3, 0);

            // Grade 2 Skybuilders' Stepladder (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29816))
                await ishgardHandin.HandInItem(29816, 2, 0);

            // Grade 2 Skybuilders' Bed (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29824))
                await ishgardHandin.HandInItem(29824, 1, 0);

            // Grade 2 Artisanal Skybuilders' Wardrobe (carpenter)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29832))
                await ishgardHandin.HandInItem(29832, 0, 0);

            // Skybuilders' Alloy (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28726))
                await ishgardHandin.HandInItem(28726, 10, 1);

            // Skybuilders' Nails (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28734))
                await ishgardHandin.HandInItem(28734, 9, 1);

            // Skybuilders' Hammer (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28742))
                await ishgardHandin.HandInItem(28742, 8, 1);

            // Skybuilders' Crosscut Saw (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28750))
                await ishgardHandin.HandInItem(28750, 7, 1);

            // Skybuilders' Oven (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28758))
                await ishgardHandin.HandInItem(28758, 6, 1);

            // Grade 2 Skybuilders' Alloy (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29793))
                await ishgardHandin.HandInItem(29793, 5, 1);

            // Grade 2 Skybuilders' Nails (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29801))
                await ishgardHandin.HandInItem(29801, 4, 1);

            // Grade 2 Skybuilders' Hammer (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29809))
                await ishgardHandin.HandInItem(29809, 3, 1);

            // Grade 2 Skybuilders' Crosscut Saw (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29817))
                await ishgardHandin.HandInItem(29817, 2, 1);

            // Grade 2 Skybuilders' Oven (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29825))
                await ishgardHandin.HandInItem(29825, 1, 1);

            // Grade 2 Artisanal Skybuilders' Chandelier (blacksmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29833))
                await ishgardHandin.HandInItem(29833, 0, 1);

            // Skybuilders' Steel Plate (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28727))
                await ishgardHandin.HandInItem(28727, 10, 2);

            // Skybuilders' Rivets (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28735))
                await ishgardHandin.HandInItem(28735, 9, 2);

            // Skybuilders' Cooking Pot (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28743))
                await ishgardHandin.HandInItem(28743, 8, 2);

            // Skybuilders' Counter (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28751))
                await ishgardHandin.HandInItem(28751, 7, 2);

            // Skybuilders' Lamppost (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28759))
                await ishgardHandin.HandInItem(28759, 6, 2);

            // Grade 2 Skybuilders' Steel Plate (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29794))
                await ishgardHandin.HandInItem(29794, 5, 2);

            // Grade 2 Skybuilders' Rivets (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29802))
                await ishgardHandin.HandInItem(29802, 4, 2);

            // Grade 2 Skybuilders' Still (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29810))
                await ishgardHandin.HandInItem(29810, 3, 2);

            // Grade 2 Skybuilders' Mesail (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29818))
                await ishgardHandin.HandInItem(29818, 2, 2);

            // Grade 2 Skybuilders' Lamppost (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29826))
                await ishgardHandin.HandInItem(29826, 1, 2);

            // Grade 2 Artisanal Skybuilders' Fireplace (armorer)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29834))
                await ishgardHandin.HandInItem(29834, 0, 2);

            // Skybuilders' Ingot (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28728))
                await ishgardHandin.HandInItem(28728, 10, 3);

            // Skybuilders' Rings (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28736))
                await ishgardHandin.HandInItem(28736, 9, 3);

            // Skybuilders' Candelabra (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28744))
                await ishgardHandin.HandInItem(28744, 8, 3);

            // Skybuilders' Stone (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28752))
                await ishgardHandin.HandInItem(28752, 7, 3);

            // Skybuilders' Brazier (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28760))
                await ishgardHandin.HandInItem(28760, 6, 3);

            // Grade 2 Skybuilders' Ingot (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29795))
                await ishgardHandin.HandInItem(29795, 5, 3);

            // Grade 2 Skybuilders' Rings (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29803))
                await ishgardHandin.HandInItem(29803, 4, 3);

            // Grade 2 Skybuilders' Embroidery Frame (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29811))
                await ishgardHandin.HandInItem(29811, 3, 3);

            // Grade 2 Skybuilders' Stone (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29819))
                await ishgardHandin.HandInItem(29819, 2, 3);

            // Grade 2 Skybuilders' Brazier (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29827))
                await ishgardHandin.HandInItem(29827, 1, 3);

            // Grade 2 Artisanal Skybuilders' Bathtub (goldsmith)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29835))
                await ishgardHandin.HandInItem(29835, 0, 3);

            // Skybuilders' Leather (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28729))
                await ishgardHandin.HandInItem(28729, 10, 4);

            // Skybuilders' Leather Straps (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28737))
                await ishgardHandin.HandInItem(28737, 9, 4);

            // Skybuilders' Rug (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28745))
                await ishgardHandin.HandInItem(28745, 8, 4);

            // Skybuilders' Longboots (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28753))
                await ishgardHandin.HandInItem(28753, 7, 4);

            // Skybuilders' Overalls (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28761))
                await ishgardHandin.HandInItem(28761, 6, 4);

            // Grade 2 Skybuilders' Leather (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29796))
                await ishgardHandin.HandInItem(29796, 5, 4);

            // Grade 2 Skybuilders' Leather Straps (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29804))
                await ishgardHandin.HandInItem(29804, 4, 4);

            // Grade 2 Skybuilders' Rug (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29812))
                await ishgardHandin.HandInItem(29812, 3, 4);

            // Grade 2 Skybuilders' Longboots (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29820))
                await ishgardHandin.HandInItem(29820, 2, 4);

            // Grade 2 Skybuilders' Overalls (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29828))
                await ishgardHandin.HandInItem(29828, 1, 4);

            // Grade 2 Artisanal Skybuilders' Overcoat (leatherworker)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29836))
                await ishgardHandin.HandInItem(29836, 0, 4);

            // Skybuilders' Rope (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28730))
                await ishgardHandin.HandInItem(28730, 10, 5);

            // Skybuilders' Cloth (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28738))
                await ishgardHandin.HandInItem(28738, 9, 5);

            // Skybuilders' Broom (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28746))
                await ishgardHandin.HandInItem(28746, 8, 5);

            // Skybuilders' Gloves (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28754))
                await ishgardHandin.HandInItem(28754, 7, 5);

            // Skybuilders' Waterproof Sheet (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28762))
                await ishgardHandin.HandInItem(28762, 6, 5);

            // Grade 2 Skybuilders' Rope (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29797))
                await ishgardHandin.HandInItem(29797, 5, 5);

            // Grade 2 Skybuilders' Cloth (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29805))
                await ishgardHandin.HandInItem(29805, 4, 5);

            // Grade 2 Skybuilders' Broom (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29813))
                await ishgardHandin.HandInItem(29813, 3, 5);

            // Grade 2 Skybuilders' Gloves (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29821))
                await ishgardHandin.HandInItem(29821, 2, 5);

            // Grade 2 Skybuilders' Gazebo (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29829))
                await ishgardHandin.HandInItem(29829, 1, 5);

            // Grade 2 Artisanal Skybuilders' Wallpaper (weaver)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29837))
                await ishgardHandin.HandInItem(29837, 0, 5);

            // Skybuilders' Ink (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28731))
                await ishgardHandin.HandInItem(28731, 10, 6);

            // Skybuilders' Plant Oil (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28739))
                await ishgardHandin.HandInItem(28739, 9, 6);

            // Skybuilders' Charcoal (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28747))
                await ishgardHandin.HandInItem(28747, 8, 6);

            // Skybuilders' Soap (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28755))
                await ishgardHandin.HandInItem(28755, 7, 6);

            // Skybuilders' Alchemic (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28763))
                await ishgardHandin.HandInItem(28763, 6, 6);

            // Grade 2 Skybuilders' Ink (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29798))
                await ishgardHandin.HandInItem(29798, 5, 6);

            // Grade 2 Skybuilders' Plant Oil (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29806))
                await ishgardHandin.HandInItem(29806, 4, 6);

            // Grade 2 Skybuilders' Dye (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29814))
                await ishgardHandin.HandInItem(29814, 3, 6);

            // Grade 2 Skybuilders' Soap (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29822))
                await ishgardHandin.HandInItem(29822, 2, 6);

            // Grade 2 Skybuilders' Alchemic (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29830))
                await ishgardHandin.HandInItem(29830, 1, 6);

            // Grade 2 Artisanal Skybuilders' Remedies (alchemist)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29838))
                await ishgardHandin.HandInItem(29838, 0, 6);

            // Skybuilders' Hemp Milk (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28732))
                await ishgardHandin.HandInItem(28732, 10, 7);

            // Skybuilders' Uncooked Pasta (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28740))
                await ishgardHandin.HandInItem(28740, 9, 7);

            // Skybuilders' Tea (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28748))
                await ishgardHandin.HandInItem(28748, 8, 7);

            // Skybuilders' All-purpose Infusion (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28756))
                await ishgardHandin.HandInItem(28756, 7, 7);

            // Skybuilders' Stew (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28764))
                await ishgardHandin.HandInItem(28764, 6, 7);

            // Grade 2 Skybuilders' Hemp Milk (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29799))
                await ishgardHandin.HandInItem(29799, 5, 7);

            // Grade 2 Skybuilders' Bread (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29807))
                await ishgardHandin.HandInItem(29807, 4, 7);

            // Grade 2 Skybuilders' Tea (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29815))
                await ishgardHandin.HandInItem(29815, 3, 7);

            // Grade 2 Skybuilders' All-purpose Infusion (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29823))
                await ishgardHandin.HandInItem(29823, 2, 7);

            // Grade 2 Skybuilders' Stew (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29831))
                await ishgardHandin.HandInItem(29831, 1, 7);

            // Grade 2 Artisanal Skybuilders' Quiche (culinarian)
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29839))
                await ishgardHandin.HandInItem(29839, 0, 7);


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

            await ishgardHandin.BuyItem(ItemId,SelectStringLine);

            return true;
        }
        
        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[Ishgard Handin] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}