using Clio.XmlEngine;
using TreeSharp;
using Action = TreeSharp.Action;

namespace ff14bot.NeoProfiles
{
    [XmlElement("LLStopBot")]
	[XmlElement("StopBot")]

    public class LLStopBotTag : ProfileBehavior
    {
        private bool _done;

        public override bool IsDone { get { return _done; } }

        protected override Composite CreateBehavior()
        {
            return new PrioritySelector(
                new Decorator(ret => TreeRoot.IsRunning,
                    new Action(r =>
                    {
                        TreeRoot.Stop();
                        _done = true;
                    })
                )
            );
        }

        /// <summary>
        /// This gets called when a while loop starts over so reset anything that is used inside the IsDone check
        /// </summary>
        protected override void OnResetCachedDone()
        {
            _done = false;
        }

        protected override void OnDone()
        {

        }
    }
}