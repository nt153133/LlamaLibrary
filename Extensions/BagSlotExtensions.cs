using System;
using System.Collections.Generic;
using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Materia;
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

        public static bool ConvertToMateria(this BagSlot bagSlot)
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
        }

        public static void Discard(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.ItemDiscardFunc, new object[3]
                    {
                        Offsets.ItemFuncParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                    });
                }
            }
        }

        public static void Desynth(this BagSlot bagSlot)
        {
            lock (Core.Memory.Executor.AssemblyLock)
            {
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.DesynthNoWindow, new object[3]
                    {
                        Offsets.ItemFuncParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                    });
                }
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
                    Core.Memory.CallInjected64<uint>(Offsets.EntrustRetainerFunc, new object[5]
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
                using (Core.Memory.TemporaryCacheState(false))
                {
                    Core.Memory.CallInjected64<uint>(Offsets.RemoveMateriaFunc, new object[3]
                    {
                        Offsets.MateriaParam,
                        (uint) bagSlot.BagId,
                        bagSlot.Slot,
                    });
                }
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
                        Offsets.AffixMateriaParam,
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
    }
}