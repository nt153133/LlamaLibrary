using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentDawn : AgentInterface<AgentDawn>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.DawnVtable;
        private static class Offsets
        {
            [Offset("Search 41 88 46 ? 0F B6 42 ? Add 3 Read8")]
            internal static int DawnTrustId;
            [Offset("Search 48 8D 05 ? ? ? ? 48 C7 43 ? ? ? ? ? 48 8D 4B ? 48 89 03 66 C7 43 ? ? ? Add 3 TraceRelative")]
            internal static IntPtr DawnVtable;
            [Offset("Search 41 88 46 ? E8 ? ? ? ? C6 43 ? ? Add 3 Read8")]
            internal static int DawnIsScenario;
        }
        protected AgentDawn(IntPtr pointer) : base(pointer)
        {
        }

        public int TrustId
        {
            get => Core.Memory.Read<byte>(Pointer + Offsets.DawnTrustId);
            set => Core.Memory.Write(Pointer + Offsets.DawnTrustId, (byte)value);
        }

        public bool IsScenario
        {
            get => Core.Memory.Read<byte>(Pointer + Offsets.DawnIsScenario) == 0;
            set => Core.Memory.Write(Pointer + Offsets.DawnIsScenario, value ? (byte)0 : (byte)1);
        }
    }
}