using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using TreeSharp;

namespace ff14bot
{
    [XmlElement("ChangeClass")]
    public class ChangeClass : ProfileBehavior
    {
        private bool _isDone;

        public override bool IsDone => _isDone;

        public override bool HighPriority => true;

        [XmlAttribute("Job")] public string job { get; set; }


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

        private async Task<bool> ChangeJob()
        {
            var gearSets = GearsetManager.GearSets.Where(i => i.InUse);

            ClassJobType newjob;
            var foundJob = Enum.TryParse(job.Trim(), true, out newjob);

            if (foundJob && gearSets.Any(gs => gs.Class == newjob))
            {
                gearSets.First(gs => gs.Class == newjob).Activate();
                await Coroutine.Sleep(1000);
            }

            _isDone = true;
            return true;
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => ChangeJob());
        }
    }
}