using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentHousingSelectBlock: AgentInterface<AgentHousingSelectBlock>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 4C 8D 2D ? ? ? ? 48 89 74 24 ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 0F B6 56 ? 0F B7 4E ? Add 3 Read8")]
            internal static int WardNumber;
            [Offset("Search 83 FB ? 0F 8D ? ? ? ? 80 7C 33 ? ? Add 2 Read8")]
            internal static int PlotOffset;
        }
        protected AgentHousingSelectBlock(IntPtr pointer) : base(pointer)
        {
        }
        
        public int WardNumber
        {
            get => Core.Memory.Read<int>(Pointer + Offsets.WardNumber);
            set => Core.Memory.Write(Pointer + Offsets.WardNumber, value);
        }
        
        public byte[] ReadPlots(int count)
        {
            return Core.Memory.ReadArray<byte>(Pointer+ Offsets.PlotOffset, count);
        }
    }
}