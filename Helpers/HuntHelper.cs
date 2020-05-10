using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteWindows;
using Newtonsoft.Json;

namespace LlamaLibrary.Helpers
{
    public class HuntHelper
    {
        private const int MaxOrderTypes = 0xE;

        public static readonly SortedDictionary<int, StoredHuntLocation> DailyHunts;

        public static readonly List<HuntBoardNpc> HuntBoards = new List<HuntBoardNpc>()
        {
            new HuntBoardNpc(2004438, 128, new Vector3(94.346436f, 40.238037f, 60.471436f), new uint[] {0, 4}), //Hunt Board  Limsa Lominsa Upper Decks(Limsa Lominsa) 
            new HuntBoardNpc(2004440, 130, new Vector3(-152.361328f, 4.226685f, -92.362915f), new uint[] {0, 4}), //Hunt Board  Ul'dah - Steps of Nald(Ul'dah) 
            new HuntBoardNpc(2004439, 132, new Vector3(-78.019409f, -0.503601f, 2.120972f), new uint[] {0, 4}), //Hunt Board  New Gridania(Gridania) 
            new HuntBoardNpc(2005909, 418, new Vector3(73.899414f, 24.307495f, 22.049255f), new uint[] {1, 2, 3, 5}), //Clan Hunt Board  Foundation(Ishgard) 
            new HuntBoardNpc(2008655, 628, new Vector3(-31.540405f, 0.076233f, -43.86969f), new uint[] {6, 7, 8, 9}), //Clan Hunt Board  Kugane(Kugane) 
            new HuntBoardNpc(2008654, 635, new Vector3(95.078857f, 0.62561f, 22.598572f), new uint[] {6, 7, 8, 9}), //Clan Hunt Board  Rhalgr's Reach(Gyr Abania) 
            new HuntBoardNpc(2010340, 819, new Vector3(-83.604248f, -0.01532f, -90.745422f), new uint[] {10, 11, 12, 13}) //Nuts Board  The Crystarium(The Crystarium) };
        };

        private static class Offsets
        {
            [Offset("Search 89 84 2A ?? ?? ?? ?? 41 0F B6 D6 Add 3 Read32")]
            internal static int AcceptedHuntBitfieldOffset;

            [Offset("Search 48 89 5C 24 ?? 56 48 83 EC ?? 40 32 F6")]
            internal static IntPtr CheckMobBoardUnlocked;

            [Offset("Search E8 ?? ?? ?? ?? 8B 57 ?? 41 B9 ?? ?? ?? ?? 33 C9 TraceCall")]
            internal static IntPtr Client__ExdData__getBNpcName;

            [Offset("Search E8 ?? ?? ?? ?? 48 8B D8 48 85 C0 0F 84 ?? ?? ?? ?? 44 8B C5 TraceCall")]
            internal static IntPtr Client__ExdData__getMobHuntOrder;

            [Offset("Search E8 ?? ?? ?? ?? 48 85 C0 74 ?? 0F B7 40 ?? 66 85 C0 74 ?? 40 FE CF TraceCall")]
            internal static IntPtr Client__ExdData__getMobHuntOrderType;

            [Offset("Search E8 ?? ?? ?? ?? 48 8B F8 48 85 C0 0F 84 ?? ?? ?? ?? 0F B7 48 ?? TraceCall")]
            internal static IntPtr Client__ExdData__getMobHuntTarget;

            [Offset("Search 49 83 F8 ?? 73 ?? 41 8B C0 Add 3 Read8")]
            internal static IntPtr CountMobHuntOrderType;

            [Offset("Search 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B CE E8 ?? ?? ?? ?? Add 3 TraceRelative")]
            internal static IntPtr HuntData;

            [Offset("Search 41 8B 44 91 ?? C3 Add 4 Read8")]
            internal static int KillCountOffset;

            [Offset("Search 48 83 EC ?? 80 FA ?? 73 ?? 0F B6 C2 45 33 C9 0F B6 D2 C7 44 24 ?? ?? ?? ?? ?? 44 0F B6 44 08 ?? B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 83 C4 ?? C3 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 48 89 54 24 ??")]
            internal static IntPtr YeetHuntOrderType;
        }

