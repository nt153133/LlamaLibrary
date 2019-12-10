﻿using System.Linq;
using ff14bot.Managers;

namespace LlamaLibrary.Extensions
{
    public static class BagExtensions
    {
        internal static BagSlot GetFirstFreeSlot(this Bag bag)
        {
            return bag.FreeSlots > 0 ? bag.First(i => !i.IsFilled) : null;
        }
    }
}