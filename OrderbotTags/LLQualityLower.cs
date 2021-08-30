using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using TreeSharp;
using static LlamaLibrary.FCWorkshopBase;
using static LlamaLibrary.Helpers.GeneralFunctions;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLQualityLower")]
    public class LLQualityLower : ProfileBehavior
    {
        [XmlAttribute("ItemIDs")]
        [XmlAttribute("ItemIds")]
        [XmlAttribute("ItemID")]
        [XmlAttribute("ItemId")]
        [DefaultValue(new int[0])]
        private int[] ItemIds { get; set; }

        [XmlAttribute("ForceLower")]
        [XmlAttribute("forcelower")]
        [XmlAttribute("Force")]
        [XmlAttribute("force")]
        [DefaultValue(false)]
        private bool ForceLower { get; set; }

        [XmlAttribute("AllItems")]
        [XmlAttribute("allitems")]
        [XmlAttribute("All")]
        [XmlAttribute("all")]
        [DefaultValue(false)]
        private bool AllItems { get; set; }

        [XmlAttribute("IgnoreGear")]
        [XmlAttribute("ignoregear")]
        [DefaultValue(true)]
        private bool IgnoreGear { get; set; }

        private bool _isDone;

        public override bool HighPriority => true;

        public override bool IsDone => _isDone;

        protected override void OnStart()
        {
        }

        protected override void OnDone()
        {
        }

        protected override void OnResetCachedDone()
        {
            _isDone = false;
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => LowerItemQuality());
        }

        private async Task LowerItemQuality()
        {
            if (_isDone)
            {
                await Coroutine.Yield();
                return;
            }

            List<BagSlot> bagSlots = IgnoreGear ? InventoryManager.FilledSlots.ToList() : InventoryManager.FilledSlots.Where(x => x.Item.StackSize == 999).ToList();

            List<uint> toLower;

            if (AllItems && ForceLower)
            {
                toLower = bagSlots.Where(x => x.IsHighQuality).Select(x => x.RawItemId).ToList();
            }
            else if (AllItems)
            {
                var HQ = bagSlots.Where(x => x.IsHighQuality).Select(x => x.RawItemId).OrderBy(x => x);
                var NQ = bagSlots.Where(x => !x.IsHighQuality).Select(x => x.RawItemId).OrderBy(x => x);
                toLower = NQ.Intersect(HQ).ToList();
            }
            else if (ForceLower)
            {
                toLower = ItemIds.Select(x => (uint) x).ToList();
            }
            else
            {
                var slotsWithId = bagSlots.Where(x => ItemIds.Contains((int) x.RawItemId)).ToList();
                var HQ = slotsWithId.Where(x => x.IsHighQuality).Select(x => x.RawItemId).OrderBy(x => x);
                var NQ = slotsWithId.Where(x => !x.IsHighQuality).Select(x => x.RawItemId).OrderBy(x => x);
                toLower = NQ.Intersect(HQ).ToList();
            }

            if (!toLower.Any())
            {
                _isDone = true;
                return;
            }

            await StopBusy(leaveDuty:false, dismount: false);

            foreach (var itemId in toLower)
            {
                await LowerQualityAndCombine((int) itemId);
                await Coroutine.Sleep(200);
            }

            _isDone = true;
        }
    }
}