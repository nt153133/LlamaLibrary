using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public static class BlueMageSpellBook
    {
        private static class Offsets
        {
            [Offset("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? 85 C0 74 ? FF C3 83 FB ? 72 ? 49 8B CF Add 3 TraceRelative")]
            internal static IntPtr ActionManager;
            
            [Offset("Search 48 8B C4 48 89 68 ? 48 89 70 ? 41 56 48 83 EC ? 48 63 F2")]
            internal static IntPtr SetSpell;

            [Offset("Search 83 FA ? 77 ? 48 63 C2 8B 84 81 ? ? ? ? C3 33 C0 C3 ? ? ? ? ? ? ? ? ? ? ? ? ? 48 89 6C 24 ? Add 2 Read8")]
            [OffsetCN("Search 83 FE ? 0F 87 ? ? ? ? 48 89 58 ? Add 2 Read8")]
            internal static int MaxActive;

            [Offset("Search 8B 84 81 ? ? ? ? C3 33 C0 C3 ? ? ? ? ? ? ? ? ? ? ? ? ? 48 89 6C 24 ? Add 3 Read32")]
            [OffsetCN("Search 8B 94 B5 ? ? ? ? 48 8B CD Add 3 Read32")]
            internal static int BluSpellActiveOffset ;
        }
        
        public static uint[] ActiveSpells => Core.Memory.ReadArray<uint>(SpellLocation, Offsets.MaxActive +1);

        public static IntPtr SpellLocation => Offsets.ActionManager + Offsets.BluSpellActiveOffset;

        public static void SetSpell(int index, uint spellId)
        {
            Core.Memory.CallInjected64<IntPtr>(Offsets.SetSpell, new object[3]
            {
                Offsets.ActionManager,
                index,
                spellId
            });
        }
        
        public static async Task SetAllSpells(uint[] spells)
        {
            if (spells.Length > Offsets.MaxActive) return;
            int index = 0;
            for (int i = 0; i < spells.Length; i++)
            {
                SetSpell(i, spells[i]);
                await Coroutine.Sleep(200);
                index++;
            }
            
            for (int i = index; i < Offsets.MaxActive + 1; i++)
            {
                SetSpell(i, 0);
                await Coroutine.Sleep(200);
            }
        }

        public static async Task SetSpells(uint[] spells)
        {
            if (spells.Length > Offsets.MaxActive) return;
            
            var currentSpells = ActiveSpells;

            if (spells.All(i => currentSpells.Contains(i))) return;

            var spellsToAdd = spells.Except(currentSpells);

            List<(int, uint)> spellsToModify = new List<(int, uint)>(); 

            foreach (var spell in spellsToAdd)
            {
                for (int i = 0; i < currentSpells.Length; i++)
                {
                    if (currentSpells[i] == 0)
                    {
                        currentSpells[i] = spell;
                        spellsToModify.Add((i,spell));
                        break;
                    }

                    if (!spells.Contains(spell))
                    {
                        currentSpells[i] = spell;
                        spellsToModify.Add((i,spell));
                        break;
                    }
                }
            }

            foreach (var pair in spellsToModify)
            {
                SetSpell(pair.Item1, pair.Item2);
                await Coroutine.Sleep(300);
            }
        }
    }
}