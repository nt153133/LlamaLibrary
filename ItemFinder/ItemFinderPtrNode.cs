using System;
using System.Runtime.InteropServices;

namespace LlamaLibrary.ItemFinder
{
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public struct ItemFinderPtrNode
    {
        [FieldOffset(0x0)]
        public readonly IntPtr Left;

        [FieldOffset(0x8)]
        public readonly IntPtr Parent;

        [FieldOffset(0x10)]
        public readonly IntPtr Right;

        [FieldOffset(0x19)]
        public readonly byte FilledStatus;

        [FieldOffset(0x20)]
        public readonly ulong RetainerId;

        [FieldOffset(0x28)]
        public readonly IntPtr RetainerInventory;

        public bool Filled => FilledStatus == 0;
    }
}