        static HuntHelper()
        {
            DailyHunts = loadResource<SortedDictionary<int, StoredHuntLocation>>(Resources.AllHunts);
        }

        private static string Name => "HuntHelper";

        public static void Test()
        {
            for (int j = 0; j < MaxOrderTypes; j++)
            {
                bool unlocked = OrderTypeUnlocked(j);

                var orderType = GetMobHuntOrderType(j);

                if (!unlocked)
                {
                    Log(string.Format("Not Unlocked {0}", orderType.Item.CurrentLocaleName));
                    continue;
                }

                var listStart = orderType.OrderStart - 1;
                var dailyNum = Core.Memory.Read<byte>(Offsets.HuntData + 8 + j);

                Log(string.Format("{0}", orderType.Item.CurrentLocaleName));
                int max = 5;
                if (orderType.Type == MobHuntType.Weekly)
                    max = 1;
                for (int i = 0; i < max; i++)
                {
                    var hunt = GetMobHuntOrder((uint) listStart + dailyNum, (uint) i);
                    var target = GetMobHuntTarget(hunt.MobHuntTarget);
                    string fate = target.FateRequired ? string.Format("Fate {0}", target.Fate) : "";
                    Log(string.Format("\t{0} ({1}) {2}", target.Name, hunt.MobHuntTarget, fate));
                    var location = DailyHunts.FirstOrDefault(h => h.Key == hunt.MobHuntTarget).Value;
                    if (location != default(StoredHuntLocation))
                        Log($"\t\t {DataManager.ZoneNameResults[location.Map].CurrentLocaleName} {location.Location}");
                    else
                        Log("No Location");
                }
            }
        }

        public static void PrintAcceptedHunts()
        {
            var accepted = GetAcceptedHunts();
            for (int j = 0; j < MaxOrderTypes; j++)
            {
                bool unlocked = OrderTypeUnlocked(j);

                var orderType = GetMobHuntOrderType(j);

                if (!unlocked)
                {
                    Log(string.Format("Not Unlocked {0}", orderType.Item.CurrentLocaleName));
                    continue;
                }

                //      if (!accepted[j])
                //      {
                //           Log(string.Format("Not Accepted {0}", orderType.Item.CurrentLocaleName));
                //           continue;
                //      }

                var listStart = orderType.OrderStart - 1;
                var v8 = Core.Memory.Read<byte>(Offsets.HuntData + j + 0x16);
                //var dailyNum = Core.Memory.Read<byte>(Offsets.HuntData + 8 + j);

                Log(string.Format("{0}", orderType.Item.CurrentLocaleName));
                int max = 5;
                if (orderType.Type == MobHuntType.Weekly)
                    max = 1;
                for (int i = 0; i < max; i++)
                {
                    var hunt = GetMobHuntOrder((uint) listStart + v8, (uint) i);
                    var target = GetMobHuntTarget(hunt.MobHuntTarget);
                    string fate = target.FateRequired ? string.Format("Fate {0}", target.Fate) : "";
                    Log(string.Format("\t{0} ({1}) {2}", target.Name, hunt.MobHuntTarget, fate));
                }
            }
        }

        public static List<DailyHuntOrder> GetServerDailyHunts(int orderTypeIndex)
        {
            var accepted = GetAcceptedHunts();
            List<DailyHuntOrder> result = new List<DailyHuntOrder>();


            bool unlocked = OrderTypeUnlocked(orderTypeIndex);

            var orderType = GetMobHuntOrderType(orderTypeIndex);
            //|| !accepted[orderTypeIndex]
            if (!unlocked || orderType.Type == MobHuntType.Weekly)
            {
                Log($"Weekly or not unlocked");
                return result;
            }

            var listStart = orderType.OrderStart - 1;
            var dailyNum = Core.Memory.Read<byte>(Offsets.HuntData + 8 + orderTypeIndex);

            int max = 5;

            for (byte i = 0; i < max; i++)
            {
                var hunt = GetMobHuntOrder((uint) listStart + dailyNum, (uint) i);
                // Log($"Server Hunt {hunt}");
                var target = GetMobHuntTarget(hunt.MobHuntTarget);
                if (target.FateRequired)
                {
                    //      Log($"Fate Required {hunt}");
                    continue;
                }

                if (!DailyHunts.ContainsKey(hunt.MobHuntTarget))
                {
                    Log($"MobHuntTarget {hunt.MobHuntTarget} Not Found!, please walk into nearest traffic");
                    continue;
                }

                result.Add(new DailyHuntOrder(hunt, target, orderTypeIndex, i, DailyHunts[hunt.MobHuntTarget]));
            }

            return result;
        }

