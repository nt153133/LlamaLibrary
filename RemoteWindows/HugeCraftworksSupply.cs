using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteAgents;

namespace LlamaLibrary.RemoteWindows
{
    public class HugeCraftworksSupply: RemoteWindow<HugeCraftworksSupply>
    {
        private const string WindowName = "HugeCraftworksSupply";

        public HugeCraftworksSupply() : base(WindowName)
        {
            _name = WindowName;
        }

        public int TurnInItemId =>  ___Elements()[9].TrimmedData;

        public void Deliver()
        {
            SendAction(1,3,0);
        }

        public async Task HandOverItems()
        {
            var slots = InventoryManager.FilledSlots.Where(i => i.RawItemId == TurnInItemId).OrderByDescending(i => i.HqFlag);

            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    AgentHandIn.Instance.HandIn(slot);
                    //await Coroutine.Sleep(500);
                    await Coroutine.Wait(5000, () => InputNumeric.IsOpen);
                    if (InputNumeric.IsOpen)
                    {
                        InputNumeric.Ok((uint) InputNumeric.Field.CurrentValue);
                    }
                    await Coroutine.Sleep(700);
                }
            }

            await Coroutine.Sleep(500);

            Deliver();

            await Coroutine.Wait(5000, () => Talk.DialogOpen);

            while (Talk.DialogOpen)
            {
                Talk.Next();
                await Coroutine.Sleep(1000);
            }
        }
    }
}