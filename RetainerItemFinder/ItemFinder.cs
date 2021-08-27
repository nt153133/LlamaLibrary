using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.Retainers;

#pragma warning disable 649
namespace LlamaLibrary.RetainerItemFinder
{
    public static class ItemFinder
    {
        internal static IntPtr Pointer;

        internal static IntPtr TreeStart => Core.Memory.ReadArray<IntPtr>(ParentStart, 3)[1];

        internal static IntPtr ParentStart => Core.Memory.Read<IntPtr>(Pointer + Offsets.TreeStartOff);

        private static readonly List<IntPtr> VisitedNodes = new List<IntPtr>();

        private static readonly Dictionary<ulong, StoredRetainerInventory> RetainerInventoryPointers = new Dictionary<ulong, StoredRetainerInventory>();

        static ItemFinder()
        {
            var framework = Core.Memory.Read<IntPtr>(Offsets.GFramework2);
            var getUiModule = Core.Memory.CallInjected64<IntPtr>(Offsets.GetUiModule, framework);
            var getRaptureItemFinder = getUiModule + Offsets.RaptureItemFinder;

            Pointer = getRaptureItemFinder;
        }

        public static async Task<Dictionary<ulong, StoredRetainerInventory>> SafelyGetCachedRetainerInventories()
        {
            var retData = await HelperFunctions.GetOrderedRetainerArray(true);

            if (retData.Length == 0)
            {
                Logging.Write(Colors.OrangeRed, $"You don't have any retainers");
                return new Dictionary<ulong, StoredRetainerInventory>(); 
            }

            return GetCachedRetainerInventories();
        }

        public static Dictionary<ulong, StoredRetainerInventory> GetCachedRetainerInventories()
        {
            VisitedNodes.Clear();
            RetainerInventoryPointers.Clear();

            VisitedNodes.Add(ParentStart);

            //Logging.Write(Colors.OrangeRed, $"ParentStart {ParentStart.ToString("X")}");

            Visit(TreeStart);

            return RetainerInventoryPointers;
        }

        private static void Visit(IntPtr nodePtr)
        {
            if (VisitedNodes.Contains(nodePtr))
            {
                return;
            }

            var node = Core.Memory.Read<ItemFinderPtrNode>(nodePtr);

            if (!node.Filled)
            {
                return;
            }
            else
            {
                //Logging.Write(Colors.OrangeRed, $"Adding node");
                RetainerInventoryPointers.Add(node.RetainerId, new StoredRetainerInventory(node.RetainerInventory));
            }

            if (!VisitedNodes.Contains(node.Left))
            {
                Visit(node.Left);
            }

            if (!VisitedNodes.Contains(node.Right))
            {
                Visit(node.Right);
            }

            VisitedNodes.Add(nodePtr);
        }

        private static class Offsets
        {
            [Offset("Search 48 8B 0D ?? ?? ?? ?? 48 8B DA E8 ?? ?? ?? ?? 48 85 C0 74 ?? 4C 8B 00 48 8B C8 41 FF 90 ?? ?? ?? ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 ?? 4C 8B 00 48 8B D3 48 8B C8 48 83 C4 ?? 5B 49 FF 60 ?? 48 83 C4 ?? 5B C3 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 40 53 Add 3 TraceRelative")]
            internal static IntPtr GFramework2;

            [Offset("Search E8 ?? ?? ?? ?? 48 8B C8 48 85 C0 74 ?? 84 DB 74 ?? 38 1D ?? ?? ?? ?? TraceCall")]
            internal static IntPtr GetUiModule;

            //Broken pattern but it should be 0x88
            [Offset("Search 48 FF A0 ? ? ? ? 48 8B 02 48 8B CA 48 83 C4 ? 5B 48 FF A0 ? ? ? ? Add 3 Read32")]
            internal static int GetRaptureItemFinder;

            [Offset("Search 49 8D 8E ? ? ? ? 33 D2 FF 50 ? 41 80 BE ? ? ? ? ? Add 3 Read32")]
            internal static int RaptureItemFinder;

            [Offset("Search 4C 8B 85 ? ? ? ? 48 89 B4 24 ? ? ? ? Add 3 Read32")]
            internal static int TreeStartOff;
        }
    }
}
