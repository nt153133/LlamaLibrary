using Buddy.Coroutines;
using Clio.Utilities;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.Managers;
using LlamaLibrary.Helpers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace ff14bot.NeoProfiles.Tags
{
	[XmlElement("LLTransport")]	
    [XmlElement("SoTransport")]
    public class SoTransport : ProfileBehavior
    {
        public override bool HighPriority
        {
            get
            {
                return true;
            }
        }

        private bool _done;

        public override bool IsDone
        {
            get
            {
                return _done;
            }
        }

        public override string StatusText
        {
            get
            {
                return $"Transporting for {QuestName}.";
            }
        }

        [XmlAttribute("NpcId")]
        public int NpcId { get; set; }

        [XmlAttribute("InteractDistance")]
        [DefaultValue(5f)]
        public float InteractDistance { get; set; }

        [XmlAttribute("XYZ")]
        public Vector3 XYZ { get; set; }

        [XmlAttribute("BlacklistAfter")]
        [DefaultValue(false)]
        public bool BlacklistAfter { get; set; }
        
        [DefaultValue(-1)]
        [XmlAttribute("DialogOption")]
        public int DialogOption { get; set; }

        public GameObject NPC
        {
            get
            {
				if (_done) return null;
                var npc = GameObjectManager.GetObjectsByNPCId((uint)NpcId).FirstOrDefault(r => r.IsVisible && r.IsTargetable);
                return npc;
            }
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => LLTransportTask());
        }

        private  async Task LLTransportTask()
        {
            var gameobj = GameObjectManager.GetObjectByNPCId((uint) NpcId);

            if (gameobj == default(GameObject)) {_done = true; return;}
            
            await LlamaLibrary.Helpers.Navigation.OffMeshMoveInteract(gameobj);

            if (gameobj.IsWithinInteractRange)
            {
                gameobj.Interact();

                await Coroutine.Wait(20000, () => !gameobj.IsVisible || Conversation.IsOpen);
                if (Conversation.IsOpen)
                {
                    if (DialogOption != -1)
                    {
                        var option = ((uint)DialogOption);
                        Conversation.SelectLine(option);
                    }
                    else
                    {
                        Conversation.SelectLine(0);
                    }
                    
                    await Coroutine.Wait(20000, () => !Conversation.IsOpen || CommonBehaviors.IsLoading);
                    
                }
            }

            _done = true;
        }
        
        protected override void OnStart()
        {

        }

        protected override void OnDone()
        {
            
        }

        protected override void OnResetCachedDone()
        {
            
        }
    }
}