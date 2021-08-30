using System;
using ff14bot;
using ff14bot.Managers;
 using LlamaLibrary.Memory.Attributes;

 namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerCharacter: AgentInterface<AgentRetainerCharacter>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 06 48 8D 4E ? 48 8D 05 ? ? ? ? 48 89 46 ? E8 ? ? ? ? 33 ED Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        protected AgentRetainerCharacter(IntPtr pointer) : base(pointer)
        {
        }

        public int iLvl => Core.Memory.Read<byte>(Pointer + 0xa78);
    }
}