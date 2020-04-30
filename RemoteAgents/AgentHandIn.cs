using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentHandIn: AgentInterface<AgentHandIn>, IAgent
    {
         public IntPtr RegisteredVtable => Offsets.VTable;
         private static class Offsets
         {
             [Offset("Search 48 8D 05 ? ? ? ? 48 89 01 48 8D 05 ? ? ? ? 48 89 41 ? 48 8B D9 Add 3 TraceRelative")]
             internal static IntPtr VTable;
             [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 48 8B 41 ? 48 8B E9 48 83 C1 ?")]
             internal static IntPtr HandInFunc;
             [Offset("Search 48 89 41 ? 48 8B D9 48 85 FF Add 3 Read8")]
             internal static int HandinParmOffset;
         }
        protected AgentHandIn(IntPtr pointer) : base(pointer)
        {
            
        }

        public void HandIn(BagSlot slot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
                Core.Memory.CallInjected64<uint>(Offsets.HandInFunc, new object[3]
                {
                    Pointer + Offsets.HandinParmOffset,
                    slot.Slot, 
                    (int)slot.BagId
                });
        }
        
    }
}