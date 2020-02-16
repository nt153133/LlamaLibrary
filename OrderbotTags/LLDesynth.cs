using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLDesynth")]
    public class LLDesynth : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("ItemIds")]
        public int[] ItemIds { get; set; }

        [DefaultValue(500)]
        [XmlAttribute("DesynthDelay")]
        public int DesynthDelay { get; set; }

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
            return new ActionRunCoroutine(r => DesynthItems(ItemIds));
        }

        private async Task DesynthItems(int[] itemId)
        {
            var itemsToDesynth = InventoryManager.FilledSlots.Where(bs => bs.IsDesynthesizable && itemId.Contains((int)bs.RawItemId));

            //Log($"{itemsToDesynth.Count()}");

            foreach (var item in itemsToDesynth)
            {
                Log($"Desynthesize Item - Name: {item.Item.CurrentLocaleName}");

                item.Desynth();

                await Coroutine.Wait(6000, () => SalvageResult.IsOpen);

                await Coroutine.Sleep(500);

                Log($"Result open: {SalvageResult.IsOpen}");

                if (SalvageResult.IsOpen)
                {
                    SalvageResult.Close();
                    await Coroutine.Wait(5000, () => !SalvageResult.IsOpen);
                }

                //await Coroutine.Sleep(DesynthDelay);
            }

            _isDone = true;
        }
    }
}