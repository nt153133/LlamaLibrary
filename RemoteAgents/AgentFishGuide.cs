using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentFishGuide: AgentInterface<AgentFishGuide>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.Vtable;

        public const int TabCount = 37;
        private static class Offsets
        {
            [Offset("Search 8D 4A ? 66 89 93 ? ? ? ? 48 89 93 ? ? ? ? Add 2 Read8")]
            internal static int TabSlotCount;
            [Offset("Search 48 8D 05 ? ? ? ? BA ? ? ? ? 48 89 03 48 8D 05 ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr Vtable;
            [Offset("Search 48 8D 43 ? 88 93 ? ? ? ? Add 3 Read8")]
            internal static int TabStart;
        }
        protected AgentFishGuide(IntPtr pointer) : base(pointer)
        {
            
        }
        public FishGuideItem[] GetTabList()
        {
            using (Core.Memory.TemporaryCacheState(enabledTemporarily: false))
            {
                //Logging.Write($"{Pointer + Offsets.TabStart - 0x6} {Offsets.TabSlotCount}");
                return Core.Memory.ReadArray<FishGuideItem>(Pointer + Offsets.TabStart - 0x6, Offsets.TabSlotCount); //.Select(x => x.FishItem) as List<uint>;
            }
        }

        
    }
    
    [StructLayout(LayoutKind.Sequential, Size = 0x8)]
    public struct FishGuideItem
    {
        public uint FishItem;
        public uint Unknown;
    }
}