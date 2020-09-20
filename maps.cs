using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary
{
    public static class TreasureMap
    {
        internal static class Offsets
        {
            /// <summary>
            ///     Pointer to where the game stores the map subkey
            /// </summary>
            [Offset("Search 48 8d 0d ?? ?? ?? ?? e8 ?? ?? ?? ?? 48 8d 0d ?? ?? ?? ?? 0f b7 d8 e8 ?? ?? ?? ?? Add 3 TraceRelative")]
            internal static IntPtr CurrentMap;
        }
        
        /// <summary>
        ///     Provides a KeyItem => TreasureHuntRank lookup because the client does the same thing.
        /// </summary>
        public static readonly Dictionary<uint, short> MapPrimary = new Dictionary<uint, short>
        {
            //KeyEventItemName
            {2001087, 1},
            {2001088, 2},
            {2001089, 3},
            {2001090, 4},
            {2001091, 5},
            {2001223, 6},
            {2001352, 7},
            {2001551, 8},
            {2001762, 9},
            {2001763, 10},
            {2001764, 11},
            {2002209, 12},
            {2002210, 13},
            {2002386, 14},
            {2002503, 16},
            {2002663, 17},
            {2002664, 18},

            //InstanceMap
            {2001977, 11},
            {2002236, 13},
            {2002260, 14},
            {2002504, 16},
            {2002665, 18},
        };

        static TreasureMap()
        {
        }

        private static short PrimaryKey
        {
            get
            {
                var items = InventoryManager.GetBagByInventoryBagId(InventoryBagId.KeyItems).FilledSlots
                    .FirstOrDefault(i => MapPrimary.ContainsKey(i.RawItemId));
                if (items != null)
                {
                    return MapPrimary[items.RawItemId];
                }

                return 0;
            }
        }

        public static short SecondaryKey => Core.Memory.Read<short>(Offsets.CurrentMap);
        
    }
}