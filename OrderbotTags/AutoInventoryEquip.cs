using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("AutoInventoryEquip")]
    public class AutoInventoryEquip : ProfileBehavior
    {

            private bool _isDone;
            
            [XmlAttribute("UpdateGearSet")]
            [XmlAttribute("updategearset")]
            [DefaultValue(true)]
            public bool UpdateGearSet { get; set; }

            public override bool HighPriority => true;

            public override bool IsDone => _isDone;

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
                return new ActionRunCoroutine(r => InventoryEquipBest());
            }

            private async Task InventoryEquipBest()
            {
                await GeneralFunctions.StopBusy(leaveDuty:false, dismount:false);
                if (!Character.Instance.IsOpen)
                {
                    AgentCharacter.Instance.Toggle();
                    await Coroutine.Wait(5000, () => Character.Instance.IsOpen);
                }

                foreach (var bagSlot in InventoryManager.EquippedItems)
                {
                    if (!bagSlot.IsValid) continue;
                    if (bagSlot.Slot == 0 && !bagSlot.IsFilled)
                    {
                        Log("How?");
                        continue;
                    }
                    
                    Item currentItem = bagSlot.Item;
                    List<ItemUiCategory> category = GetUiCategory(bagSlot.Slot);
                    float itemWeight = bagSlot.IsFilled ? ItemWeightsManager.GetItemWeight(bagSlot.Item) : -1;
                    
                    BagSlot betterItem = InventoryManager.FilledInventoryAndArmory
                                                         .Where(bs => category.Contains(bs.Item.EquipmentCatagory) && bs.Item.IsValidForCurrentClass && bs.Item.RequiredLevel <= Core.Me.ClassLevel && bs.BagId != InventoryBagId.EquippedItems)
                                                         .OrderByDescending(r => ItemWeightsManager.GetItemWeight(r.Item))
                                                         .FirstOrDefault();
                    /*
                    Log($"# of Candidates: {betterItemCount}");
                    if (betterItem != null) Log($"{betterItem.Name}");
                    else Log("Betteritem was null.");
                    */
                    if (betterItem == null || !betterItem.IsValid || !betterItem.IsFilled || betterItem == bagSlot || itemWeight >= ItemWeightsManager.GetItemWeight(betterItem.Item)) continue;

                    Log(bagSlot.IsFilled ? $"Equipping {betterItem.Name} over {bagSlot.Name}." : $"Equipping {betterItem.Name}.");

                    betterItem.Move(bagSlot);
                    await Coroutine.Wait(3000, () => bagSlot.Item != currentItem);
                    if (bagSlot.Item == currentItem)
                    {
                        Log("Something went wrong. Item remained unchanged.");
                        continue;
                    }
                    await Coroutine.Sleep(300);
                }

                if (UpdateGearSet)
                {
                    if (!Character.Instance.IsOpen)
                    {
                        AgentCharacter.Instance.Toggle();
                        await Coroutine.Wait(5000, () => Character.Instance.IsOpen);
                    }

                    if (Character.Instance.IsOpen)
                    {
                        if (!Character.Instance.CanUpdateGearSet()) Character.Instance.Close();
                        else
                        {
                            Character.Instance.UpdateGearSet();
                            await Coroutine.Wait(2000, () => !Character.Instance.CanUpdateGearSet());
                            await Coroutine.Sleep(300);
                        }
                    }
                }

                Character.Instance.Close();

                _isDone = true;
            }

            private static List<ItemUiCategory> MainHands => (from itemUi in (ItemUiCategory[]) Enum.GetValues(typeof(ItemUiCategory))
                                                              let name = Enum.GetName(typeof(ItemUiCategory), itemUi)
                                                              where name != null
                                                              where name.EndsWith("_Arm") || name.EndsWith("_Arms") || name.EndsWith("_Primary_Tool") || name.EndsWith("_Grimoire") select itemUi)
                                                             .ToList();

            private static List<ItemUiCategory> OffHands => (from itemUi in (ItemUiCategory[]) Enum.GetValues(typeof(ItemUiCategory))
                                                             let name = Enum.GetName(typeof(ItemUiCategory), itemUi)
                                                             where name != null
                                                             where name.Equals("Shield") || name.EndsWith("_Secondary_Tool") select itemUi)
                                                            .ToList();

            private static List<ItemUiCategory> GetUiCategory(ushort slotId)
            {
                if (slotId == 0) return MainHands;
                if (slotId == 1) return OffHands;
                if (slotId == 2) return new List<ItemUiCategory> {ItemUiCategory.Head};
                if (slotId == 3) return new List<ItemUiCategory> {ItemUiCategory.Body};
                if (slotId == 4) return new List<ItemUiCategory> {ItemUiCategory.Hands};
                if (slotId == 5) return new List<ItemUiCategory> {ItemUiCategory.Waist};
                if (slotId == 6) return new List<ItemUiCategory> {ItemUiCategory.Legs};
                if (slotId == 7) return new List<ItemUiCategory> {ItemUiCategory.Feet};
                if (slotId == 8) return new List<ItemUiCategory> {ItemUiCategory.Earrings};
                if (slotId == 9) return new List<ItemUiCategory> {ItemUiCategory.Necklace};
                if (slotId == 10) return new List<ItemUiCategory> {ItemUiCategory.Bracelets};
                if (slotId == 11 || slotId == 12) return new List<ItemUiCategory> {ItemUiCategory.Ring};
                if (slotId == 13) return new List<ItemUiCategory> {ItemUiCategory.Soul_Crystal}; 
                return null;
            }
    }
    
}