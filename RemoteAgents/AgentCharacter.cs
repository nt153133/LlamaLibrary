using System;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentCharacter: AgentInterface<AgentCharacter>, IAgent
    {
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 89 77 ? Add 3 TraceRelative")]
            internal static IntPtr vtable;
        }

        protected AgentCharacter(IntPtr pointer) : base(pointer)
        {
        }

        public IntPtr RegisteredVtable => Offsets.vtable;
    }
}