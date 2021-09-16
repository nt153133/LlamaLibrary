using System;
using System.Collections.Generic;
using System.Linq;
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
                    catch (Exception ex)
                    {
                        AutoRetainerSort.LogCritical($"Error! Couldn't get item data for ID {RawItemId.ToString()}.");
                        throw new ArgumentException($"Unable to get ItemInfo. TrueId: {TrueItemId}, RawId: {RawItemId}", "ItemInfo", ex);
                    }
                }
                // Check again to make sure it's not still null.
                if (_itemInfo == null)
                {
                    AutoRetainerSort.LogCritical($"Error! Couldn't get item data for ID {RawItemId.ToString()}.");
                    throw new ArgumentException($"Unable to get ItemInfo. TrueId: {TrueItemId}, RawId: {RawItemId}", "ItemInfo");
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

        public const int CollectableOffset = 500_000;
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

        private bool _checkedIndexes;

        private int[] _matchingIndexes = Array.Empty<int>();

        public int[] MatchingIndexes
        {
            get
            {
                // Special unique-only loop first. Don't cache the result because it might end up in a different inventory due to uniques.
                if (ItemInfo.Unique)
                {
                    var indexList = new HashSet<int>();
                    foreach (var sortInfoPair in AutoRetainerSortSettings.Instance.InventoryOptions)
                    {
                        if (sortInfoPair.Value.ContainsId(TrueItemId))
                        {
                            indexList.Add(sortInfoPair.Key);
                        }
                    }
                    
                    // OrderByDescending so we place any uniques in Player Inventory last.
                    return indexList.Any() ? indexList.OrderByDescending(x => x).ToArray() : Array.Empty<int>();
                }

                // Second loop for finding by SortType and ID both. Cache this result. We don't care about ordering here.
                if (!_checkedIndexes && _matchingIndexes.Length == 0)
                {
                    var indexList = new HashSet<int>();

                    foreach (var sortInfoPair in AutoRetainerSortSettings.Instance.InventoryOptions)
                    {
                        if (sortInfoPair.Value.ContainsType(SortType))
                        {
                            indexList.Add(sortInfoPair.Key);
                        }
                        if (sortInfoPair.Value.ContainsId(TrueItemId))
                        {
                            indexList.Add(sortInfoPair.Key);
                            break;
                        }
                    }

                    _matchingIndexes = indexList.Any() ? indexList.OrderByDescending(x => x).ToArray() : Array.Empty<int>();
                    _checkedIndexes = true;
                }

                return _matchingIndexes;
            }
        }

        public ItemIndexStatus IndexStatus(int index)
        {
            int[] localMatchCache = MatchingIndexes.ToArray();

            if (localMatchCache.Length == 0) return ItemIndexStatus.NoMatchingIndex;

            if (ItemSortStatus.FilledAndSortedInventories.Contains(index)) return ItemIndexStatus.NoSpaceInIndex;

            if (localMatchCache.All(x => ItemSortStatus.FilledAndSortedInventories.Contains(x))) return ItemIndexStatus.NoSpaceInAnyIndex;
            
            if (localMatchCache.Length == 1 && localMatchCache[0] == index) return ItemIndexStatus.BelongsInIndex;

            if (localMatchCache.Length > 1 && localMatchCache.Any(x => x == index)) return ItemIndexStatus.BelongsInIndexAndOthers;

            if (ItemInfo.Unique)
            {
                if (localMatchCache.All(x => ItemSortStatus.GetByIndex(x).ItemCounts.ContainsKey(TrueItemId))) return ItemIndexStatus.UniqueButNoSpace;
                if (localMatchCache.All(x => ItemSortStatus.GetByIndex(x).ItemCounts.ContainsKey(TrueItemId))) return ItemIndexStatus.UniqueButNoSpace;
            }

            if (localMatchCache.All(x => x != index && x != int.MinValue)) return ItemIndexStatus.BelongsElsewhere;

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
        BelongsInIndex = 1 << 1,
        BelongsInIndexAndOthers = BelongsInIndex | BelongsElsewhere,
        UniqueButNoSpace = 1 << 2,
        NoSpaceInAnyIndex = 1 << 3,
        NoSpaceInIndex = 1 << 4,
        NoMatchingIndex = 1 << 5,
        Unknown = 1 << 6,
    }

    public enum SortStatus
    {
        Unknown = 0,
        NoMatchingIndex = 1,
        BelongsInIndex = 2,
        MoveButUnable = 3,
        Move = 4
    }

    public static class IndexExtensions
    {
        public static SortStatus SortStatus(this ItemSortInfo sortInfo, int index)
        {
            switch (sortInfo.IndexStatus(index))
            {
                case ItemIndexStatus.NoMatchingIndex:
                    return Classes.SortStatus.NoMatchingIndex;

                case ItemIndexStatus.BelongsInIndexAndOthers:
                case ItemIndexStatus.BelongsInIndex:
                    return Classes.SortStatus.BelongsInIndex;

                case ItemIndexStatus.NoSpaceInAnyIndex:
                case ItemIndexStatus.NoSpaceInIndex:
                case ItemIndexStatus.UniqueButNoSpace:
                    return Classes.SortStatus.MoveButUnable;

                case ItemIndexStatus.BelongsElsewhere:
                    return Classes.SortStatus.Move;

                case ItemIndexStatus.Unknown:
                    return Classes.SortStatus.Unknown;

                default:
                    return Classes.SortStatus.Unknown;
            }
        }
    }
}