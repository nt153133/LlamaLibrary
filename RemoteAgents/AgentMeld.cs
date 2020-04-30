using System;
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
            [OffsetCN("Search 48 8D 05 ? ? ? ? 48 8D 4B ? 48 89 03 E8 ? ? ? ? 48 8D 4B ? E8 ? ? ? ? 33 C9 Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        protected AgentMeld(IntPtr pointer) : base(pointer)
        {
        }
    }
}