using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Retainers;
using TreeSharp;
using static LlamaLibrary.Helpers.GeneralFunctions;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLSellItem")]
    public class LLSellItem : ProfileBehavior
    {
        [XmlAttribute("ItemIDs")]
        [XmlAttribute("ItemIds")]
        [XmlAttribute("ItemID")]
        [XmlAttribute("ItemId")]
        [DefaultValue(new int[0])]
        private int[] ItemIds { get; set; }
        
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
            return new ActionRunCoroutine(r => SellItemsToRetainers());
        }

        private async Task SellItemsToRetainers()
        {
            if (_isDone)
            {
                await Coroutine.Yield();
                return;
            }

            await RetainerSellItems(InventoryManager.FilledInventoryAndArmory.Where(x => ItemIds.Contains((int) x.RawItemId)));

            _isDone = true;
        }
    }
}