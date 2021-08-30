using System;
using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Sequential, Size = 0x90)]
    public unsafe struct RequestItem
    {
        public readonly uint ItemId;
        public readonly uint Count;
        //[FieldOffset(0x8)]
        public readonly uint IconID;
        //[FieldOffset(0xC)]
        private readonly uint unk1;
        //[FieldOffset(0x10)]
        public IntPtr ptr;
        //[FieldOffset(0x18)]
        private readonly uint unk2;
        private readonly uint unk3;
        private readonly uint unk4;
        private readonly uint unk5;
        private readonly uint unk6;
        private readonly uint unk7;
        private readonly uint unk8;
        private readonly uint unk9;
        private readonly uint unk10;
        private readonly uint unk11;

        [MarshalAs (UnmanagedType.ByValArray, SizeConst=0x38)]
        private readonly char[] _name;
        //[FieldOffset(0x78)]
        public readonly byte _HQ;
        //[FieldOffset(0x7A)]
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=5)]
        public readonly ushort[] MateriaRow;
        //[FieldOffset(0x84)]
        [MarshalAs (UnmanagedType.ByValArray, SizeConst=5)]
        public readonly byte[] MateriaColumn;
        //[FieldOffset(0x89)]
        private readonly byte unkByte1;
        //[FieldOffset(0x8A)]
        private readonly ushort unkUshort;
        //[FieldOffset(0x8C)]
        private readonly byte _collectable;
        //[FieldOffset(0x8D)]
        public readonly byte Stackable;
        //[FieldOffset(0x8E)]
        private readonly byte unkByte4;
        //[FieldOffset(0x8F)]
        private readonly byte unkByte6; 

        public string Name => DataManager.GetItem(ItemId).CurrentLocaleName;

        public bool HQ => (_HQ > 0);
        public bool Collectable => (_HQ > 0);
    }
}