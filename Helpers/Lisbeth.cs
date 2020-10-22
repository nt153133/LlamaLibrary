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
        private static Func<Task> _stopGently;
        private static Action<string, Func<Task>> _addHook;
        private static Action<string> _removeHook;
        private static Func<List<string>> _getHookList;
        private static Func<Task<bool>> _exitCrafting;
        private static Func<string, Vector3, Task<bool>> _travelToWithArea;
        private static Func<uint, uint, Vector3, Task<bool>> _travelTo;
        private static Func<uint, Vector3, Task<bool>> _travelToWithoutSubzone;

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
            var apiObject = lisbeth.GetType().GetProperty("Api")?.GetValue(lisbeth);
            if (lisbeth == null || orderMethod == null) return;
            if (apiObject != null)
            {
                var m = apiObject.GetType().GetMethod("GetCurrentAreaName");
                if (m != null)
                {
                    try
                    {
                        _getCurrentAreaName = (Func<string>) Delegate.CreateDelegate(typeof(Func<string>), apiObject, "GetCurrentAreaName");
                        _stopGently = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGently");
                        //_stopGentlyAndWait = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGentlyAndWait");
                        _addHook = (Action<string, Func<Task>>) Delegate.CreateDelegate(typeof(Action<string, Func<Task>>), apiObject, "AddHook");
                        _removeHook = (Action<string>) Delegate.CreateDelegate(typeof(Action<string>), apiObject, "RemoveHook");
                        _getHookList = (Func<List<string>>) Delegate.CreateDelegate(typeof(Func<List<string>>), apiObject, "GetHookList");
                        _exitCrafting = (Func<Task<bool>>) Delegate.CreateDelegate(typeof(Func<Task<bool>>), apiObject, "ExitCrafting");
                        _equipOptimalGear = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "EquipOptimalGear");
                        _extractMateria = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "ExtractMateria");
                        _selfRepair = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "SelfRepair");
                        _selfRepairWithMenderFallback = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "SelfRepairWithMenderFallback");
                        _travelTo = (Func<uint, uint, Vector3, Task<bool>>) Delegate.CreateDelegate(typeof(Func<uint, uint, Vector3, Task<bool>>), apiObject, "TravelTo");
                        _travelToWithArea = (Func<string, Vector3, Task<bool>>) Delegate.CreateDelegate(typeof(Func<string, Vector3, Task<bool>>), apiObject, "TravelToWithArea");
                        _travelToWithoutSubzone = (Func<uint, Vector3, Task<bool>>) Delegate.CreateDelegate(typeof(Func<uint, Vector3, Task<bool>>), apiObject, "TravelToWithoutSubzone");
                    }
                    catch (Exception e)
                    {
                        Logger.LogCritical(e.ToString());
                    }
                }
            }

            _orderMethod = orderMethod;
            _lisbeth = lisbeth;
            //_travelMethod = travelMethod;

            Logging.Write("Lisbeth found.");
        }

        public static string GetCurrentAreaName => _getCurrentAreaName.Invoke();

        internal static async Task<bool> ExecuteOrders(string json)
        {
            if (_orderMethod != null) return await (Task<bool>) _orderMethod.Invoke(_lisbeth, new object[] {json, false});

            FindLisbeth();
            if (_orderMethod == null)
                return false;

            return await (Task<bool>) _orderMethod.Invoke(_lisbeth, new object[] {json, false});
        }
        
        internal static async Task<bool> ExecuteOrdersIgnoreHome(string json)
        {
            if (_orderMethod != null) return await (Task<bool>) _orderMethod.Invoke(_lisbeth, new object[] {json, true});

            FindLisbeth();
            if (_orderMethod == null)
                return false;

            return await (Task<bool>) _orderMethod.Invoke(_lisbeth, new object[] {json, true});
        }

        internal static async Task<bool> TravelTo(string area, Vector3 position)
        {
            if (_travelToWithArea != null) return await _travelToWithArea(area, position);

            FindLisbeth();
            if (_travelToWithArea == null)
                return false;

            return await _travelToWithArea(area, position);
        }
        
        public static async Task<bool> TravelToZones(uint zoneId, uint subzoneId, Vector3 position)
        {
            return await _travelTo(zoneId, subzoneId, position);
        }
        
        public static async Task<bool> TravelToZones(uint zoneId, Vector3 position)
        {
            return await _travelToWithoutSubzone(zoneId, position);
        }
        
        public static async Task StopGently()
        {
            await _stopGently();
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
        
        public static Task<bool> ExitCrafting()
        {
            return _exitCrafting?.Invoke();
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