        public static List<DailyHuntOrder> GetCurrentDailyHunts(int orderTypeIndex)
        {
            var accepted = GetAcceptedHunts();
            List<DailyHuntOrder> result = new List<DailyHuntOrder>();


            bool unlocked = OrderTypeUnlocked(orderTypeIndex);

            var orderType = GetMobHuntOrderType(orderTypeIndex);
            //|| !accepted[orderTypeIndex]
            if (!unlocked || orderType.Type == MobHuntType.Weekly)
            {
                return result;
            }

            var listStart = orderType.OrderStart - 1;
            var v8 = Core.Memory.Read<byte>(Offsets.HuntData + orderTypeIndex + 0x16);

            int max = 5;

            for (byte i = 0; i < max; i++)
            {
                var hunt = GetMobHuntOrder((uint) listStart + v8, (uint) i);
                var target = GetMobHuntTarget(hunt.MobHuntTarget);
                if (target.FateRequired) continue;

                if (!DailyHunts.ContainsKey(hunt.MobHuntTarget))
                {
                    Log($"MobHuntTarget {hunt.MobHuntTarget} Not Found!, please walk into nearest traffic");
                    continue;
                }
                // string fate = target.FateRequired ? string.Format("Fate {0}", target.Fate) : "";

                result.Add(new DailyHuntOrder(hunt, target, orderTypeIndex, i, DailyHunts[hunt.MobHuntTarget]));
                //Log(string.Format("\t{0} ({1}) {2}", target.Name, hunt.MobHuntTarget, fate));
            }


            return result;
        }


        public static List<DailyHuntOrder> GetAcceptedDailyHunts(int orderTypeIndex)
        {
            var accepted = GetAcceptedHunts();
            List<DailyHuntOrder> result = new List<DailyHuntOrder>();


            bool unlocked = OrderTypeUnlocked(orderTypeIndex);

            var orderType = GetMobHuntOrderType(orderTypeIndex);
            //
            if (!unlocked || !accepted[orderTypeIndex] || orderType.Type == MobHuntType.Weekly)
            {
                return result;
            }

            var listStart = orderType.OrderStart - 1;
            var v8 = Core.Memory.Read<byte>(Offsets.HuntData + orderTypeIndex + 0x16);

            int max = 5;

            for (byte i = 0; i < max; i++)
            {
                var hunt = GetMobHuntOrder((uint) listStart + v8, (uint) i);
                var target = GetMobHuntTarget(hunt.MobHuntTarget);
                if (target.FateRequired) continue;

                if (!DailyHunts.ContainsKey(hunt.MobHuntTarget))
                {
                    Log($"MobHuntTarget {hunt.MobHuntTarget} Not Found!, please walk into nearest traffic");
                    continue;
                }
                // string fate = target.FateRequired ? string.Format("Fate {0}", target.Fate) : "";

                result.Add(new DailyHuntOrder(hunt, target, orderTypeIndex, i, DailyHunts[hunt.MobHuntTarget]));
                //Log(string.Format("\t{0} ({1}) {2}", target.Name, hunt.MobHuntTarget, fate));
            }


            return result;
        }

        public static void PrintKillCounts()
        {
            var accepted = GetAcceptedHunts();
            for (int orderType = 0; orderType < MaxOrderTypes; orderType++)
            {
                var orderTypeObj = GetMobHuntOrderType(orderType);
                Log($"--{orderTypeObj.Item.CurrentLocaleName}-- Accepted ({accepted[orderType]})");
                if (!accepted[orderType])
                    continue;

                int max = 5;
                if (orderTypeObj.Type == MobHuntType.Weekly)
                    max = 1;
                for (int mobIndex = 0; mobIndex < max; mobIndex++)
                {
                    var v1 = mobIndex + orderType * 5;
                    var v3 = v1 * 4 + Offsets.KillCountOffset;
                    Log($"{mobIndex} - Killed: {Core.Memory.Read<byte>(Offsets.HuntData + v3)}");
                }
            }
        }

