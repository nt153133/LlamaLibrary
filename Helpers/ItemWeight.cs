using System.Collections.Generic;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Extensions;

namespace LlamaLibrary.Helpers
{
    public static class ItemWeight
    {
        public static float GetItemWeight(Item item, ClassJobType job = ClassJobType.Adventurer)
        {
            if (!MainHandsAndOffHands.Contains(item.EquipmentCatagory) && !item.IsArmor && !item.IsWeapon) return -1f;
            if (job == ClassJobType.Adventurer) job = Core.Me.CurrentJob;
            if (!item.IsValidForClass(job)) return -1f;
            ushort level;
            if (job == ClassJobType.Scholar || job == ClassJobType.Summoner) level = Core.Me.Levels[ClassJobType.Arcanist];
            else level = Core.Me.Levels[job];
            if ((item.Id == 2634 || item.Id == 2633) && level <= 10) return 5000f;
            if (item.Id == 8567 && level <= 25) return 5000f;
            if (item.Id == 14043 && level <= 30) return 5000f;
            if (item.Id == 16039 && level <= 50) return 5000f;
            if (item.Id == 24589 && level <= 70) return 5000f;

            float weight = 0;
            Dictionary<ItemAttribute, float> values;
            if (job.IsDoh()) values = DoHWeights;
            else if (job.IsDol()) values = DoLWeights;
            else values = ClassItemWeightStorage.Instance.Values;

            foreach (var itemStat in item.Attributes)
            {
                if (values.TryGetValue(itemStat.Key, out var value))
                {
                    weight += itemStat.Value * value;
                }
            }
            return weight;
        }

        private static readonly Dictionary<ItemAttribute, float> DoHWeights = new Dictionary<ItemAttribute, float>
        {
            {ItemAttribute.Craftsmanship, 1},
            {ItemAttribute.Control, 1},
            {ItemAttribute.CP, 1}
        };
        
        private static readonly Dictionary<ItemAttribute, float> DoLWeights = new Dictionary<ItemAttribute, float>
        {
            {ItemAttribute.Gathering, 1},
            {ItemAttribute.Perception, 1},
            {ItemAttribute.GP, 1}
        };
        
        /*
        private static List<ItemUiCategory> MainHands => (from itemUi in (ItemUiCategory[]) Enum.GetValues(typeof(ItemUiCategory))
                                                          let name = Enum.GetName(typeof(ItemUiCategory), itemUi)
                                                          where name != null
                                                          where name.EndsWith("_Arm") || name.EndsWith("_Arms") || name.EndsWith("_Primary_Tool") || name.EndsWith("_Grimoire") select itemUi)
                                                         .ToList();

        private static List<ItemUiCategory> OffHands => (from itemUi in (ItemUiCategory[]) Enum.GetValues(typeof(ItemUiCategory))
                                                         let name = Enum.GetName(typeof(ItemUiCategory), itemUi)
                                                         where name != null
                                                         where name.Equals("Shield") || name.EndsWith("_Secondary_Tool") select itemUi)
                                                        .ToList();
        */

