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
    [XmlElement("LLCommandMission")]
    public class LLCommandMission : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("Index")] public int Index { get; set; }

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
            return new ActionRunCoroutine(r => JoinCommandMission(Index));
        }

        private async Task JoinCommandMission(int Index)
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

                //Inside Barracks

                await Navigation.OffMeshMoveInteract(GameObjectManager.GetObjectByNPCId(GrandCompanyHelper.GetNpcByType(GCNpc.Squadron_Sergeant)));

                GameObjectManager.GetObjectByNPCId(GrandCompanyHelper.GetNpcByType(GCNpc.Squadron_Sergeant)).Interact();

                await Coroutine.Wait(10000, () => Talk.DialogOpen);

                if (!Talk.DialogOpen)
                {
                    await Navigation.OffMeshMoveInteract(GameObjectManager.GetObjectByNPCId(GrandCompanyHelper.GetNpcByType(GCNpc.Squadron_Sergeant)));

                   GameObjectManager.GetObjectByNPCId(GrandCompanyHelper.GetNpcByType(GCNpc.Squadron_Sergeant)).Interact();                   
                }

                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(1000, () => !Talk.DialogOpen);
                    await Coroutine.Wait(1000, () => Talk.DialogOpen);
                    await Coroutine.Yield();
                }            

                await Coroutine.Wait(10000, () => SelectString.IsOpen);
                await Buddy.Coroutines.Coroutine.Sleep(500);
                {
                    Log($"Choosing Command Missions");
                    ff14bot.RemoteWindows.SelectString.ClickSlot(0);
                }            

                await Coroutine.Wait(10000, () => GcArmyCapture.Instance.IsOpen);
                {
                    GcArmyCapture.Instance.SelectDuty(Index);
                    GcArmyCapture.Instance.Commence();
                } 

                await Coroutine.Wait(5000, () => ContentsFinderConfirm.IsOpen);
                await Buddy.Coroutines.Coroutine.Sleep(500);

                if (ff14bot.RemoteWindows.ContentsFinderConfirm.IsOpen)						
                {
                    Log($"Commencing Duty.");
                    ff14bot.RemoteWindows.ContentsFinderConfirm.Commence();
                    await Coroutine.Wait(10000, () => CommonBehaviors.IsLoading);
                    if (CommonBehaviors.IsLoading)
                    {
                        await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                    }									
                }

                Log($"Should be in duty");

                var director = ((ff14bot.Directors.InstanceContentDirector) DirectorManager.ActiveDirector);
                if (director !=null)
                {

                        if (director.TimeLeftInDungeon >= new TimeSpan(1,30,0))
                        {
                            Log($"Barrier up");
                            await Coroutine.Wait(-1, () => director.TimeLeftInDungeon < new TimeSpan(1,29,58));
                        }

                }
                else
                {
                    Log($"Director is null");
                }

                Log($"Should be ready");                

                _isDone = true;
        }
    }
}
