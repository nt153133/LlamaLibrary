namespace LlamaLibrary.Structs
{
    public class CraftingRelicTurnin
    {
        public uint ItemID;
        public int Job;
        public int Position;
        public int MinCollectability;
        public uint RewardItem;

        public CraftingRelicTurnin(uint itemId, int job, int position, int minCollectability, uint rewardItem)
        {
            ItemID = itemId;
            Job = job;
            Position = position;
            MinCollectability = minCollectability;
            RewardItem = rewardItem;
        }
    }
}