using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.Retainers
{
    public static class HelperFunctions
    {
        public static readonly InventoryBagId[] PlayerInventoryBagIds = new InventoryBagId[6]
        {
            InventoryBagId.Bag1,
            InventoryBagId.Bag2,
            InventoryBagId.Bag3,
            InventoryBagId.Bag4,
            InventoryBagId.Bag5,
            InventoryBagId.Bag6
        };


        public static readonly InventoryBagId[] RetainerBagIds =
        {
            InventoryBagId.Retainer_Page1, InventoryBagId.Retainer_Page2, InventoryBagId.Retainer_Page3,
            InventoryBagId.Retainer_Page4, InventoryBagId.Retainer_Page5, InventoryBagId.Retainer_Page6,
            InventoryBagId.Retainer_Page7
        };

        public const InventoryBagId RetainerGilId = InventoryBagId.Retainer_Gil;

        public const InventoryBagId PlayerGilId = InventoryBagId.Currency;

        private static readonly uint GilItemId = DataManager.GetItem("Gil").Id;// 1;
        
        public static int UnixTimestamp => (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


        public static bool FilterStackable(BagSlot item)
        {
            if (item.IsCollectable)
                return false;

            if (item.Item.StackSize < 2)
                return false;

            if (item.Count == item.Item.StackSize)
                return false;

            return true;
        }

        public static uint NormalRawId(uint trueItemId)
        {
            if (trueItemId > 1000000U)
                return trueItemId - 1000000U;

            return trueItemId;
        }

        public static bool MoveItem(BagSlot fromBagSlot, BagSlot toBagSlot)
        {
            return fromBagSlot.Count + toBagSlot.Count <= toBagSlot.Item.StackSize && fromBagSlot.Move(toBagSlot);
        }

        public static int GetNumberOfRetainersLua()
        {
            var bell = GetBellLuaString();
            var numOfRetainers = 0;

            if (bell.Length > 0)
                numOfRetainers = Lua.GetReturnVal<int>($"return _G['{bell}']:GetRetainerEmployedCount();");

            return numOfRetainers;
        }

        public static string GetRetainerName()
        {
            if (GetBellLuaString().Length > 0 && CanGetName())
                return Lua.GetReturnVal<string>($"return _G['{GetBellLuaString()}']:GetRetainerName();");

            return "";
        }

        public static string GetBellLuaString()
        {
            var func = "local values = '' for key,value in pairs(_G) do if string.match(key, '{0}:') then return key;   end end return values;";
            var searchString = "CmnDefRetainerBell";
            var bell = Lua.GetReturnVal<string>(string.Format(func, searchString)).Trim();

            return bell;
        }

        private static bool CanGetName()
        {
            return RaptureAtkUnitManager.GetWindowByName("InventoryRetainer") != null ||
                   RaptureAtkUnitManager.GetWindowByName("InventoryRetainerLarge") != null || SelectString.IsOpen;
        }

        public static GameObject NearestSummoningBell()
        {
            var list = GameObjectManager.GameObjects
                .Where(r => r.Name == Translator.SummoningBell)
                .OrderBy(j => j.Distance())
                .ToList();

            if (list.Count <= 0)
            {
                LogCritical("No Summoning Bell Found");
                return null;
            }

            var bell = list[0];

            LogCritical($"Found nearest bell: {bell} Distance: {bell.Distance2D(Core.Me.Location)}");

            return bell;
        }

        private static void LogCritical(string text, params object[] args)
        {
            var msg = $"[Helpers] {text}";
            Logging.Write(Colors.OrangeRed, msg);
        }

        private static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, text);
        }

        public static IEnumerable<BagSlot> GetPlayerStackableBagSlots()
        {
            return InventoryManager.FilledSlots.Where(x => x.BagId == InventoryBagId.Bag1 || x.BagId == InventoryBagId.Bag2 || x.BagId == InventoryBagId.Bag3 || x.BagId == InventoryBagId.Bag4).Where(FilterStackable);
        }

        public static bool GetRetainerGil()
        {
            var playerGilSlot = InventoryManager.GetBagByInventoryBagId(PlayerGilId).Where(r => r.IsFilled).FirstOrDefault(item => item.RawItemId == GilItemId);
            var retainerGilSlot = InventoryManager.GetBagByInventoryBagId(RetainerGilId).Where(r => r.IsFilled).FirstOrDefault(item => item.RawItemId == GilItemId);

            if (retainerGilSlot == null || playerGilSlot == null || retainerGilSlot.Count <= 0) return false;

            LogCritical($"Retainer: {retainerGilSlot.Count:n0}  Player: {playerGilSlot.Count:n0}");

            return retainerGilSlot.Move(playerGilSlot);
        }

        public static List<BagSlot> GetItemsByCategory()
        {
            var result = new List<BagSlot>();
            foreach (var item in InventoryManager.FilledSlots.Where(x =>
                x.BagId == InventoryBagId.Bag1 || x.BagId == InventoryBagId.Bag2 ||
                x.BagId == InventoryBagId.Bag3 || x.BagId == InventoryBagId.Bag4))
            {
                string test = $"Name: {item.Item.CurrentLocaleName}\tCategory {item.Item.EquipmentCatagory}";
                //Log(test);
            }
            
            foreach (var item in InventoryManager.FilledArmorySlots.Where(i=> i.BagId == InventoryBagId.Armory_MainHand))
            {
                string test = $"Name: {item.Item.CurrentLocaleName}\tCategory {item.Item.EquipmentCatagory}";
                //Log(test);
            }

            foreach (var action in ActionManager.CurrentActions)
            {
               // Log(string.Format("{0} - {1}",action.Key, action.Value.Name));
            }

            //ActionManager.CurrentActions
            return result;
        }

        public static bool FilterByCategory(BagSlot item, ItemUiCategory category)
        {
            return (item.Item.EquipmentCatagory == category);
        }

        internal static async Task<bool> UseSummoningBell()
        {
            var bell = NearestSummoningBell();

            if (bell == null)
            {
                LogCritical("No summoning bell near by");
                return false;
            }

            if (bell.Distance2D(Core.Me.Location) >= 3)
            {
                await MoveSummoningBell(bell.Location);
                if (bell.Distance2D(Core.Me.Location) >= 3) return false;
            }

            bell.Interact();
            await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            return true;
        }
        
        internal static async Task<bool> MoveSummoningBell(Vector3 loc)
        {
            var moving = MoveResult.GeneratingPath;
            while (!(moving == MoveResult.Done ||
                     moving == MoveResult.ReachedDestination ||
                     moving == MoveResult.Failed ||
                     moving == MoveResult.Failure ||
                     moving == MoveResult.PathGenerationFailed))
            {
                moving = Flightor.MoveTo(new FlyToParameters(loc));

                await Coroutine.Yield();
            }

            return true;
        }

        private static void Log(string test)
        {
            var msg = string.Format("[Helper] " + test);
            Logging.Write(Colors.Pink, msg);
        }

        internal static async Task<bool> VerifiedRetainerData2()
        {
            if (Core.Memory.Read<uint>(Offsets.RetainerData) != 0)
            {
                return true;
            }
            AgentContentsInfo.Instance.Toggle();
            await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName("ContentsInfo") != null);
            await Coroutine.Sleep(500);
            await Coroutine.Wait(3000, () => Core.Memory.Read<uint>(Offsets.RetainerData) != 0);
            //AgentContentsInfo.Instance.Toggle();
            RaptureAtkUnitManager.GetWindowByName("ContentsInfo").SendAction(1, 3uL, 4294967295uL);
            return Core.Memory.Read<uint>(Offsets.RetainerData) != 0;
        }
        
        internal static async Task<bool> VerifiedRetainerData()
        {
            if (Core.Memory.Read<uint>(Offsets.RetainerData) != 0)
            {
                return true;
            }
            RequestRetainerData();
            await Coroutine.Wait(3000, () => Core.Memory.Read<uint>(Offsets.RetainerData) != 0);
            return Core.Memory.Read<uint>(Offsets.RetainerData) != 0;
        }

        public static async Task<int> GetNumberOfRetainers()
        {
            var verified = await VerifiedRetainerData();
            if (!verified)
                return 0;

            lock (Core.Memory.Executor.AssemblyLock)
                return Core.Memory.CallInjected64<int>(Offsets.GetNumberOfRetainers,
                                                       Offsets.RetainerData);
        }

        public static string GetRetainerName(int index)
        {
            IntPtr retainerPtr;
            lock (Core.Memory.Executor.AssemblyLock)
                retainerPtr = Core.Memory.CallInjected64<IntPtr>(Offsets.GetRetainerPointer,
                                                                 Offsets.RetainerData,
                                                                 index);
            if (Core.Memory.Read<uint>(retainerPtr) == 0)
                return "";
    
            return Core.Memory.ReadString(retainerPtr + Offsets.RetainerName, Encoding.UTF8);
        }

        public static async Task<List<KeyValuePair<int, uint>>> GetVentureFinishTimes()
        {
            
            IntPtr retainerPtr;
            List<KeyValuePair<int, uint>> results = new List<KeyValuePair<int, uint>>();
            var verified = await VerifiedRetainerData();
            if (! verified)
                return results;
            
            var numRetainers = await GetNumberOfRetainers();
            for (int i = 0; i < numRetainers; i++)
            {
                lock (Core.Memory.Executor.AssemblyLock)
                    retainerPtr = Core.Memory.CallInjected64<IntPtr>(Offsets.GetRetainerPointer,
                                                                     Offsets.RetainerData,
                                                                     i);
                if (Core.Memory.Read<uint>(retainerPtr) == 0)
                    break;
                
                var task = Core.Memory.Read<uint>(retainerPtr + Offsets.VentureTask);

                if (task == 0)
                {
                    continue;
                }

                var finish = Core.Memory.Read<uint>(retainerPtr + Offsets.VentureFinishTime);
                results.Add(new KeyValuePair<int, uint>(i, finish));
                
                /*var timeLeft = finish - unixTimestamp;
    
                if (timeLeft < 0)
                {
                    Log("\tVenture Complete");
                }
                else
                {
                    Log("\t" + timeLeft/60 + " Minutes Left");
                }*/
            }

            return results;
        }

        public static void RequestRetainerData()
        {
            lock (Core.Memory.Executor.AssemblyLock)
                Core.Memory.CallInjected64<IntPtr>(Offsets.RequestRetainerData,
                                                   Offsets.RetainerNetworkPacket,
                                                   0,
                                                   0,
                                                   0,
                                                   0);
        }
    }
}