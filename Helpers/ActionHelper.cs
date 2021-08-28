using System;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Enums;
using LlamaLibrary.Extensions;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public static class ActionHelper
    {
        internal static class Offsets
        {
            [Offset("Search 40 53 55 57 41 54 41 57 48 83 EC ? 83 BC 24 ? ? ? ? ?")]
            internal static IntPtr DoAction;

            [Offset("Search 48 8D 0D ? ? ? ? 44 8D 42 ? E8 ? ? ? ? 85 C0 Add 3 TraceRelative")]
            internal static IntPtr ActionManagerParam;
            
            //41 B8 ? ? ? ? 89 5C 24 ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 84 C0 75 ?
            
            [Offset("Search 41 B8 ? ? ? ? 89 5C 24 ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 84 C0 75 ? Add 2 Read32")]
            internal static int DecipherSpell;
        }

        public static bool DoActionDecipher(BagSlot slot)
        {
            if ((slot.Item.MyItemRole() != MyItemRole.Map) || HasMap())return false;
            return Core.Memory.CallInjected64<byte>(Offsets.DoAction, new object[6]
            {
                Offsets.ActionManagerParam, //rcx
                (uint) ff14bot.Enums.ActionType.Spell, //rdx
                (uint) Offsets.DecipherSpell, //r8
                (long) Core.Player.ObjectId, //r9
                (int)slot.Item.Id, //a5 +0x28
                0 //a6 + 0x30
            }) == 1;
        }
        
        public static bool HasMap()
        {
            uint[] questMaps = new uint[]{2001351,2001705,2001772,200974};
            return InventoryManager.GetBagByInventoryBagId(InventoryBagId.KeyItems).FilledSlots.Any(i => i.EnglishName.EndsWith("map", StringComparison.InvariantCultureIgnoreCase) && !questMaps.Contains(i.RawItemId));
        }

        public static void DiscardCurrentMap()
        {
            var map = CurrentMap();

            if (map != default(BagSlot))
            {
                map.Discard();
            }
        }

        public static BagSlot CurrentMap()
        {
            uint[] questMaps = new uint[]{2001351,2001705,2001772,200974};
            var map = InventoryManager.GetBagByInventoryBagId(InventoryBagId.KeyItems).FilledSlots.Where(i => i.EnglishName.EndsWith("map", StringComparison.InvariantCultureIgnoreCase) && !questMaps.Contains(i.RawItemId)).ToList();
            return map.Any() ? map.First() : default(BagSlot);
        }

    }
}