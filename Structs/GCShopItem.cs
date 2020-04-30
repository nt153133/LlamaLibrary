using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    
    [StructLayout(LayoutKind.Explicit, Size = 0x138)]
    public struct GCShopItem
    {
        [FieldOffset(0)]
        public uint ItemID;

        [FieldOffset(8)]
        public uint Cost;

        [FieldOffset(0x10)]
        public uint Index;
        
        [FieldOffset(0x18)]
        public uint InBag;
        
       // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x44)]
       // [FieldOffset(0x4C)]
       // public byte Name;

       public Item Item => DataManager.GetItem(ItemID);
    }
}