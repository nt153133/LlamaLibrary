using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using Ishgard;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace IshgardHandinBase
{
    public class IshgardHandinBase : BotBase
    {
        private Composite _root;
        public override string Name => "Ishgard Handin";
        public override PulseFlags PulseFlags => PulseFlags.All;

        //public override Composite Root => new ActionAlwaysFail();
        public override bool IsAutonomous => true;

        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        private async Task<bool> Run()
        {
            await Handin();

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

        public async Task<bool> Handin()
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
    }
}