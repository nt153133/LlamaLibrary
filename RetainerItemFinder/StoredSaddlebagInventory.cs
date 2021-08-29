using System.Collections.Generic;

namespace LlamaLibrary.RetainerItemFinder
{
    public class StoredSaddleBagInventory : IStoredInventory
    {
        public Dictionary<uint, int> Inventory { get; } = new Dictionary<uint, int>();

        public int FreeSlots { get; }

        public StoredSaddleBagInventory(uint[] itemIds, ushort[] itemQuantities)
        {
            // Assumes a non-expanded saddlebag, but who has expanded saddlebags...?
            for (var i = 0; i < 70; i++)
            {
                if (itemIds[i] == 0)
                {
                    FreeSlots++;
                    continue;
                }
                
                if (Inventory.ContainsKey(itemIds[i]))
                {
                    Inventory[itemIds[i]] += itemQuantities[i];
                }
                else
                {
                    Inventory.Add(itemIds[i], itemQuantities[i]);
                }
            }
        }
    }
}