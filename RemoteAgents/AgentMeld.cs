using System;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentMeld: AgentInterface<AgentMeld>
    {
        protected AgentMeld(IntPtr pointer) : base(pointer)
        {
        }
    }
}