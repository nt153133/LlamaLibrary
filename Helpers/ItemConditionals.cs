using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot.Enums;
using ff14bot.Managers;
using LlamaLibrary.Enums;
using LlamaLibrary.Extensions;
using Newtonsoft.Json;

namespace LlamaLibrary.Helpers
{
    public class ItemRoleConditional : Conditional
    {
        public ItemRoleConditional(string name, ActionType action, List<string> parameters) : base(name, action, parameters)
        {
            Type = ConditionalType.ItemRole;
        }

        public ItemRoleConditional(Conditional conditional)
        {
            Type = ConditionalType.ItemRole;
            Name = conditional.Name;
            Action = conditional.Action;
            Parameters = conditional.Parameters;
        }

        private List<MyItemRole> Roles
        {
            get
            {
                var temp = new List<MyItemRole>();

                foreach (var cat in Parameters)
                {
                    if (Enum.TryParse(cat, out MyItemRole enumOut))
                        temp.Add(enumOut);
                }

                return temp;
            }
        }

        public override IEnumerable<BagSlot> CheckCondition(IEnumerable<BagSlot> slots)
        {
            return slots.Where(i => Roles.Contains(i.Item.MyItemRole()));
        }

        public override string ToString()
        {
            return "Where(i=> _roles.Contains(i.MyItemRole()))";
        }
    }

    public class ItemCategoryConditional : Conditional
    {
        public ItemCategoryConditional(string name, ActionType action, List<string> parameters) : base(name, action, parameters)
        {
            Type = ConditionalType.ItemCategory;
        }

        public ItemCategoryConditional(Conditional conditional)
        {
            Type = ConditionalType.ItemCategory;
            Name = conditional.Name;
            Action = conditional.Action;
            Parameters = conditional.Parameters;
        }

        private List<ItemUiCategory> Categories
        {
            get
            {
                var temp = new List<ItemUiCategory>();

                foreach (var cat in Parameters)
                {
                    if (Enum.TryParse(cat, out ItemUiCategory enumOut))
                        temp.Add(enumOut);
                }

                return temp;
            }
        }

        public override IEnumerable<BagSlot> CheckCondition(IEnumerable<BagSlot> slots)
        {
            return slots.Where(i => Categories.Contains(i.Item.EquipmentCatagory));
        }

        public override string ToString()
        {
            return "Where(i=> _categories.Contains(i.EquipmentCatagory))";
        }
    }

    public class NameConditional : Conditional
    {
        public NameConditional(string name, ActionType action, List<string> parameters) : base(name, action, parameters)
        {
            Type = ConditionalType.Name;
        }

        public NameConditional(Conditional conditional)
        {
            Type = ConditionalType.Name;
            Name = conditional.Name;
            Action = conditional.Action;
            Parameters = conditional.Parameters;
        }

        private bool HasMatch(string inString)
        {
            return Parameters.Any(inString.Contains);
        }

        public override IEnumerable<BagSlot> CheckCondition(IEnumerable<BagSlot> slots)
        {
            return slots.Where(i => HasMatch(i.Name));
        }

        public override string ToString()
        {
            return "Where(i=> hasMatch(i.Name))";
        }
    }

    public class IdConditional : Conditional
    {
        public IdConditional(string name, ActionType action, List<string> parameters) : base(name, action, parameters)
        {
            Type = ConditionalType.Id;
        }

        public IdConditional(Conditional conditional)
        {
            Type = ConditionalType.Id;
            Name = conditional.Name;
            Action = conditional.Action;
            Parameters = conditional.Parameters;
        }

        private List<uint> Ids
        {
            get
            {
                return Parameters.Select(cat => (uint) int.Parse(cat)).ToList();
            }
        }

        public override IEnumerable<BagSlot> CheckCondition(IEnumerable<BagSlot> slots)
        {
            return slots.Where(i => Ids.Contains(i.RawItemId));
        }

        public override string ToString()
        {
            return "Where(i=> _ids.Contain(i.RawItemID))";
        }
    }

    public class Conditional
    {
        public ActionType Action;
        public string Name;

        public List<string> Parameters;

        protected ConditionalType Type;

        public Conditional(string name, ConditionalType type, List<string> parameters)
        {
            Name = name;
            Type = type;
            Parameters = parameters;
            Action = ActionType.Sell;
        }

        [JsonConstructor]
        public Conditional(string name, ConditionalType type, ActionType action, List<string> parameters)
        {
            Name = name;
            Type = type;
            Action = action;
            Parameters = parameters;
        }

        protected Conditional(string name, ActionType action, List<string> parameters)
        {
            Name = name;
            Action = action;
            Parameters = parameters;
        }

        protected Conditional()
        {
        }

        public virtual IEnumerable<BagSlot> CheckCondition(IEnumerable<BagSlot> slots)
        {
            return slots;
        }
    }

    public enum ConditionalType
    {
        ItemRole,
        ItemCategory,
        Name,
        Id
    }

    public enum ActionType
    {
        Sell,
        Discard,
        Desynth
    }
}