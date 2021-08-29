using System.Text;
using ff14bot.Managers;

namespace LlamaLibrary.AutoRetainerSort.Classes
{
    public class ItemSortInfo
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

        public uint TrueItemId;
        
        public const int CollectableOffset = 5_000_000;
        private const int QualityOffset = 1_000_000;

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

        public bool BelongsInIndex(int index)
        {
            if (MatchingIndex == int.MinValue) return true;

            if (ItemSortStatus.FilledAndSortedInventories.Contains(index)) return false;

            return MatchingIndex == index;
        }

        public ItemSortInfo(uint trueItemId)
        {
            TrueItemId = trueItemId;
        }
    }
}