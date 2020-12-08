using System;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using LlamaLibrary.Memory;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Extensions
{

    public static class LocalPlayerExtensions
    {
        internal static class Offsets
        {
            /*[Offset("Search 44 88 84 0A ? ? ? ? C3 ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? 88 91 ? ? ? ? Add 4 Read32")]
            internal static int GatheringStateOffset;*/
            [Offset("Search 0F B6 15 ? ? ? ? 8D 42 ? 3C ? 77 ? FE CA 48 8D 0D ? ? ? ? Add 3 TraceRelative")]
            internal static IntPtr CurrentGC;
            [Offset("Search 48 83 EC ? 48 8B 05 ? ? ? ? 44 8B C1 BA ? ? ? ? 48 8B 88 ? ? ? ? E8 ? ? ? ? 48 85 C0 75 ? 48 83 C4 ? C3 48 8B 00 48 83 C4 ? C3 ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? 48 83 EC ? 80 F9 ?")]
            internal static IntPtr GCGetMaxSealsByRank;
        }
        
        /*internal static byte GatheringStatus(this LocalPlayer player)
        {
            return Core.Memory.Read<byte>(player.Pointer + Offsets.GatheringStateOffset);
        }*/

        internal static uint GCSeals(this LocalPlayer player)
        {
            uint[] sealTypes = {20, 21, 22};
            var bagslot = InventoryManager.GetBagByInventoryBagId(InventoryBagId.Currency).FirstOrDefault(i => i.RawItemId == sealTypes[(int)Core.Me.GrandCompany -1]);
            return bagslot?.Count ?? (uint) 0;
        }

        internal static int MaxGCSeals(this LocalPlayer player)
        {
            byte gc = Core.Memory.Read<byte>(Offsets.CurrentGC);
            if (gc == 0) return 0;

            var Rank = Core.Memory.Read<byte>(Offsets.CurrentGC + gc);
            IntPtr rankRow;
            lock (Core.Memory.Executor.AssemblyLock)
                rankRow = Core.Memory.CallInjected64<IntPtr>(Offsets.GCGetMaxSealsByRank,
                                                             Rank);
            return Core.Memory.Read<int>(rankRow);
        }
    }
}