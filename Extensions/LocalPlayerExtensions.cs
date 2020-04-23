using System;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using LlamaLibrary.Memory;

namespace LlamaLibrary.Extensions
{
    public static class LocalPlayerExtensions
    {
        internal static byte GatheringStatus(this LocalPlayer player)
        {
            return Core.Memory.Read<byte>(player.Pointer + Offsets.GatheringStateOffset);
        }

        internal static uint GCSeals(this LocalPlayer player)
        {
            uint[] sealTypes = {20, 21, 22};
            var bagslot = InventoryManager.GetBagByInventoryBagId(InventoryBagId.Currency).FirstOrDefault(i => sealTypes.Contains(i.RawItemId));
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