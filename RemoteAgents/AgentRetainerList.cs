using System;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentRetainerList: AgentInterface<AgentRetainerList>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 89 06 48 8D 5E ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 48 8D 8E ? ? ? ? 33 D2 41 B8 ? ? ? ? E8 ? ? ? ? 48 8B 5C 24 ? 48 8B C6 48 8B 74 24 ? 48 83 C4 ? 5F C3 ? ? ? ? ? ? 48 89 5C 24 ? Add 3 Read32")]
            internal static int AgentRetainerOffset;
            [Offset("Search 83 FB ? 72 ? 33 D2 48 8D 4C 24 ? E8 ? ? ? ? 48 8D 15 ? ? ? ? Add 2 Read8")]
            internal static int MaxRetainers;
        }
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