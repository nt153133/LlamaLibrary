using System.Collections.Generic;

namespace LlamaLibrary.RetainerItemFinder
{
    public interface IStoredInventory
    {
        Dictionary<uint, int> Inventory { get; }

        int FreeSlots { get; }
    }
}