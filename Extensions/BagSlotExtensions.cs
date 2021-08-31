using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;

namespace LlamaLibrary.Extensions
{
    public static class BagSlotExtensions
    {
        public static bool Split(this BagSlot bagSlot, int amount)
        {
            if (bagSlot.Count > amount)
            {
                lock (Core.Memory.Executor.AssemblyLock)
                {
                    using (Core.Memory.TemporaryCacheState(false))
                    {
                        return Core.Memory.CallInjected64<uint>(Offsets.ItemSplitFunc, new object[4]
                        {
                            Offsets.ItemFuncParam,
                            (uint) bagSlot.BagId,
                            bagSlot.Slot,
                            amount
                        }) == 0;
                    }
                }
            }

            return false;
        }

        /*public static bool ConvertToMateria(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    return Core.Memory.CallInjected64<uint>(Offsets.ConvertToMateria, new object[3]
                    {
                        Offsets.ItemFuncParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot
                    }) == 0;
                }
            }
        }*/

        public static void Discard(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {

                    Core.Memory.CallInjected64<uint>(Offsets.ItemDiscardFunc, new object[3]
                    {
                        Offsets.ItemFuncParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                    });
            }
        }

        public static void Desynth(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                Core.Memory.CallInjected64<uint>(Offsets.RemoveMateriaFunc, new object[5]
                {
                    Offsets.EventHandler,
                    0x390000,
                    (uint) bagSlot.BagId,
                    bagSlot.Slot,
                    2
                });
            }
        }

        public static bool LowerQuality(this BagSlot bagSlot)
        {
            if (bagSlot.IsHighQuality || bagSlot.IsCollectable)
            {
                lock (Core.Memory.Executor.AssemblyLock)
                {
                    using (Core.Memory.TemporaryCacheState(false))
                    {
                        Core.Memory.CallInjected64<uint>(Offsets.ItemLowerQualityFunc, new object[3]
                        {
                            Offsets.ItemFuncParam,
                            (uint) bagSlot.BagId,
                            bagSlot.Slot,
                        });
                    }
                }

                return !bagSlot.IsHighQuality;
            }

            return false;
        }

