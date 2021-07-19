using System;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentInventoryBuddy: AgentInterface<AgentInventoryBuddy>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;

        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 8B F1 48 89 01 48 8D 05 ? ? ? ? 48 89 41 ? 48 8B 89 ? ? ? ? 48 85 C9 74 ? E8 ? ? ? ? 48 C7 86 ? ? ? ? ? ? ? ? BF ? ? ? ? 48 8D 9E ? ? ? ? 90 Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        
        protected AgentInventoryBuddy(IntPtr pointer) : base(pointer)
        {
        }
        
    }
}