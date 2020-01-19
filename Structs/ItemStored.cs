namespace LlamaLibrary.Structs
{
    public struct ItemStored
    {
        public uint ItemId;
        public bool IsHq;
        public uint Count;
        public uint Ilvl;
        public string ItemUICategory;
        public string Name;

        public ItemStored(uint itemId, bool isHq, uint count, uint ilvl, string itemUiCategory, string name)
        {
            ItemId = itemId;
            IsHq = isHq;
            Count = count;
            ItemUICategory = itemUiCategory;
            Name = name;
            Ilvl = ilvl;
        }

        public override string ToString()
        {
            return $"{ItemId},{IsHq},{Count},{Ilvl},{ItemUICategory},{Name}";
        }
    }
}