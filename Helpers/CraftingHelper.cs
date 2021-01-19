using System;
using ff14bot;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Structs;

namespace LlamaLibrary.Helpers
{
    public static class CraftingHelper
    {
        private static class Offsets
        {
            [Offset("Search 4C 8D 0D ?? ?? ?? ?? 4D 8B 13 49 8B CB Add 3 TraceRelative")]
            internal static IntPtr DohLastAction;
        }

        public static CraftingStatus Status => Core.Memory.Read<CraftingStatus>(Offsets.DohLastAction);

        public static bool AnimationLocked => CraftingManager.AnimationLocked;

        public static bool IsValid(CraftingStatus statis)
        {
            if (Status.Stage == 9 || Status.Stage == 10) return true;
            return false;
        }

        public static int Quality
        {
            get
            {
                var status = Status;
                if (IsValid(status))
                {
                    return (int) status.Quality;
                }
                else
                {
                    return CraftingManager.Quality;
                }
            }
        }

        public static int Step
        {
            get
            {
                var status = Status;
                if (IsValid(status))
                {
                    return (int) status.Step;
                }
                else
                {
                    return CraftingManager.Step;
                }
            }
        }

        public static int HQPercent
        {
            get
            {
                var status = Status;
                if (IsValid(status))
                {
                    return (int) status.HQ;
                }
                else
                {
                    return CraftingManager.HQPercent;
                }
            }
        }

        public static int Durability
        {
            get
            {
                var status = Status;
                if (IsValid(status))
                {
                    return (int) status.Durability;
                }
                else
                {
                    return CraftingManager.Durability;
                }
            }
        }

        public static int Progress
        {
            get
            {
                var status = Status;
                if (IsValid(status))
                {
                    return (int) status.Progress;
                }
                else
                {
                    return CraftingManager.Progress;
                }
            }
        }

        public static uint LastActionId
        {
            get
            {
                var status = Status;
                if (IsValid(status))
                {
                    return status.LastAction;
                }
                else
                {
                    return CraftingManager.LastActionId;
                }
            }
        }

        public static int ProgressRequired => CraftingManager.ProgressRequired;
        public static int DurabilityCap => (int)Synthesis.GetProperty("DurabilityCap");
        public static int QualityCap => (int)Synthesis.GetProperty("QualityCap");
        public static int IconId => (int)Synthesis.GetProperty("IconId");
        public static bool IsCrafting => CraftingManager.IsCrafting;
        public static bool CanCraft => CraftingManager.CanCraft;
        public static RecipeData CurrentRecipe => CraftingManager.CurrentRecipe;

    }
}