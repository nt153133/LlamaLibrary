using System;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot.Enums;
using ff14bot.Navigation;
using ff14bot.RemoteAgents;
using LlamaLibrary.Extensions;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Retainers;
using Character = LlamaLibrary.RemoteWindows.Character;

namespace LlamaLibrary.Helpers
{
    public static class GeneralFunctions
    {
        static bool IsJumping => Core.Memory.NoCacheRead<byte>(Offsets.Conditions + Offsets.JumpingCondition) != 0;
        
        #region StopBusy
        
        private static bool CheckIfBusy(bool leaveDuty, bool stopFishing, bool dismount)
        {
            if (stopFishing && FishingManager.State != FishingState.None) return true;
            if (leaveDuty && DutyManager.InInstance) return true;
            if (dismount && Core.Me.IsMounted) return true;
            if (CraftingLog.IsOpen) return true;
            if (CraftingManager.IsCrafting) return true;
            if (MovementManager.IsOccupied) return true;
            if (InSmallTalk) return true;
            return false;
        }
        public static async Task StopBusy(bool leaveDuty = true, bool stopFishing = true, bool dismount = true)
        {
            for (var tryStep = 1; tryStep < 6; tryStep++)
            {
                if (!CheckIfBusy(leaveDuty, stopFishing, dismount)) break;

                Log($"We're occupied. Trying to exit out. Attempt #{tryStep}");

                if (stopFishing && FishingManager.State != FishingState.None)
                {
                    var quit = ActionManager.CurrentActions.Values.FirstOrDefault(i => i.Id == 299);
                    if (quit != default(SpellData))
                    {
                        Log($"Exiting Fishing.");
                        if (ActionManager.CanCast(quit, Core.Me))
                        {
                            ActionManager.DoAction(quit, Core.Me);
                            await Coroutine.Wait(6000, () => FishingManager.State == FishingState.None);
                        }
                    }
                }

                if (CraftingLog.IsOpen || CraftingManager.IsCrafting || Synthesis.IsOpen)
                {
                    Log($"Closing Crafting Window.");
                    await Lisbeth.ExitCrafting();
                    Synthesis.Close();
                    await Coroutine.Wait(6000, () => !Synthesis.IsOpen);
                    await Coroutine.Wait(1500, () => CraftingLog.IsOpen);
                    CraftingLog.Close();
                    await Coroutine.Wait(6000, () => !CraftingLog.IsOpen);
                    await Coroutine.Wait(6000, () => !CraftingManager.IsCrafting && !MovementManager.IsOccupied);
                }

                if (leaveDuty && DutyManager.InInstance)
                {
                    Log($"Leaving Diadem.");
                    DutyManager.LeaveActiveDuty();

                    if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
                    {
                        await Coroutine.Yield();
                        await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                        await Coroutine.Sleep(5000);
                    }
                }

                if (dismount && Core.Me.IsMounted)
                {
                    Log("Dismounting.");
                    ActionManager.Dismount();
                    await Coroutine.Wait(3000, () => !Core.Me.IsMounted);
                }

                if (InSmallTalk)
                {
                    Log("Skipping smalltalk.");
                    await SmallTalk();
                }

                await Coroutine.Sleep(2500);
            }

            if (CheckIfBusy(leaveDuty, stopFishing, dismount))
            {
                Log("Something went wrong, we're still occupied.");
                TreeRoot.Stop("Stopping bot.");
            }
        }

        private static bool InSmallTalk => SelectYesno.IsOpen || SelectString.IsOpen || SelectIconString.IsOpen || Talk.DialogOpen || JournalAccept.IsOpen || QuestLogManager.InCutscene || CommonBehaviors.IsLoading;

        public static async Task SmallTalk(int waitTime = 500)
        {
            await Coroutine.Wait(waitTime, () => InSmallTalk);
            
            while (InSmallTalk)
            {
                await Coroutine.Yield();

                if (CommonBehaviors.IsLoading)
                {
                    await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }
                
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.ClickNo();
                }
                
                if (SelectString.IsOpen)
                {
                    if (!await WindowEscapeSpam("SelectString"))
                    {
                        if (SelectString.Lines().Contains("Cancel")) SelectString.ClickLineContains("Cancel");
                        else if (SelectString.Lines().Contains("Quit")) SelectString.ClickLineContains("Quit");
                        else if (SelectString.Lines().Contains("Exit")) SelectString.ClickLineContains("Exit");
                        else SelectString.ClickSlot((uint)(SelectString.LineCount - 1));
                    }
                }
                
                if (SelectIconString.IsOpen)
                {
                    if (!await WindowEscapeSpam("SelectIconString"))
                    {
                        if (SelectIconString.Lines().Contains("Cancel")) SelectString.ClickLineContains("Cancel");
                        else if (SelectIconString.Lines().Contains("Quit")) SelectString.ClickLineContains("Quit");
                        else if (SelectIconString.Lines().Contains("Exit")) SelectString.ClickLineContains("Exit");
                        else SelectIconString.ClickSlot((uint)(SelectIconString.LineCount - 1));
                    }
                }
                
                while (QuestLogManager.InCutscene)
                {
                    AgentCutScene.Instance.PromptSkip();
                    if (AgentCutScene.Instance.CanSkip && SelectString.IsOpen) SelectString.ClickSlot(0);
                    await Coroutine.Yield();
                }

                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(100, () => !Talk.DialogOpen);
                    await Coroutine.Wait(100, () => Talk.DialogOpen);
                    await Coroutine.Yield();
                }

