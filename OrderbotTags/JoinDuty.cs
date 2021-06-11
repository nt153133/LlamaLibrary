using System;
using System.ComponentModel;
using System.Configuration;
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
		
        [XmlAttribute("Raid")] 
        [DefaultValue(false)]
        public bool Raid { get; set; }		
		
        [XmlAttribute("Undersized")] 
        [DefaultValue(true)]
        public bool Undersized { get; set; }		
        
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
            return new ActionRunCoroutine(r => JoinDutyTask(DutyId, Undersized, Trial, Raid));
        }

        private async Task JoinDutyTask(int DutyId, bool Undersized, bool Trial, bool Raid)
        {
           if (Undersized)
            {
				Logging.WriteDiagnostic("Joining Duty as Undersized party.");
				GameSettingsManager.JoinWithUndersizedParty = true;
            }
            else
            {
				Logging.WriteDiagnostic("Joining Duty as normal group.");
				GameSettingsManager.JoinWithUndersizedParty = false;
            }

           if (!PartyManager.IsInParty || PartyManager.IsInParty && PartyManager.IsPartyLeader)
           {
	           while (DutyManager.QueueState == QueueState.None)
	           {
		           Logging.WriteDiagnostic("Queuing for " + DataManager.InstanceContentResults[(uint) DutyId].CurrentLocaleName);
		           DutyManager.Queue(DataManager.InstanceContentResults[(uint) DutyId]);
		           await Coroutine.Wait(10000, () => (DutyManager.QueueState == QueueState.CommenceAvailable || DutyManager.QueueState == QueueState.JoiningInstance));
		           if (DutyManager.QueueState != QueueState.None)
		           {
			           Logging.WriteDiagnostic("Queued for Dungeon");
		           }

		           if (DutyManager.QueueState == QueueState.None)
		           {
			           Logging.WriteDiagnostic("Something went wrong, queueing again...");
			           Logging.WriteDiagnostic("Queuing for " + DataManager.InstanceContentResults[(uint) DutyId].CurrentLocaleName);
			           DutyManager.Queue(DataManager.InstanceContentResults[(uint) DutyId]);
		           }
	           }
           }
           else
           {
			   Logging.WriteDiagnostic("Waiting for dungeon queue.");
	           await Coroutine.Wait(-1, () => (DutyManager.QueueState == QueueState.CommenceAvailable || DutyManager.QueueState == QueueState.JoiningInstance));
	           Logging.WriteDiagnostic("Queued for Dungeon");
           }


           while (DutyManager.QueueState != QueueState.None || DutyManager.QueueState != QueueState.InDungeon || CommonBehaviors.IsLoading)
           {
	           if (DutyManager.QueueState == QueueState.CommenceAvailable)
	           {
		           Logging.WriteDiagnostic("Waiting for queue pop.");
		           await Coroutine.Wait(-1,
			           () => (DutyManager.QueueState == QueueState.JoiningInstance ||
			                  DutyManager.QueueState == QueueState.None));
	           }

	           if (DutyManager.QueueState == QueueState.JoiningInstance)
	           {
		           Logging.WriteDiagnostic("Dungeon popped, commencing in 3.");
		           await Coroutine.Sleep(3000);
		           DutyManager.Commence();
		           await Coroutine.Wait(-1,
			           () => (DutyManager.QueueState == QueueState.LoadingContent ||
			                  DutyManager.QueueState == QueueState.CommenceAvailable));
	           }

	           if (DutyManager.QueueState == QueueState.LoadingContent)
	           {
		           Logging.WriteDiagnostic("Waiting for everyone to accept");
		           await Coroutine.Wait(-1, () => (CommonBehaviors.IsLoading || DutyManager.QueueState == QueueState.CommenceAvailable));
		           await Coroutine.Sleep(1000);
	           }

	           if (CommonBehaviors.IsLoading)
	           {
		           break;
	           }
	           await Coroutine.Sleep(500);
           }
           
           if (DutyManager.QueueState == QueueState.None) return;
           
           await Coroutine.Sleep(500);
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
						   await Coroutine.Wait(-1, () => director.TimeLeftInDungeon < new TimeSpan(0,59,58));
					   }
				   }
				   if (Raid)
				   {
					   if (director.TimeLeftInDungeon >= new TimeSpan(2,0,0))
					   {
						   Logging.WriteDiagnostic("Barrier up");
						   await Coroutine.Wait(-1, () => director.TimeLeftInDungeon < new TimeSpan(1,59,58));
					   }
				   }				   
				   else
				   {
					   if (director.TimeLeftInDungeon >= new TimeSpan(1,30,0))
					   {
						   Logging.WriteDiagnostic("Barrier up");
						   await Coroutine.Wait(-1, () => director.TimeLeftInDungeon < new TimeSpan(1,29,58));
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