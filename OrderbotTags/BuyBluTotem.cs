using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteWindows;
using TreeSharp;
using Character = ff14bot.Objects.Character;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary.OrderbotTags
{
[XmlElement("BuyBluTotem")]
    public class BuyBluTotem : ProfileBehavior
    {
        private bool _isDone;
        private bool _isOpening;

        public override bool IsDone => _isDone;

        [XmlAttribute("NpcId")]
        public int NpcId { get; set; }

        [XmlAttribute("ItemId")]
        public int ItemId { get; set; }

        [XmlAttribute("SelectString")]
        public int selectString { get; set; }


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
            return new ActionRunCoroutine(r => BuyItem(ItemId, NpcId,  selectString));
        }

        private async Task BuyItem(int itemId, int npcId, int selectString)
        {
            var unit = GameObjectManager.GetObjectsByNPCId<Character>((uint) npcId).OrderBy(r => r.Distance()).FirstOrDefault();

            if (unit == null)
            {
                _isDone = true;
                return;
            }

            if (!FreeShop.Instance.IsOpen && unit.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(unit.Location);
                await Coroutine.Sleep(500);
            }

            unit.Interact();

            await Coroutine.Wait(5000, () => Conversation.IsOpen);

            if (Conversation.IsOpen)
            {
                Conversation.SelectLine((uint) selectString);
                
                await Coroutine.Wait(5000, () => DialogOpen || FreeShop.Instance.IsOpen);

                if (DialogOpen) Next();
                
                await Coroutine.Wait(5000, () => FreeShop.Instance.IsOpen);
                
                if (FreeShop.Instance.IsOpen)
                {
                    //Log("Opened");
                    await FreeShop.Instance.BuyItem((uint) itemId);
                }

                await Coroutine.Wait(2000, () => SelectYesno.IsOpen);

                if (SelectYesno.IsOpen)
                {
                    SelectYesno.Yes();
                    await Coroutine.Wait(2000, () => !SelectYesno.IsOpen);
                    await Coroutine.Sleep(500);
                }

                
                
                if (FreeShop.Instance.IsOpen)
                {
                    FreeShop.Instance.Close();
                }
            }

            _isDone = true;
        }
    }
}