        public static bool[] GetAcceptedHunts()
        {
            bool[] accepted = new bool[MaxOrderTypes];

            var bit = Core.Memory.Read<int>(Offsets.HuntData + Offsets.AcceptedHuntBitfieldOffset);
            int[] myInts = new int[1] {bit};
            BitArray myBA5 = new BitArray(myInts);

            for (int j = 0; j < MaxOrderTypes; j++)
            {
                accepted[j] = myBA5.Get(j);
            }

            return accepted;
        }

        public static MobHuntTarget GetMobHuntTarget(int mob)
        {
            return Core.Memory.Read<MobHuntTarget>(Core.Memory.CallInjected64<IntPtr>(Offsets.Client__ExdData__getMobHuntTarget, (uint) mob));
        }

        public static MobHuntOrderType GetMobHuntOrderType(int typeKey)
        {
            return Core.Memory.Read<MobHuntOrderType>(Core.Memory.CallInjected64<IntPtr>(Offsets.Client__ExdData__getMobHuntOrderType, (uint) typeKey));
        }

        public static bool OrderTypeUnlocked(int typeKey)
        {
            return Core.Memory.CallInjected64<byte>(Offsets.CheckMobBoardUnlocked,
                                                    new object[2]
                                                    {
                                                        Offsets.HuntData,
                                                        (uint) typeKey
                                                    }) > 0;
        }

        public static byte GetKillCount(int huntOrderType, int mobIndex)
        {
            var v1 = mobIndex + huntOrderType * 5;
            var v3 = v1 * 4 + Offsets.KillCountOffset;
            return Core.Memory.NoCacheRead<byte>(Offsets.HuntData + v3);
        }

        public static MobHuntOrder GetMobHuntOrder(uint typeKey, uint mobIndex)
        {
            return Core.Memory.Read<MobHuntOrder>(Core.Memory.CallInjected64<IntPtr>(Offsets.Client__ExdData__getMobHuntOrder, typeKey, mobIndex));
        }

        public static void DiscardMobHuntType(uint typekey)
        {
            Core.Memory.CallInjected64<IntPtr>(Offsets.YeetHuntOrderType,
                                               Offsets.HuntData,
                                               typekey);
        }

