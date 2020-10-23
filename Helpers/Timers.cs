using System;
using System.Runtime.InteropServices;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public static class Timers
    {
        internal static class Offsets
        {
            [Offset("Search 48 83 EC ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 74 ? 48 8B C8 48 83 C4 ? E9 ? ? ? ? E8 ? ? ? ?")]
            internal static IntPtr GetCurrentTime;

            [Offset("Search E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 41 0F B7 14 5C TraceCall")]
            internal static IntPtr GetCycleExd;
        }

        private const int MaxRows = 6;
        private static string Name => "Timers";
        private static CycleTime[] _cycles = new CycleTime[MaxRows];
        private static readonly string[] Description = {"","Duty/Beast Tribe Dailies", "Weekly Reset", "Unknown", "GC/Rowena", "Unknown"};

        public static DateTimeOffset CurrentTime => DateTimeOffset.FromUnixTimeSeconds((long) CurrentTimeStamp).LocalDateTime;

        static Timers()
        {
            for (int i = 0; i < MaxRows; i++)
            {
                _cycles[i] = GetCycleRow(i);
            }
        }

        public static void PrintTimers()
        {
            Log($"Current Time: ({CurrentTime.LocalDateTime})"); 
            for (int i = 1; i < MaxRows; i++)
            {
                var time = DateTimeOffset.FromUnixTimeSeconds(GetNextCycle(i));

                Log($"{time.LocalDateTime} ({Description[i]})"); 
            }
        }

        internal static ulong CurrentTimeStamp
        {
            get
            {
                ulong currentTime;
                lock (Core.Memory.Executor.AssemblyLock)
                    currentTime = Core.Memory.CallInjected64<ulong>(Offsets.GetCurrentTime, 0);
                return currentTime;
            }
        }

        internal static long GetNextCycle(int index)
        {
            var row = _cycles[index];
            Log($"Getting Cycle: ({index})"); 
            return row.FirstCycle + row.Cycle * ((uint)(ushort)(((uint)CurrentTimeStamp - row.FirstCycle) / row.Cycle) + 1);
        }
        
        private static void Log(string text)
        {
            Logging.Write(Colors.Peru, $"[{Name}] {text}");
        }

        internal static CycleTime GetCycleRow(int index)
        {
            IntPtr CyclePtr;
            lock (Core.Memory.Executor.AssemblyLock)
                CyclePtr = Core.Memory.CallInjected64<IntPtr>(Offsets.GetCycleExd, index);

            if (CyclePtr != IntPtr.Zero)
                return Core.Memory.Read<CycleTime>(CyclePtr);
            
            return new CycleTime();
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x8)]
        public struct CycleTime
        {
            [FieldOffset(0)]
            public uint FirstCycle;

            [FieldOffset(0x4)]
            public uint Cycle;
        }

    }
    

}