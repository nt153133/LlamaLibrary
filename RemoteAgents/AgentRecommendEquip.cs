using System;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentRecommendEquip: AgentInterface<AgentRecommendEquip>
    {
        protected AgentRecommendEquip(IntPtr pointer) : base(pointer)
        {
        }
    }
}