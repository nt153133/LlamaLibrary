using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentHWDScore: AgentInterface<AgentHWDScore>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;

        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? C7 47 ? ? ? ? ? 48 8D 4F ? 48 89 07 C7 47 ? ? ? ? ? 44 8D 42 ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }

        protected AgentHWDScore(IntPtr pointer) : base(pointer)
        {
        }

        public int[] ReadTotalScores()
        {
            return Core.Memory.ReadArray<int>(Pointer + 0x90, 11);
        }
    }
}