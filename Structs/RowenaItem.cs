using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ff14bot.Enums;
using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public struct RowenaItem
    {
        [FieldOffset(0x0)]
        public readonly uint ItemId;

        [FieldOffset(0x4)]
        public readonly sbyte byte1;

        [FieldOffset(0x5)]
        public readonly sbyte _Starred;

        [FieldOffset(0x6)]
        public readonly sbyte N00000077;

        //[FieldOffset(0x7)]
        //public readonly sbyte N0000007E;

        [FieldOffset(0x8)]
        public readonly int XP;

        [FieldOffset(0xC)]
        public readonly ScripType ScripType;

        [FieldOffset(0x10)]
        public readonly int ScripReward;

        [FieldOffset(0x14)]
        private readonly short short1;
        
        [FieldOffset(0x16)]
        private readonly short short2;

        [FieldOffset(0x18)]
        public readonly short Collectability;

        [FieldOffset(0x1A)]
        public readonly byte Qty;

        [FieldOffset(0x1C)]
        public readonly ClassJobType ClassJob;

        [FieldOffset(0x20)]
        public readonly int MinLevel;

        [FieldOffset(0x24)]
        public readonly byte ItemLevel;

        [FieldOffset(0x28)]
        public readonly byte StarLevel;
        
        [FieldOffset(0x29)]
        public readonly byte byte2;
        
        [FieldOffset(0x2A)]
        public readonly byte byte3;

        [FieldOffset(0x2B)]
        public readonly byte byte4;

        [FieldOffset(0x30)]
        private readonly IntPtr DumbPtr;

        [FieldOffset(0x38)]
        public readonly int N00000065;

        [FieldOffset(0x3C)]
        public readonly int N000000A5;

        [FieldOffset(0x40)]
        public readonly int N00000066;

        [FieldOffset(0x44)]
        public readonly int N000000A8;

        [FieldOffset(0x48)]
        public readonly int N00000067;

        [FieldOffset(0x4C)]
        public readonly int N000000AB;

        [FieldOffset(0x60)]
        [MarshalAs(UnmanagedType.LPUTF8Str, SizeConst = 0x30)]
        public string _name;

        [FieldOffset(0x98)]
        public readonly int int1;
        
        [FieldOffset(0x9C)]
        public readonly byte N000000BF;

        [FieldOffset(0x9D)]
        public readonly byte N000000C4;

        [FieldOffset(0x9E)]
        public readonly byte N000000C8; 
        
        [FieldOffset(0x9F)]
        public readonly byte N000000BB;

        [FieldOffset(0xA0)]
        public readonly int int2;

        [FieldOffset(0xA4)]
        public readonly byte byte5;

        [FieldOffset(0xa6)]
        public readonly short short3;
        

        public bool Starred => (_Starred > 0);

        public bool Enabled => (Qty > 0);
        public string Name => DataManager.GetItem(ItemId).CurrentLocaleName;
        //''
        public string ToString1()
        {
            return ff14bot.Helpers.Utils.DynamicString(this);
        }
        
        public override string ToString()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendFormat("{0}: ", GetType().Name);
            FieldInfo[] array = fields;
            foreach (FieldInfo fieldInfo in array)
            {
                object value = fieldInfo.GetValue(this);
                stringBuilder.AppendFormat(" {0}, ", value ?? "null");
            }
            stringBuilder.AppendFormat(" {0}, ", Name ?? "null");
            stringBuilder.AppendFormat(" {0}, ", Starred);
            stringBuilder.AppendFormat(" {0}, ", Enabled);
            return stringBuilder.ToString();
        }
        
        
    }

    public enum ScripType : uint
    {
        Yellow_Gather = 17834,
        White_Gather = 25200,
        Yellow_Crafting = 17833,
        White_Crafting = 25199
    }
}