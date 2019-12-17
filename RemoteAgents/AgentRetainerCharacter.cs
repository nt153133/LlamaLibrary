﻿using System;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerCharacter: AgentInterface<AgentRetainerCharacter>
    {
        protected AgentRetainerCharacter(IntPtr pointer) : base(pointer)
        {
        }
        
        public int iLvl => Core.Memory.Read<byte>(Pointer + 0xa78);
    }
}