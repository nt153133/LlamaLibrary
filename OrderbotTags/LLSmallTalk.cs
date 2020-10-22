using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLSmallTalk")]
    public class LLSmallTalk : ProfileBehavior
    {
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
            return new ActionRunCoroutine(r => AwaitSmallTalk());
        }

        private async Task AwaitSmallTalk()
        {
            if (_isDone)
            {
                await Coroutine.Yield();
                return;
            }
            await Helpers.GeneralFunctions.SmallTalk();

            _isDone = true;
        }
    }
}