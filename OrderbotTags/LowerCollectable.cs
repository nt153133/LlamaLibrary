using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using LlamaLibrary.Extensions;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LowerCollectables")]
    public class LowerCollectable: ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("Collectability")] public int MaxCollectability { get; set; }
        
        public override bool HighPriority => true;
        
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
            return new ActionRunCoroutine(r => LowerQualityAndCombine(MaxCollectability));
        }

        private async Task LowerQualityAndCombine(int collectability)
        {
            var HQslots = InventoryManager.FilledSlots.Where(slot => slot.IsCollectable && slot.Collectability < collectability);
            List<uint> ids = new List<uint>();

            if (HQslots.Any())
            {
                foreach (var slot in HQslots)
                {
                    Logging.Write(Colors.Chocolate, $"[LowerQuality] Lower {slot}");
                    slot.LowerQuality();
                    ids.Add(slot.RawItemId);
                    await Coroutine.Sleep(1000);
                }
            }



            foreach (var id in ids.Distinct())
            {
                var NQslots = InventoryManager.FilledSlots.Where(slot => slot.RawItemId == id  && !slot.IsCollectable && !slot.IsHighQuality && slot.Item.StackSize > 1);

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

            _isDone = true;
        }

        public override bool IsDone => _isDone;
    }
    
    
}