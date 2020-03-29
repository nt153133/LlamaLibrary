using System;
using System.Collections.Generic;
using System.Linq;
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
using ff14bot.Pathing.Service_Navigation;
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
            Task.Factory.StartNew(() =>
            {
                Init();
                _init = true;
                Log("INIT DONE");
            });
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

        private List<RetainerTaskData> VentureData { get; set; }

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
            VentureData = loadResource<List<RetainerTaskData>>(Resources.Ventures);
            Log("Loaded venture.json");
        }

        private static T loadResource<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + botName + "] " + text, args);
            Logging.Write(Colors.MediumSlateBlue, msg);
        }

        private void LogCritical(string text, params object[] args)
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

        public async Task RetainerRun()
        {
            var bell = NearestSummoningBell();

            if (bell == null)
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

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);
            var nextVenture = rets.Where(i => i.VentureTask != 0).OrderBy(i => i.VentureEndTimestamp).First();
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
                        await CheckVentures();
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

        public async Task<bool> CheckVentures()
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

                var task = VentureData.First(i => i.Id == taskId);

                Log($"Finished Venture {task.Name}");
                Log($"Reassigning Venture {task.Name}");

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

                await Coroutine.Wait(1500, () => DialogOpen);
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