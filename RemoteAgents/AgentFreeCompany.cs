using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteAgents
{

    public class AgentFreeCompany: AgentInterface<AgentFreeCompany>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 33 F6 48 8B D9 Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 8B 93 ? ? ? ? 39 93 ? ? ? ? Add 2 Read32")]
            internal static int HistoryCount;
        }

        protected AgentFreeCompany(IntPtr pointer) : base(pointer)
        {
        }

        public IntPtr GetRosterPtr()
        {
            var ptr1 = Core.Memory.Read<IntPtr>(Pointer + 0x48);
            //  Log(ptr1);
            var ptr2 = Core.Memory.Read<IntPtr>(ptr1 + 0x98);
            // Log(ptr2);
            return ptr2;
        }

        
        public List<(string, bool)> GetMembers()
        {
            int i = 0;
            List<(string, bool)> result = new List<(string, bool)>();
            IntPtr start = GetRosterPtr();
            byte testByte = 0;
            do
            {
                var addr = (start + (i * 0x60));
                testByte = Core.Memory.Read<byte>(addr);
                if (testByte != 0)
                    result.Add((Core.Memory.ReadStringUTF8(addr + 0x22), Core.Memory.Read<byte>(addr + 0xD) != 0));
                i++;
            }
            while (testByte != 0);

            return result;
        }

        public byte HistoryLineCount => Core.Memory.Read<byte>(Pointer + Offsets.HistoryCount);
    }

}