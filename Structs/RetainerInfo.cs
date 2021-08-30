using System;
using System.Runtime.InteropServices;
using Clio.Utilities;
using ff14bot.Enums;
using LlamaLibrary.PersonalTester;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Sequential, Size = 0x48)]
    public struct RetainerInfo
    {
        //0x0
        public ulong Unique;

        //0x8
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] name_bytes;

        //0x28
        public byte enabled;

        //0x29
        public ClassJobType Job;

        //0x2A
        public byte Level;

        //0x2B
        public byte FilledInventorySlots;

        //0x2C
        public int Gil;

        //0x30
        public RetainerCity MarketZone;

        //0x31
        public byte NumberOfMbItems;

        //0x32
        private byte Unknown1;

        //0x33
        private byte Unknown2;

        //0x34
        public int MBTimeOutTimestamp;

        //0x38
        public int VentureTask;

        //0x3C
        public int VentureEndTimestamp;

        //0x40
        private int Unknown3;

        //0x44
        private int Unknown4;

        public bool Active => enabled == 1;

        public string Name => name_bytes.ToUTF8String();

        public string DisplayName => Name;

        public override string ToString()
        {
            return $"{Name} ({(enabled == 1 ? "enabled" : "disabled")}) - {Job} ({Level}) Gil: {Gil} Selling: {NumberOfMbItems} Venture: {VentureTask} VentureEnd: {UnixTimeStampToDateTime(VentureEndTimestamp)} {Unique}";
        }

        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }
    }
}