using ff14bot.Managers;

namespace LlamaLibrary.Materia
{
    public class MateriaItem
    {
        public int Key;
        public int Tier;
        public int Value;
        internal Item Item => DataManager.GetItem((uint)Key);
        public string ItemName => Item.CurrentLocaleName;

        public string Stat;

        public MateriaItem(int key, int tier, int value, string stat)
        {
            this.Key = key;
            this.Tier = tier;
            this.Value = value;
            this.Stat = stat;
        }

        public override string ToString()
        {
            return $"{DataManager.GetItem((uint)Key).CurrentLocaleName} {Tier} {Value}";
        }
    }
}