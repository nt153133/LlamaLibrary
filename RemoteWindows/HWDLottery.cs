using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteWindows
{
    public class HWDLottery: RemoteWindow<HWDLottery>
    {
        private const string WindowName = "HWDLottery";

        internal static class Offsets
        {
            [Offset("Search 48 89 74 24 ? 57 48 83 EC ? 48 83 79 ? ? 8B F2 48 8B F9 0F 84 ? ? ? ? 48 89 5C 24 ? 4C 8D 44 24 ? BB ? ? ? ? 48 89 6C 24 ? 8B C3 33 ED 41 89 28 4D 8D 40 ? 48 83 E8 ? 75 ? 8D 50 ? 48 8D 4C 24 ? E8 ? ? ? ? BA ? ? ? ? C7 44 24 ? ? ? ? ? 48 8D 4C 24 ? E8 ? ? ? ? 48 8B 4F ? 4C 8D 44 24 ? 89 74 24 ? 48 8D 54 24 ? 44 8B CB 48 89 6C 24 ? 48 8B 01 FF 10 48 8D 4C 24 ? E8 ? ? ? ? 48 8B 6C 24 ? 48 8D 7C 24 ?")]
            internal static IntPtr KupoFunction;
        }

        public override void Close()
        {
            SendAction(1,3,2);
        }

        public HWDLottery() : base(WindowName)
        {
            _name = WindowName;
        }

        public async Task ClickSpot(int slot)
        {
            //var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            //IntPtr KupoClick = patternFinder.Find("E8 ? ? ? ? 32 C0 48 8B 5C 24 ? 48 8B 74 24 ? 48 83 C4 ? 5F C3 48 8B 03 48 8B CB FF 50 ? TraceCall");

            if (IsOpen)
            {
                var agent = WindowByName.TryFindAgentInterface();

                if (agent != null)
                {
                    Core.Memory.CallInjected64<uint>(Offsets.KupoFunction, new object[2]
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