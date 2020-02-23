using System;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentHousingSelectBlock: AgentInterface<AgentHousingSelectBlock>
    {
        protected AgentHousingSelectBlock(IntPtr pointer) : base(pointer)
        {
        }
        
        public int WardNumber
        {
            get => Core.Memory.Read<int>(Pointer + 0x34);
            set => Core.Memory.Write(Pointer + 0x34, value);
        }
        
        public byte[] ReadPlots(int count)
        {
            return Core.Memory.ReadArray<byte>(Pointer+ 0x3C, count);
        }
    }
}