        public static void RetainerRetrieveQuantity(this BagSlot bagSlot, int amount)
        {
            if (bagSlot.Count < amount)
                amount = (int) bagSlot.Count;

            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.RetainerRetrieveQuantity, new object[4]
                    {
                        Offsets.ItemFuncParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                        amount
                    });
                }
            }
        }

        public static void RetainerEntrustQuantity(this BagSlot bagSlot, int amount)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    var result = Core.Memory.CallInjected64<uint>(Offsets.EntrustRetainerFunc, new object[5]
                    {
                        AgentRetainerInventory.Instance.Pointer,
                        0,
                        (uint)bagSlot.BagId,
                        bagSlot.Slot,
                        amount
                    });
                }
            }
        }

        public static void RetainerSellItem(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.SellFunc, new object[4]
                    {
                        AgentRetainerInventory.Instance.RetainerShopPointer,
                        bagSlot.Slot,
                        (uint)bagSlot.BagId,
                        0
                    });
                }
            }
        }

        public static void RemoveMateria(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {

                    Core.Memory.CallInjected64<uint>(Offsets.RemoveMateriaFunc, new object[5]
                    {
                        Offsets.EventHandler,
                        0x390001,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                        0
                    });

            }
        }

        public static void Reduce(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {

                    Core.Memory.CallInjected64<uint>(Offsets.RemoveMateriaFunc, new object[5]
                    {
                        Offsets.EventHandler,
                        0x390002,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                        0
                    });

            }
        }

        public static void ExtractMateria(this BagSlot bagSlot)
        {
            if ((int) bagSlot.SpiritBond != 100) return;
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.ExtractMateriaFunc, new object[3]
                    {
                        Offsets.ExtractMateriaParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                    });
                }
            }
        }

        public static void AffixMateria(this BagSlot bagSlot, BagSlot materia)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                Core.Memory.CallInjected64<uint>(Offsets.AffixMateriaFunc, new object[3]
                    {
                        Offsets.ExtractMateriaParam,
                        bagSlot.Pointer,
                        materia.Pointer
                    });
            }
        }

        public static void OpenMeldInterface(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.MeldWindowFunc, new object[2]
                    {
                        AgentMeld.Instance.Pointer,
                        bagSlot.Pointer,
                    });
                }
            }
        }

        public static bool HasMateria(this BagSlot bagSlot)
        {
            var materiaType = Core.Memory.ReadArray<ushort>(bagSlot.Pointer + 0x20, 5);
            for (var i = 0; i < 5; i++)
            {
                if (materiaType[i] > 0) return true;
            }

            return false;
        }

        public static int MateriaCount(this BagSlot bagSlot)
        {
            var materiaType = Core.Memory.ReadArray<ushort>(bagSlot.Pointer + 0x20, 5);
            int count = 0;
            for (var i = 0; i < 5; i++)
            {
                if (materiaType[i] > 0) count++;
            }

            return count;
        }

        public static void TradeItem(this BagSlot bagSlot)
        {
            uint result = 0;
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    result = Core.Memory.CallInjected64<uint>(Offsets.TradeBagSlot, new object[3]
                    {
                        Offsets.ItemFuncParam,
                        bagSlot.Slot,
                        (uint)bagSlot.BagId
                    });
                }
            }

            if (result != 0)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    result = Core.Memory.CallInjected64<uint>(Offsets.TradeBagSlot, new object[3]
                    {
                        Offsets.ItemFuncParam,
                        bagSlot.Slot,
                        (uint)bagSlot.BagId
                    });
                }
            }
        }

        public static bool CanTrade(this BagSlot slot)
        {
            return !slot.Item.Untradeable && !slot.IsCollectable && !(slot.SpiritBond > 0);
        }

        public static void PlaceAetherWheel(this BagSlot bagSlot)
        {
            PlaceAetherWheel((uint) bagSlot.BagId, bagSlot.Slot);
        }

        public static bool AddToSaddlebagQuantity(this BagSlot bagSlot, uint amount)
        { 
            return AddToSaddleCall(Offsets.ItemFuncParam, (uint) bagSlot.BagId, bagSlot.Slot, (uint) amount) == IntPtr.Zero;
        }

        public static bool RemoveFromSaddlebagQuantity(this BagSlot bagSlot, uint amount)
        {
            return RemoveFromSaddleCall(Offsets.ItemFuncParam, (uint) bagSlot.BagId, bagSlot.Slot, (uint) amount) == IntPtr.Zero;
        }

        public static void UseItemRaw(this BagSlot bagSlot)
        {
            BagSlotUseItemCall(Offsets.ItemFuncParam, bagSlot.TrueItemId, (uint) bagSlot.BagId, bagSlot.Slot);
        }

        internal static byte BagSlotUseItemCall(IntPtr InventoryManager, uint TrueItemId, uint inventoryContainer, int inventorySlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    return Core.Memory.CallInjected64<byte>(Offsets.BagSlotUseItem,
                                                            InventoryManager,
                                                            TrueItemId,
                                                            inventoryContainer,
                                                            inventorySlot);
                }
            }
        }

        internal static IntPtr RemoveFromSaddleCall(IntPtr InventoryManager, uint inventoryContainer, ushort inventorySlot, uint count)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    return Core.Memory.CallInjected64<IntPtr>(Offsets.RemoveFromSaddle,
                                                              InventoryManager,
                                                              inventoryContainer,
                                                              inventorySlot,
                                                              count);
                }
            }
        }

        internal static IntPtr AddToSaddleCall(IntPtr InventoryManager, uint inventoryContainer, ushort inventorySlot, uint count)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    return Core.Memory.CallInjected64<IntPtr>(Offsets.AddToSaddle,
                                                              InventoryManager,
                                                              inventoryContainer,
                                                              inventorySlot,
                                                              count);
                }
            }
        }

        internal static IntPtr PlaceAetherWheel(uint inventoryContainer, ushort inventorySlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    return Core.Memory.CallInjected64<IntPtr>(Offsets.PlaceAetherWheel, new object[3]
                    {
                        AgentBagSlot.Instance.PointerForAether,
                        (int) inventorySlot,
                        inventoryContainer,
                    });
                }
            }
        }

        public static void RetainerEntrustQuantity(this BagSlot bagSlot, uint amount)
        {
            bagSlot.RetainerEntrustQuantity((int)amount);
        }

        public static void RetainerRetrieveQuantity(this BagSlot bagSlot, uint amount)
        {
            bagSlot.RetainerRetrieveQuantity((int)amount);
        }

        public static string ItemName(this BagSlot bagSlot)
        {
            return GetItemName(bagSlot.TrueItemId);
        }

        public static string GetItemName(uint ItemId)
        {
            if (ItemId >= 1000000)
            {
                return $"{DataManager.GetItem(ItemId - 1000000).CurrentLocaleName} (HQ)";
            }

            return DataManager.GetItem(ItemId).CurrentLocaleName;
        }
        
        public const int DefaultBagSlotMoveWait = 600;

        private static async Task<bool> BagSlotMoveWait(BagSlot bagSlot, uint curSlotCount, int waitMs = DefaultBagSlotMoveWait)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (await Coroutine.Wait(waitMs, () => !bagSlot.IsValid || !bagSlot.IsFilled || bagSlot.Count < curSlotCount))
            {
                sw.Stop();
                int remainingMs = waitMs - (int)sw.ElapsedMilliseconds;
                if (remainingMs > 0)
                {
                    await Coroutine.Sleep(remainingMs);
                }
                return true;
            }
            sw.Stop();
            return false;
        }

        public static async Task<bool> TryAddToSaddlebag(this BagSlot bagSlot, uint moveCount, int waitMs = DefaultBagSlotMoveWait)
        {
            uint curSlotCount = bagSlot.Count;
            bagSlot.AddToSaddlebagQuantity(moveCount);
            return await BagSlotMoveWait(bagSlot, curSlotCount, waitMs);
        }
        
        public static async Task<bool> TryRemoveFromSaddlebag(this BagSlot bagSlot, uint moveCount, int waitMs = DefaultBagSlotMoveWait)
        {
            uint curSlotCount = bagSlot.Count;
            bagSlot.RemoveFromSaddlebagQuantity(moveCount);
            return await BagSlotMoveWait(bagSlot, curSlotCount, waitMs);
        }
        
        public static async Task<bool> TryEntrustToRetainer(this BagSlot bagSlot, uint moveCount, int waitMs = DefaultBagSlotMoveWait)
        {
            uint curSlotCount = bagSlot.Count;
            bagSlot.RetainerEntrustQuantity(moveCount);
            return await BagSlotMoveWait(bagSlot, curSlotCount, waitMs);
        }
        
        public static async Task<bool> TryRetrieveFromRetainer(this BagSlot bagSlot, uint moveCount, int waitMs = DefaultBagSlotMoveWait)
        {
            uint curSlotCount = bagSlot.Count;
            bagSlot.RetainerRetrieveQuantity(moveCount);
            return await BagSlotMoveWait(bagSlot, curSlotCount, waitMs);
        }
    }
}