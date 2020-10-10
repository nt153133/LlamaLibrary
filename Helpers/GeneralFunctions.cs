using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LlamaLibrary.Helpers
{
    class GeneralFunctions
    {
        public static async Task<bool> StopCrafting()
        {
            for (int tryStep = 1; tryStep < 6; tryStep++)
            {
                if (!(DutyManager.InInstance || CraftingLog.IsOpen || FishingManager.State != FishingState.None || MovementManager.IsOccupied || CraftingManager.IsCrafting)) break;

                Log($"We're occupied. Trying to exit out. Attempt #{tryStep}");

                if (FishingManager.State != FishingState.None)
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

                if (DutyManager.InInstance)
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

                await Coroutine.Sleep(2500);
            }

            if (DutyManager.InInstance || CraftingLog.IsOpen || FishingManager.State != FishingState.None || MovementManager.IsOccupied || CraftingManager.IsCrafting)
            {
                Log("Something went wrong, we're still occupied.");
                TreeRoot.Stop("Stopping bot.");
                return false;
            }
            return true;
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[GeneralFunctions] " + text, args);
            Logging.Write(Colors.Aquamarine, msg);
        }
    }
}
