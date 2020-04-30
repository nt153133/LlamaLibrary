using System;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentMasterPieceSupply : AgentInterface<AgentMasterPieceSupply>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;

        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 07 48 8D 05 ? ? ? ? 48 89 47 ? 48 8D 05 ? ? ? ? 48 89 4F ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }

        protected AgentMasterPieceSupply(IntPtr pointer) : base(pointer)
        {
        }
    }
}