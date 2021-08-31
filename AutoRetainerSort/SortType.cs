// ReSharper disable InconsistentNaming
using System;
using System.Linq;
using ff14bot.Enums;
using Newtonsoft.Json;

namespace LlamaLibrary.AutoRetainerSort
{
    public enum SortType : byte
    {
        UNKNOWN = 0,
        Weapons = 1,
        Armor = 2,
        Accessories = 3,
        Tools = 4,
        Housing = 5,
        Medicine = 6,
        Ingredient = 7,
        Meal = 8,
        Seafood = 9,
        Stone = 10,
        Metal = 11,
        Lumber = 12,
        Cloth = 13,
        Leather = 14,
        Bone = 15,
        Reagent = 16,
        Dye = 17,
        Part = 18,
        Materia = 19,
        Crystal = 20,
        Catalyst = 21,
        Miscellany = 22,
        Other = 23,
        Minion = 24,
        Demimateria = 25,
        Currency = 26,
        Seasonal_Miscellany = 27,
        Triple_Triad_Card = 28,
        Orchestrion_Roll = 29,
        Fishing_Tackle = 30,
        AirshipSubmersible = 31,
        Soul_Crystal = 32,
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class SortTypeWithCount : IEquatable<SortTypeWithCount>, IEquatable<SortType>
    {
        [JsonProperty("SortType")]
        public readonly SortType SortType;

        public int Count =>
            ItemSortStatus.GetAllInventories()
                .Sum(inv => inv.ItemSlotsTakenCounts
                    .Where(pair => ItemSortStatus.GetSortInfo(pair.Key).SortType == SortType)
                    .Sum(pair => pair.Value));

        public SortTypeWithCount(SortType sortType)
        {
            SortType = sortType;
        }

        public override string ToString() => $"{SortType.ToString()} [{Count.ToString()}]";

        public bool Equals(SortTypeWithCount other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SortType == other.SortType;
        }

        public bool Equals(SortType other)
        {
            return other == SortType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            Type type = obj.GetType();
            if (type == typeof(SortType)) return Equals((SortType)obj);
            if (type != GetType()) return false;
            return Equals((SortTypeWithCount)obj);
        }

        public override int GetHashCode()
        {
            return (int)SortType;
        }
    }