        public static readonly List<ItemUiCategory> MainHandsAndOffHands = new List<ItemUiCategory>
        {
            ItemUiCategory.Pugilists_Arm,
            ItemUiCategory.Gladiators_Arm,
            ItemUiCategory.Marauders_Arm,
            ItemUiCategory.Archers_Arm,
            ItemUiCategory.Lancers_Arm,
            ItemUiCategory.One_handed_Thaumaturges_Arm,
            ItemUiCategory.Two_handed_Thaumaturges_Arm,
            ItemUiCategory.One_handed_Conjurers_Arm,
            ItemUiCategory.Two_handed_Conjurers_Arm,
            ItemUiCategory.Arcanists_Grimoire,
            ItemUiCategory.Shield,
            ItemUiCategory.Carpenters_Primary_Tool,
            ItemUiCategory.Carpenters_Secondary_Tool,
            ItemUiCategory.Blacksmiths_Primary_Tool,
            ItemUiCategory.Blacksmiths_Secondary_Tool,
            ItemUiCategory.Armorers_Primary_Tool,
            ItemUiCategory.Armorers_Secondary_Tool,
            ItemUiCategory.Goldsmiths_Primary_Tool,
            ItemUiCategory.Goldsmiths_Secondary_Tool,
            ItemUiCategory.Leatherworkers_Primary_Tool,
            ItemUiCategory.Leatherworkers_Secondary_Tool,
            ItemUiCategory.Weavers_Primary_Tool,
            ItemUiCategory.Weavers_Secondary_Tool,
            ItemUiCategory.Alchemists_Primary_Tool,
            ItemUiCategory.Alchemists_Secondary_Tool,
            ItemUiCategory.Culinarians_Primary_Tool,
            ItemUiCategory.Culinarians_Secondary_Tool,
            ItemUiCategory.Miners_Primary_Tool,
            ItemUiCategory.Miners_Secondary_Tool,
            ItemUiCategory.Botanists_Primary_Tool,
            ItemUiCategory.Botanists_Secondary_Tool,
            ItemUiCategory.Fishers_Primary_Tool,
            ItemUiCategory.Rogues_Arms,
            ItemUiCategory.Dark_Knights_Arm,
            ItemUiCategory.Machinists_Arm,
            ItemUiCategory.Astrologians_Arm,
            ItemUiCategory.Samurais_Arm,
            ItemUiCategory.Red_Mages_Arm,
            ItemUiCategory.Scholars_Arm,
            ItemUiCategory.Fishers_Secondary_Tool,
            ItemUiCategory.Blue_Mages_Arm,
            ItemUiCategory.Gunbreakers_Arm,
            ItemUiCategory.Dancers_Arm
        };

        public static readonly List<ItemUiCategory> MainHands = new List<ItemUiCategory>
        {
            ItemUiCategory.Pugilists_Arm,
            ItemUiCategory.Gladiators_Arm,
            ItemUiCategory.Marauders_Arm,
            ItemUiCategory.Archers_Arm,
            ItemUiCategory.Lancers_Arm,
            ItemUiCategory.One_handed_Thaumaturges_Arm,
            ItemUiCategory.Two_handed_Thaumaturges_Arm,
            ItemUiCategory.One_handed_Conjurers_Arm,
            ItemUiCategory.Two_handed_Conjurers_Arm,
            ItemUiCategory.Arcanists_Grimoire,
            ItemUiCategory.Carpenters_Primary_Tool,
            ItemUiCategory.Blacksmiths_Primary_Tool,
            ItemUiCategory.Armorers_Primary_Tool,
            ItemUiCategory.Goldsmiths_Primary_Tool,
            ItemUiCategory.Leatherworkers_Primary_Tool,
            ItemUiCategory.Weavers_Primary_Tool,
            ItemUiCategory.Alchemists_Primary_Tool,
            ItemUiCategory.Culinarians_Primary_Tool,
            ItemUiCategory.Miners_Primary_Tool,
            ItemUiCategory.Botanists_Primary_Tool,
            ItemUiCategory.Fishers_Primary_Tool,
            ItemUiCategory.Rogues_Arms,
            ItemUiCategory.Dark_Knights_Arm,
            ItemUiCategory.Machinists_Arm,
            ItemUiCategory.Astrologians_Arm,
            ItemUiCategory.Samurais_Arm,
            ItemUiCategory.Red_Mages_Arm,
            ItemUiCategory.Scholars_Arm,
            ItemUiCategory.Blue_Mages_Arm,
            ItemUiCategory.Gunbreakers_Arm,
            ItemUiCategory.Dancers_Arm
        };

        public static readonly List<ItemUiCategory> OffHands = new List<ItemUiCategory>
        {
            ItemUiCategory.Shield,
            ItemUiCategory.Carpenters_Secondary_Tool,
            ItemUiCategory.Blacksmiths_Secondary_Tool,
            ItemUiCategory.Armorers_Secondary_Tool,
            ItemUiCategory.Goldsmiths_Secondary_Tool,
            ItemUiCategory.Leatherworkers_Secondary_Tool,
            ItemUiCategory.Weavers_Secondary_Tool,
            ItemUiCategory.Alchemists_Secondary_Tool,
            ItemUiCategory.Culinarians_Secondary_Tool,
            ItemUiCategory.Miners_Secondary_Tool,
            ItemUiCategory.Botanists_Secondary_Tool,
            ItemUiCategory.Fishers_Secondary_Tool
        };
    }
}