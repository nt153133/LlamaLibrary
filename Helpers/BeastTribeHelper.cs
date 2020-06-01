using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public class BeastTribeHelper
    {
        internal static class Offsets
        {
            [Offset("Search E8 ? ? ? ? BA ? ? ? ? 48 8B C8 48 83 C4 ? E9 ? ? ? ? ? ? ? ? ? ? E9 ? ? ? ? TraceCall")]
            internal static IntPtr GetQuestPointer;
            [Offset("Search 48 8D 81 ? ? ? ? 66 0F 1F 44 00 ? 66 39 50 ? 74 ? 41 FF C0 Add 3 Read32")]
            internal static int DailyQuestOffset;
            [Offset("Search 41 83 F8 ? 72 ? 32 C0 C3 0F B6 40 ? Add 3 Read8")]
            internal static int DailyQuestCount;
            [Offset("Search E8 ? ? ? ? 48 85 C0 74 ? 3A 58 ? TraceCall")]
            internal static IntPtr GetBeastTribeExd;
            [Offset("Search 4C 8D 1D ? ? ? ? 88 44 24 ? Add 3 TraceRelative")]
            internal static IntPtr QuestPointer;
            [Offset("Search 0F B6 9C C8 ? ? ? ? Add 4 Read32")]
            internal static int BeastTribeRank;
            [Offset("Search 66 89 BC C8 ? ? ? ? Add 4 Read32")]
            internal static int BeastTribeRep;
            [Offset("Search 83 FB ? 73 ? E8 ? ? ? ? 8B CB 48 03 C9 0F B6 9C C8 ? ? ? ? Add 2 Read8")]
            internal static int BeastTribeCount;
        }
        
        private static string Name => "BeastTribeHelper";

        public static void PrintDailies()
        {
            var dailies = GetCurrentDailies();
            var accepted = dailies.Where(i => i.Accepted).Count();
            var finished = dailies.Where(i => i.Accepted && i.IsComplete).Count();
            var unfinished = dailies.Where(i => i.Accepted && !i.IsComplete).Select(i=> i.ID);
            
            Log($"Daily quests left: {Offsets.DailyQuestCount - accepted}\n\tAccepted: {accepted}\n\tFinished: {finished}\n\tCurrentDailies: {string.Join(",", unfinished)}");
            
        }

        public static DailyQuestRead[] GetCurrentDailies()
        {
            Log($"{(Offsets.QuestPointer + Offsets.DailyQuestOffset).ToString("X")}");
            return Core.Memory.ReadArray<DailyQuestRead>(Offsets.QuestPointer + Offsets.DailyQuestOffset, Offsets.DailyQuestCount);
        }
        
        private static void Log(string text)
        {
            Logging.Write(Colors.Gold, $"[{Name}] {text}");
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 0x10)]
        public struct DailyQuestRead
        {
            [FieldOffset(8)]
            public ushort IDRaw;

            [FieldOffset(0xA)]
            public ushort CompleteRaw;

            public bool IsComplete => CompleteRaw == 1;

            public int ID         
            {
                get
                {
                    if (IDRaw != 0)
                    {
                        return IDRaw + 0x10000;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            public bool Accepted => IDRaw != 0;
        }
    }
}