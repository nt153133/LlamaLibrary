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
using ff14bot.NeoProfiles;
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

        static List<(uint, Vector3)> SummoningBells = new List<(uint, Vector3)>
        {
            (129, new Vector3(-223.743042f, 16.006714f, 41.306152f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
            (129, new Vector3(-266.376831f, 16.006714f, 41.275635f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
            (129, new Vector3(-149.279053f, 18.203979f, 20.553894f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
            (129, new Vector3(-123.888062f, 17.990356f, 21.469421f)), //Limsa Lominsa Lower Decks(Limsa Lominsa) 
            (131, new Vector3(148.91272f, 3.982544f, -44.205383f)), //Ul'dah - Steps of Thal(Ul'dah) 
            (131, new Vector3(111.161987f, 4.104675f, -72.343079f)), //Ul'dah - Steps of Thal(Ul'dah) 
            (131, new Vector3(153.185303f, 3.982544f, 13.229492f)), //Ul'dah - Steps of Thal(Ul'dah) 
            (131, new Vector3(118.547363f, 4.013123f, -93.003784f)), //Ul'dah - Steps of Thal(Ul'dah) 
            (133, new Vector3(160.234863f, 15.671021f, -55.649719f)), //Old Gridania(Gridania) 
            (133, new Vector3(169.726074f, 15.487854f, -81.895203f)), //Old Gridania(Gridania) 
            (133, new Vector3(171.007812f, 15.487854f, -101.487854f)), //Old Gridania(Gridania) 
            (133, new Vector3(160.234863f, 15.671021f, -136.369934f)), //Old Gridania(Gridania) 
            (156, new Vector3(34.50061f, 28.976807f, -762.233948f)), //Mor Dhona(Mor Dhona) 
            (156, new Vector3(11.001709f, 28.976807f, -734.554077f)), //Mor Dhona(Mor Dhona) 
            (419, new Vector3(-151.171204f, -12.64978f, -11.764771f)), //The Pillars(Ishgard) 
            (478, new Vector3(34.775269f, 208.148193f, -50.858398f)), //Idyllshire(Dravania) 
            (478, new Vector3(0.38147f, 206.469727f, 51.407593f)), //Idyllshire(Dravania) 
            (628, new Vector3(19.394226f, 4.043579f, 53.025024f)), //Kugane(Kugane) 
            (635, new Vector3(-57.633362f, -0.01532f, 49.30188f)), //Rhalgr's Reach(Gyr Abania) 
            (819, new Vector3(-69.840576f, -7.705872f, 123.491211f)), //The Crystarium(The Crystarium) 
            (819, new Vector3(-64.255798f, 19.97406f, -144.274109f)), //The Crystarium(The Crystarium) 
            (820, new Vector3(7.186951f, 83.17688f, 31.448853f)) //Eulmore(Eulmore) 
        };

        private static readonly uint GilItemId = DataManager.GetItem("Gil").Id; // 1;

        public static int UnixTimestamp => (Int32) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


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
                .OrderBy(j => j.DistanceSqr())
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
            var msg = $"[Helpers] {text}";
            Logging.Write(Colors.OrangeRed, msg);
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

            foreach (var item in InventoryManager.FilledArmorySlots.Where(i => i.BagId == InventoryBagId.Armory_MainHand))
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
            await GoToSummoningBell();
            var bell = FindSummoningBell();

            if (bell == null || !bell.IsWithinInteractRange)
            {
                LogCritical("No summoning bell near by");
                return false;
            }

            bell.Interact();
            await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            return true;
        }

        public static async Task<bool> GoToSummoningBell()
        {
            var searchBell = FindSummoningBell();
            if (searchBell != null)
            {
                if (searchBell.IsWithinInteractRange)
                {
                    Log($"Found bell in Interact Range");
                    return true;
                }

                if (await Navigation.GetTo(WorldManager.ZoneId, searchBell.Location))
                {
                    Log($"Used Navgraph/Flightor to get there");
                    if (searchBell.IsWithinInteractRange)
                        return true;
                }
            }

            (uint, Vector3) bellLocation;
            int tries = 0;
            if (SummoningBells.Any(i => i.Item1 == WorldManager.ZoneId))
            {
                Log($"Found a bell in our zone");
                bellLocation = SummoningBells.Where(i => i.Item1 == WorldManager.ZoneId).OrderBy(r => Core.Me.Location.DistanceSqr(r.Item2)).First();
            }
            else
            {
                bool foundBell = false;
                Random rand = new Random();
                do
                {
                    tries++;
                    var index = rand.Next(0, SummoningBells.Count);
                    bellLocation = SummoningBells[index];
                    var ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.ZoneId == bellLocation.Item1 && i.IsAetheryte);

                    if (ae == default(AetheryteResult))
                    {
                        switch (bellLocation.Item1)
                        {
                            case 131:
                                ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == 9);
                                break;
                            case 133:
                                ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == 2);
                                break;
                            case 419:
                                ae = DataManager.AetheryteCache.Values.FirstOrDefault(i => i.Id == 70);
                                break;
                        }
                    }

                    if (ae != default(AetheryteResult))
                    {
                        if (ConditionParser.HasAetheryte(ae.Id))
                        {
                            Log($"{bellLocation.Item1} can get to ConditionParser.HasAetheryte({ae.Id}) = {ConditionParser.HasAetheryte(ae.Id)} {ae.EnglishName}");
                            foundBell = true;
                        }
                    }
                    else
                    {
                        Log($"{bellLocation.Item1} can't find AE");
                    }
                }
                while (!foundBell && tries < 5);
            }

            Log($"Going to bell {bellLocation.Item1} {bellLocation.Item2}");
            if (await Navigation.GetTo(bellLocation.Item1, bellLocation.Item2))
            {
                var bell = FindSummoningBell();
                Log(bell != null ? $"{bell.Name} {bell.Location} {WorldManager.CurrentZoneName} {bell.IsWithinInteractRange}" : $"Couldn't find bell at {bellLocation.Item2} {bellLocation.Item1}");
                return bell != null;
            }
            else
            {
                return false;
            }
        }

        //Log("Name:{0}, Location:{1} {2}", unit, unit.Location,WorldManager.CurrentZoneName);
        public static GameObject FindSummoningBell()
        {
            uint[] bellIds = {2000072, 2000401, 2000403, 2000439, 2000441, 2000661, 2001271, 2001358, 2006565, 2010284, 196630};
            return GameObjectManager.GameObjects.Where(i => i.IsVisible && bellIds.Contains(i.NpcId)).OrderBy(r => r.DistanceSqr()).FirstOrDefault();
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
            if (!verified)
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