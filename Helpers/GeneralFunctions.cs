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
using Clio.Utilities;
using ff14bot.Enums;
using ff14bot.Navigation;
using ff14bot.RemoteAgents;
using LlamaLibrary.Extensions;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
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
                        else if (SelectString.Lines().Contains("Nothing")) SelectString.ClickLineContains("Nothing");
                        else SelectString.ClickSlot((uint) (SelectString.LineCount - 1));
                    }
                }

                if (SelectIconString.IsOpen)
                {
                    if (!await WindowEscapeSpam("SelectIconString"))
                    {
                        if (SelectIconString.Lines().Contains("Cancel")) SelectString.ClickLineContains("Cancel");
                        else if (SelectIconString.Lines().Contains("Quit")) SelectString.ClickLineContains("Quit");
                        else if (SelectIconString.Lines().Contains("Exit")) SelectString.ClickLineContains("Exit");
                        else if (SelectIconString.Lines().Contains("Nothing")) SelectString.ClickLineContains("Nothing");
                        else SelectIconString.ClickSlot((uint) (SelectIconString.LineCount - 1));
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

            int armoryCount = 0;

            if (useRecommendEquip)
            {
                foreach (var bagSlot in InventoryManager.EquippedItems)
                {
                    if (!bagSlot.IsValid) continue;
                    if (bagSlot.Slot == 0 && !bagSlot.IsFilled)
                    {
                        Log("MainHand slot isn't filled. How?");
                        continue;
                    }

                    float itemWeight = bagSlot.IsFilled ? ItemWeight.GetItemWeight(bagSlot.Item) : -1;

                    BagSlot betterItem = InventoryManager.FilledArmorySlots
                        .Where(bs =>
                                   GetEquipUiCategory(bagSlot.Slot).Contains(bs.Item.EquipmentCatagory) &&
                                   bs.Item.IsValidForCurrentClass &&
                                   bs.Item.RequiredLevel <= Core.Me.ClassLevel &&
                                   bs.BagId != InventoryBagId.EquippedItems)
                        .OrderByDescending(r => ItemWeight.GetItemWeight(r.Item))
                        .FirstOrDefault();

                    if (betterItem == null || !betterItem.IsValid || !betterItem.IsFilled || betterItem == bagSlot || itemWeight >= ItemWeight.GetItemWeight(betterItem.Item)) continue;
                    armoryCount++;
                }

                if (armoryCount > 1)
                {
                    if (!RecommendEquip.Instance.IsOpen) AgentRecommendEquip.Instance.Toggle();
                    await Coroutine.Wait(3500, () => RecommendEquip.Instance.IsOpen);
                    RecommendEquip.Instance.Confirm();
                    await Coroutine.Sleep(800);
                }
            }

            foreach (var bagSlot in InventoryManager.EquippedItems)
            {
                if (!bagSlot.IsValid) continue;
                if (bagSlot.Slot == 0 && !bagSlot.IsFilled)
                {
                    Log("MainHand slot isn't filled. How?");
                    continue;
                }

                float itemWeight = bagSlot.IsFilled ? ItemWeight.GetItemWeight(bagSlot.Item) : -1;

                BagSlot betterItem = InventoryManager.FilledInventoryAndArmory
                    .Where(bs =>
                               GetEquipUiCategory(bagSlot.Slot).Contains(bs.Item.EquipmentCatagory) &&
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
                Item currentItem = bagSlot.Item;
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
            if (slotId == 2) return new List<ItemUiCategory> { ItemUiCategory.Head };
            if (slotId == 3) return new List<ItemUiCategory> { ItemUiCategory.Body };
            if (slotId == 4) return new List<ItemUiCategory> { ItemUiCategory.Hands };
            if (slotId == 5) return new List<ItemUiCategory> { ItemUiCategory.Waist };
            if (slotId == 6) return new List<ItemUiCategory> { ItemUiCategory.Legs };
            if (slotId == 7) return new List<ItemUiCategory> { ItemUiCategory.Feet };
            if (slotId == 8) return new List<ItemUiCategory> { ItemUiCategory.Earrings };
            if (slotId == 9) return new List<ItemUiCategory> { ItemUiCategory.Necklace };
            if (slotId == 10) return new List<ItemUiCategory> { ItemUiCategory.Bracelets };
            if (slotId == 11 || slotId == 12) return new List<ItemUiCategory> { ItemUiCategory.Ring };
            if (slotId == 13) return new List<ItemUiCategory> { ItemUiCategory.Soul_Crystal };
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
                await StopBusy(leaveDuty: false, stopFishing: false, dismount: false);
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
                        Core.Memory.CallInjected64<IntPtr>(repairWindow,
                                                           new object[4]
                                                           {
                                                               ff14bot.Managers.AgentModule.GetAgentInterfaceById(AgentId).Pointer,
                                                               0,
                                                               0,
                                                               repairVendor
                                                           });
                    }

                    await Coroutine.Wait(1500, () => Repair.IsOpen);
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
            uint[] privateHousing = new uint[] { 59, 60, 61, 97 };
            uint[] FCHousing = new uint[] { 56, 57, 58, 96 };

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

            var entranceIds = new uint[] { houseEntranceId, aptEntranceId };

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

        public static async Task TurninOddlyDelicate()
        {
            Dictionary<uint, CraftingRelicTurnin> TurnItemList = new Dictionary<uint, CraftingRelicTurnin>
            {
                // BaseItemID, Tab, Index, Mini Collectability, RewardItemID
                { 31750, new CraftingRelicTurnin(31750, 0, 0, 2500, 31736) }, //Carpenter Oddly Delicate Pine Lumber --> Oddly Delicate Saw Part
                { 31751, new CraftingRelicTurnin(31751, 1, 0, 2500, 31737) }, //Blacksmith Oddly Delicate Silver gear --> Oddly Delicate Cross-pein Hammer part
                { 31752, new CraftingRelicTurnin(31752, 2, 0, 2500, 31738) }, //Armorer Oddly Delicate Wolfram Square --> Oddly Delicate Raising Hammer part
                { 31753, new CraftingRelicTurnin(31753, 3, 0, 2500, 31739) }, //Goldsmith Oddly Delicate Celestine --> Oddly Delicate Lapidary Hammer Part
                { 31754, new CraftingRelicTurnin(31754, 4, 0, 2500, 31740) }, //Leatherworker Oddly Delicate Gazelle Leather --> Oddly Delicate Round Knife Part
                { 31755, new CraftingRelicTurnin(31755, 5, 0, 2500, 31741) }, //Weaver Oddly Delicate Rhea Cloth --> Oddly Delicate Needle Part
                { 31756, new CraftingRelicTurnin(31756, 6, 0, 2500, 31742) }, //Alchemist Oddly Delicate Holy Water --> Oddly Delicate Alembic Part
                { 31757, new CraftingRelicTurnin(31757, 7, 0, 2500, 31743) }, //Cooking Oddly Delicate Shark Oil --> Oddly Delicate Frypan Part
                { 31768, new CraftingRelicTurnin(31768, 8, 0, 400, 31746) }, //Mining Oddly Delicate Adamantite Ore --> Oddly Delicate Pickaxe Part
                { 31766, new CraftingRelicTurnin(31766, 9, 0, 400, 31744) }, //Botany Oddly Delicate Feather --> Oddly Delicate Hatchet Part
                { 31770, new CraftingRelicTurnin(31770, 10, 0, 126, 31748) }, //Fishing Flinstrike --> Oddly Delicate Fishing Rod part
                { 31771, new CraftingRelicTurnin(31771, 10, 1, 62, 31749) } //Fishing Pickled Pom --> Oddly Delicate Fishing Reel part
            };

            var collectables = InventoryManager.FilledSlots.Where(i => i.IsCollectable).Select(x => x.RawItemId).Distinct();
            var collectablesAll = InventoryManager.FilledSlots.Where(i => i.IsCollectable);
            var npcId = GameObjectManager.GetObjectByNPCId(1035014);

            if (collectables.Any(i => TurnItemList.Keys.Contains(i)))
            {
                Log("Have collectables");
                foreach (var collectable in collectablesAll)
                {
                    if (TurnItemList.Keys.Contains(collectable.RawItemId))
                    {
                        var turnin = TurnItemList[collectable.RawItemId];
                        if (collectable.Collectability < turnin.MinCollectability)
                        {
                            Log($"Discarding {collectable.Name} is at {collectable.Collectability} which is under {turnin.MinCollectability}");
                            collectable.Discard();
                        }
                    }
                }

                collectables = InventoryManager.FilledSlots.Where(i => i.IsCollectable).Select(x => x.RawItemId).Distinct();

                if (!npcId.IsWithinInteractRange)
                {
                    var _target = npcId.Location;
                    Navigator.PlayerMover.MoveTowards(_target);
                    while (_target.Distance2D(Core.Me.Location) >= 4)
                    {
                        Navigator.PlayerMover.MoveTowards(_target);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                }

                npcId.Interact();

                await Coroutine.Wait(10000, () => SelectIconString.IsOpen);

                if (!SelectIconString.IsOpen)
                {
                    npcId.Interact();
                    await Coroutine.Wait(10000, () => SelectIconString.IsOpen);
                }

                await Buddy.Coroutines.Coroutine.Sleep(500);
                {
                    Logging.WriteDiagnostic("Choosing 'Oddly Delicate Materials Exchange'.");
                    ff14bot.RemoteWindows.SelectIconString.ClickSlot(0);
                }

                await Coroutine.Wait(10000, () => CollectablesShop.Instance.IsOpen);


                if (CollectablesShop.Instance.IsOpen)
                {
                    // Log("Window open");
                    foreach (var item in collectables)
                    {
                        Log($"Turning in {DataManager.GetItem(item).CurrentLocaleName}");
                        var turnin = TurnItemList[item];

                        // Log($"Pressing job {turnin.Job}");
                        CollectablesShop.Instance.SelectJob(turnin.Job);
                        await Coroutine.Sleep(500);
                        //  Log($"Pressing position {turnin.Position}");
                        CollectablesShop.Instance.SelectItem(turnin.Position);
                        await Coroutine.Sleep(1000);
                        int i = 0;
                        while (CollectablesShop.Instance.TurninCount > 0)
                        {
                            // Log($"Pressing trade {i}");
                            i++;
                            CollectablesShop.Instance.Trade();
                            await Coroutine.Sleep(100);
                        }
                    }

                    CollectablesShop.Instance.Close();
                    await Coroutine.Wait(10000, () => !CollectablesShop.Instance.IsOpen);
                }
            }
        }

        public static async Task TurninResplendentCrafting()
        {
            Dictionary<uint, CraftingRelicTurnin> TurnItemList = new Dictionary<uint, CraftingRelicTurnin>
            {
                { 33162, new CraftingRelicTurnin(33162, 0, 2, 6300, 33194) },
                { 33163, new CraftingRelicTurnin(33163, 1, 2, 6300, 33195) },
                { 33164, new CraftingRelicTurnin(33164, 2, 2, 6300, 33196) },
                { 33165, new CraftingRelicTurnin(33165, 3, 2, 6300, 33197) },
                { 33166, new CraftingRelicTurnin(33166, 4, 2, 6300, 33198) },
                { 33167, new CraftingRelicTurnin(33167, 5, 2, 6300, 33199) },
                { 33168, new CraftingRelicTurnin(33168, 6, 2, 6300, 33200) },
                { 33169, new CraftingRelicTurnin(33169, 7, 2, 6300, 33201) },

                { 33170, new CraftingRelicTurnin(33170, 0, 1, 6500, 33202) },
                { 33171, new CraftingRelicTurnin(33171, 1, 1, 6500, 33203) },
                { 33172, new CraftingRelicTurnin(33172, 2, 1, 6500, 33204) },
                { 33173, new CraftingRelicTurnin(33173, 3, 1, 6500, 33205) },
                { 33174, new CraftingRelicTurnin(33174, 4, 1, 6500, 33206) },
                { 33175, new CraftingRelicTurnin(33175, 5, 1, 6500, 33207) },
                { 33176, new CraftingRelicTurnin(33176, 6, 1, 6500, 33208) },
                { 33177, new CraftingRelicTurnin(33177, 7, 1, 6500, 33209) },

                { 33178, new CraftingRelicTurnin(33178, 0, 0, 7000, 33210) },
                { 33179, new CraftingRelicTurnin(33179, 1, 0, 7000, 33211) },
                { 33180, new CraftingRelicTurnin(33180, 2, 0, 7000, 33212) },
                { 33181, new CraftingRelicTurnin(33181, 3, 0, 7000, 33213) },
                { 33182, new CraftingRelicTurnin(33182, 4, 0, 7000, 33214) },
                { 33183, new CraftingRelicTurnin(33183, 5, 0, 7000, 33215) },
                { 33184, new CraftingRelicTurnin(33184, 6, 0, 7000, 33216) },
                { 33185, new CraftingRelicTurnin(33185, 7, 0, 7000, 33217) }
            };

            var collectables = InventoryManager.FilledSlots.Where(i => i.IsCollectable).Select(x => x.RawItemId).Distinct();
            var collectablesAll = InventoryManager.FilledSlots.Where(i => i.IsCollectable);

            if (collectables.Any(i => TurnItemList.Keys.Contains(i)))
            {
                Log("Have collectables");
                foreach (var collectable in collectablesAll)
                {
                    if (TurnItemList.Keys.Contains(collectable.RawItemId))
                    {
                        var turnin = TurnItemList[collectable.RawItemId];
                        if (collectable.Collectability < turnin.MinCollectability)
                        {
                            Log($"Discarding {collectable.Name} is at {collectable.Collectability} which is under {turnin.MinCollectability}");
                            collectable.Discard();
                        }
                    }
                }

                collectables = InventoryManager.FilledSlots.Where(i => i.IsCollectable).Select(x => x.RawItemId).Distinct();

                var npc = GameObjectManager.GetObjectByNPCId(1027566);
                if (npc == null)
                {
                    await Navigation.GetTo(820, new Vector3(21.06303f, 82.05f, -14.24131f));
                    npc = GameObjectManager.GetObjectByNPCId(1027566);
                }

                if (npc != null && !npc.IsWithinInteractRange)
                {
                    await Navigation.GetTo(820, new Vector3(21.06303f, 82.05f, -14.24131f));
                }

                if (npc != null && npc.IsWithinInteractRange)
                {
                    npc.Interact();
                    await Coroutine.Wait(10000, () => Conversation.IsOpen);
                    if (Conversation.IsOpen)
                    {
                        Conversation.SelectLine((uint) 0);
                    }
                }

                await Coroutine.Wait(10000, () => CollectablesShop.Instance.IsOpen);


                if (CollectablesShop.Instance.IsOpen)
                {
                    // Log("Window open");
                    foreach (var item in collectables)
                    {
                        Log($"Turning in {DataManager.GetItem(item).CurrentLocaleName}");
                        var turnin = TurnItemList[item];

                        // Log($"Pressing job {turnin.Job}");
                        CollectablesShop.Instance.SelectJob(turnin.Job);
                        await Coroutine.Sleep(500);
                        //  Log($"Pressing position {turnin.Position}");
                        CollectablesShop.Instance.SelectItem(turnin.Position);
                        await Coroutine.Sleep(1000);
                        int i = 0;
                        while (CollectablesShop.Instance.TurninCount > 0)
                        {
                            // Log($"Pressing trade {i}");
                            i++;
                            CollectablesShop.Instance.Trade();
                            await Coroutine.Sleep(100);
                        }
                    }

                    CollectablesShop.Instance.Close();
                    await Coroutine.Wait(10000, () => !CollectablesShop.Instance.IsOpen);
                }
            }
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[GeneralFunctions] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}