using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.RemoteAgents
{

    public class AgentFreeCompany: AgentInterface<AgentFreeCompany>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? 48 8B F9 48 89 01 48 8D 05 ? ? ? ? 48 89 41 ? 48 8D 05 ? ? ? ? 48 89 41 ? 48 81 C1 ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 8B 93 ? ? ? ? 39 93 ? ? ? ? Add 2 Read32")]
            internal static int HistoryCount;
            [Offset("Search 48 8B 05 ? ? ? ? C3 ? ? ? ? ? ? ? ? 48 8B 05 ? ? ? ? C3 ? ? ? ? ? ? ? ? 0F B6 81 ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr AtkStage;
            [Offset("Search 48 8B 41 ? 48 8B 40 ? C3 ? ? ? ? ? ? ? 48 8B 41 ? 48 8B 40 ? C3 ? ? ? ? ? ? ? 48 8B 41 ? 48 8B 40 ? C3 ? ? ? ? ? ? ? 48 89 5C 24 ? Add 3 Read8")]
            internal static int off1;
            [Offset("Search 48 8B 40 ? C3 ? ? ? ? ? ? ? 48 8B 41 ? 48 8B 40 ? C3 ? ? ? ? ? ? ? 48 8B 41 ? 48 8B 40 ? C3 ? ? ? ? ? ? ? 48 89 5C 24 ? Add 3 Read8")]
            internal static int off2;
            [Offset("Search 4C 8B 80 ? ? ? ? 4D 85 C0 0F 84 ? ? ? ? 83 EB ? Add 3 Read32")]
            internal static int off3;
            [Offset("Search 49 8B 40 ? 48 63 D1 0F B7 1C 90 Add 3 Read8")]
            internal static int off4;
            [Offset("Search 8B 70 ? 85 F6 75 ? 8B 91 ? ? ? ? Add 2 Read8")]
            internal static int CurrentCount;
            [Offset("Search 8B 78 ? 85 FF 75 ? 8B 93 ? ? ? ? Add 2 Read8")]
            internal static int ActionCount;
        }

        protected AgentFreeCompany(IntPtr pointer) : base(pointer)
        {
        }

        public IntPtr GetRosterPtr()
        {
            var ptr1 = Core.Memory.Read<IntPtr>(Pointer + 0x48);
            //  Log(ptr1);
            var ptr2 = Core.Memory.Read<IntPtr>(ptr1 + 0x98);
            // Log(ptr2);
            return ptr2;
        }


        public List<(string, bool)> GetMembers()
        {
            int i = 0;
            List<(string, bool)> result = new List<(string, bool)>();
            IntPtr start = GetRosterPtr();
            byte testByte = 0;
            do
            {
                var addr = (start + (i * 0x60));
                testByte = Core.Memory.Read<byte>(addr);
                if (testByte != 0)
                    result.Add((Core.Memory.ReadStringUTF8(addr + 0x22), Core.Memory.Read<byte>(addr + 0xD) != 0));
                i++;
            }
            while (testByte != 0);

            return result;
        }

        public byte HistoryLineCount => Core.Memory.Read<byte>(Pointer + Offsets.HistoryCount);

        public IntPtr ActionAddress
        {
            get
            {
                var one = Core.Memory.Read<IntPtr>(Offsets.AtkStage);
                var two = Core.Memory.Read<IntPtr>(one + Offsets.off1);
                var three = Core.Memory.Read<IntPtr>(two + Offsets.off2);
                var four = Core.Memory.Read<IntPtr>(three + Offsets.off3);
                var final = Core.Memory.Read<IntPtr>(four + Offsets.off4);
                return final;
            }
        }

        public async Task<FcAction[]> GetCurrentActions()
        {
            bool wasopen = FreeCompany.Instance.IsOpen;
            if (!FreeCompany.Instance.IsOpen)
            {
                Instance.Toggle(); 
                await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
            }

            if (FreeCompany.Instance.IsOpen)
            {
                FreeCompany.Instance.SelectActions();
                await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                if (FreeCompanyAction.Instance.IsOpen)
                {
                    uint numCurrentActions = Core.Memory.NoCacheRead<uint>(ActionAddress + Offsets.CurrentCount);
                    var currentActions = Core.Memory.ReadArray<FcAction>(ActionAddress + 0x8, (int) numCurrentActions);
                    if (!wasopen)
                        FreeCompany.Instance.Close();
                    return currentActions;
                }
            }

            return new FcAction[0];
        }

        public async Task<FcAction[]> GetAvailableActions()
        {
            bool wasopen = FreeCompany.Instance.IsOpen;
            if (!FreeCompany.Instance.IsOpen)
            {
                Instance.Toggle(); 
                await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
            }

            if (FreeCompany.Instance.IsOpen)
            {
                FreeCompany.Instance.SelectActions();
                await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                if (FreeCompanyAction.Instance.IsOpen)
                {
                    var actionCount = Core.Memory.NoCacheRead<uint>(ActionAddress + Offsets.ActionCount);
                    var actions = Core.Memory.ReadArray<FcAction>(ActionAddress + 0x30, (int) actionCount);
                    if (!wasopen)
                        FreeCompany.Instance.Close();
                    return actions;
                }
            }

            return new FcAction[0];
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0xC)]
    public struct FcAction
    {
        public uint id;
        public uint iconId;
        public uint unk;
    }
}