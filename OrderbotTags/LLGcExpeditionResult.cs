using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLGcExpeditionResult")]
    public class LLGcExpeditionResult : ProfileBehavior
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
            return new ActionRunCoroutine(r => GcExpeditionResultTask());
        }

        private async Task GcExpeditionResultTask()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            
            //Inside Barracks

            await Navigation.OffMeshMoveInteract(GameObjectManager.GetObjectByNPCId(GrandCompanyHelper.GetNpcByType(GCNpc.Squadron_Sergeant)));
                
            GameObjectManager.GetObjectByNPCId(GrandCompanyHelper.GetNpcByType(GCNpc.Squadron_Sergeant)).Interact();
            
            await Coroutine.Wait(10000, () => GcArmyExpeditionResult.Instance.IsOpen);
            GcArmyExpeditionResult.Instance.Close();

            _isDone = true;
        }
    }
}
