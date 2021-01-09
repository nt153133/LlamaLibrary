using System;
using System.Runtime.InteropServices;
using System.Text;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentMinionNoteBook: AgentInterface<AgentMinionNoteBook>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;

        public IntPtr MinionListAddress => this.Pointer + Offsets.AgentOffset;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 03 48 8B C3 48 83 C4 ? 5B C3 ? ? ? ? ? ? ? ? ? 40 53 48 83 EC ? 48 8D 05 ? ? ? ? 48 8B D9 48 89 01 48 81 C1 ? ? ? ? E8 ? ? ? ? 48 8B 4B ? 48 85 C9 74 ? 48 8B 53 ? 41 B8 ? ? ? ? 48 2B D1 48 83 E2 ? E8 ? ? ? ? 33 C0 48 89 43 ? 48 89 43 ? 48 89 43 ? 48 8B CB 48 83 C4 ? 5B E9 ? ? ? ? ? ? ? ? ? ? ? 48 83 EC ? BA ? ? ? ? E8 ? ? ? ? 48 85 C0 74 ? 48 8B 80 ? ? ? ? 8B 40 ? C1 E8 ? F6 D0 Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 48 8B 43 ? 44 0F B7 0C 90 41 3B F1 Add 3 Read8")]
            internal static int AgentOffset;
            [Offset("Search 89 15 ? ? ? ? 41 8D 49 ? ? ? ? 48 C1 E9 ? 46 84 84 29 ? ? ? ? 74 ? FF C2 89 15 ? ? ? ? 41 8B C1 Add 2 TraceRelative")]
            internal static IntPtr MinionCount;
            [Offset("Search E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 0F B7 58 ? 48 8D 4C 24 ? TraceCall")]
            internal static IntPtr GetCompanion;
        }

        protected AgentMinionNoteBook(IntPtr pointer) : base(pointer)
        {
        }

        public MinionStruct[] GetCurrentMinions()
        {
            var address = this.Pointer + Offsets.AgentOffset;
            var address1 = Core.Memory.Read<IntPtr>(address);
            var count = Core.Memory.Read<uint>(Offsets.MinionCount);
            return Core.Memory.ReadArray<MinionStruct>(address1, (int) count);
        }
        
        public static string GetMinionName(int index)
        {
            var result = Core.Memory.CallInjected64<IntPtr>(Offsets.GetCompanion, index);
            return result != IntPtr.Zero ? Core.Memory.ReadString(result + 0x30, Encoding.UTF8) : "";
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 0x4)]
    public struct MinionStruct
    {
        [FieldOffset(0)]
        public ushort MinionId;

        [FieldOffset(2)]
        public ushort unknown;
    }
    
}