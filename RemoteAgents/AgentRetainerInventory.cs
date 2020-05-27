﻿using System;
using ff14bot;
using ff14bot.Managers;
 using LlamaLibrary.Memory.Attributes;

 namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerInventory: AgentInterface<AgentRetainerInventory>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 6F ? 48 89 07 48 8D 9F ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
        }
        protected AgentRetainerInventory(IntPtr pointer) : base(pointer)
        {
        }

        public IntPtr RetainerShopPointer => Core.Memory.Read<IntPtr>(Pointer + 0x6728);
        
    }
}