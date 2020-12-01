namespace LlamaLibrary.Structs
{
    public class FcActionShopItem
    {
        public int ActionId;
        public int Rank;
        public int Cost;
        public int ShopIndex;
        public string Name;

        public FcActionShopItem(int actionId, int rank, int cost, int shopIndex, string name)
        {
            ActionId = actionId;
            Rank = rank;
            Cost = cost;
            ShopIndex = shopIndex;
            Name = name;
        }
    }
}