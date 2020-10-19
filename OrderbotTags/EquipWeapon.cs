using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using LlamaLibrary.Helpers;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("EquipWeapon")]
    public class EquipWeapon : ProfileBehavior
    {
        private bool _isDone;


        [XmlAttribute("itemIDs")]
        [XmlAttribute("ItemIDs")]
        [XmlAttribute("itemID")]
        [XmlAttribute("ItemID")]
        public int[] Item { get; set; }

        public override bool HighPriority => true;

        protected override void OnStart()
        {
        }

        protected override void OnDone()
        {
        }

        protected override void OnResetCachedDone()
        {
            _isDone = false;
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => EquipWeapons(Item));
        }

        private async Task EquipWeapons(int[] weapons)
        {
            foreach (var weapon in weapons)
            {
                var itemRole = DataManager.GetItem((uint) weapon).ItemRole;
                BagSlot EquipSlot = ff14bot.Managers.InventoryManager.GetBagByInventoryBagId(ff14bot.Enums.InventoryBagId.EquippedItems)[ff14bot.Enums.EquipmentSlot.MainHand];
                if (itemRole == ItemRole.Shield)
                    EquipSlot = ff14bot.Managers.InventoryManager.GetBagByInventoryBagId(ff14bot.Enums.InventoryBagId.EquippedItems)[ff14bot.Enums.EquipmentSlot.OffHand];

                var item1 = ff14bot.Managers.InventoryManager.FilledInventoryAndArmory.FirstOrDefault(i => i.RawItemId == (uint) weapon);
                if (item1 != default(BagSlot))
                    item1.Move(EquipSlot);
            }

            _isDone = true;
        }

        public override bool IsDone => _isDone;
    }
}