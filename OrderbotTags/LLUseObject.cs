using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLUseObject")]
    public class LLUseObject : ProfileBehavior
    {
        private bool _isDone;

        public override bool HighPriority => true;

        public override bool IsDone => _isDone;
        
        [XmlAttribute("NpcId")] public int NpcId { get; set; }

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
            return new ActionRunCoroutine(r => LLUseObjectTask((uint) NpcId));
        }

        private async Task LLUseObjectTask(uint NpcId)
        {
            var gameobj = GameObjectManager.GetObjectByNPCId(NpcId);

            if (gameobj == default(GameObject)) {_isDone = true; return;}
            
            await Navigation.FlightorMove(gameobj.Location);

            if (gameobj.IsWithinInteractRange)
            {
                gameobj.Interact();

                await Coroutine.Wait(20000, () => !gameobj.IsVisible || SelectYesno.IsOpen);
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.Yes();
                    await Coroutine.Wait(20000, () => !SelectYesno.IsOpen);
                }
            }
            _isDone = true;
        }
    }
}