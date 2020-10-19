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
using ff14bot.RemoteAgents;
using GreyMagic;
using LlamaLibrary.Memory;
using TreeSharp;

namespace LlamaLibrary.Helpers
{
    public static class GeneralFunctions
    {
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

                if (CraftingLog.IsOpen || CraftingManager.IsCrafting)
                {
                    Log($"Closing Crafting Window.");
                    await Lisbeth.ExitCrafting();
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
                    ActionManager.Dismount();
                    await Coroutine.Wait(3000, () => !Core.Me.IsMounted);
                }

                if (InSmallTalk) await SmallTalk();

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
            await Coroutine.Wait(1000, () => InSmallTalk);
            
            while (InSmallTalk)
            {
                await Coroutine.Yield();
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.ClickYes();
                }
                if (SelectString.IsOpen)
                {
                    RaptureAtkUnitManager.GetWindowByName("SelectString").SendAction(1, 3UL, (ulong)uint.MaxValue);
                    await Coroutine.Wait(1000, () => !SelectString.IsOpen);
                    if (SelectString.IsOpen)
                    {
                        if (SelectString.Lines().Contains("Cancel")) SelectString.ClickLineContains("Cancel");
                        else SelectString.ClickSlot(0);
                    }
                }
                if (SelectIconString.IsOpen)
                {
                    RaptureAtkUnitManager.GetWindowByName("SelectIconString").SendAction(1, 3UL, (ulong)uint.MaxValue);
                    await Coroutine.Wait(1000, () => !SelectIconString.IsOpen);
                    if (SelectIconString.IsOpen)
                    {
                        if (SelectIconString.Lines().Contains("Cancel")) SelectIconString.ClickLineContains("Cancel");
                        else SelectIconString.ClickSlot(0);
                    }
                }

                if (Talk.DialogOpen)
                {
                    Talk.Next();
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

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[GeneralFunctions] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}
