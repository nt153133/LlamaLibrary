using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLLeaveDuty")]
    public class LeaveDuty : ProfileBehavior
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
            return new ActionRunCoroutine(r => LeaveDutyTask());
        }

        private async Task LeaveDutyTask()
        {
            ff14bot.Managers.DutyManager.LeaveActiveDuty();
            await Coroutine.Wait(20000, () => CommonBehaviors.IsLoading);
            if (CommonBehaviors.IsLoading)
            {
                await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
            }

            _isDone = true;
        }
    }
}