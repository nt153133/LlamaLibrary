using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;
using static LlamaLibrary.Retainers.HelperFunctions;
using Action = System.Action;

namespace LlamaLibrary.Retainers
{
    public class RetainersPull : BotBase
    {
        private static readonly string botName = "Retainers";

        private bool _init = false;

        private Composite _root;

        private SettingsForm _settings;

        public RetainersPull()
        {
            // Task.Factory.StartNew(() =>
            //  {
            Init();
            _init = true;
            //     Log("INIT DONE");
            //   });
        }

        public override string Name
        {
            get
            {
#if RB_CN
                return "雇员拉";
#else
                return "Retainers";
#endif
            }
        }

        public override bool WantButton => true;

        public override string EnglishName => "Retainers";

        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        private static Lazy<List<RetainerTaskData>> VentureData;

        public override void Initialize()
        {
        }

        public override void OnButtonPress()
        {
            if (_settings == null || _settings.IsDisposed)
                _settings = new SettingsForm();
            try
            {
                _settings.Show();
                _settings.Activate();
            }
            catch (ArgumentOutOfRangeException ee)
            {
            }
        }

        private void Init()
        {
            OffsetManager.Init();

            Log("Load venture.json");
            VentureData = new Lazy<List<RetainerTaskData>>(() => loadResource<List<RetainerTaskData>>(Resources.Ventures));
            Log("Loaded venture.json");
        }

        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + botName + "] " + text, args);
            Logging.Write(Colors.MediumSlateBlue, msg);
        }

        private static void LogCritical(string text, params object[] args)
        {
            var msg = string.Format("[" + botName + "] " + text, args);
            Logging.Write(Colors.OrangeRed, msg);
        }

        public override void Start()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            _root = new ActionRunCoroutine(r => RetainerTest());
        }

        private async Task<bool> RetainerTest()
        {
            Log("====================Retainers=====================");

            await RetainerRun();

            return true;
        }

        public static async Task CheckVentureTask()
        {
            var verified = await VerifiedRetainerData();
            if (!verified) return;
            
            var count = await HelperFunctions.GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);
            var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            if (rets.Any(i => i.Active && i.VentureTask !=0 && (i.VentureEndTimestamp - now) <= 0 && SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.Venture) > 2))
            {
                if (FishingManager.State != FishingState.None)
                {
                    var quit = ActionManager.CurrentActions.Values.FirstOrDefault(i => i.Id == 299);
                    if (quit != default(SpellData))
                    {
                        Log($"Exiting Fishing");
                        if (ActionManager.CanCast(quit, Core.Me))
                        {
                            ActionManager.DoAction(quit, Core.Me);
                            await Coroutine.Wait(6000, () => FishingManager.State == FishingState.None);
                        }
                    }
                }

                if (CraftingLog.IsOpen)
                {
                    Log($"Closing Crafting Window");
                    CraftingLog.Close();
                    await Coroutine.Wait(6000, () => !CraftingLog.IsOpen);
                }
                
                if (DutyManager.InInstance)
                {
                    Log($"Leaving Diadem");
                    DutyManager.LeaveActiveDuty();

                    if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
                    {
                        await Coroutine.Yield();
                        await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
                        await Coroutine.Sleep(5000);
                    }
                }

                if (DutyManager.InInstance || CraftingLog.IsOpen || FishingManager.State != FishingState.None || MovementManager.IsOccupied)
                {
                    Log("Something went wrong");
                    return;
                }
                
                var bell = await GoToSummoningBell();

                if (bell == false)
                {
                    LogCritical("No summoning bell near by");
                    return;
                }
                await RetainerRoutine.ReadRetainers(RetainerCheckOnlyVenture);
            }
            else
            {
                Log("No Ventures Complete");
            }
        }

        public async Task RetainerRun()
        {
            var bell = await GoToSummoningBell();

            if (bell == false)
            {
                LogCritical("No summoning bell near by");
                TreeRoot.Stop("Done playing with retainers");
                return;
            }

            await RetainerRoutine.ReadRetainers(RetainerCheck);
            await Coroutine.Sleep(1000);
            if (!RetainerSettings.Instance.Loop || !RetainerSettings.Instance.ReassignVentures)
            {
                LogCritical($"Loop Setting {RetainerSettings.Instance.Loop} ReassignVentures {RetainerSettings.Instance.ReassignVentures}");
                TreeRoot.Stop("Done playing with retainers");
            }

            if (RetainerSettings.Instance.Loop && InventoryManager.FreeSlots < 2)
            {
                LogCritical($"I am overburdened....free up some space you hoarder");
                TreeRoot.Stop("Done playing with retainers");
            }

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            if (!rets.Any(i => i.VentureTask != 0 && i.Active))
            {
                LogCritical($"No ventures assigned or completed");
                TreeRoot.Stop("Done playing with retainers");
            }

            var nextVenture = rets.Where(i => i.VentureTask != 0 && i.Active).OrderBy(i => i.VentureEndTimestamp).First();
            if (nextVenture.VentureEndTimestamp == 0)
            {
                LogCritical($"No ventures running");
                TreeRoot.Stop("Done playing with retainers");
            }

            if (SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.Venture) <= 2)
            {
                LogCritical($"Get more venture tokens...bum");
                TreeRoot.Stop("Done playing with retainers");
            }

            var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var timeLeft = nextVenture.VentureEndTimestamp - now;

            Log($"Waiting till {RetainerInfo.UnixTimeStampToDateTime(nextVenture.VentureEndTimestamp)}");
            await Coroutine.Sleep(timeLeft * 1000);
            await Coroutine.Sleep(30000);
            Log($"{nextVenture.Name} Venture should be done");
        }

        public async Task<bool> RetainerCheck(RetainerInfo retainer)
        {
            if (RetainerSettings.Instance.ReassignVentures && retainer.Job != ClassJobType.Adventurer)
            {
                if (retainer.VentureTask != 0)
                {
                    var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    var timeLeft = retainer.VentureEndTimestamp - now;

                    if (timeLeft <= 0 && SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.Venture) > 2)
                    {
                        await RetainerHandleVentures();
                    }
                    else
                    {
                        Log($"Venture will be done at {RetainerInfo.UnixTimeStampToDateTime(retainer.VentureEndTimestamp)}");
                    }
                }
            }

            if (RetainerSettings.Instance.DepositFromPlayer) await RetainerRoutine.DumpItems();

            if (RetainerSettings.Instance.GetGil)
                GetRetainerGil();

            return true;
        }

        public static async Task<bool> RetainerCheckOnlyVenture(RetainerInfo retainer)
        {
            if (retainer.VentureTask != 0)
            {
                var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var timeLeft = retainer.VentureEndTimestamp - now;

                if (timeLeft <= 0 && SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.Venture) > 2)
                {
                    await RetainerHandleVentures();
                }
                else
                {
                    Log($"Venture will be done at {RetainerInfo.UnixTimeStampToDateTime(retainer.VentureEndTimestamp)}");
                }
            }
            
            return true;
        }

        public static async Task<bool> RetainerHandleVentures()
        {
            if (!SelectString.IsOpen)
            {
                return false;
            }

            if (SelectString.Lines().Contains(Translator.VentureCompleteText))
            {
                //Log("Venture Done");
                SelectString.ClickLineEquals(Translator.VentureCompleteText);

                await Coroutine.Wait(5000, () => RetainerTaskResult.IsOpen);

                if (!RetainerTaskResult.IsOpen)
                {
                    Log("RetainerTaskResult didn't open");
                    return false;
                }

                var taskId = AgentRetainerVenture.Instance.RetainerTask;

                var task = VentureData.Value.FirstOrDefault(i => i.Id == taskId);

                if (task != default(RetainerTaskData))
                {
                    Log($"Finished Venture {task.Name}");
                    Log($"Reassigning Venture {task.Name}");
                }
                else
                {
                    Log($"Finished Venture");
                    Log($"Reassigning Venture");
                }

                RetainerTaskResult.Reassign();

                await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen);
                if (!RetainerTaskAsk.IsOpen)
                {
                    Log("RetainerTaskAsk didn't open");
                    return false;
                }

                await Coroutine.Wait(2000, RetainerTaskAskExtensions.CanAssign);
                if (RetainerTaskAskExtensions.CanAssign())
                {
                    RetainerTaskAsk.Confirm();
                }
                else
                {
                    Log($"RetainerTaskAsk Error: {RetainerTaskAskExtensions.GetErrorReason()}");
                    RetainerTaskAsk.Close();
                }

                await Coroutine.Wait(1500, () => DialogOpen || SelectString.IsOpen);
                await Coroutine.Sleep(200);
                if (DialogOpen) Next();
                await Coroutine.Sleep(200);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
            }
            else
            {
                Log("Venture Not Done");
            }


            return true;
        }
    }
}