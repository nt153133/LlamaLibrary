using System;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentOutOnLimb: AgentInterface<AgentOutOnLimb>
    {
        private IntPtr addressLocation = IntPtr.Zero;
        Random rnd = new Random();
        
        protected AgentOutOnLimb(IntPtr pointer) : base(pointer)
        {
        }
        
        public bool CursorLocked
        {
            get => Core.Memory.Read<byte>(Pointer + 0xE1) != 1;
            set => Core.Memory.Write(Pointer + 0xE1, (byte) (value ? 0:1));
        }
        
        public int CursorLocation
        {
            get => Core.Memory.Read<ushort>(addressLocation);
            set => Core.Memory.Write(addressLocation, locationValue(value));
        }
        
        public bool IsReadyBotanist => Core.Memory.Read<byte>(Pointer + 0xE0) == 3;
        
        public bool IsReadyAimg => Core.Memory.Read<byte>(Pointer + 0xE0) == 2;

        public void Refresh()
        {
            IntPtr intptr_0 = Core.Memory.Read<IntPtr>(Offsets.SearchResultPtr);
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