using System;
using ff14bot;
using ff14bot.Managers;
 using LlamaLibrary.Memory.Attributes;

 namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerVenture: AgentInterface<AgentRetainerVenture>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 03 B9 ? ? ? ? 89 53 ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 41 8B 4E ? E8 ? ? ? ? 4C 8B F8 Add 3 Read8")]
            internal static int RetainerTask;
        }
        protected AgentRetainerVenture(IntPtr pointer) : base(pointer)
        {
        }

        public int ExperiencedGain => Core.Memory.Read<int>(Pointer + 0x3c);
        public int RewardItem1 => Core.Memory.Read<int>(Pointer + 0x48);
        public int RewardItem2 => Core.Memory.Read<int>(Pointer + 0x4C);
        public int RewardCount1 => Core.Memory.Read<int>(Pointer + 0x50);
        public int RewardCount2 => Core.Memory.Read<int>(Pointer + 0x54);
        public int RetainerTask => Core.Memory.Read<int>(Pointer + Offsets.RetainerTask);
    }
}