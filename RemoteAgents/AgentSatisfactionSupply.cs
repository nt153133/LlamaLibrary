using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.RemoteAgents
{
    public class AgentSatisfactionSupply: AgentInterface<AgentSatisfactionSupply>, IAgent
    {
        public IntPtr RegisteredVtable => Offsets.VTable;
        private static class Offsets
        {
            [Offset("Search 48 8D 05 ? ? ? ? C6 43 ? ? 48 89 03 48 8D 4B ? Add 3 TraceRelative")]
            internal static IntPtr VTable;
            [Offset("Search 4C 8D 47 ? BA ? ? ? ? 48 8D 0D ? ? ? ? Add 3 Read8")]
            internal static int DoHItemId;
            [Offset("Search 4C 8D 87 ? ? ? ? BA ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 84 C0 0F 84 ? ? ? ? 4C 8D 87 ? ? ? ? Add 3 Read32")]
            internal static int DoLItemId;
            [Offset("Search 4C 8D 87 ? ? ? ? BA ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 84 C0 0F 84 ? ? ? ? 8B 4F ? Add 3 Read32")]
            internal static int FshItemId;
            [Offset("Search 0F B7 73 ? 48 8D 8D ? ? ? ? Add 3 Read8")]
            internal static int CurrentRep;
            [Offset("Search 0F B7 73 ? BA ? ? ? ? E8 ? ? ? ? Add 3 Read8")]
            internal static int MaxRep;
            [Offset("Search 8B 53 ? 4C 8D 43 ? 48 8B CB Add 2 Read8")]
            //[OffsetCN("Search 8B 53 ? 48 8B CB E8 ? ? ? ? BA ? ? ? ? Add 2 Read8")]
            internal static int Npc;
            [Offset("Search 89 43 ? E8 ? ? ? ? 48 8B D0 48 8D 4D ? E8 ? ? ? ? Add 2 Read8")]
            internal static int HeartLevel;
            [Offset("Search 44 0F B6 43 ? 89 43 ? Add 4 Read8")]
            internal static int DeliveriesRemaining;
            [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 48 8B 01 41 0F B6 E9 41 8B F8")]
            internal static IntPtr OpenWindow;
            [Offset("Search 4C 8D 43 ? 48 8B CB E8 ? ? ? ? BA ? ? ? ? Add 3 Read8")]
            //[OffsetCN("Search 4C 8D 43 ? 8B 53 ? 48 8B CB Add 3 Read8")]
            internal static int NpcId;

        }
        protected AgentSatisfactionSupply(IntPtr pointer) : base(pointer)
        {

        }

        public byte DeliveriesRemaining => Core.Memory.Read<byte>(Pointer + Offsets.DeliveriesRemaining);
        public byte HeartLevel => Core.Memory.Read<byte>(Pointer + Offsets.HeartLevel);
        public uint DoHItemId => Core.Memory.Read<ushort>(Pointer + Offsets.DoHItemId);
        public uint DoLItemId => Core.Memory.Read<ushort>(Pointer + Offsets.DoLItemId);
        public uint FshItemId => Core.Memory.Read<ushort>(Pointer + Offsets.FshItemId);
        public byte Npc => Core.Memory.Read<byte>(Pointer + Offsets.Npc);

        public uint NpcId => Core.Memory.Read<uint>(Pointer + Offsets.NpcId);
        public uint CurrentRep => Core.Memory.Read<ushort>(Pointer + Offsets.CurrentRep);
        public uint MaxRep => Core.Memory.Read<ushort>(Pointer + Offsets.MaxRep);

        public bool HasDoHTurnin => InventoryManager.FilledSlots.Any(i => i.RawItemId == AgentSatisfactionSupply.Instance.DoHItemId);
        public bool HasDoLTurnin => InventoryManager.FilledSlots.Any(i => i.RawItemId == AgentSatisfactionSupply.Instance.DoLItemId);
        public bool HasFshTurnin => InventoryManager.FilledSlots.Any(i => i.RawItemId == AgentSatisfactionSupply.Instance.FshItemId);

        public bool HasAnyTurnin => HasDoHTurnin || HasDoLTurnin || HasFshTurnin;

        public async Task LoadWindow(uint npc)
        {
            if (SatisfactionSupply.Instance.IsOpen)
            {
                SatisfactionSupply.Instance.Close();
                await Coroutine.Wait(5000, () => !SatisfactionSupply.Instance.IsOpen);
            }

            Core.Memory.CallInjected64<IntPtr>(Offsets.OpenWindow,
                                               Pointer,
                                               (uint)0,
                                               (npc),
                                               (uint)1);

            await Coroutine.Wait(5000, () => SatisfactionSupply.Instance.IsOpen);

            if (SatisfactionSupply.Instance.IsOpen)
            {
                SatisfactionSupply.Instance.Close();
                await Coroutine.Wait(5000, () => !SatisfactionSupply.Instance.IsOpen);
            }
        }
    }
}