using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentMeld: AgentInterface<AgentMeld>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 33 FF 48 89 03 48 8D 4B ? Add 3 TraceRelative")]
            //[OffsetCN("Search 48 8D 05 ? ? ? ? 48 8D 4B ? 48 89 03 E8 ? ? ? ? 48 8D 4B ? E8 ? ? ? ? 33 C9 Add 3 TraceRelative")]
            internal static IntPtr VTable;

            [Offset("Search 38 9F ? ? ? ? 48 8D 8D ? ? ? ? Add 2 Read32")]
            internal static int CanMeld;
        }
        protected AgentMeld(IntPtr pointer) : base(pointer)
        {
        }

        public bool CanMeld => Core.Memory.NoCacheRead<byte>(Pointer + Offsets.CanMeld) == 1;

        public bool Ready => Core.Memory.NoCacheRead<byte>(LlamaLibrary.Memory.Offsets.Conditions + 7) == 1;
    }
}