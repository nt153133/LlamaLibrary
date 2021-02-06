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

        private static uint[] items20Old = {28725, 29792, 28726, 29793, 28727, 29794, 28728, 29795, 28729, 29796, 28730, 29797, 28731, 29798, 28732, 29799};
        private static uint[] items40Old = {28733, 29800, 28734, 29801, 28735, 29802, 28736, 29803, 28737, 29804, 28738, 29805, 28739, 29806, 28740, 29807};
        private static uint[] items150Old = {28741, 29808, 28742, 29809, 28743, 29810, 28744, 29811, 28745, 29812, 28746, 29813, 28747, 29814, 28748, 29815};
        private static uint[] items290Old = {28749, 29816, 28750, 29817, 28751, 29818, 28752, 29819, 28753, 29820, 28754, 29821, 28755, 29822, 28756, 29823};
        private static uint[] items430Old = {28757, 29824, 28758, 29825, 28759, 29826, 28760, 29827, 28761, 29828, 28762, 29829, 28763, 29830, 28764, 29831};
        private static uint[] items481Old = {29832, 29833, 29834, 29835, 29836, 29837, 29838, 29839};
        private static uint[] items511Old = {31224, 31225, 31226, 31227, 31228, 31229, 31230, 31231};
        private static uint[] items20 = { 28725,29792,31184,31913,28726,29793,31185,31914,28727,29794,31186,31915,28728,29795,31187,31916,28729,29796,31188,31917,28730,29797,31189,31918,28731,29798,31190,31919,28732,29799,31191,31920 };
        private static uint[] items40 = { 28733,29800,31192,31921,28734,29801,31193,31922,28735,29802,31194,31923,28736,29803,31195,31924,28737,29804,31196,31925,28738,29805,31197,31926,28739,29806,31198,31927,28740,29807,31199,31928 };
        private static uint[] items150 = { 28741,29808,31200,31929,28742,29809,31201,31930,28743,29810,31202,31931,28744,29811,31203,31932,28745,29812,31204,31933,28746,29813,31205,31934,28747,29814,31206,31935,28748,29815,31207,31936 };
        private static uint[] items290 = { 28749,29816,31208,31937,28750,29817,31209,31938,28751,29818,31210,31939,28752,29819,31211,31940,28753,29820,31212,31941,28754,29821,31213,31942,28755,29822,31214,31943,28756,29823,31215,31944 };
        private static uint[] items430 = { 28757,29824,31216,31945,28758,29825,31217,31946,28759,29826,31218,31947,28760,29827,31219,31948,28761,29828,31220,31949,28762,29829,31221,31950,28763,29830,31222,31951,28764,29831,31223,31952 };
        private static uint[] items481 = { 29832,29833,29834,29835,29836,29837,29838,29839 };
        private static uint[] items511 = { 31224,31225,31226,31227,31228,31229,31230,31231 };
        private static uint[] items513 = { 31953,31954,31955,31956,31957,31958,31959,31960 };



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
            await GeneralFunctions.StopBusy(dismount: false);
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

        public static async Task<bool> HandinOld()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();

            if (DiscardCollectable)
            {
                foreach (var item in InventoryManager.FilledSlots.Where(i => items20Old.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 50))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items40Old.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 90))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items150Old.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 300))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items290Old.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 480))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items430Old.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 1350))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }

                foreach (var item in InventoryManager.FilledSlots.Where(i => items481Old.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 4500))
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
                
                foreach (var item in InventoryManager.FilledSlots.Where(i => items513.Contains(i.RawItemId) && i.IsCollectable && i.Collectability < 5800))
                {
                    item.Discard();
                    Log($"Discarding {item.Name}");
                    await Coroutine.Sleep(3000);
                }
            }

            // Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28725))
                await ishgardHandin.HandInItem(28725, 22, 0);

            // Skybuilders' Wain (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28733))
                await ishgardHandin.HandInItem(28733, 21, 0);

            // Skybuilders' Barrel (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28741))
                await ishgardHandin.HandInItem(28741, 20, 0);

            // Skybuilders' Pedestal (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28749))
                await ishgardHandin.HandInItem(28749, 19, 0);

            // Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28757))
                await ishgardHandin.HandInItem(28757, 18, 0);

            // Grade 2 Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29792))
                await ishgardHandin.HandInItem(29792, 17, 0);

            // Grade 2 Skybuilders' Crate (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29800))
                await ishgardHandin.HandInItem(29800, 16, 0);

            // Grade 2 Skybuilders' Grindstone (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29808))
                await ishgardHandin.HandInItem(29808, 15, 0);

            // Grade 2 Skybuilders' Stepladder (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29816))
                await ishgardHandin.HandInItem(29816, 14, 0);

            // Grade 2 Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29824))
                await ishgardHandin.HandInItem(29824, 13, 0);

            // Grade 2 Artisanal Skybuilders' Wardrobe (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29832))
                await ishgardHandin.HandInItem(29832, 12, 0);

            // Grade 3 Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31184))
                await ishgardHandin.HandInItem(31184, 11, 0);

            // Grade 3 Skybuilders' Crate (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31192))
                await ishgardHandin.HandInItem(31192, 10, 0);

            // Grade 3 Skybuilders' Grinding Wheel (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31200))
                await ishgardHandin.HandInItem(31200, 9, 0);

            // Grade 3 Skybuilders' Stepladder (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31208))
                await ishgardHandin.HandInItem(31208, 8, 0);

            // Grade 3 Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31216))
                await ishgardHandin.HandInItem(31216, 7, 0);

            // Grade 3 Artisanal Skybuilders' Goldsmithing Bench (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31224))
                await ishgardHandin.HandInItem(31224, 6, 0);

            // Grade 4 Skybuilders' Plywood (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31913))
                await ishgardHandin.HandInItem(31913, 5, 0);

            // Grade 4 Skybuilders' Crate (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31921))
                await ishgardHandin.HandInItem(31921, 4, 0);

            // Grade 4 Skybuilders' Spinning Wheel (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31929))
                await ishgardHandin.HandInItem(31929, 3, 0);

            // Grade 4 Skybuilders' Stepladder (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31937))
                await ishgardHandin.HandInItem(31937, 2, 0);

            // Grade 4 Skybuilders' Bed (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31945))
                await ishgardHandin.HandInItem(31945, 1, 0);

            // Grade 4 Artisanal Skybuilders' Ice Box (Carpenter) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31953))
                await ishgardHandin.HandInItem(31953, 0, 0);

            // Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28726))
                await ishgardHandin.HandInItem(28726, 22, 1);

            // Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28734))
                await ishgardHandin.HandInItem(28734, 21, 1);

            // Skybuilders' Hammer (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28742))
                await ishgardHandin.HandInItem(28742, 20, 1);

            // Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28750))
                await ishgardHandin.HandInItem(28750, 19, 1);

            // Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28758))
                await ishgardHandin.HandInItem(28758, 18, 1);

            // Grade 2 Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29793))
                await ishgardHandin.HandInItem(29793, 17, 1);

            // Grade 2 Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29801))
                await ishgardHandin.HandInItem(29801, 16, 1);

            // Grade 2 Skybuilders' Hammer (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29809))
                await ishgardHandin.HandInItem(29809, 15, 1);

            // Grade 2 Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29817))
                await ishgardHandin.HandInItem(29817, 14, 1);

            // Grade 2 Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29825))
                await ishgardHandin.HandInItem(29825, 13, 1);

            // Grade 2 Artisanal Skybuilders' Chandelier (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29833))
                await ishgardHandin.HandInItem(29833, 12, 1);

            // Grade 3 Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31185))
                await ishgardHandin.HandInItem(31185, 11, 1);

            // Grade 3 Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31193))
                await ishgardHandin.HandInItem(31193, 10, 1);

            // Grade 3 Skybuilders' Pickaxe (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31201))
                await ishgardHandin.HandInItem(31201, 9, 1);

            // Grade 3 Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31209))
                await ishgardHandin.HandInItem(31209, 8, 1);

            // Grade 3 Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31217))
                await ishgardHandin.HandInItem(31217, 7, 1);

            // Grade 3 Artisanal Skybuilders' Smithing Bench (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31225))
                await ishgardHandin.HandInItem(31225, 6, 1);

            // Grade 4 Skybuilders' Alloy (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31914))
                await ishgardHandin.HandInItem(31914, 5, 1);

            // Grade 4 Skybuilders' Nails (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31922))
                await ishgardHandin.HandInItem(31922, 4, 1);

            // Grade 4 Skybuilders' Hatchet (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31930))
                await ishgardHandin.HandInItem(31930, 3, 1);

            // Grade 4 Skybuilders' Crosscut Saw (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31938))
                await ishgardHandin.HandInItem(31938, 2, 1);

            // Grade 4 Skybuilders' Oven (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31946))
                await ishgardHandin.HandInItem(31946, 1, 1);

            // Grade 4 Artisanal Skybuilders' Chocobo Weathervane (Blacksmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31954))
                await ishgardHandin.HandInItem(31954, 0, 1);

            // Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28727))
                await ishgardHandin.HandInItem(28727, 22, 2);

            // Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28735))
                await ishgardHandin.HandInItem(28735, 21, 2);

            // Skybuilders' Cooking Pot (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28743))
                await ishgardHandin.HandInItem(28743, 20, 2);

            // Skybuilders' Counter (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28751))
                await ishgardHandin.HandInItem(28751, 19, 2);

            // Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28759))
                await ishgardHandin.HandInItem(28759, 18, 2);

            // Grade 2 Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29794))
                await ishgardHandin.HandInItem(29794, 17, 2);

            // Grade 2 Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29802))
                await ishgardHandin.HandInItem(29802, 16, 2);

            // Grade 2 Skybuilders' Still (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29810))
                await ishgardHandin.HandInItem(29810, 15, 2);

            // Grade 2 Skybuilders' Mesail (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29818))
                await ishgardHandin.HandInItem(29818, 14, 2);

            // Grade 2 Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29826))
                await ishgardHandin.HandInItem(29826, 13, 2);

            // Grade 2 Artisanal Skybuilders' Fireplace (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29834))
                await ishgardHandin.HandInItem(29834, 12, 2);

            // Grade 3 Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31186))
                await ishgardHandin.HandInItem(31186, 11, 2);

            // Grade 3 Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31194))
                await ishgardHandin.HandInItem(31194, 10, 2);

            // Grade 3 Skybuilders' Cookpot (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31202))
                await ishgardHandin.HandInItem(31202, 9, 2);

            // Grade 3 Skybuilders' Mesail (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31210))
                await ishgardHandin.HandInItem(31210, 8, 2);

            // Grade 3 Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31218))
                await ishgardHandin.HandInItem(31218, 7, 2);

            // Grade 3 Artisanal Skybuilders' Door (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31226))
                await ishgardHandin.HandInItem(31226, 6, 2);

            // Grade 4 Skybuilders' Steel Plate (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31915))
                await ishgardHandin.HandInItem(31915, 5, 2);

            // Grade 4 Skybuilders' Rivets (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31923))
                await ishgardHandin.HandInItem(31923, 4, 2);

            // Grade 4 Skybuilders' Cookpot (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31931))
                await ishgardHandin.HandInItem(31931, 3, 2);

            // Grade 4 Skybuilders' Mesail (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31939))
                await ishgardHandin.HandInItem(31939, 2, 2);

            // Grade 4 Skybuilders' Lamppost (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31947))
                await ishgardHandin.HandInItem(31947, 1, 2);

            // Grade 4 Artisanal Skybuilders' Company Chest (Armorer) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31955))
                await ishgardHandin.HandInItem(31955, 0, 2);

            // Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28728))
                await ishgardHandin.HandInItem(28728, 22, 3);

            // Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28736))
                await ishgardHandin.HandInItem(28736, 21, 3);

            // Skybuilders' Candelabra (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28744))
                await ishgardHandin.HandInItem(28744, 20, 3);

            // Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28752))
                await ishgardHandin.HandInItem(28752, 19, 3);

            // Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28760))
                await ishgardHandin.HandInItem(28760, 18, 3);

            // Grade 2 Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29795))
                await ishgardHandin.HandInItem(29795, 17, 3);

            // Grade 2 Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29803))
                await ishgardHandin.HandInItem(29803, 16, 3);

            // Grade 2 Skybuilders' Embroidery Frame (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29811))
                await ishgardHandin.HandInItem(29811, 15, 3);

            // Grade 2 Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29819))
                await ishgardHandin.HandInItem(29819, 14, 3);

            // Grade 2 Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29827))
                await ishgardHandin.HandInItem(29827, 13, 3);

            // Grade 2 Artisanal Skybuilders' Bathtub (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29835))
                await ishgardHandin.HandInItem(29835, 12, 3);

            // Grade 3 Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31187))
                await ishgardHandin.HandInItem(31187, 11, 3);

            // Grade 3 Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31195))
                await ishgardHandin.HandInItem(31195, 10, 3);

            // Grade 3 Skybuilders' Embroidery Frame (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31203))
                await ishgardHandin.HandInItem(31203, 9, 3);

            // Grade 3 Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31211))
                await ishgardHandin.HandInItem(31211, 8, 3);

            // Grade 3 Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31219))
                await ishgardHandin.HandInItem(31219, 7, 3);

            // Grade 3 Artisanal Skybuilders' Chronometer (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31227))
                await ishgardHandin.HandInItem(31227, 6, 3);

            // Grade 4 Skybuilders' Ingot (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31916))
                await ishgardHandin.HandInItem(31916, 5, 3);

            // Grade 4 Skybuilders' Rings (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31924))
                await ishgardHandin.HandInItem(31924, 4, 3);

            // Grade 4 Skybuilders' Embroidery Frame (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31932))
                await ishgardHandin.HandInItem(31932, 3, 3);

            // Grade 4 Skybuilders' Stone (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31940))
                await ishgardHandin.HandInItem(31940, 2, 3);

            // Grade 4 Skybuilders' Brazier (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31948))
                await ishgardHandin.HandInItem(31948, 1, 3);

            // Grade 4 Artisanal Skybuilders' Astroscope (Goldsmith) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31956))
                await ishgardHandin.HandInItem(31956, 0, 3);

            // Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28729))
                await ishgardHandin.HandInItem(28729, 22, 4);

            // Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28737))
                await ishgardHandin.HandInItem(28737, 21, 4);

            // Skybuilders' Rug (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28745))
                await ishgardHandin.HandInItem(28745, 20, 4);

            // Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28753))
                await ishgardHandin.HandInItem(28753, 19, 4);

            // Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28761))
                await ishgardHandin.HandInItem(28761, 18, 4);

            // Grade 2 Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29796))
                await ishgardHandin.HandInItem(29796, 17, 4);

            // Grade 2 Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29804))
                await ishgardHandin.HandInItem(29804, 16, 4);

            // Grade 2 Skybuilders' Rug (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29812))
                await ishgardHandin.HandInItem(29812, 15, 4);

            // Grade 2 Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29820))
                await ishgardHandin.HandInItem(29820, 14, 4);

            // Grade 2 Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29828))
                await ishgardHandin.HandInItem(29828, 13, 4);

            // Grade 2 Artisanal Skybuilders' Overcoat (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29836))
                await ishgardHandin.HandInItem(29836, 12, 4);

            // Grade 3 Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31188))
                await ishgardHandin.HandInItem(31188, 11, 4);

            // Grade 3 Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31196))
                await ishgardHandin.HandInItem(31196, 10, 4);

            // Grade 3 Skybuilders' Leather Sack (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31204))
                await ishgardHandin.HandInItem(31204, 9, 4);

            // Grade 3 Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31212))
                await ishgardHandin.HandInItem(31212, 8, 4);

            // Grade 3 Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31220))
                await ishgardHandin.HandInItem(31220, 7, 4);

            // Grade 3 Artisanal Skybuilders' Leather Chair (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31228))
                await ishgardHandin.HandInItem(31228, 6, 4);

            // Grade 4 Skybuilders' Leather (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31917))
                await ishgardHandin.HandInItem(31917, 5, 4);

            // Grade 4 Skybuilders' Leather Straps (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31925))
                await ishgardHandin.HandInItem(31925, 4, 4);

            // Grade 4 Skybuilders' Leather Sack (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31933))
                await ishgardHandin.HandInItem(31933, 3, 4);

            // Grade 4 Skybuilders' Longboots (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31941))
                await ishgardHandin.HandInItem(31941, 2, 4);

            // Grade 4 Skybuilders' Overalls (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31949))
                await ishgardHandin.HandInItem(31949, 1, 4);

            // Grade 4 Artisanal Skybuilders' Tool Belt (Leatherworker) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31957))
                await ishgardHandin.HandInItem(31957, 0, 4);

            // Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28730))
                await ishgardHandin.HandInItem(28730, 22, 5);

            // Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28738))
                await ishgardHandin.HandInItem(28738, 21, 5);

            // Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28746))
                await ishgardHandin.HandInItem(28746, 20, 5);

            // Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28754))
                await ishgardHandin.HandInItem(28754, 19, 5);

            // Skybuilders' Waterproof Sheet (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28762))
                await ishgardHandin.HandInItem(28762, 18, 5);

            // Grade 2 Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29797))
                await ishgardHandin.HandInItem(29797, 17, 5);

            // Grade 2 Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29805))
                await ishgardHandin.HandInItem(29805, 16, 5);

            // Grade 2 Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29813))
                await ishgardHandin.HandInItem(29813, 15, 5);

            // Grade 2 Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29821))
                await ishgardHandin.HandInItem(29821, 14, 5);

            // Grade 2 Skybuilders' Gazebo (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29829))
                await ishgardHandin.HandInItem(29829, 13, 5);

            // Grade 2 Artisanal Skybuilders' Wallpaper (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29837))
                await ishgardHandin.HandInItem(29837, 12, 5);

            // Grade 3 Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31189))
                await ishgardHandin.HandInItem(31189, 11, 5);

            // Grade 3 Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31197))
                await ishgardHandin.HandInItem(31197, 10, 5);

            // Grade 3 Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31205))
                await ishgardHandin.HandInItem(31205, 9, 5);

            // Grade 3 Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31213))
                await ishgardHandin.HandInItem(31213, 8, 5);

            // Grade 3 Skybuilders' Awning (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31221))
                await ishgardHandin.HandInItem(31221, 7, 5);

            // Grade 3 Artisanal Skybuilders' Apron (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31229))
                await ishgardHandin.HandInItem(31229, 6, 5);

            // Grade 4 Skybuilders' Rope (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31918))
                await ishgardHandin.HandInItem(31918, 5, 5);

            // Grade 4 Skybuilders' Cloth (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31926))
                await ishgardHandin.HandInItem(31926, 4, 5);

            // Grade 4 Skybuilders' Broom (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31934))
                await ishgardHandin.HandInItem(31934, 3, 5);

            // Grade 4 Skybuilders' Gloves (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31942))
                await ishgardHandin.HandInItem(31942, 2, 5);

            // Grade 4 Skybuilders' Awning (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31950))
                await ishgardHandin.HandInItem(31950, 1, 5);

            // Grade 4 Artisanal Skybuilders' Vest (Weaver) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31958))
                await ishgardHandin.HandInItem(31958, 0, 5);

            // Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28731))
                await ishgardHandin.HandInItem(28731, 22, 6);

            // Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28739))
                await ishgardHandin.HandInItem(28739, 21, 6);

            // Skybuilders' Charcoal (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28747))
                await ishgardHandin.HandInItem(28747, 20, 6);

            // Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28755))
                await ishgardHandin.HandInItem(28755, 19, 6);

            // Skybuilders' Alchemic (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28763))
                await ishgardHandin.HandInItem(28763, 18, 6);

            // Grade 2 Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29798))
                await ishgardHandin.HandInItem(29798, 17, 6);

            // Grade 2 Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29806))
                await ishgardHandin.HandInItem(29806, 16, 6);

            // Grade 2 Skybuilders' Dye (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29814))
                await ishgardHandin.HandInItem(29814, 15, 6);

            // Grade 2 Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29822))
                await ishgardHandin.HandInItem(29822, 14, 6);

            // Grade 2 Skybuilders' Alchemic (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29830))
                await ishgardHandin.HandInItem(29830, 13, 6);

            // Grade 2 Artisanal Skybuilders' Remedies (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29838))
                await ishgardHandin.HandInItem(29838, 12, 6);

            // Grade 3 Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31190))
                await ishgardHandin.HandInItem(31190, 11, 6);

            // Grade 3 Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31198))
                await ishgardHandin.HandInItem(31198, 10, 6);

            // Grade 3 Skybuilders' Holy Water (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31206))
                await ishgardHandin.HandInItem(31206, 9, 6);

            // Grade 3 Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31214))
                await ishgardHandin.HandInItem(31214, 8, 6);

            // Grade 3 Skybuilders' Growth Formula (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31222))
                await ishgardHandin.HandInItem(31222, 7, 6);

            // Grade 3 Artisanal Skybuilders' Tiled Flooring (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31230))
                await ishgardHandin.HandInItem(31230, 6, 6);

            // Grade 4 Skybuilders' Ink (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31919))
                await ishgardHandin.HandInItem(31919, 5, 6);

            // Grade 4 Skybuilders' Plant Oil (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31927))
                await ishgardHandin.HandInItem(31927, 4, 6);

            // Grade 4 Skybuilders' Holy Water (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31935))
                await ishgardHandin.HandInItem(31935, 3, 6);

            // Grade 4 Skybuilders' Soap (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31943))
                await ishgardHandin.HandInItem(31943, 2, 6);

            // Grade 4 Skybuilders' Growth Formula (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31951))
                await ishgardHandin.HandInItem(31951, 1, 6);

            // Grade 4 Artisanal Skybuilders' Tincture (Alchemist) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31959))
                await ishgardHandin.HandInItem(31959, 0, 6);

            // Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28732))
                await ishgardHandin.HandInItem(28732, 22, 7);

            // Skybuilders' Uncooked Pasta (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28740))
                await ishgardHandin.HandInItem(28740, 21, 7);

            // Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28748))
                await ishgardHandin.HandInItem(28748, 20, 7);

            // Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28756))
                await ishgardHandin.HandInItem(28756, 19, 7);

            // Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 28764))
                await ishgardHandin.HandInItem(28764, 18, 7);

            // Grade 2 Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29799))
                await ishgardHandin.HandInItem(29799, 17, 7);

            // Grade 2 Skybuilders' Bread (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29807))
                await ishgardHandin.HandInItem(29807, 16, 7);

            // Grade 2 Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29815))
                await ishgardHandin.HandInItem(29815, 15, 7);

            // Grade 2 Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29823))
                await ishgardHandin.HandInItem(29823, 14, 7);

            // Grade 2 Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29831))
                await ishgardHandin.HandInItem(29831, 13, 7);

            // Grade 2 Artisanal Skybuilders' Quiche (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 29839))
                await ishgardHandin.HandInItem(29839, 12, 7);

            // Grade 3 Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31191))
                await ishgardHandin.HandInItem(31191, 11, 7);

            // Grade 3 Skybuilders' Sesame Cookie (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31199))
                await ishgardHandin.HandInItem(31199, 10, 7);

            // Grade 3 Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31207))
                await ishgardHandin.HandInItem(31207, 9, 7);

            // Grade 3 Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31215))
                await ishgardHandin.HandInItem(31215, 8, 7);

            // Grade 3 Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31223))
                await ishgardHandin.HandInItem(31223, 7, 7);

            // Grade 3 Artisanal Skybuilders' Luncheon (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31231))
                await ishgardHandin.HandInItem(31231, 6, 7);

            // Grade 4 Skybuilders' Hemp Milk (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31920))
                await ishgardHandin.HandInItem(31920, 5, 7);

            // Grade 4 Skybuilders' Sesame Cookie (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31928))
                await ishgardHandin.HandInItem(31928, 4, 7);

            // Grade 4 Skybuilders' Tea (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31936))
                await ishgardHandin.HandInItem(31936, 3, 7);

            // Grade 4 Skybuilders' All-purpose Infusion (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31944))
                await ishgardHandin.HandInItem(31944, 2, 7);

            // Grade 4 Skybuilders' Stew (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31952))
                await ishgardHandin.HandInItem(31952, 1, 7);

            // Grade 4 Artisanal Skybuilders' Sorbet (Culinarian) 
            if (InventoryManager.FilledSlots.Any(i => i.RawItemId == 31960))
                await ishgardHandin.HandInItem(31960, 0, 7);


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

        public static async Task<bool> BuyItem(uint ItemId, int maxCount, int SelectStringLine = 0)
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            var ishgardHandin = new IshgardHandin();

            await ishgardHandin.BuyItem(ItemId, maxCount, SelectStringLine);

            return true;
        }
        
        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[Ishgard Handin] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}