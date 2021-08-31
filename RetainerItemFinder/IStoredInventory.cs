using System.Collections.Generic;

namespace LlamaLibrary.RetainerItemFinder
{
    public interface IStoredInventory
    {
        Dictionary<uint, int> Inventory { get; }
        
        Dictionary<uint, int> SlotCount { get; }

        int FreeSlots { get; }
    }
}