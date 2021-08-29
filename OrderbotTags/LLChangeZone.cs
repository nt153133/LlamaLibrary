using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLChangeZone")]
    public class LLChangeZone : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("Heading")] public float Heading { get; set; }

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
            return new ActionRunCoroutine(r => ChangeZoneTask(Heading));
        }

        private async Task ChangeZoneTask(float Heading)
        {
            MovementManager.SetFacing(Heading);
            MovementManager.MoveForwardStart();
            while (!CommonBehaviors.IsLoading) { await Coroutine.Yield(); }
            MovementManager.MoveStop();

            _isDone = true;
        }
    }
}