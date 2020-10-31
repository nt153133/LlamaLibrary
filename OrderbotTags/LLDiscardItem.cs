using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using LlamaLibrary.Extensions;
using TreeSharp;
using static LlamaLibrary.Helpers.GeneralFunctions;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLDiscardItem")]
    public class LLDiscardItem : ProfileBehavior
    {
        [XmlAttribute("ItemIDs")]
        [XmlAttribute("ItemIds")]
        [XmlAttribute("ItemID")]
        [XmlAttribute("ItemId")]
        [DefaultValue(new int[0])]
        private int[] ItemIds { get; set; }
        
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
            return new ActionRunCoroutine(r => DiscardItems());
        }

        private async Task DiscardItems()
        {
            if (_isDone)
            {
                await Coroutine.Yield();
                return;
            }

            List<BagSlot> slots = InventoryManager.FilledSlots.Where(x => ItemIds.Contains((int)x.RawItemId)).ToList();

            if (!slots.Any())
            {
                _isDone = true;
                return;
            }
            
            await StopBusy(leaveDuty:false, dismount:false);

            foreach (var slot in slots)
            {
                string name = slot.Name;
                slot.Discard();
                if (await Coroutine.Wait(5000, () => !slot.IsValid || !slot.IsFilled))
                {
                    Log($"Discarded item: {name}.");
                    await Coroutine.Sleep(800);
                }
                else
                {
                    Log($"Could not discard item: {name}.");
                }
            }

            _isDone = true;
        }
    }
}