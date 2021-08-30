using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentAetherWheel: AgentInterface<AgentAetherWheel>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 03 48 8D 73 ? 48 8D 05 ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 49 8D 74 24 ? F3 0F 10 3D ? ? ? ? Add 4 Read8")]
            internal static int ArrayOffset;
        }

        protected AgentAetherWheel(IntPtr pointer) : base(pointer)
        {
        }

        public AetherWheelSlot[] GetWheelSlots()
        {
            int count = 6;

            if (AetherialWheel.Instance.IsOpen) count = AetherialWheel.Instance.MaxSlots;

            using (Core.Memory.TemporaryCacheState(enabledTemporarily: false))
            {
                return Core.Memory.ReadArray<AetherWheelSlot>(Pointer + Offsets.ArrayOffset, count); 
            }
        }

    }
}