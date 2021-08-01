using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Explicit, Size = 0xC0)]
    public struct AetherWheelSlot
    {
        [FieldOffset(0x0)]
        public ushort Grade;
        
        [FieldOffset(0x2)]
        public bool InUse;
        
        [FieldOffset(0x6)]
        public uint MinutesLeft;
        
        [FieldOffset(0x8E)]
        public uint ItemId;
        
        [FieldOffset(0x8E)]
        public uint StartingItemId;
        
        [FieldOffset(0xAA)]
        public uint TotalMinutes;
        
        [FieldOffset(0xAE)]
        public uint ResultingItemId;

        [FieldOffset(0xB2)]
        public bool Primed;

        [FieldOffset(0xB6)]
        public byte SlotIndex;
        
        public Item Item => DataManager.GetItem(ItemId);

        public override string ToString()
        {
            return $"{nameof(Grade)}: {Grade}, {nameof(InUse)}: {InUse}, {nameof(MinutesLeft)}: {MinutesLeft}, {nameof(ItemId)}: {ItemId}, {nameof(StartingItemId)}: {StartingItemId}, {nameof(TotalMinutes)}: {TotalMinutes}, {nameof(ResultingItemId)}: {ResultingItemId}, {nameof(Primed)}: {Primed}, {nameof(SlotIndex)}: {SlotIndex}, {nameof(Item)}: {Item}";
        }
    }
}