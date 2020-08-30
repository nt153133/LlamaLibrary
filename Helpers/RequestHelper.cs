using System;
using System.Collections.Generic;
using ff14bot;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Structs;

namespace LlamaLibrary.Helpers
{
    public static class RequestHelper
    {
        internal static class Offsets
        {
            [Offset("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? 0F B6 D8 EB ? Add 3 TraceRelative")]
            internal static IntPtr RequestInfo;
            [Offset("Search 44 8B 44 CB ? 48 8B 8B ? ? ? ? E8 ? ? ? ? 48 8B 8B ? ? ? ? 48 8B 93 ? ? ? ? 48 8B 01 48 8B 5C 24 ? 48 83 C4 ? 5F 48 FF A0 ? ? ? ? 48 83 BB ? ? ? ? ? Add 4 Read8")]
            internal static int ItemListStart;
            [Offset("Search 0F B6 89 ? ? ? ? 0F B6 43 ? Add 3 Read32")]
            internal static int ItemCount;
            [Offset("Search 0F B6 43 ? 3A C8 0F 83 ? ? ? ? Add 3 Read8")]
            internal static int ItemCount2;
        }
        
        public static ushort ItemCount => Core.Memory.Read<ushort>(Offsets.RequestInfo + Offsets.ItemCount);
        public static ushort ItemCount2 => Core.Memory.Read<ushort>(Offsets.RequestInfo + Offsets.ItemCount2);
        
        public static IntPtr ItemListStart => new IntPtr((long) (Offsets.RequestInfo + Offsets.ItemListStart));

        public static RequestItem[] GetItems()
        {
            return Core.Memory.ReadArray<RequestItem>(ItemListStart, ItemCount);
        }
    }
}