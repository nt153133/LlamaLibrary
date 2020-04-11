using System.ComponentModel;
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

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("BuyShopExchangeItem")]
    public class BuyShopExchangeItem : ProfileBehavior
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

        [XmlAttribute("Count")]
        [DefaultValue(1)]
        public int count { get; set; }

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
            return new ActionRunCoroutine(r => BuyItem(ItemId, NpcId, count, selectString));
        }

        private async Task BuyItem(int itemId, int npcId, int count, int selectString)
        {
            var unit = GameObjectManager.GetObjectsByNPCId<Character>((uint) npcId).OrderBy(r => r.Distance()).FirstOrDefault();

            if (unit == null)
            {
                _isDone = true;
                return;
            }

            if (!ShopExchangeItem.Instance.IsOpen && unit.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(unit.Location);
                await Coroutine.Sleep(500);
            }

            unit.Interact();

            await Coroutine.Wait(5000, () => SelectIconString.IsOpen);

            if (SelectIconString.IsOpen)
            {
                SelectIconString.ClickSlot((uint) selectString);

                await Coroutine.Wait(5000, () => ShopExchangeItem.Instance.IsOpen);

                
                if (ShopExchangeItem.Instance.IsOpen)
                {
                    //Log("Opened");
                    await ShopExchangeItem.Instance.Purchase((uint) itemId, (uint) count);
                }

                await Coroutine.Wait(2000, () => ShopExchangeItem.Instance.IsOpen);
                if (ShopExchangeItem.Instance.IsOpen)
                {
                    ShopExchangeItem.Instance.Close();
                }
            }

            _isDone = true;
        }
    }
}