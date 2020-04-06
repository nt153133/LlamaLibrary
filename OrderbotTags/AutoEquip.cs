using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using TreeSharp;
using Action = TreeSharp.Action;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("AutoEquip")]
    public class AutoEquip : ProfileBehavior
    {
        private bool _isDone;
        private bool _isOpening;

        public override bool IsDone => _isDone;

        [XmlAttribute("UpdateGearSet")]
        [DefaultValue(false)]
        public bool UpdateGearSet { get; set; } = false;

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
            _isOpening = false;
        }

        protected override Composite CreateBehavior()
        {
            return RepairBehavior;
        }

        public Composite RepairBehavior
        {
            get
            {
                return new PrioritySelector(new PrioritySelector(new Decorator(r => RecommendEquip.Instance.IsOpen,
                                                                               new Sequence(new Action(r => Log($"{RecommendEquip.Instance.Name} Window open")),
                                                                                            new Action(r => RecommendEquip.Instance.Confirm()),
                                                                                            new Sleep(1000),
                                                                                            new Action(r => _isOpening = false),
                                                                                            new Action(r => _isDone = true),
                                                                                            new Decorator(r => UpdateGearSet, new Action(async r => await ScriptConditions.Helpers.UpdateGearSet())))),

                                                                 new Decorator(r => !RecommendEquip.Instance.IsOpen && !IsDone && !_isOpening,
                                                                               new Sequence(new Action(r => AgentRecommendEquip.Instance.Toggle()),
                                                                                            new Action(r => _isOpening = true)))));
            }
        }
    }
}