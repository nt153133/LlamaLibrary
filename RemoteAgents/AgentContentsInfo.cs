using System;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentContentsInfo: AgentInterface<AgentContentsInfo>
    {
        protected AgentContentsInfo(IntPtr pointer) : base(pointer)
        {
        }
    }
}