using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    public struct FCWorkshopItem
    {
        public int ItemId;
        public int Qty;
        public int TurnInsRequired;

        public FCWorkshopItem(int itemId, int qty, int turnInsRequired)
        {
            ItemId = itemId;
            Qty = qty;
            TurnInsRequired = turnInsRequired;
        }

        public override string ToString()
        {
            return $"{DataManager.GetItem((uint) ItemId).CurrentLocaleName} x {(Qty * TurnInsRequired)}";
        }
    }
}