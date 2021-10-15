using System;
using System.Runtime.InteropServices;
using ff14bot;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    public struct SharedFateProgress
    {
        [FieldOffset(0x0)]
        public IntPtr ZonePtr;
        
        [FieldOffset(0x8)]
        public IntPtr ZoneNamePtr;

        [FieldOffset(0x70)]
        public int Rank;

        [FieldOffset(0x74)]
        public int CurrentFateAmount;
        
        [FieldOffset(0x78)]
        public int FatesNeeded;
        
        [FieldOffset(0x7C)]
        public int DisplayOrder;

        public uint Zone => Core.Memory.Read<uint>(ZonePtr);

        public string ZoneName => Core.Memory.ReadStringUTF8(ZoneNamePtr);

        public override string ToString()
        {
            return $"Rank: {Rank}, CurrentFateAmount: {CurrentFateAmount}, FatesNeeded: {FatesNeeded}, Zone: {Zone}, ZoneName: {ZoneName}";
        }
    }
}