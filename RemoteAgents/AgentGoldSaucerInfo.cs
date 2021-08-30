using System;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentGoldSaucerInfo: AgentInterface<AgentGoldSaucerInfo>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 03 48 8D 4B ? 48 89 73 ? 48 8D 05 ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        protected AgentGoldSaucerInfo(IntPtr pointer) : base(pointer)
        {

        }
    }
}