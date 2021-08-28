using System;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentContentsInfo: AgentInterface<AgentContentsInfo>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? BF ? ? ? ? 48 89 03 48 8D 73 ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        protected AgentContentsInfo(IntPtr pointer) : base(pointer)
        {
        }
    }
}