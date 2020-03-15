using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;

namespace LlamaLibrary.RemoteWindows
{
    public class HWDLottery: RemoteWindow<HWDLottery>
    {
        private const string WindowName = "HWDLottery";

        public HWDLottery() : base(WindowName)
        {
            _name = WindowName;
        }

        public async Task ClickSpot(int slot)
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            IntPtr KupoClick = patternFinder.Find("E8 ? ? ? ? 32 C0 48 8B 5C 24 ? 48 8B 74 24 ? 48 83 C4 ? 5F C3 48 8B 03 48 8B CB FF 50 ? TraceCall");
            
            if (IsOpen)
            {
                var agent = WindowByName.TryFindAgentInterface();

                if (agent != null)
                {
                    Core.Memory.CallInjected64<uint>(KupoClick, new object[2]
                    {
                        agent.Pointer,
                        (uint) 1
                    });

                    await Coroutine.Sleep(2000);
                }
            }
        }
    }
}