using System;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentRecommendEquip: AgentInterface<AgentRecommendEquip>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? C6 43 ? ? 48 89 03 48 8B C3 C7 43 ? ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        protected AgentRecommendEquip(IntPtr pointer) : base(pointer)
        {
        }
    }
}