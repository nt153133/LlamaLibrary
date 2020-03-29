using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerList: AgentInterface<AgentRetainerList>
    {
        protected AgentRetainerList(IntPtr pointer) : base(pointer)
        {
        }

        public IntPtr[] RetainerList => Core.Memory.ReadArray<IntPtr>(Pointer + Offsets.AgentRetainerOffset, Offsets.MaxRetainers);
        
        public RetainerInfo[] OrderedRetainerList (RetainerInfo[] retainers)
        {
            int count = RetainerList.Count(i => i != IntPtr.Zero);

            if (count == 0)
                return retainers;
            
            var result = new RetainerInfo[count]; // new List<KeyValuePair<int, RetainerInfo>>();

            //IntPtr[] RetainerList = Core.Memory.ReadArray<IntPtr>(new IntPtr(0x18FD0C64510) + 0x4a8, 0xA);
            int index = 0;
            foreach (var ptr in RetainerList.Where(i => i != IntPtr.Zero))
            {
                var next = Core.Memory.Read<IntPtr>(ptr);

                result[index] = retainers.First(j => j.Name.Equals(Core.Memory.ReadStringUTF8(next)));
                index++;
            }
            
            return result;
        }

    }
}