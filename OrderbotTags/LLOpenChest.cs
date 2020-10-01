using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLOpenChest")]
    public class LLOpenChest : ProfileBehavior
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
            return new ActionRunCoroutine(r => OpenChestTask());
        }

        private async Task OpenChestTask()
        {
            while (GameObjectManager.GetObjectsOfType<Treasure>().Any(r => r.Distance() < 50 && (r.Name == "宝箱" || r.Name == "Treasure Coffer" || r.Name == "treasure coffer")))
            {
                var _chest = GameObjectManager.GetObjectsOfType<Treasure>().FirstOrDefault(r => r.Distance() < 50 && (r.Name == "宝箱" || r.Name == "Treasure Coffer" || r.Name == "treasure coffer"));
                while (Core.Me.Distance(_chest.Location) > 1)
                {
                    await CommonTasks.MoveTo(_chest.Location);
                    await Coroutine.Yield();
                }

                Navigator.PlayerMover.MoveStop();
                await Coroutine.Sleep(1000);
                _chest.Interact();
                await Coroutine.Sleep(3000);
            }

            _isDone = true;
        }
    }
}