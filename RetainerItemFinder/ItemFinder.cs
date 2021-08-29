using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Helpers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteWindows;
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

        private static bool firstTimeSaddleRead = true;

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
        
        public static async Task<Dictionary<uint, int>> GetCachedSaddlebagInventories()
        {
            var result = new Dictionary<uint, int>();

            var ids = Core.Memory.ReadArray<uint>(Pointer + Offsets.SaddleBagItemIds, 140);
            var qtys = Core.Memory.ReadArray<ushort>(Pointer + Offsets.SaddleBagItemQtys, 140);

            if (firstTimeSaddleRead && ids.All(i => i == 0))
            {
                if (await InventoryBuddy.Instance.Open())
                {
                    await Coroutine.Sleep(200);
                    InventoryBuddy.Instance.Close();
                    await Coroutine.Wait(2000, () => !InventoryBuddy.Instance.IsOpen);
                    await Coroutine.Sleep(300);
                    ids = Core.Memory.ReadArray<uint>(Pointer + Offsets.SaddleBagItemIds, 140);
                    qtys = Core.Memory.ReadArray<ushort>(Pointer + Offsets.SaddleBagItemQtys, 140);
                }

                firstTimeSaddleRead = false;
            }

            for (int i = 0; i < 140; i++)
            {
                if (ids[i] == 0) continue;
                
                if (result.ContainsKey(ids[i]))
                {
                    result[ids[i]] += qtys[i];
                }
                else
                {
                    result.Add(ids[i],qtys[i]);
                }
            }
            
            return result;
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
            
            [Offset("Search 48 8D 83 ? ? ? ? 48 89 74 24 ? 48 8D 8B ? ? ? ? Add 3 Read32")]
            internal static int SaddleBagItemIds;

            [Offset("Search 48 8D 8B ? ? ? ? 48 89 7C 24 ? 4C 89 64 24 ? Add 3 Read32")]
            internal static int SaddleBagItemQtys;
        }
    }
}
