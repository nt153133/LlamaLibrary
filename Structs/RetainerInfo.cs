using System;
using System.Runtime.InteropServices;
using ff14bot.Enums;
using LlamaLibrary.PersonalTester;

namespace LlamaLibrary.Structs
{
    [StructLayout(LayoutKind.Sequential, Size = 0x48)]
    public struct RetainerInfo
    {
        public int Unique;
        
        public int Other;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string Name;

        public byte enabled;

        public ClassJobType Job;

        public byte Level;

        public byte FilledInventorySlots;

        public int Gil;

        public RetainerCity MarketZone;
        
        public byte NumberOfMbItems;
        
        private byte Unknown1;
        
        private byte Unknown2;
        
        public int MBTimeOutTimestamp;
        
        public int VentureTask;
        
        public int VentureEndTimestamp;
        
        private int Unknown3;
        
        private int Unknown4;

        public bool Active => enabled == 1;

        public override string ToString()
        {
            return $"{Name} ({(enabled == 1 ? "enabled" : "disabled")}) - {Job} ({Level}) Gil: {Gil} Selling: {NumberOfMbItems} Venture: {VentureTask} VentureEnd: {UnixTimeStampToDateTime(VentureEndTimestamp)}";
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