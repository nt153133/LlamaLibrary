using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("PassOnLoot")]
    public class PassOnLoot : ProfileBehavior
    {
        private bool _isDone;

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
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => PassLoot());
        }

        public async Task PassLoot()
        {
            //if (!NeedGreed.Instance.IsOpen)
            var window = RaptureAtkUnitManager.GetWindowByName("_Notification");

            if (!NeedGreed.Instance.IsOpen && window != null)
            {
                window.SendAction(3, 3, 0, 3, 2, 6, 0x375B30E7);
                await Coroutine.Wait(5000, () => NeedGreed.Instance.IsOpen);
            }

            if (NeedGreed.Instance.IsOpen)
            {
                for (int i = 0; i < NeedGreed.Instance.NumberOfItems; i++)
                {
                    NeedGreed.Instance.PassItem(i);
                    await Coroutine.Sleep(500);
                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                    if (SelectYesno.IsOpen)
                        SelectYesno.Yes();
                }
            }
            
            if (NeedGreed.Instance.IsOpen)
                NeedGreed.Instance.Close();
        }

        public override bool IsDone => _isDone;
    }
}