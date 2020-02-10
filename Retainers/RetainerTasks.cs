using Buddy.Coroutines;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.Retainers
{
    public class RetainerTasks
    {
        public static bool IsOpen => SelectString.IsOpen;

        public static bool OpenInventory()
        {
            if (IsOpen)
            {
                SelectString.ClickSlot(0);
                //await Coroutine.Wait(5000, IsInventoryOpen);
                //return IsInventoryOpen();
                return true;
            }
            Logging.Write("Retainer task window not open");
            return false;

        }

        public static bool CloseInventory()
        {
            if (!IsInventoryOpen()) return true;

            if (RaptureAtkUnitManager.GetWindowByName("InventoryRetainer") != null)
            {
                RaptureAtkUnitManager.GetWindowByName("InventoryRetainer").SendAction(1, 3, (ulong) uint.MaxValue);
                return true;
            }

            if (RaptureAtkUnitManager.GetWindowByName("InventoryRetainerLarge") != null)
            {
                RaptureAtkUnitManager.GetWindowByName("InventoryRetainerLarge").SendAction(1, 3, (ulong) uint.MaxValue);
                return true;
            }

            return false;
        }

        public static bool CloseTasks()
        {
            return !IsOpen || SelectString.ClickLineEquals(RetainerTaskStrings.Quit);
        }

        public static bool IsInventoryOpen()
        {
            return RaptureAtkUnitManager.GetWindowByName("InventoryRetainer") != null ||
                   RaptureAtkUnitManager.GetWindowByName("InventoryRetainerLarge") != null;
        }

        internal static class RetainerTaskStrings
        {
            //For partial string searches use SelectIconString.ClickLineContains(string) and not Equals
            internal static string Inventory = "Entrust or withdraw items.";
            internal static string Gil = "Entrust or withdraw gil.";
            internal static string SellYourInventory = "Sell items in your inventory on the market";
            internal static string SellRetainerInventory = "Sell items in your retainer's inventory on the market.";
            internal static string SaleHistory = "View sale history.";
            internal static string ViewVentureReport = "View venture report."; //Use Partial Search
            internal static string AssignVenture = "Assign venture."; //Use Partial Search since it adds (Complete) or (In Progress)
            internal static string ViewGear = "View retainer attributes and gear.";
            internal static string ResetClass = "Reset retainer class."; //Use Partial Search
            internal static string Quit = "Quit.";
        }
    }
}