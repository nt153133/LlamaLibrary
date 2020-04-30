using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentOutOnLimb: AgentInterface<AgentOutOnLimb>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 8D 4F ? 48 89 07 E8 ? ? ? ? 33 C9 Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 41 80 BE ? ? ? ? ? 0F 84 ? ? ? ? BA ? ? ? ? Add 3 Read32")]
            internal static int IsReady;
            [Offset("Search 41 C6 86 ? ? ? ? ? EB ? 41 C6 86 ? ? ? ? ? Add 3 Read32")]
            internal static int CursorLocked;
        }
        private IntPtr addressLocation = IntPtr.Zero;
        Random rnd = new Random();
        
        protected AgentOutOnLimb(IntPtr pointer) : base(pointer)
        {
        }
        
        public bool CursorLocked
        {
            get => Core.Memory.Read<byte>(Pointer + Offsets.CursorLocked) != 1;
            set => Core.Memory.Write(Pointer + Offsets.CursorLocked, (byte) (value ? 0:1));
        }
        
        public int CursorLocation
        {
            get => Core.Memory.Read<ushort>(addressLocation);
            set => Core.Memory.Write(addressLocation, locationValue(value));
        }
        
        public bool IsReadyBotanist => Core.Memory.Read<byte>(Pointer + Offsets.IsReady) == 3;
        
        public bool IsReadyAimg => Core.Memory.Read<byte>(Pointer + + Offsets.IsReady) == 2;

        public void Refresh()
        {
            IntPtr intptr_0 = Core.Memory.Read<IntPtr>(Memory.Offsets.SearchResultPtr);
            IntPtr intptr_1 = Core.Memory.Read<IntPtr>(intptr_0 + 0x38);
            IntPtr intptr_2 = Core.Memory.Read<IntPtr>(intptr_1 + 0x18);
            IntPtr intptr_3 = Core.Memory.Read<IntPtr>(intptr_2 + 0x310);
            addressLocation = Core.Memory.Read<IntPtr>(intptr_3 + 0x20);
        }

        private ushort locationValue(int percent)
        {
            ushort location = (ushort) ((percent * 100) + rnd.Next(0, 99));
            //Logger.Info($"Setting Location {location}");
           return location;
        }
    }
}