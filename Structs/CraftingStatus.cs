using System.Runtime.InteropServices;

namespace LlamaLibrary.Structs
{
        [StructLayout(LayoutKind.Explicit, Size = 0x44)]
        public struct CraftingStatus
        {
            [FieldOffset(0x0)]
            public readonly uint Stage;
            
            [FieldOffset(0x10)]
            public readonly uint LastAction;

            [FieldOffset(0x18)]
            public readonly uint Step;

            [FieldOffset(0x1C)]
            public readonly uint Progress;

            [FieldOffset(0x24)]
            public readonly uint Quality;
            
            [FieldOffset(0x2C)]
            public readonly uint HQ;
            
            [FieldOffset(0x30)]
            public readonly uint Durability;
            
            [FieldOffset(0x38)]
            public readonly uint ConditionValue;
            
            [FieldOffset(0x40)]
            public readonly byte Unkown;
                        
            [FieldOffset(0x43)]
            public readonly byte CraftingAction;
            
           // public CraftingCondition Condition => (CraftingCondition) ConditionValue;
        }
}