﻿using System;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.Extensions
{
    public static class BagSlotExtentions
    {
        public static bool Split(this BagSlot bagSlot, int amount)
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            IntPtr move= patternFinder.Find("48 8D 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 74 ? 4C 63 78 ? Add 3 TraceRelative");
            IntPtr SplitFunc= patternFinder.Find("40 55 53 56 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 45 ? 8D 82 ? ? ? ?");
            
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    return Core.Memory.CallInjected64<uint>(SplitFunc, new object[4]
                    {
                        move,
                        (uint)bagSlot.BagId,
                        bagSlot.Slot,
                        amount
                    }) == 0;
                }
            }
        }
    }
}