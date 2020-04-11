using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteWindows
{
    public class ShopExchangeItem: RemoteWindow<ShopExchangeItem>
    {
        private const string WindowName = "ShopExchangeItem";

        public ShopExchangeItem() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public async Task<uint> Purchase(uint itemId, uint itemCount = 1)
        {
            if (!IsOpen) return 0u;

            var items = SpecialShopManager.Items;

            var specialShopItem = items?.Cast<SpecialShopItem?>().FirstOrDefault(i => i.HasValue && i.Value.ItemIds.Contains(itemId));

            if (!specialShopItem.HasValue) return 0u;

            if (itemCount > specialShopItem.Value.Item0.StackSize) itemCount = specialShopItem.Value.Item0.StackSize;

            if (!CanAfford(specialShopItem.Value))
                return 0;
            
            var index = items.IndexOf(specialShopItem.Value);
            var obj = new ulong[8]
            {
                3uL,
                0uL,
                3uL,
                0uL,
                3uL,
                0uL,
                0uL,
                0uL
            };
            obj[3] = (uint) index;
            obj[5] = itemCount;
            SendAction(4, obj);

            await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName("ShopExchangeItemDialog") != null);

            if (RaptureAtkUnitManager.GetWindowByName("ShopExchangeItemDialog") != null)
            {
                RaptureAtkUnitManager.GetWindowByName("ShopExchangeItemDialog").SendAction(1,3,0);
                await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName("ShopExchangeItemDialog") == null);
                
                await Coroutine.Wait(2000, () => SelectYesno.IsOpen);

                if (SelectYesno.IsOpen)
                {
                    SelectYesno.Yes();
                    await Coroutine.Wait(2000, () => !SelectYesno.IsOpen);
                    
                    await Coroutine.Wait(2000, () => Request.IsOpen);

                    if (Request.IsOpen)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var item = InventoryManager.FilledInventoryAndArmory.FirstOrDefault(j => j.RawItemId == specialShopItem.Value.CurrencyTypes[i] && j.Count >= specialShopItem.Value.CurrencyCosts[i]);

                            if (item != null)
                            {
                                item.Handover();
                                await Coroutine.Sleep(500);
                            }
                        }
                        if (Request.HandOverButtonClickable)
                            Request.HandOver();
                        
                        await Coroutine.Sleep(500);
                    }
                }
            }
            
            
            
            return itemCount;
        }

        private static bool CanAfford(SpecialShopItem item)
        {
            for (int i = 0; i < 3; i++)
            {
                if (item.CurrencyCosts[i] == 0)
                {
                    continue;
                }

                if (!InventoryManager.FilledInventoryAndArmory.Any(j => j.RawItemId == item.CurrencyTypes[i] && j.Count >= item.CurrencyCosts[i]))
                    return false;
            }
            return true;
        }
    }
}