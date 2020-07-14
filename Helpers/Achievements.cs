using System;
using ff14bot;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public static class Achievements
    {
        private static class Offsets
        {
            [Offset("Search 48 8D 0D ? ? ? ? 8B D5 E8 ? ? ? ? 84 C0 0F 84 ? ? ? ? 8B CE Add 3 TraceRelative")]
            internal static IntPtr Achieve;

            [Offset("Search 4C 8B C9 81 FA ? ? ? ? 77 ? 8B C2 99 83 E2 ? 03 C2 44 8B C0 83 E0 ? 2B C2 41 C1 F8 ? 49 63 C8 42 0F B6 54 09 ? 8B C8 B8 ? ? ? ? D3 E0 84 D0 0F 95 C0 C3 32 C0 C3 ? ? ? ? ? ? 83 FA ?")]
            internal static IntPtr CheckById;
        }

        public static bool HasAchievement(int achievementId)
        {
            bool done;
            lock (Core.Memory.Executor.AssemblyLock)
                done = Core.Memory.CallInjected64<bool>(Offsets.CheckById,
                                                        Offsets.Achieve,
                                                        achievementId);
            return done;
        }
    }
}