        public static List<(uint, HuntOrderStatus)> GetDailyStatus()
        {
            int[] dailyOrderTypes = new[] {0, 1, 2, 3, 6, 7, 8, 10, 11, 12};
            var result = new List<(uint, HuntOrderStatus)>();
            foreach (var orderType in dailyOrderTypes.Where((HuntHelper.OrderTypeUnlocked)))
            {
                bool accepted = HuntHelper.GetAcceptedHunts()[orderType];
                var orderTypeObj = HuntHelper.GetMobHuntOrderType(orderType);
                var dailies = HuntHelper.GetCurrentDailyHunts(orderType);
                var serverDailies = HuntHelper.GetServerDailyHunts(orderType);

                var oldDailies = dailies.Where((t, i) => t.NpcID != serverDailies[i].NpcID).Any();

                //LogCritical($"oldDilies = {oldDailies} count server {serverDailies.Count}");

                if ((accepted && dailies.All(i => i.IsFinished)) || !accepted)
                {
                    if (accepted && dailies.All(i => i.IsFinished) && !oldDailies)
                    {
                       // Log($"{orderTypeObj.Item.CurrentLocaleName} - Only Fates left for today's dailies so done");
                        result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.OnlyFatesLeft));
                    }
                    else if (accepted && dailies.All(i => i.IsFinished))
                    {
                       // Log($"{orderTypeObj.Item.CurrentLocaleName} - Only Fates left for old dailies so should yeet and get new ones");
                        result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.OnlyFatesLeftOld));
                    }
                    else if (!accepted && dailies.All(i => i.IsFinished && i.CurrentKills > 0) && oldDailies)
                    {
                       // Log($"{orderTypeObj.Item.CurrentLocaleName} - Not Accepted and last accepted were old and we so should get new ones?");
                        result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.NotAcceptedOld));
                    }
                    else if (!accepted && dailies.All(i => i.IsFinished && i.CurrentKills > 0) && !oldDailies)
                    {
                        //Log($"{orderTypeObj.Item.CurrentLocaleName} - Finished today's dailies");
                        result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.Complete));
                    }
                    else
                    {
                        //Log($"{orderTypeObj.Item.CurrentLocaleName} - Have not accepted today's hunts");
                        result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.NotAccepted));
                    }

                    continue;
                }

                if (dailies.Any(i => !i.IsFinished) && oldDailies)
                {
                   // Log($"{orderTypeObj.Item.CurrentLocaleName} - Unfinished old dailies");
                    result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.UnFinishedOld));
                }
                else if (dailies.Any(i => !i.IsFinished) && !oldDailies)
                {
                   // Log($"{orderTypeObj.Item.CurrentLocaleName} - Unfinished current dailies");
                    result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.Unfinished));
                }
                else
                {
                  //  Log($"All possible {orderTypeObj.Item.CurrentLocaleName} dailies done {dailies.Count()}");
                    result.Add(((uint, HuntOrderStatus)) (orderType,HuntOrderStatus.Complete));
                }
            }

            return result;
        }

        public static async Task GetHuntsByOrderType(uint orderType)
        {
            var board = HuntBoards.FirstOrDefault(i => i.OrderTypes.Contains(orderType));

            if (orderType == 0 || orderType == 4)
            {
                board = HuntBoards.FirstOrDefault(i => i.NpcId == GrandCompanyHelper.GetNpcByType(GCNpc.Hunt_Board));
            }
            
            if (board != default(HuntBoardNpc))
            {
                await board.GetHuntOrderType(orderType);
            }
        }
        
        private static void Log(object read)
        {
            Logging.Write(Colors.Gold, $"[{Name}] {read}");
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Gold, msg);
        }

        private static void Log(string text)
        {
            Logging.Write(Colors.Gold, $"[{Name}] {text}");
        }

        private void Log(IntPtr ptr)
        {
            Logging.Write(Colors.Gold, $"[{Name}] {ptr.ToString("X")}");
        }
        
        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xB)]
    public struct MobHuntOrderType
    {
        [FieldOffset(0)]
        public uint QuestId;

        [FieldOffset(4)]
        public uint EventItem;

        [FieldOffset(8)]
        public short OrderStart;

        [FieldOffset(10)]
        public MobHuntType Type;

        [FieldOffset(11)]
        public byte Amount;

        public Item Item => DataManager.GetItem(EventItem);

        public override string ToString()
        {
            return $"{QuestId} Item: {DataManager.GetItem(EventItem).CurrentLocaleName} {OrderStart} {Type} {Amount}";
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x6)]
    public struct MobHuntOrder
    {
        //[FieldOffset(0)]
        public short MobHuntTarget;

        // [FieldOffset(4)]
        public byte NeededKills;

        // [FieldOffset(8)]
        public MobHuntTypeARR Type;

        //  [FieldOffset(10)]
        public byte Rank;

        //  [FieldOffset(11)]
        public byte MobHuntReward;

        //public Item Item => DataManager.GetItem(EventItem);
        public override string ToString()
        {
            return $"{MobHuntTarget} Kills: {NeededKills} {Type} {Rank} {MobHuntReward}";
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MobHuntTarget
    {
        [FieldOffset(4)]
        public short BNpcName;

        [FieldOffset(6)]
        public short Fate;

        [FieldOffset(8)]
        public short Territory;


        public bool FateRequired => Fate > 0;

        public string Name => DataManager.GetBattleNPCData((uint) BNpcName).CurrentLocaleName;

        public override string ToString()
        {
            return $"{BNpcName} {Territory} {Fate}";
        }
    }

    public class HuntOrder
    {
        public readonly short HuntTarget;
        public readonly byte KillsNeeded;
        public readonly byte MobIndex;
        public readonly uint NpcID;
        public readonly int OrderTypeIndex;
        private short Fate;
        public Vector3 Location;
        public uint MapId;
        public MobHuntType Type;

        public HuntOrder(MobHuntOrder huntOrder, MobHuntTarget huntTarget, int orderTypeIndex, byte mobIndex, StoredHuntLocation location, MobHuntType type)
        {
            MobIndex = mobIndex;
            OrderTypeIndex = orderTypeIndex;
            HuntTarget = huntOrder.MobHuntTarget;
            NpcID = (uint) huntTarget.BNpcName;
            KillsNeeded = huntOrder.NeededKills;
            Location = location.Location;
            MapId = location.Map;
            Fate = huntTarget.Fate;
            Type = type;
        }

        public byte CurrentKills => HuntHelper.GetKillCount(OrderTypeIndex, MobIndex);

        public bool IsFinished => CurrentKills >= KillsNeeded;

        protected bool Equals(HuntOrder other)
        {
            return NpcID == other.NpcID && OrderTypeIndex == other.OrderTypeIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals((HuntOrder) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (HuntTarget.GetHashCode() * 397) ^ OrderTypeIndex;
            }
        }
    }

    public class DailyHuntOrder : HuntOrder
    {
        public DailyHuntOrder(MobHuntOrder huntOrder, MobHuntTarget huntTarget, int orderTypeIndex, byte mobIndex, StoredHuntLocation location) : base(huntOrder, huntTarget, orderTypeIndex, mobIndex, location, MobHuntType.Daily)
        {
        }

        public override string ToString()
        {
            return $"{DataManager.GetBattleNPCData(NpcID).CurrentLocaleName} Kills: {CurrentKills}/{KillsNeeded} Map: {DataManager.ZoneNameResults[MapId].CurrentLocaleName} Location: {Location}";
        }
    }

    public enum MobHuntType : byte
    {
        Unknown = 0,
        Daily = 1,
        Weekly = 2
    }

    public enum MobHuntTypeARR : byte
    {
        Unknown = 0,
        NonFate = 1,
        Fate = 2
    }

    public enum HuntOrderStatus
    {
        OnlyFatesLeft,
        OnlyFatesLeftOld,
        NotAccepted,
        NotAcceptedOld,
        Complete,
        CompleteOld,
        Unfinished,
        UnFinishedOld,
    }

    public class StoredHuntLocation
    {
        public int BNpcNameKey;
        public Vector3 Location;
        public uint Map;

        public StoredHuntLocation(int name, uint mapId, Vector3 location)
        {
            Map = mapId;
            Location = location;
            BNpcNameKey = name;
        }
    }

    public class HuntBoardNpc
    {
        public uint NpcId;
        public uint ZoneId;
        public Vector3 Location;
        public uint[] OrderTypes;

        public HuntBoardNpc(uint npcId, uint zoneId, Vector3 location, uint[] orderTypes)
        {
            NpcId = npcId;
            ZoneId = zoneId;
            Location = location;
            OrderTypes = orderTypes;
        }

        public async Task<bool> GetTo()
        {
            await Navigation.GetTo(ZoneId, Location);
            await Navigation.OffMeshMoveInteract(GetNpc);
            Navigation.LogCritical("GoTo");
            return (GetNpc.IsWithinInteractRange);
        }

        public async Task<bool> GetHuntOrderType(uint orderType)
        {
            if (GetNpc == null || !GetNpc.IsWithinInteractRange)
            {
                //Navigation.LogCritical("GoTo");
                await GetTo();
            }

            uint slot = 0;
            for (int i = 0; i < OrderTypes.Length; i++)
            {
                if (OrderTypes[i] != orderType)
                    continue;

                slot = (uint) i;
                break;
            }

            if (GetNpc == null || !GetNpc.IsWithinInteractRange)
            {
                return false;
            }

            var unit = GetNpc;
            unit.Target();
            unit.Interact();
            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            if (!SelectString.IsOpen) return false;
            SelectString.ClickSlot(slot);
            await Coroutine.Wait(5000, () => Mobhunt.Instance.IsOpen);
            Navigation.LogCritical($"is open {Mobhunt.Instance.IsOpen}");
            Mobhunt.Instance.Accept();
            Navigation.LogCritical($"accepted");
            await Coroutine.Sleep(1000);

            return HuntHelper.GetAcceptedHunts()[orderType];
        }

        public GameObject GetNpc => GameObjectManager.GetObjectByNPCId(NpcId);
    }
    
    
}