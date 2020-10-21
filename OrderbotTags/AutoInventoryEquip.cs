using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
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
            private bool UpdateGearSet { get; set; }
            
            [XmlAttribute("RecommendEquip")]
            [XmlAttribute("recommendequip")]
            [DefaultValue(true)]
            private bool UseRecommendEquip { get; set; }

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
                if (_isDone)
                {
                    await Coroutine.Yield();
                    return;
                }
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
                        Log("MainHand slot isn't filled. How?");
                        continue;
                    }
                    
                    Item currentItem = bagSlot.Item;
                    List<ItemUiCategory> category = GetUiCategory(bagSlot.Slot);
                    float itemWeight = bagSlot.IsFilled ? ItemWeight.GetItemWeight(bagSlot.Item) : -1;
                    
                    BagSlot betterItem = InventoryManager.FilledInventoryAndArmory
                                                         .Where(bs =>
                                                                      category.Contains(bs.Item.EquipmentCatagory) &&
                                                                      bs.Item.IsValidForCurrentClass &&
                                                                      bs.Item.RequiredLevel <= Core.Me.ClassLevel &&
                                                                      bs.BagId != InventoryBagId.EquippedItems)
                                                         .OrderByDescending(r => ItemWeight.GetItemWeight(r.Item))
                                                         .FirstOrDefault();
                    /*
                    Log($"# of Candidates: {betterItemCount}");
                    if (betterItem != null) Log($"{betterItem.Name}");
                    else Log("Betteritem was null.");
                    */
                    if (betterItem == null || !betterItem.IsValid || !betterItem.IsFilled || betterItem == bagSlot || itemWeight >= ItemWeight.GetItemWeight(betterItem.Item)) continue;

                    Log(bagSlot.IsFilled ? $"Equipping {betterItem.Name} over {bagSlot.Name}." : $"Equipping {betterItem.Name}.");

                    betterItem.Move(bagSlot);
                    await Coroutine.Wait(3000, () => bagSlot.Item != currentItem);
                    if (bagSlot.Item == currentItem)
                    {
                        Log("Something went wrong. Item remained unchanged.");
                        continue;
                    }
                    await Coroutine.Sleep(500);
                }

                if (UseRecommendEquip)
                {
                    if (!RecommendEquip.Instance.IsOpen) AgentRecommendEquip.Instance.Toggle();
                    await Coroutine.Wait(1500, () => RecommendEquip.Instance.IsOpen);
                    RecommendEquip.Instance.Confirm();
                    await Coroutine.Sleep(500);
                }

                if (UpdateGearSet) await UpdateGearSetTask();

                Character.Instance.Close();
                if (!await Coroutine.Wait(800, () => !Character.Instance.IsOpen)) AgentCharacter.Instance.Toggle();

                _isDone = true;
            }
            
            private async Task<bool> UpdateGearSetTask()
            {
            
                if (!Character.Instance.IsOpen)
                {
                    AgentCharacter.Instance.Toggle();
                    await Coroutine.Wait(10000, () => Character.Instance.IsOpen);
                    if (!Character.Instance.IsOpen)
                    {
                        Log("Character window didn't open.");
                        return false;
                    }
                }

                if (!Character.Instance.IsOpen) return false;

                if (!await Coroutine.Wait(1200, () => Character.Instance.CanUpdateGearSet()))
                {
                    Character.Instance.Close();
                    return false;
                }

                Character.Instance.UpdateGearSet();

                if (await Coroutine.Wait(1500, () => SelectYesno.IsOpen)) SelectYesno.Yes();
                else
                {
                    if (Character.Instance.IsOpen)
                    {
                        Character.Instance.Close();
                    }

                    return true;
                }

                await Coroutine.Wait(10000, () => !SelectYesno.IsOpen);
                if (SelectYesno.IsOpen) return true;

                if (Character.Instance.IsOpen) Character.Instance.Close();

                return true;
            }

            private static List<ItemUiCategory> GetUiCategory(ushort slotId)
            {
                if (slotId == 0) return ItemWeight.MainHands;
                if (slotId == 1) return ItemWeight.OffHands;
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