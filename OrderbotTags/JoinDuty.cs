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
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLJoinDuty")]
    public class LLJoinDuty : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("DutyId")] public int DutyId { get; set; }
        
        [XmlAttribute("Trial")] 
        [DefaultValue(false)]
        public bool Trial { get; set; }
        
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
            return new ActionRunCoroutine(r => JoinDutyTask(DutyId, Trial));
        }

        private async Task JoinDutyTask(int DutyId, bool Trial)
        {
           Logging.WriteDiagnostic("Queuing for Dungeon");
		    GameSettingsManager.JoinWithUndersizedParty = true;
           DutyManager.Queue(DataManager.InstanceContentResults[(uint) DutyId]);
           await Coroutine.Wait(5000, () => (DutyManager.QueueState == QueueState.InQueue || DutyManager.QueueState == QueueState.JoiningInstance));

           Logging.WriteDiagnostic("Queued for Dungeon");

           await Coroutine.Wait(10000, () => (DutyManager.QueueState == QueueState.JoiningInstance));
			
           await Coroutine.Wait(10000, () => (RaptureAtkUnitManager.GetWindowByName("ContentsFinderConfirm") != null));

           Logging.WriteDiagnostic("Commencing");
           DutyManager.Commence();
           Logging.WriteDiagnostic("Waiting for Loading");
           await Coroutine.Wait(10000, () => CommonBehaviors.IsLoading || QuestLogManager.InCutscene);
			
           if (CommonBehaviors.IsLoading)
           {
               await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
           }

           if (QuestLogManager.InCutscene)
           {
               TreeRoot.StatusText = "InCutscene";
               if (ff14bot.RemoteAgents.AgentCutScene.Instance != null)
               {
                   ff14bot.RemoteAgents.AgentCutScene.Instance.PromptSkip();
                   await Coroutine.Wait(250, () => SelectString.IsOpen);
                   if (SelectString.IsOpen)
                       SelectString.ClickSlot(0);
               }
           }

           Logging.WriteDiagnostic("Should be in duty");
		   
           var director = ((ff14bot.Directors.InstanceContentDirector) DirectorManager.ActiveDirector);
           if (director !=null)
		   {
               if (Trial)
               {
                   if (director.TimeLeftInDungeon >= new TimeSpan(0,60,0))
                   {
                       Logging.WriteDiagnostic("Barrier up");
                       await Coroutine.Wait(30000, () => director.TimeLeftInDungeon < new TimeSpan(0,59,58));
                   }
               }
               else
               {
                   if (director.TimeLeftInDungeon >= new TimeSpan(1,30,0))
                   {
                       Logging.WriteDiagnostic("Barrier up");
                       await Coroutine.Wait(30000, () => director.TimeLeftInDungeon < new TimeSpan(1,29,58));
                   } 
               }

		   }
		   else
		   {
			Logging.WriteDiagnostic("Director is null");
		   }
			   
	      Logging.WriteDiagnostic("Should be ready");

            _isDone = true;
        }
    }
}