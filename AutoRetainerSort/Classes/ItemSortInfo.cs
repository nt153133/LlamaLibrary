using System;
using System.Text;
using ff14bot.Managers;
using Newtonsoft.Json;

namespace LlamaLibrary.AutoRetainerSort.Classes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ItemSortInfo : IEquatable<ItemSortInfo>
    {
        private Item _itemInfo;

        public Item ItemInfo
        {
            get
            {
                if (_itemInfo == null)
                {
                    try
                    {
                        _itemInfo = DataManager.GetItem(RawItemId);
                    }
                    catch
                    {
                        AutoRetainerSort.LogCritical($"Warning! Couldn't get item data for ID {RawItemId.ToString()}, sorting results may be invalid.");
                    }
                }

                return _itemInfo;
            }
        }

        private string _name = string.Empty;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    try
                    {
                        if (ItemInfo == null) return "UNKNOWN";
                        if (string.IsNullOrEmpty(ItemInfo.CurrentLocaleName)) return "UNKNOWN";

                        _name = ItemInfo.CurrentLocaleName;
                    }
                    catch
                    {
                        return "UNKNOWN";
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(_name);
                if (IsCollectable)
                {
                    sb.Append(" (Collectable)");
                }
                else if (IsHighQuality)
                {
                    sb.Append(" (HQ)");
                }
                return sb.ToString();
            }
        }

        [JsonProperty("TrueItemId")]
        public readonly uint TrueItemId;
        
        public const int CollectableOffset = 5_000_000;
        public const int QualityOffset = 1_000_000;

        public uint RawItemId
        {
            get
            {
                if (IsCollectable) return TrueItemId - CollectableOffset;
                if (IsHighQuality) return TrueItemId - QualityOffset;
                return TrueItemId;
            }
        }

        private bool IsCollectable => TrueItemId > CollectableOffset;

        private bool IsHighQuality => TrueItemId > QualityOffset && TrueItemId < CollectableOffset;

        private SortType? _sortType;

        public SortType SortType
        {
            get
            {
                if (_sortType == null)
                {
                    if (ItemInfo == null) return SortType.UNKNOWN;

                    _sortType = ItemInfo.EquipmentCatagory.GetSortType();
                }

                return _sortType.Value;
            }
        }

        private int _matchingIndex = int.MaxValue;

        public int MatchingIndex
        {
            get
            {
                if (_matchingIndex == int.MaxValue)
                {
                    var index = int.MinValue;

                    foreach (var sortInfoPair in AutoRetainerSortSettings.Instance.InventoryOptions)
                    {
                        if (sortInfoPair.Value.ContainsId(TrueItemId))
                        {
                            index = sortInfoPair.Key;
                            break;
                        }

                        if (sortInfoPair.Value.ContainsType(SortType))
                        {
                            index = sortInfoPair.Key;
                        }
                    }

                    _matchingIndex = index;
                }

                return _matchingIndex;
            }
        }

        public ItemIndexStatus IndexStatus(int index)
        {
            if (MatchingIndex == int.MinValue) return ItemIndexStatus.Unknown;
            
            if (MatchingIndex == index) return ItemIndexStatus.BelongsInCurrentIndex;

            if (ItemSortStatus.FilledAndSortedInventories.Contains(MatchingIndex)) return ItemIndexStatus.CantMove;

            if (MatchingIndex != index) return ItemIndexStatus.BelongsElsewhere;

            return ItemIndexStatus.Unknown;
        }

        public ItemSortInfo(uint trueItemId)
        {
            TrueItemId = trueItemId;
        }

        public bool Equals(ItemSortInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TrueItemId == other.TrueItemId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ItemSortInfo)obj);
        }

        public override int GetHashCode() => (int)TrueItemId;

        public override string ToString() => Name;
    }

    [Flags]
    public enum ItemIndexStatus
    {
        BelongsElsewhere = 1 << 0,
        BelongsInCurrentIndex = 1 << 1,
        CantMove = 1 << 2,
        Unknown = 1 << 3,
        DontMove = BelongsInCurrentIndex | CantMove | Unknown
    }

    public static class IndexExtensions
    {
        public static bool ShouldMove(this ItemIndexStatus indexStatus) => (indexStatus & ItemIndexStatus.DontMove) == 0;
    }
}