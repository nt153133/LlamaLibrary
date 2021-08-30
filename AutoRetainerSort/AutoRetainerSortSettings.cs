using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using ff14bot.Helpers;
using ff14bot.Managers;
using LlamaLibrary.AutoRetainerSort.Classes;
using LlamaLibrary.Helpers;
using Newtonsoft.Json;

namespace LlamaLibrary.AutoRetainerSort
{
    public class AutoRetainerSortSettings : JsonSettings
    {
        private static AutoRetainerSortSettings _settings;

        public AutoRetainerSortSettings() : base(Path.Combine(JsonHelper.UniqueCharacterSettingsDirectory, "AutoRetainerSort.json"))
        {
        }

        public static AutoRetainerSortSettings Instance => _settings ?? (_settings = new AutoRetainerSortSettings());

        private Dictionary<int, InventorySortInfo> _inventoryOptions;

        private bool _autoGenLisbeth;

        private Point _windowPosition = Point.Empty;

        [Browsable(false)]
        public Dictionary<int, InventorySortInfo> InventoryOptions
        {
            get
            {
                if (_inventoryOptions != null) return _inventoryOptions;

                _inventoryOptions = new Dictionary<int, InventorySortInfo>
                {
                    {ItemSortStatus.PlayerInventoryIndex, new InventorySortInfo("Player Inventory")},
                    {ItemSortStatus.SaddlebagInventoryIndex, new InventorySortInfo("Chocobo Saddlebag")},
                };

                return _inventoryOptions;
            }
            set
            {
                if (_inventoryOptions == value)
                {
                    return;
                }

                _inventoryOptions = value;
                Save();
            }
        }

        [Browsable(false)]
        public Point WindowPosition
        {
            get => _windowPosition;
            set
            {
                if (_windowPosition == value)
                {
                    return;
                }

                _windowPosition = value;
                Save();
            }
        }

        [Setting]
        [DisplayName("Auto-Gen Lisbeth")]
        [Description("Auto-generate Lisbeth storage rules for the current setup?")]
        [DefaultValue(false)]
        public bool AutoGenLisbeth
        {
            get => _autoGenLisbeth;
            set
            {
                if (_autoGenLisbeth == value)
                {
                    return;
                }

                _autoGenLisbeth = value;
                Save();
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class InventorySortInfo
    {
        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("SortTypes")]
        public BindingList<SortType> SortTypes;

        [JsonProperty("SpecificItems")]
        public BindingList<ItemSortInfo> SpecificItems;

        public bool ContainsType(SortType type) => SortTypes.Contains(type);

        public bool ContainsId(uint trueItemId) => SpecificItems.Any(x => x.TrueItemId == trueItemId);

        public bool ContainsItem(ItemSortInfo sortInfo) => SpecificItems.Any(x => x.Equals(sortInfo));

        public bool ContainsSlot(BagSlot bagSlot) => ContainsId(bagSlot.TrueItemId) || ContainsType(bagSlot.Item.EquipmentCatagory.GetSortType());

        public bool Any() => SortTypes.Any() || SpecificItems.Any();

        public override string ToString() => Name;

        public InventorySortInfo(string name)
        {
            Name = name;
            SortTypes = new BindingList<SortType>();
            SpecificItems = new BindingList<ItemSortInfo>();
        }
    }
}