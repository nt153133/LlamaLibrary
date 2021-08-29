using System.Threading.Tasks;
using Clio.XmlEngine;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLGoHome")]
    public class LLGoHome : ProfileBehavior
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
            return new ActionRunCoroutine(r => LLGoHomeTask());
        }

        private async Task LLGoHomeTask()
        {
            await LlamaLibrary.Helpers.GeneralFunctions.GoHome();	

            _isDone = true;
        }
    }
}