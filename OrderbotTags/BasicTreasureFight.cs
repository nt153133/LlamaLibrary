using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using LlamaLibrary.Helpers;
using TreeSharp;
using ActionType = ff14bot.Enums.ActionType;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("BasicTreasureFight")]
    public class BasicTreasureFight: ProfileBehavior
    {
        [XmlAttribute("XYZ")]
        public Vector3 XYZ { get; set; }

        [XmlAttribute("ZoneId")]
        public int ZoneId { get; set; }

        private static IEnumerable<uint> Items => InventoryManager.GetBagByInventoryBagId(InventoryBagId.KeyItems)
            .FilledSlots.Select(i => i.RawItemId);

        //we are done when the item is gone
        public override bool IsDone => !TreasureMap.MapPrimary.Keys.Any(i => Items.Contains(i));

        protected override Composite CreateBehavior()
        {
            return new PrioritySelector(
                CommonBehaviors.HandleLoading,
                new ActionRunCoroutine(t => Lisbeth.TravelTo(ZoneId.ToString(), XYZ)),
                //We have not dug yet.
                new Decorator(r => Navigator.InPosition(Core.Me.Location, XYZ, 10) && !GameObjectManager.GameObjects.Any(i => i.Type == GameObjectType.Treasure), 
                        new ActionRunCoroutine(async s =>
                        {
                            ActionManager.DoAction(ActionType.General, 20, Core.Me);
                            return await Coroutine.Wait(10000,
                                () => GameObjectManager.GameObjects.Any(i => i.Type == GameObjectType.Treasure));
                        })),
                new Decorator(r => GameObjectManager.Attackers.Any() && Poi.Current.Type != PoiType.Kill, new ActionRunCoroutine(
                    async s =>
                    {
                        Poi.Current = new Poi(GameObjectManager.Attackers.OrderBy(i => i.NpcId).First(), PoiType.Kill);
                        return true;
                    })),
                new Decorator(s => !GameObjectManager.Attackers.Any() && GameObjectManager.GameObjects.Any(i => i.Type == GameObjectType.Treasure), new ActionRunCoroutine(
                    async s =>
                    {
                        var coffer = GameObjectManager.GameObjects.First(i => i.Type == GameObjectType.Treasure);
                        if (coffer.Distance2D() > 3)
                        {
                            await CommonTasks.MoveTo(coffer.Location, "treasure");
                            return true;
                        }

                        await CommonTasks.StopAndDismount();
                        coffer.Interact();
                        await Coroutine.Wait(10000, () => GameObjectManager.Attackers.Any() || Core.Me.IsCasting);
                        if (Core.Me.IsCasting && !GameObjectManager.Attackers.Any())
                        {
                            await Coroutine.Wait(10000, () => GameObjectManager.Attackers.Any() || !Core.Me.IsCasting);
                        }
                        return true;
                    }))
                );
        }
    }
}