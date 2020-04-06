using System;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentCharacter: AgentInterface<AgentCharacter>
    {
        protected AgentCharacter(IntPtr pointer) : base(pointer)
        {
        }
    }
}