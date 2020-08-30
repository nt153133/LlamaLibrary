using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Explicit, Size = 0x90)]
    public struct RequestItem
    {
        [FieldOffset(0x0)]
        public readonly uint ItemId;

        [FieldOffset(0x4)]
        public readonly uint Count;
        
        [FieldOffset(0x8)]
        public readonly uint IconID;
        
        [FieldOffset(0x7A)]
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=5)]
        public readonly ushort[] MateriaRow;

        [FieldOffset(0x84)]
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=5)]
        public readonly byte[] MateriaColumn;
        
        [FieldOffset(0x78)]
        public readonly byte _HQ;
        
        [FieldOffset(0x88)]
        public readonly byte unkByte1; 
        
        [FieldOffset(0x8A)]
        public readonly uint unkUnt2;
        
        [FieldOffset(0x8C)]
        public readonly byte unkByte2; 
        
        [FieldOffset(0x8D)]
        public readonly byte Stackable; 
        
        [FieldOffset(0x8E)]
        public readonly byte unkByte4; 
        
        public string Name => DataManager.GetItem(ItemId).CurrentLocaleName;
        
        public bool HQ => (_HQ > 0);
    }
}