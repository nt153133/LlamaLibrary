using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using ff14bot.Helpers;
using ff14bot.Managers;
using Newtonsoft.Json;

namespace LlamaLibrary.AutoRetainerSort
{
    public class AutoRetainerSortSettings : JsonSettings
    {
        private static AutoRetainerSortSettings _settings;

        public AutoRetainerSortSettings() : base(Path.Combine(CharacterSettingsDirectory, "AutoRetainerSort.json"))
        {
        }
        
        public static AutoRetainerSortSettings Instance => _settings ?? (_settings = new AutoRetainerSortSettings());

        private Dictionary<int, InventorySortInfo> _inventoryOptions;

        private bool _autoGenLisbeth = true;

        private Point _windowPosition = Point.Empty;

        [Browsable(false)]
        public Dictionary<int, InventorySortInfo> InventoryOptions
        {
            get
            {
                if (_inventoryOptions != null) return _inventoryOptions;

                _inventoryOptions = new Dictionary<int, InventorySortInfo>
                {
                    {-2, new InventorySortInfo("Player Inventory")},
                    {-1, new InventorySortInfo("Chocobo Saddlebag")},
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
        [DefaultValue(true)]
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

        [JsonProperty("ItemIds")]
        public BindingList<uint> ItemIds;

        public bool Contains(SortType type) => SortTypes.Contains(type);

        public bool Contains(uint itemId) => ItemIds.Contains(itemId);

        public bool Contains(BagSlot bagSlot) => Contains(bagSlot.TrueItemId) || Contains(bagSlot.Item.EquipmentCatagory.GetSortType());

        public bool Any() => SortTypes.Any() || ItemIds.Any();

        public override string ToString() => Name;

        public InventorySortInfo(string name)
        {
            Name = name;
            SortTypes = new BindingList<SortType>();
            ItemIds = new BindingList<uint>();
        }
    }
}