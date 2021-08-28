using System.ComponentModel;
using System.Threading.Tasks;
using Clio.XmlEngine;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("BuyGCItem")]
    public class BuyGCItem: ProfileBehavior
    {

            private bool _isDone;

            [XmlAttribute("ItemId")] public int ItemId { get; set; }
        
            [XmlAttribute("Count")] 
            [DefaultValue(1)]
            public int Count { get; set; }

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
                return new ActionRunCoroutine(r => BuyGCKnownItem(ItemId, Count));
            }

            private async Task BuyGCKnownItem(int itemId, int Count)
            {
                await LlamaLibrary.Helpers.GrandCompanyShop.BuyKnownItem((uint) itemId, Count);

                _isDone = true;
            }
        }
    
}