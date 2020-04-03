using ff14bot.Managers;
using Newtonsoft.Json;

namespace LlamaLibrary.Retainers
{
    public class RetainerTaskData
    {
        public int Id { get; set; }
        public byte ClassJobCategory { get; set; }
        public bool IsRandom { get; set; }
        public int RetainerLevel { get; set; }
        public int VentureCost { get; set; }
        public int MaxTime { get; set; }
        public int Experience { get; set; }
        public int RequiredItemLevel { get; set; }
        public int RequiredGathering { get; set; }
        public string NameRaw { get; set; }
        public int ItemId { get; set; }

        [JsonIgnore]
        public string Name
        {
            get
            {
                if (IsRandom)
                    return NameRaw;
                else
                    return DataManager.GetItem((uint)ItemId).CurrentLocaleName;
            }
            
        }

        [JsonConstructor]
        public RetainerTaskData(int id, byte classJobCategory, bool isRandom, int retainerLevel, int ventureCost, int maxTime, int experience, int requiredItemLevel, int requiredGathering, string nameRaw, int itemId)
        {
            Id = id;
            ClassJobCategory = classJobCategory;
            IsRandom = isRandom;
            RetainerLevel = retainerLevel;
            VentureCost = ventureCost;
            MaxTime = maxTime;
            Experience = experience;
            RequiredItemLevel = requiredItemLevel;
            RequiredGathering = requiredGathering;
            NameRaw = nameRaw;
            ItemId = itemId;
        }

        public override string ToString()
        {
            return $"{Id} - {IsRandom} {NameRaw}";
        }
    }
}