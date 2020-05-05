using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public class HuntHelper
    {
        private static string Name => "HuntHelper";
        private const int MaxOrderTypes = 0xE;

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
                if (orderType.Type == 2)
                    max = 1;
                for (int i = 0; i < max; i++)
                {
                    var hunt = GetMobHuntOrder((uint) listStart + dailyNum, (uint) (i));
                    var target = GetMobHuntTarget(hunt.MobHuntTarget);
                    string fate = target.FateRequired ? string.Format("Fate {0}", target.Fate) : "";
                    Log(string.Format("\t{0} ({1}) {2}", target.Name, hunt.MobHuntTarget, fate));
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

                if (!accepted[j])
                {
                    Log(string.Format("Not Accepted {0}", orderType.Item.CurrentLocaleName));
                    continue;
                }

                var listStart = orderType.OrderStart - 1;
                var v8 = Core.Memory.Read<byte>(Offsets.HuntData + j + 0x16) ;
                //var dailyNum = Core.Memory.Read<byte>(Offsets.HuntData + 8 + j);

                Log(string.Format("{0}", orderType.Item.CurrentLocaleName));
                int max = 5;
                if (orderType.Type == 2)
                    max = 1;
                for (int i = 0; i < max; i++)
                {
                    var hunt = GetMobHuntOrder((uint) listStart + v8, (uint) (i));
                    var target = GetMobHuntTarget(hunt.MobHuntTarget);
                    string fate = target.FateRequired ? string.Format("Fate {0}", target.Fate) : "";
                    Log(string.Format("\t{0} ({1}) {2}", target.Name, hunt.MobHuntTarget, fate));
                }
            }
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
                if (orderTypeObj.Type == 2)
                    max = 1;
                for (int mobIndex = 0; mobIndex < max; mobIndex++)
                {
                    var v1 = mobIndex + orderType * 5;
                    var v3 = v1 * 4 + Offsets.KillCountOffset;
                    Log($"{mobIndex} - Killed: {Core.Memory.Read<byte>(Offsets.HuntData+ v3)}");
                }
            }
        }

        public static bool[] GetAcceptedHunts()
        {
            bool[] accepted = new bool[MaxOrderTypes];
            
            var bit = (Core.Memory.Read<int>( Offsets.HuntData + Offsets.AcceptedHuntBitfieldOffset));
           // Log(string.Format("{0}", Convert.ToString(bit, toBase: 2)));

            int[]  myInts  = new int[1] { bit  };
            BitArray myBA5 = new BitArray( myInts );

            for (int j = 0; j < MaxOrderTypes; j++)
            {
                accepted[j] = myBA5.Get(j);
               // Log(j + ": " +);
            }

            return accepted;
        }

        public static MobHuntTarget GetMobHuntTarget(int mob)
        {
            return Core.Memory.Read<MobHuntTarget>( Core.Memory.CallInjected64<IntPtr>(Offsets.Client__ExdData__getMobHuntTarget, (uint) mob));
        }

        public static MobHuntOrderType GetMobHuntOrderType(int typeKey)
        {
            return Core.Memory.Read<MobHuntOrderType>( Core.Memory.CallInjected64<IntPtr>(Offsets.Client__ExdData__getMobHuntOrderType, (uint) typeKey));
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

        public static MobHuntOrder GetMobHuntOrder(uint typeKey, uint mobIndex)
        {
            return Core.Memory.Read<MobHuntOrder>(Core.Memory.CallInjected64<IntPtr>(Offsets.Client__ExdData__getMobHuntOrder, typeKey, mobIndex));
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
        public byte Type;

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
        public byte Type;

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


        public bool FateRequired => (Fate > 0);

        public string Name => DataManager.GetBattleNPCData((uint) BNpcName).CurrentLocaleName;

        public override string ToString()
        {
            return $"{BNpcName} {Territory} {Fate}";
        }
    }
}