                if (JournalAccept.IsOpen)
                {
                    JournalAccept.Decline();
                }

                await Coroutine.Wait(500, () => InSmallTalk);
            }
        }

        private static async Task<bool> WindowEscapeSpam(string windowName)
        {
            for (var i = 0; i < 5 && RaptureAtkUnitManager.GetWindowByName(windowName) != null; i++)
            {
                RaptureAtkUnitManager.Update();

                if (RaptureAtkUnitManager.GetWindowByName(windowName) != null)
                {
                    RaptureAtkUnitManager.GetWindowByName(windowName).SendAction(1, 3UL, (ulong) uint.MaxValue);
                }

                await Coroutine.Wait(300, () => RaptureAtkUnitManager.GetWindowByName(windowName) == null);
                await Coroutine.Wait(300, () => RaptureAtkUnitManager.GetWindowByName(windowName) != null);
                await Coroutine.Yield();
            }

            return RaptureAtkUnitManager.GetWindowByName(windowName) == null;
        }
        
        #endregion StopBusy

        #region InventoryEquip
        
        public static async Task InventoryEquipBest(bool updateGearSet = true, bool useRecommendEquip = true)
        {
            await StopBusy(leaveDuty: false, dismount: false);
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
                List<ItemUiCategory> category = GetEquipUiCategory(bagSlot.Slot);
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

            if (useRecommendEquip)
            {
                if (!RecommendEquip.Instance.IsOpen) AgentRecommendEquip.Instance.Toggle();
                await Coroutine.Wait(3500, () => RecommendEquip.Instance.IsOpen);
                RecommendEquip.Instance.Confirm();
                await Coroutine.Sleep(800);
            }

            if (updateGearSet) await UpdateGearSet();

            Character.Instance.Close();
        }

        public static async Task<bool> UpdateGearSet()
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

        private static List<ItemUiCategory> GetEquipUiCategory(ushort slotId)
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
        
        #endregion InventoryEquip

        public static IEnumerable<BagSlot> NonGearSetItems()
        {
            return InventoryManager.FilledArmorySlots.Where(bs => !GearsetManager.GearSets.SelectMany(gs => gs.Gear).Select(g => g.Item).Contains(bs.Item));
        }

        public static async Task RetainerSellItems(IEnumerable<BagSlot> items)
        {
            if (await HelperFunctions.GetNumberOfRetainers() == 0)
            {
                Log("No retainers found to sell items to.");
                return;
            }
            
            List<BagSlot> bagSlots = items.ToList();
            if (!bagSlots.Any())
            {
                Log("No items found to sell.");
                return;
            }

            await StopBusy();
            if (!await HelperFunctions.UseSummoningBell())
            {
                Log("Couldn't get to summoning bell.");
                return;
            }
            
            await RetainerRoutine.SelectRetainer(0);
            RetainerTasks.OpenInventory();
            if (!await Coroutine.Wait(3000, RetainerTasks.IsInventoryOpen))
            {
                Log("Couldn't get Retainer inventory open.");
                RetainerTasks.CloseInventory();
                await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);
                RetainerTasks.CloseTasks();
                await Coroutine.Wait(3000, () => Talk.DialogOpen);
                if (Talk.DialogOpen) Talk.Next();
                await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);
                await RetainerRoutine.CloseRetainers();
                return;
            }

            int itemCount = bagSlots.Count;
            int i = 1;
            foreach (var bagSlot in bagSlots)
            {
                if (!bagSlot.IsValid || !bagSlot.IsFilled)
                {
                    Log("BagSlot isn't valid or filled.");
                    i++;
                    continue;
                }
                
                string name = bagSlot.Name;
                Log($"Attempting to sell #{i++} of {itemCount}: {name}");
                int waitTime = 600;
                
                bagSlot.RetainerSellItem();
                
                if (await Coroutine.Wait(500, () => SelectYesno.IsOpen)) SelectYesno.ClickYes();
                else waitTime -= 500;
                
                if (!await Coroutine.Wait(5000, () => !bagSlot.IsValid || !bagSlot.IsFilled)) Log($"We couldn't sell {name}.");
                else Log($"Sold {name}.");
                
                await Coroutine.Sleep(waitTime);
            }
            
            RetainerTasks.CloseInventory();
            await Coroutine.Wait(3000, () => RetainerTasks.IsOpen);
            RetainerTasks.CloseTasks();
            await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
            SelectYesno.ClickYes();
            await Coroutine.Wait(3000, () => Talk.DialogOpen);
            if (Talk.DialogOpen) Talk.Next();
            await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);
            await RetainerRoutine.CloseRetainers();
        }

        public static async Task RepairAll()
        {
            if (InventoryManager.EquippedItems.Any(item => item.Item != null && item.Item.RepairItemId != 0 && item.Condition < 50))
            {
                Log("Repairing items.");
                await StopBusy(leaveDuty:false, stopFishing:false, dismount:false);
                if (!Repair.IsOpen)
                {
                    var repairVTable = Offsets.RepairVTable;
                    var repairVendor = Offsets.RepairVendor;
                    var repairWindow = Offsets.RepairWindowOpen;
                    var repairAgent = AgentModule.FindAgentIdByVtable(repairVTable);
                    var AgentId = repairAgent;
                    Log($"OPEN: AgentId {AgentId} Offset {repairVendor.ToInt64():X} Func {repairWindow.ToInt64():X}");
                    lock (Core.Memory.Executor.AssemblyLock)
                    {
                        Core.Memory.CallInjected64<IntPtr>(repairWindow, new object[4]
                        {
                            ff14bot.Managers.AgentModule.GetAgentInterfaceById(AgentId).Pointer,
                            0,
                            0,
                            repairVendor
                        });
                    }
                    await Coroutine.Wait(1500,() => Repair.IsOpen);
                }

                Repair.RepairAll();
                await Coroutine.Wait(1500, () => SelectYesno.IsOpen);
                SelectYesno.ClickYes();
                Repair.Close();
            }
            else Log("No items to repair.");
        }

        public static int GetGearSetiLvl(GearSet gs)
        {
            List<ushort> gear = gs.Gear.Select(i => i.Item.ItemLevel).Where(x => x > 0).ToList();
            if (!gear.Any()) return 0;
            return (int) gear.Sum(i => i) / gear.Count;
        }
        
        #region GoHome

        public static async Task GoHome()
        {
            uint[] privateHousing = new uint[] {59, 60, 61, 97};
            uint[] FCHousing = new uint[] {56,57,58,96};

            var AE = WorldManager.AvailableLocations;

            var PrivateHouses = AE.Where(x => privateHousing.Contains(x.AetheryteId)).OrderBy(x => x.GilCost);
            var FCHouses = AE.Where(x => FCHousing.Contains(x.AetheryteId)).OrderBy(x => x.GilCost);
            
            bool HavePrivateHousing = PrivateHouses.Any();
            bool HaveFCHousing = FCHouses.Any();


            Log($"Private House Access: {HavePrivateHousing} FC House Access: {HaveFCHousing}");
            
            //await GoToHousingBell(FCHouses.First());
            
            
            if (HavePrivateHousing)
            {
                await GoToHousingBell(PrivateHouses.First());
            }
            else if (HaveFCHousing)
            {
                await GoToHousingBell(FCHouses.First());
            }
        }
        
        private static async Task<bool> GoToHousingBell(WorldManager.TeleportLocation house)
        {
            Log($"Teleporting to housing: (ZID: {house.ZoneId}, AID: {house.AetheryteId}) {house.Name}");
            await CommonTasks.Teleport(house.AetheryteId);

            Log("Waiting for zone to change");
            await Coroutine.Wait(20000, () => WorldManager.ZoneId == house.ZoneId);

            Log("Getting closest housing entrance");
            uint houseEntranceId = 2002737;
            uint aptEntranceId = 2007402;

            var entranceIds = new uint[] {houseEntranceId, aptEntranceId};

            var entrance = GameObjectManager.GetObjectsByNPCIds<GameObject>(entranceIds).OrderBy(x => x.Distance2D()).FirstOrDefault();
            if (entrance != null)
            {
                Log("Found housing entrance, approaching");
                await Navigation.FlightorMove(entrance.Location);

                if (entrance.IsWithinInteractRange)
                {
                    Navigator.NavigationProvider.ClearStuckInfo();
                    Navigator.Stop();
                    await Coroutine.Wait(5000, () => !IsJumping);

                    entrance.Interact();

                    // Handle different housing entrance menus
                    if (entrance.NpcId == houseEntranceId)
                    {
                        Log("Entering house");

                        await Coroutine.Wait(10000, () => SelectYesno.IsOpen);
                        if (SelectYesno.IsOpen)
                        {
                            SelectYesno.Yes();
                        }
                    }
                    else if (entrance.NpcId == aptEntranceId)
                    {
                        Log("Entering apartment");

                        await Coroutine.Wait(10000, () => SelectString.IsOpen);
                        if (SelectString.IsOpen)
                        {
                            SelectString.ClickSlot(0);
                        }
                    }

                    await CommonTasks.HandleLoading();

                    Log("Getting best summoning bell");
                    var bell = HelperFunctions.FindSummoningBell();
                    if (bell != null)
                    {
                        Log("Found summoning bell, approaching");
                        await HelperFunctions.GoToSummoningBell();
                        return true;
                    }
                    else
                    {
                        Log("Couldn't find any summoning bells");
                    }
                }
            }
            else
            {
                Log($"Couldn't find any housing entrances.  Are we in the right zone?  Current: ({WorldManager.ZoneId}) {WorldManager.CurrentZoneName}");
            }

            return false;
        }
        
        #endregion GoHome

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[GeneralFunctions] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}
