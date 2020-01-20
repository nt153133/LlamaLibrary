using System.Linq;
using ff14bot.Managers;

namespace LlamaLibrary.ScriptConditions
{
    public static class Helpers
    {
        private static readonly uint[] idList =
        {
            28725, 28726, 28727, 28728, 28729, 28730, 28731, 28732, 28733, 28734, 28735, 28736, 28737, 28738, 28739, 28740, 28741, 28742, 28743, 28744, 28745, 28746, 28747, 28748, 28749, 28750, 28751, 28752, 28753, 28754, 28755, 28756,
            28757,
            28758, 28759, 28760, 28761, 28762, 28763, 28764
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
    }
}