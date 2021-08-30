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
    [XmlElement("BuyShopExchangeCurrency")]
    public class BuyShopExchangeCurrency : ProfileBehavior
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

        [XmlAttribute("Dialog")]
        [XmlAttribute("dialog")]
        [DefaultValue(false)]
        public bool dialog { get; set; } = false;		

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

            if (!ShopExchangeCurrency.Open && unit.Location.Distance(Core.Me.Location) > 4f)
            {
                await Navigation.OffMeshMove(unit.Location);
                await Coroutine.Sleep(500);
            }

            unit.Interact();

            if (dialog)
            {
                await Coroutine.Wait(5000, () => Talk.DialogOpen);

                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Sleep(1000);
                }
            }			

            await Coroutine.Wait(5000, () => Conversation.IsOpen);

            if (Conversation.IsOpen)
            {
                Conversation.SelectLine((uint) selectString);

                if (dialog)
                {
                    await Coroutine.Wait(5000, () => Talk.DialogOpen);

                    while (Talk.DialogOpen)
                    {
                        Talk.Next();
                        await Coroutine.Sleep(1000);
                    }
                }			

                await Coroutine.Wait(5000, () => ShopExchangeCurrency.Open);


                if (ShopExchangeCurrency.Open)
                {
                    //Log("Opened");
                    ShopExchangeCurrency.Purchase((uint) itemId, (uint) count);
                    await Coroutine.Wait(2000, () => SelectYesno.IsOpen || Request.IsOpen);

                    if (SelectYesno.IsOpen)
                    {
                        SelectYesno.Yes();
                        await Coroutine.Sleep(1000);
                    }
                }

                await Coroutine.Wait(2000, () => ShopExchangeCurrency.Open);
                if (ShopExchangeCurrency.Open)
                {
                    ShopExchangeCurrency.Close();
                }
            }

            _isDone = true;
        }
    }
}