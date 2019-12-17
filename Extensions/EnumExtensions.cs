﻿using ff14bot.Enums;
using LlamaLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlamaLibrary.Extensions
{
    public static class EnumExtensions
    {

        /// <summary>
        ///     is a dow class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsDow(this ClassJobType type)
        {
            return type != ClassJobType.Adventurer &&
                   type != ClassJobType.Alchemist &&
                   type != ClassJobType.Armorer &&
                   type != ClassJobType.Blacksmith &&
                   type != ClassJobType.Botanist &&
                   type != ClassJobType.Carpenter &&
                   type != ClassJobType.Culinarian &&
                   type != ClassJobType.Fisher &&
                   type != ClassJobType.Goldsmith &&
                   type != ClassJobType.Leatherworker &&
                   type != ClassJobType.Miner &&
                   type != ClassJobType.Weaver;
        }

        internal static ClassJobType ClassJob(this RetainerRole type)
        {
            return (ClassJobType)Enum.Parse(typeof(ClassJobType), type.ToString());
        }

        internal static ClassJobCategory ClassJobCategory(this RetainerRole type)
        {
            if (type.ClassJob().IsDow())
                return Enums.ClassJobCategory.DOW;

            switch (type.ClassJob())
            {
                case ClassJobType.Miner: return Enums.ClassJobCategory.MIN;
                case ClassJobType.Fisher: return Enums.ClassJobCategory.FSH;
                case ClassJobType.Botanist: return Enums.ClassJobCategory.BOT;

                default: return Enums.ClassJobCategory.ANY;
            }
        }
    }
}
