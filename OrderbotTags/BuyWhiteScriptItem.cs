using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("BuyWhiteScriptItem")]
    public class BuyWhiteScriptItem : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("ItemId")] public int ItemId { get; set; }

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
            return new ActionRunCoroutine(r => BuyWhiteScrip(ItemId));
        }

        private async Task BuyWhiteScrip(int itemId)
        {
            await Coroutine.Sleep(500);
            await IshgardHandinBase.BuyItem((uint) itemId);

            _isDone = true;
        }
    }
}