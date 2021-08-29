using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Helpers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Structs;

namespace LlamaLibrary.RemoteWindows
{
    public class FreeCompanyExchange: RemoteWindow<FreeCompanyExchange>
    {
        private const string WindowName = "FreeCompanyExchange";
        
        public static List<FcActionShopItem> FcShopActions = new List<FcActionShopItem>()
        {
            new FcActionShopItem(1, 5, 3010, 0, "The Heat of Battle"),
            new FcActionShopItem(2, 5, 3010, 1, "Earth and Water"),
            new FcActionShopItem(3, 5, 3010, 2, "Helping Hand"),
            new FcActionShopItem(4, 5, 2408, 3, "A Man's Best Friend"),
            new FcActionShopItem(5, 5, 2408, 4, "Mark Up"),
            new FcActionShopItem(6, 5, 2408, 5, "Seal Sweetener"),
            new FcActionShopItem(17, 5, 2408, 6, "Jackpot"),
            new FcActionShopItem(7, 5, 1505, 7, "Brave New World"),
            new FcActionShopItem(8, 5, 2408, 8, "Live off the Land"),
            new FcActionShopItem(9, 5, 2408, 9, "What You See"),
            new FcActionShopItem(10, 5, 2408, 10, "Eat from the Hand"),
            new FcActionShopItem(11, 5, 2408, 11, "In Control"),
            new FcActionShopItem(12, 5, 3010, 12, "That Which Binds Us"),
            new FcActionShopItem(13, 5, 2408, 13, "Meat and Mead"),
            new FcActionShopItem(14, 5, 3010, 14, "Proper Care"),
            new FcActionShopItem(15, 5, 2408, 15, "Back on Your Feet"),
            new FcActionShopItem(16, 5, 3010, 16, "Reduced Rates"),
            new FcActionShopItem(31, 8, 6020, 17, "The Heat of Battle II"),
            new FcActionShopItem(32, 8, 6020, 18, "Earth and Water II"),
            new FcActionShopItem(33, 8, 6020, 19, "Helping Hand II"),
            new FcActionShopItem(34, 8, 6020, 20, "A Man's Best Friend II"),
            new FcActionShopItem(35, 8, 6020, 21, "Mark Up II"),
            new FcActionShopItem(36, 8, 6020, 22, "Seal Sweetener II"),
            new FcActionShopItem(47, 8, 6020, 23, "Jackpot II"),
            new FcActionShopItem(37, 8, 3010, 24, "Brave New World II"),
            new FcActionShopItem(38, 8, 6020, 25, "Live off the Land II"),
            new FcActionShopItem(39, 8, 6020, 26, "What You See II"),
            new FcActionShopItem(40, 8, 6020, 27, "Eat from the Hand II"),
            new FcActionShopItem(41, 8, 6020, 28, "In Control II"),
            new FcActionShopItem(42, 8, 6020, 29, "That Which Binds Us II"),
            new FcActionShopItem(43, 8, 6020, 30, "Meat and Mead II"),
            new FcActionShopItem(44, 8, 6020, 31, "Proper Care II"),
            new FcActionShopItem(45, 8, 4816, 32, "Back on Your Feet II"),
            new FcActionShopItem(46, 8, 6020, 33, "Reduced Rates II")
        };


        public FreeCompanyExchange() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public async Task<bool> BuyAction(int actionId)
        {
            if (IsOpen)
            {
                Logging.Write($"Buying {FcShopActions.First(i=> i.ActionId == actionId).Name}");
                ClickItem(FcShopActions.First(i=> i.ActionId == actionId).ShopIndex);
                //Logging.Write("Waiting for Yes/No");
                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                if (SelectYesno.IsOpen)
                {
                    //Logging.Write("Yes/No Open");
                    SelectYesno.Yes();
                    await Coroutine.Wait(5000, () => !SelectYesno.IsOpen);
                    await Coroutine.Sleep(500);
                }
            }
            return true;
        }
        
        public void ClickItem(int index)
        {
            if (index >= 0)
            {
                //Logging.Write("Send Action");
                SendAction(2, 3, 2, 4, (ulong) index);
            }
        }
    }
}