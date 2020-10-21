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
using ff14bot.Navigation;
using ff14bot.RemoteAgents;
using LlamaLibrary.Memory;
using LlamaLibrary.Retainers;

namespace LlamaLibrary.Helpers
{
    public static class GeneralFunctions
    {
        static bool IsJumping => Core.Memory.NoCacheRead<byte>(Offsets.Conditions + Offsets.JumpingCondition) != 0;
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

        public static async Task SmallTalk()
        {
            await Coroutine.Wait(500, () => InSmallTalk);
            
            while (InSmallTalk)
            {
                await Coroutine.Yield();
                
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.ClickYes();
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

                if (Talk.DialogOpen)
                {
                    while (Talk.DialogOpen)
                    {
                        Talk.Next();
                        await Coroutine.Wait(100, () => !Talk.DialogOpen);
                        await Coroutine.Wait(100, () => Talk.DialogOpen);
                        await Coroutine.Yield();
                    }
                }

                if (JournalAccept.IsOpen)
                {
                    JournalAccept.Decline();
                }
                
                if (QuestLogManager.InCutscene)
                {
                    AgentCutScene.Instance.PromptSkip();
                    if (AgentCutScene.Instance.CanSkip && SelectString.IsOpen)
                    {
                        SelectString.ClickSlot(0);
                    }
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
                    RaptureAtkUnitManager.GetWindowByName(windowName).SendAction(1, 3UL, (ulong)uint.MaxValue);
                }
                
                await Coroutine.Wait(300, () => RaptureAtkUnitManager.GetWindowByName(windowName) == null);
                await Coroutine.Wait(300, () => RaptureAtkUnitManager.GetWindowByName(windowName) != null);
                await Coroutine.Yield();
            }

            return RaptureAtkUnitManager.GetWindowByName(windowName) == null;
        }

        public static IEnumerable<BagSlot> NonGearSetItems()
        {
            return InventoryManager.FilledArmorySlots.Where(bs => !GearsetManager.GearSets.SelectMany(gs => gs.Gear).Select(g => g.Item).Contains(bs.Item));
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

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[GeneralFunctions] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}
