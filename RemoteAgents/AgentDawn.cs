using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory;

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
            get => Core.Memory.Read<byte>(Pointer + Offsets.DawnTrustId);
            set => Core.Memory.Write(Pointer + Offsets.DawnTrustId, (byte) value);
        }

        public bool IsScenario
        {
            get => Core.Memory.Read<byte>(Pointer + Offsets.DawnIsScenario) == 0;
            set => Core.Memory.Write(Pointer + Offsets.DawnIsScenario, value ? (byte) 0 : (byte) 1);
        }
    }
}