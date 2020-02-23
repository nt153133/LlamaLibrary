using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary
{
    public class HouseChecker : BotBase
    {
        private Composite _root;

        public override string Name => "Housing Checker";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;


        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
        }

        private async Task<bool> Run()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

            var output = new List<string>();

            if (ConditionParser.HasAetheryte(2))
                output.AddRange(await GetLavenderPlots());

            if (ConditionParser.HasAetheryte(8))
                output.AddRange(await GetMistsPlots());

            if (ConditionParser.HasAetheryte(9))
                output.AddRange(await GetGobletPlots());

            if (ConditionParser.HasAetheryte(111))
                output.AddRange(await GetShiroganePlots());

            foreach (var line in output)
            {
                if (line.Contains("Small"))
                    Log($"{line}");
                else if (line.Contains("Medium"))
                {
                    Log1($"{line}");
                }
                else if (line.Contains("Large"))
                {
                    Log1($"{line}");
                }
                else
                {
                    Log($"{line}");
                }
            }

            TreeRoot.Stop("Stop Requested");
            return true;
        }


        public static async Task<List<string>> GetMistsPlots()
        {
            if (ConditionParser.IsQuestCompleted(66750))
                await GetToResidential(8);
            else
                await GetToMistsWindow();

            if (!SelectString.IsOpen)
                return new List<string>();

            await OpenHousingWards();
            var list = await HousingWards();

            if (ConditionParser.IsQuestCompleted(66750))
                await CloseHousingWardsNoLoad();
            else
                await CloseHousingWards();

            return list;
        }

        public static async Task<List<string>> GetLavenderPlots()
        {
            if (ConditionParser.IsQuestCompleted(66748))
                await GetToResidential(2);
            else
                await GetToLavenderWindow();

            if (!SelectString.IsOpen)
                return new List<string>();

            await OpenHousingWards();
            var list = await HousingWards();

            if (ConditionParser.IsQuestCompleted(66748))
                await CloseHousingWardsNoLoad();
            else
                await CloseHousingWards();

            return list;
        }

        public static async Task<List<string>> GetGobletPlots()
        {
            if (ConditionParser.IsQuestCompleted(66749))
                await GetToResidential(9);
            else
                await GetToGobletWindow();

            if (!SelectString.IsOpen)
                return new List<string>();

            await OpenHousingWards();
            var list = await HousingWards();

            if (ConditionParser.IsQuestCompleted(66749))
                await CloseHousingWardsNoLoad();
            else
                await CloseHousingWards();

            return list;
        }

        public static async Task<List<string>> GetShiroganePlots()
        {
            if (ConditionParser.IsQuestCompleted(68167))
                await GetToResidential(111);
            else
                await GetToShiroganeWindow();

            if (!SelectString.IsOpen)
                return new List<string>();

            await OpenHousingWards();
            var list = await HousingWards();

            if (ConditionParser.IsQuestCompleted(68167))
                await CloseHousingWardsNoLoad();
            else
                await CloseHousingWards();

            return list;
        }

        private static async Task GetToResidential(uint aetheryteId)
        {
            if (!ConditionParser.HasAetheryte(aetheryteId)) return;

            if (!WorldManager.TeleportById(aetheryteId)) return;

            do
            {
                await Coroutine.Sleep(2000);
            } while (Core.Me.IsCasting);

            if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);

            await Coroutine.Wait(10000, () => GameObjectManager.GetObjectByNPCId(aetheryteId) != null);
            await Coroutine.Sleep(2000);

            var unit = GameObjectManager.GetObjectByNPCId(aetheryteId);

            if (!unit.IsWithinInteractRange)
            {
                var target = unit.Location;
                Navigator.PlayerMover.MoveTowards(target);
                while (!unit.IsWithinInteractRange)
                {
                    Navigator.PlayerMover.MoveTowards(target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            unit.Target();
            unit.Interact();

            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            if (SelectString.IsOpen)
                SelectString.ClickLineContains("Residential");

            await Coroutine.Sleep(500);
            await Coroutine.Wait(5000, () => SelectString.IsOpen);
        }

        private static async Task GetToLavenderWindow()
        {
            await Navigation.GetTo(133, new Vector3(180.3376f, -2.291125f, -240.7838f));

            uint FerryNpc = 1001263;

            var unit = GameObjectManager.GetObjectByNPCId(FerryNpc);

            if (!unit.IsWithinInteractRange)
            {
                var target = unit.Location;
                Navigator.PlayerMover.MoveTowards(target);
                while (!unit.IsWithinInteractRange)
                {
                    Navigator.PlayerMover.MoveTowards(target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            unit.Target();
            unit.Interact();

            await Coroutine.Wait(5000, () => SelectIconString.IsOpen);

            if (SelectIconString.IsOpen)
            {
                SelectIconString.ClickLineContains("Lavender Beds");

                await Coroutine.Wait(5000, () => DialogOpen);
            }

            if (DialogOpen) Next();

            await Coroutine.Wait(3000, () => SelectString.IsOpen);
        }

        private static async Task OpenHousingWards()
        {
            if (SelectString.IsOpen)
            {
                SelectString.ClickLineContains("Go to specified");

                await Coroutine.Wait(5000, () => HousingSelectBlock.Instance.IsOpen);
            }
        }

        private static async Task GetToGobletWindow()
        {
            await Navigation.GetTo(140, new Vector3(317.0663f, 67.27534f, 232.8395f));

            var zoneChange = new Vector3(316.7798f, 67.13619f, 236.8774f);

            while (!SelectString.IsOpen)
            {
                Navigator.PlayerMover.MoveTowards(zoneChange);
                await Coroutine.Sleep(50);
                Navigator.PlayerMover.MoveStop();
            }

            Navigator.PlayerMover.MoveStop();
        }

        private static async Task GetToMistsWindow()
        {
            await Navigation.GetTo(135, new Vector3(597.4801f, 61.59979f, -110.7737f));

            var zoneChange = new Vector3(598.1823f, 61.52054f, -108.3216f);

            while (!SelectString.IsOpen)
            {
                Navigator.PlayerMover.MoveTowards(zoneChange);
                await Coroutine.Sleep(50);
                Navigator.PlayerMover.MoveStop();
            }

            Navigator.PlayerMover.MoveStop();
        }

        private static async Task GetToShiroganeWindow()
        {
            await Navigation.GetTo(628, new Vector3(-116.2294f, -7.010099f, -40.55866f));

            uint FerryNpc = 1019006;

            var unit = GameObjectManager.GetObjectByNPCId(FerryNpc);

            if (!unit.IsWithinInteractRange)
            {
                var target = unit.Location;
                Navigator.PlayerMover.MoveTowards(target);
                while (!unit.IsWithinInteractRange)
                {
                    Navigator.PlayerMover.MoveTowards(target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            unit.Target();
            unit.Interact();

            await Coroutine.Wait(5000, () => DialogOpen);

            if (DialogOpen) Next();

            await Coroutine.Wait(3000, () => SelectString.IsOpen);
        }

        private static async Task CloseHousingWards()
        {
            if (HousingSelectBlock.Instance.IsOpen)
            {
                HousingSelectBlock.Instance.Close();

                await Coroutine.Wait(5000, () => SelectString.IsOpen);

                if (SelectString.IsOpen)
                {
                    SelectString.ClickSlot((uint) (SelectString.LineCount - 1));
                    await Coroutine.Wait(5000, () => !SelectString.IsOpen);
                }

                await Coroutine.Sleep(500);

                if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
            }
        }

        private static async Task CloseHousingWardsNoLoad()
        {
            if (HousingSelectBlock.Instance.IsOpen)
            {
                HousingSelectBlock.Instance.Close();

                await Coroutine.Wait(5000, () => SelectString.IsOpen);

                if (SelectString.IsOpen)
                {
                    SelectString.ClickSlot((uint) (SelectString.LineCount - 1));
                    await Coroutine.Wait(5000, () => !SelectString.IsOpen);
                }

                await Coroutine.Sleep(500);
            }
        }

        private static async Task<List<string>> HousingWards()
        {
            var output = new List<string>();
            if (HousingSelectBlock.Instance.IsOpen)
                for (var i = 0; i < HousingSelectBlock.Instance.NumberOfWards; i++)
                {
                    HousingSelectBlock.Instance.SelectWard(i);

                    await Coroutine.Sleep(500);

                    var plotStatus = AgentHousingSelectBlock.Instance.ReadPlots(HousingSelectBlock.Instance.NumberOfPlots);

                    for (var j = 0; j < plotStatus.Length; j++)
                    {
                        if (plotStatus[j] == 0)
                        {
                            var price = HousingSelectBlock.Instance.PlotPrice(j);
                            var size = "";

                            var bytes = Encoding.ASCII.GetBytes(HousingSelectBlock.Instance.PlotString(j).Split(' ')[1]);
                            if (bytes.Length > 9)
                                switch (bytes[9])
                                {
                                    case 72:
                                        size = " (Small) ";
                                        break;
                                    case 1:
                                        size = " (Medium) ";
                                        break;
                                    case 2:
                                        size = " (Large) ";
                                        break;
                                }

                            //Log($"{HousingSelectBlock.Instance.HousingWard} Plot {j+1} {size} -  {price}");
                            output.Add($"{HousingSelectBlock.Instance.HousingWard} Plot {j + 1} {size} -  {price}");
                        }
                    }
                    
                    await Coroutine.Sleep(200);
                }

            return output;
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Pink, msg);
        }
        
        private void Log1(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Yellow, msg);
        }
        
        private void Log2(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Red, msg);
        }
    }
}