using System;
using System.Collections.Generic;
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
        public static Func<string> _getCurrentAreaName;
        private static Func<Task> _stopGentlyAndWait, _equipOptimalGear, _extractMateria, _selfRepair, _selfRepairWithMenderFallback;
        private static Action _stopGently;
        private static Action<string, Func<Task>> _addHook;
        private static Action<string> _removeHook;
        private static Func<List<string>> _getHookList;

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
            var apiObject = lisbeth.GetType().GetProperty("Api")?.GetValue(lisbeth);
            if (lisbeth == null || orderMethod == null) return;
            if (apiObject != null)
            {
                var m = apiObject.GetType().GetMethod("GetCurrentAreaName");
                if (m != null)
                {
                    _getCurrentAreaName = (Func<string>) Delegate.CreateDelegate(typeof(Func<string>), apiObject, "GetCurrentAreaName");
                    _stopGently = (Action) Delegate.CreateDelegate(typeof(Action), apiObject, "StopGently");
                    _stopGentlyAndWait = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGentlyAndWait");
                    _addHook = (Action<string, Func<Task>>) Delegate.CreateDelegate(typeof(Action<string, Func<Task>>), apiObject, "AddHook");
                    _removeHook = (Action<string>) Delegate.CreateDelegate(typeof(Action<string>), apiObject, "RemoveHook");
                    _getHookList = (Func<List<string>>) Delegate.CreateDelegate(typeof(Func<List<string>>), apiObject, "GetHookList");

                    _equipOptimalGear = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "EquipOptimalGear");
                    _extractMateria = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "ExtractMateria");
                    _selfRepair = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "SelfRepair");
                    _selfRepairWithMenderFallback = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "SelfRepairWithMenderFallback");
                }
            }

            _orderMethod = orderMethod;
            _lisbeth = lisbeth;
            _travelMethod = travelMethod;

            Logging.Write("Lisbeth found.");
        }

        public static string GetCurrentAreaName => _getCurrentAreaName.Invoke();

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
        
        public static void StopGently()
        {
            _stopGently?.Invoke();
        }

        public static async Task StopGentlyAndWait()
        {
            if (_stopGentlyAndWait == null) { return; }
            await _stopGentlyAndWait();
        }

        public static void AddHook(string name, Func<Task> function)
        {
            _addHook?.Invoke(name, function);
        }

        public static void RemoveHook(string name)
        {
            _removeHook?.Invoke(name);
        }

        public static List<string> GetHookList()
        {
            return _getHookList?.Invoke();
        }

        public static async Task EquipOptimalGear()
        {
            await _equipOptimalGear();
        }

        public static async Task ExtractMateria()
        {
            await _extractMateria();
        }

        public static async Task SelfRepair()
        {
            await _selfRepair();
        }

        public static async Task SelfRepairWithMenderFallback()
        {
            await _selfRepairWithMenderFallback();
        }
    }
}