using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing.Service_Navigation;
using LlamaLibrary.Helpers;
using TreeSharp;

namespace LlamaLibrary
{
    public class AutoFollowBase : BotBase
    {
        private AutoFollowSettingsFrm _settings;

        public override string Name { get; } = "AutoFollow";

        public override PulseFlags PulseFlags { get; } = PulseFlags.All;
        private Composite _root;
        public override Composite Root => _root;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override bool WantButton => true;

        internal static bool Paused { get; set; }

        public Composite FollowBehavior
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(r => !Paused,
                        new PrioritySelector(
                            new Decorator(r => CommonBehaviors.IsLoading, CommonBehaviors.HandleLoading),
                            new Decorator(r => true, MoveTo(Vector3.Zero, 9f))
                        )
                    )
                );
            }
        }

        public override void OnButtonPress()
        {
            if (_settings == null || _settings.IsDisposed)
                _settings = new AutoFollowSettingsFrm();
            try
            {
                _settings.Show();
                _settings.Activate();
            }
            catch (ArgumentOutOfRangeException ee)
            {
            }
        }

        public AutoFollowBase()
        {
        }

        public Composite MoveTo(Vector3 location, float distance)
        {
            return new PrioritySelector(CommonBehaviors.CreateMountBehavior(), CommonBehaviors.CreateSprintBehavior(), new ActionRunCoroutine(i => MoveToTask(PartyManager.PartyLeader.BattleCharacter, 3f)));
            //return  new ActionRunCoroutine((i) => MoveToTask(GetTargetLocation(), 9f));
        }

        public static async Task<bool> MoveToTask(BattleCharacter target, float distance)
        {
            //Logger.LogCritical($"Move To task");
            if (target.Location == Vector3.Zero) return false;
            //Logger.LogCritical($"move to {location}");
            while (target.Location.Distance2DSqr(Core.Me.Location) >= distance)
            {
                Navigator.PlayerMover.MoveTowards(target.Location);
                await Coroutine.Yield();
                // Navigator.PlayerMover.MoveStop();
            }

            Navigator.PlayerMover.MoveStop();

            return true;
        }

        public Vector3 GetTargetLocation()
        {
            if (PartyManager.IsInParty && AutoFollowSettings.Instance.FollowLeader)
            {
                Logger.LogCritical($"set target location {PartyManager.PartyLeader.BattleCharacter.Location}");
                return PartyManager.PartyLeader.BattleCharacter.Location;
            }
            
            return Vector3.Zero;
        }

        public override void Initialize()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
        }

        public override void Start()
        {
            
            _root = FollowBehavior;
        }

        public override void Stop()
        {
            _root = null;
        }
    }
}