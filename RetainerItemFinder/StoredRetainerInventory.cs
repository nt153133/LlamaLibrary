using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;

namespace LlamaLibrary.RetainerItemFinder
{
    public class StoredRetainerInventory : IStoredInventory
    {
        public Dictionary<uint, int> Inventory { get; } = new Dictionary<uint, int>();

        public Dictionary<uint, int> SlotCount { get; } = new Dictionary<uint, int>();

        public int FreeSlots { get; }

        public List<uint> EquippedItems;

        public StoredRetainerInventory(IntPtr pointer)
        {
            IntPtr position = pointer;
            EquippedItems = new List<uint>(Core.Memory.ReadArray<uint>(position, 14).Where(i => i != 0));
            position = position + (14 * sizeof(uint));
            var itemIds = Core.Memory.ReadArray<uint>(position, 175);
            position = position + (175 * sizeof(uint));
            var qtys = Core.Memory.ReadArray<ushort>(position, 175);
            position = position + (175 * sizeof(ushort));
            var crystalQtys = Core.Memory.ReadArray<ushort>(position, 18);

            for (int i = 0; i < 18; i++)
            {
                if (crystalQtys[i] == 0)
                {
                    continue;
                }

                Inventory.Add((uint)(i + 2), crystalQtys[i]);
                SlotCount.Add((uint)(i + 2), 1);
            }

            for (int i = 0; i < 175; i++)
            {
                if (itemIds[i] == 0)
                {
                    FreeSlots++;
                    continue;
                }

                if (Inventory.ContainsKey(itemIds[i]))
                {
                    Inventory[itemIds[i]] += qtys[i];
                    SlotCount[itemIds[i]]++;
                }
                else
                {
                    Inventory.Add(itemIds[i], qtys[i]);
                    SlotCount.Add(itemIds[i], 1);
                }
            }
        }
    }
}