using System.Linq;
using ff14bot.Managers;

namespace LlamaLibrary.ScriptConditions
{
    public static class Helpers
    {
        private static readonly uint[] idList =
        {
            28725,28733,28741,28749,28757,29792,29800,29808,29816,29824,29832,28726,28734,28742,28750,28758,29793,29801,29809,29817,29825,29833,28727,28735,28743,28751,28759,29794,29802,29810,29818,29826,29834,28728,28736,28744,28752,28760,29795,29803,29811,29819,29827,29835,28729,28737,28745,28753,28761,29796,29804,29812,29820,29828,29836,28730,28738,28746,28754,28762,29797,29805,29813,29821,29829,29837,28731,28739,28747,28755,28763,29798,29806,29814,29822,29830,29838,28732,28740,28748,28756,28764,29799,29807,29815,29823,29831,
            29839
        };

        public static int HasIshgardItem()
        {
            return InventoryManager.FilledSlots.Count(i => idList.Contains(i.RawItemId) && i.IsCollectable && i.Collectability > 50);
        }

        public static bool LLHasItemNQ(int itemID)
        {
            return InventoryManager.FilledSlots.Count(i => i.RawItemId == itemID && i.IsHighQuality == false) > 1;
        }

        public static bool LLHasItemHQ(int itemID)
        {
            return InventoryManager.FilledSlots.Count(i => i.RawItemId == itemID && i.IsHighQuality) > 1;
        }

        public static int GetSkybuilderScrips()
        {
            return (int) SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency) 28063);
        }

        public static int AverageItemLevel()
        {
            return InventoryManager.EquippedItems.Where(k => k.IsFilled).Sum(i => i.Item.ItemLevel) / InventoryManager.EquippedItems.Count(k => k.IsFilled);
        }
    }
}