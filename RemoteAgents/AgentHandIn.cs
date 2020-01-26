using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentHandIn: AgentInterface<AgentHandIn>
    {
        protected AgentHandIn(IntPtr pointer) : base(pointer)
        {
            
        }

        public void HandIn(BagSlot slot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
                Core.Memory.CallInjected64<uint>(Offsets.HandInFunc, new object[3]
                {
                    Pointer + 0x20,
                    slot.Slot, 
                    (int)slot.BagId
                });
        }
        
    }
}