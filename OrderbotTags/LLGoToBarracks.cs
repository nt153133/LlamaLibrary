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
    [XmlElement("LLGoToBarracks")]
    public class LLGoToBarracks : ProfileBehavior
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
            return new ActionRunCoroutine(r => GoToBarracksTask());
        }

        private async Task GoToBarracksTask()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

            // Not in Barracks
                Log($"Moving to Barracks");
                await GrandCompanyHelper.InteractWithNpc(GCNpc.Entrance_to_the_Barracks);
                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                await Buddy.Coroutines.Coroutine.Sleep(500);
                if (ff14bot.RemoteWindows.SelectYesno.IsOpen)
                {
                    Log($"Selecting Yes.");
                    ff14bot.RemoteWindows.SelectYesno.ClickYes(); 
                }
                await Coroutine.Wait(5000, () => CommonBehaviors.IsLoading);
                while (CommonBehaviors.IsLoading)
                {
                    Log($"Waiting for zoning to finish...");
                    await Coroutine.Wait(-1, () => (!CommonBehaviors.IsLoading));
                    
                }               
                
            
 

				_isDone = true;
        }
    }
}