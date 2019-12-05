using System;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    /* AgentModule
     *  {
			typeof(AgentDawn),
			Core.Memory.ImageBase + 0x164B7B0
	    }
     */
    public class AgentDawn : AgentInterface<AgentDawn>
    {
        protected AgentDawn(IntPtr pointer) : base(pointer)
        {
        }

        public int TrustId
        {
            get => Core.Memory.Read<byte>(Pointer + 0x28);
            set => Core.Memory.Write(Pointer + 0x28, (byte) value);
        }

        public bool IsScenario
        {
            get => Core.Memory.Read<byte>(Pointer + 0x29) == 0;
            set => Core.Memory.Write(Pointer + 0x29, value ? (byte) 0 : (byte) 1);
        }
    }
}