using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("BuyScripItem")]
    public class BuyScripItem: ProfileBehavior
    {
        private bool _isDone;
        private bool _isOpening;

        public override bool IsDone => _isDone;
        
        static uint[] npcIds = {1001617, 1003077, 1003633, 1012301, 1013397, 1019458, 1027541, 1031501};

        [XmlAttribute("ItemId")] public int ItemId { get; set; }
        
        [XmlAttribute("SelectString")] public int selectString { get; set; }
        
        [XmlAttribute("Count")] public int count { get; set; }
        
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
            return new ActionRunCoroutine(r => BuyScrip(ItemId, count, selectString));
        }

        private async Task BuyScrip(int itemId,int count,  int selectString)
        {
            await Coroutine.Sleep(500);

            

            var unit = GameObjectManager.GetObjectsByNPCIds<Character>(npcIds).OrderBy(r => r.Distance()).FirstOrDefault();

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

            await Coroutine.Wait(5000, () => SelectIconString.IsOpen);

            if (SelectIconString.IsOpen)
            {
                SelectIconString.ClickSlot((uint) 0);
                
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                
                SelectString.ClickSlot((uint) selectString);
                
                await Coroutine.Wait(5000, () => ShopExchangeCurrency.Open);

                if (ShopExchangeCurrency.Open)
                {
                    ShopExchangeCurrency.Purchase((uint) itemId, (uint) count);
                    
                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);

                    SelectYesno.ClickYes();

                    await Coroutine.Sleep(1000);

                    ShopExchangeCurrency.Close();
                    
                    await Coroutine.Wait(5000, () => SelectString.IsOpen);
                    
                    SelectString.ClickSlot((uint) (SelectString.LineCount -1) );
                    
                    await Coroutine.Wait(5000, () => SelectString.IsOpen);
                    
                }
            }
            
            _isDone = true;
        }
        
    }
}