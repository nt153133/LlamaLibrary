using System;
using System.Collections.Generic;
using ff14bot.Managers;
using LlamaLibrary.Structs;

namespace LlamaLibrary.Retainers
{
    public class CompleteRetainer
    {
        public RetainerInfo Info;
        
        public int MBCount => ItemsForSale.Count;
        public List<BagSlot> ItemsForSale;
        public List<BagSlot> Inventory;
        public int Index;
        public int FreeSlots => 175 - Inventory.Count;
        public int FreeSlotsMB => 20 - ItemsForSale.Count;
            
        public DateTime MBUpdated => DateTimeOffset.FromUnixTimeSeconds(Info.MBTimeOutTimestamp).LocalDateTime;
        public DateTime VentureEnd => DateTimeOffset.FromUnixTimeSeconds(Info.VentureEndTimestamp).LocalDateTime;
        
        public CompleteRetainer(RetainerInfo info, int index, List<BagSlot> itemsForSale, List<BagSlot> inventory)
        {
            Info = info;
            ItemsForSale = itemsForSale;
            Inventory = inventory;
            Index = index;
        }
    }
}