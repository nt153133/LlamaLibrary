﻿using System;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerInventory: AgentInterface<AgentRetainerInventory>
    {
        protected AgentRetainerInventory(IntPtr pointer) : base(pointer)
        {
        }

        public IntPtr RetainerShopPointer => Core.Memory.Read<IntPtr>(Pointer + 0x6728);
        
        
    }
}