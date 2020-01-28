using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Clio.Utilities;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace LlamaLibrary.Helpers
{

    public static class Lisbeth
    {
        private static object _lisbeth;
        private static MethodInfo _orderMethod;
        private static MethodInfo _travelMethod;
        
        static Lisbeth()
        {
            FindLisbeth();
        }

        internal static void FindLisbeth()
        {
            var loader = BotManager.Bots
                .FirstOrDefault(c => c.Name == "Lisbeth");

            if (loader == null) return;

            var lisbethObjectProperty = loader.GetType().GetProperty("Lisbeth");
            var lisbeth = lisbethObjectProperty?.GetValue(loader);
            var orderMethod = lisbeth?.GetType().GetMethod("ExecuteOrders");
            var travelMethod = lisbeth?.GetType().GetMethod("TravelTo");
            if (lisbeth == null || orderMethod == null) return;

            _orderMethod = orderMethod;
            _lisbeth = lisbeth;
            _travelMethod = travelMethod;

            Logging.Write("Lisbeth found.");
        }

        internal static async Task<bool> ExecuteOrders(string json)
        {
            if (_orderMethod != null) return await (Task<bool>) _orderMethod.Invoke(_lisbeth, new object[] {json});
            
            FindLisbeth();
            if (_orderMethod == null)
                return false;

            return await (Task<bool>) _orderMethod.Invoke(_lisbeth, new object[] {json});
        }

        internal static async Task<bool> TravelTo(string area, Vector3 position)
        {
            if (_travelMethod != null) return await (Task<bool>) _travelMethod.Invoke(_lisbeth, new object[] {area, position});
            
            FindLisbeth();
            if (_travelMethod == null)
                return false;

            return await (Task<bool>) _travelMethod.Invoke(_lisbeth, new object[] {area, position});
        }
    }
}