    public static class SortExtensions
    {
        public static SortType GetSortType(this ItemUiCategory category)
        {
            switch (category)
            {
                case ItemUiCategory.Pugilists_Arm:
                case ItemUiCategory.Gladiators_Arm:
                case ItemUiCategory.Marauders_Arm:
                case ItemUiCategory.Archers_Arm:
                case ItemUiCategory.Lancers_Arm:
                case ItemUiCategory.One_handed_Thaumaturges_Arm:
                case ItemUiCategory.Two_handed_Thaumaturges_Arm:
                case ItemUiCategory.One_handed_Conjurers_Arm:
                case ItemUiCategory.Two_handed_Conjurers_Arm:
                case ItemUiCategory.Arcanists_Grimoire:
                case ItemUiCategory.Blue_Mages_Arm:
                case ItemUiCategory.Gunbreakers_Arm:
                case ItemUiCategory.Dancers_Arm:
                case ItemUiCategory.Samurais_Arm:
                case ItemUiCategory.Red_Mages_Arm:
                case ItemUiCategory.Scholars_Arm:
                case ItemUiCategory.Dark_Knights_Arm:
                case ItemUiCategory.Machinists_Arm:
                case ItemUiCategory.Astrologians_Arm:
                case ItemUiCategory.Rogues_Arms:
                case ItemUiCategory.Shield:
                    return SortType.Weapons;
                case ItemUiCategory.Carpenters_Primary_Tool:
                case ItemUiCategory.Carpenters_Secondary_Tool:
                case ItemUiCategory.Blacksmiths_Primary_Tool:
                case ItemUiCategory.Blacksmiths_Secondary_Tool:
                case ItemUiCategory.Armorers_Primary_Tool:
                case ItemUiCategory.Armorers_Secondary_Tool:
                case ItemUiCategory.Goldsmiths_Primary_Tool:
                case ItemUiCategory.Goldsmiths_Secondary_Tool:
                case ItemUiCategory.Leatherworkers_Primary_Tool:
                case ItemUiCategory.Leatherworkers_Secondary_Tool:
                case ItemUiCategory.Weavers_Primary_Tool:
                case ItemUiCategory.Weavers_Secondary_Tool:
                case ItemUiCategory.Alchemists_Primary_Tool:
                case ItemUiCategory.Alchemists_Secondary_Tool:
                case ItemUiCategory.Culinarians_Primary_Tool:
                case ItemUiCategory.Culinarians_Secondary_Tool:
                case ItemUiCategory.Miners_Primary_Tool:
                case ItemUiCategory.Miners_Secondary_Tool:
                case ItemUiCategory.Botanists_Primary_Tool:
                case ItemUiCategory.Botanists_Secondary_Tool:
                case ItemUiCategory.Fishers_Primary_Tool:
                case ItemUiCategory.Fishers_Secondary_Tool:
                    return SortType.Tools;
                case ItemUiCategory.Fishing_Tackle:
                    return SortType.Fishing_Tackle;
                case ItemUiCategory.Head:
                case ItemUiCategory.Body:
                case ItemUiCategory.Legs:
                case ItemUiCategory.Hands:
                case ItemUiCategory.Feet:
                case ItemUiCategory.Waist:
                    return SortType.Armor;
                case ItemUiCategory.Necklace:
                case ItemUiCategory.Earrings:
                case ItemUiCategory.Bracelets:
                case ItemUiCategory.Ring:
                    return SortType.Accessories;
                case ItemUiCategory.Medicine:
                    return SortType.Medicine;
                case ItemUiCategory.Ingredient:
                    return SortType.Ingredient;
                case ItemUiCategory.Meal:
                    return SortType.Meal;
                case ItemUiCategory.Seafood:
                    return SortType.Seafood;
                case ItemUiCategory.Stone:
                    return SortType.Stone;
                case ItemUiCategory.Metal:
                    return SortType.Metal;
                case ItemUiCategory.Lumber:
                    return SortType.Lumber;
                case ItemUiCategory.Cloth:
                    return SortType.Cloth;
                case ItemUiCategory.Leather:
                    return SortType.Leather;
                case ItemUiCategory.Bone:
                    return SortType.Bone;
                case ItemUiCategory.Reagent:
                    return SortType.Reagent;
                case ItemUiCategory.Dye:
                    return SortType.Dye;
                case ItemUiCategory.Part:
                    return SortType.Part;
                case ItemUiCategory.Materia:
                    return SortType.Materia;
                case ItemUiCategory.Crystal:
                    return SortType.Crystal;
                case ItemUiCategory.Catalyst:
                    return SortType.Catalyst;
                case ItemUiCategory.Miscellany:
                    return SortType.Miscellany;
                case ItemUiCategory.Soul_Crystal:
                    return SortType.Soul_Crystal;
                case ItemUiCategory.Other:
                    return SortType.Other;
                case ItemUiCategory.Furnishing:
                case ItemUiCategory.Construction_Permit:
                case ItemUiCategory.Roof:
                case ItemUiCategory.Exterior_Wall:
                case ItemUiCategory.Window:
                case ItemUiCategory.Door:
                case ItemUiCategory.Roof_Decoration:
                case ItemUiCategory.Exterior_Wall_Decoration:
                case ItemUiCategory.Placard:
                case ItemUiCategory.Fence:
                case ItemUiCategory.Interior_Wall:
                case ItemUiCategory.Flooring:
                case ItemUiCategory.Ceiling_Light:
                case ItemUiCategory.Outdoor_Furnishing:
                case ItemUiCategory.Table:
                case ItemUiCategory.Tabletop:
                case ItemUiCategory.Wall_mounted:
                case ItemUiCategory.Rug:
                case ItemUiCategory.Gardening:
                case ItemUiCategory.Painting:
                    return SortType.Housing;
                case ItemUiCategory.Minion:
                    return SortType.Minion;
                case ItemUiCategory.Demimateria:
                    return SortType.Demimateria;
                case ItemUiCategory.Seasonal_Miscellany:
                    return SortType.Seasonal_Miscellany;
                case ItemUiCategory.Triple_Triad_Card:
                    return SortType.Triple_Triad_Card;
                case ItemUiCategory.Orchestrion_Roll:
                    return SortType.Orchestrion_Roll;
                case ItemUiCategory.Airship_Hull:
                case ItemUiCategory.Airship_Rigging:
                case ItemUiCategory.Airship_Aftcastle:
                case ItemUiCategory.Airship_Forecastle:
                case ItemUiCategory.Submersible_Hull:
                case ItemUiCategory.Submersible_Stern:
                case ItemUiCategory.Submersible_Bow:
                case ItemUiCategory.Submersible_Bridge:
                    return SortType.AirshipSubmersible;
                case ItemUiCategory.Currency:
                    return SortType.Currency;
                default:
                    return SortType.UNKNOWN;
            }
        }
    }
}