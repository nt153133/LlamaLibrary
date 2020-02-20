using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.Memory;
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
            var agentSalvageInterface = AgentInterface<AgentSalvage>.Instance;
            var agentSalvage = Offsets.SalvageAgent;
            //Log($"{itemsToDesynth.Count()}");

            foreach (var item in itemsToDesynth)
            {
                Log($"Desynthesize Item - Name: {item.Item.CurrentLocaleName}");

                lock (Core.Memory.Executor.AssemblyLock)
                {
                    Core.Memory.CallInjected64<int>(agentSalvage, agentSalvageInterface.Pointer, item.Pointer, 14);
                }

                // await Coroutine.Sleep(500);


                await Coroutine.Wait(5000, () => SalvageDialog.IsOpen);

                if (SalvageDialog.IsOpen)
                {
                    RaptureAtkUnitManager.GetWindowByName("SalvageDialog").SendAction(1, 3, 0);
                    //await Coroutine.Sleep(500);
                    await Coroutine.Wait(10000, () => SalvageResult.IsOpen);

                    if (SalvageResult.IsOpen)
                    {
                        SalvageResult.Close();
                        //await Coroutine.Sleep(500);
                        await Coroutine.Wait(5000, () => !SalvageResult.IsOpen);
                    }
                    else
                    {
                        Log("Result didn't open");
                        break;
                    }
                }
                else
                {
                    Log("SalvageDialog didn't open");
                    break;
                }
            }

            _isDone = true;
        }
    }
}