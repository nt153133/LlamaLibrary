using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using QuickGraph.Algorithms.Search;
using TreeSharp;

namespace LlamaLibrary
{
    public class OutOnALimbBase: BotBase
    {
        private Composite _root;
        public override string Name => "Out On A Limb";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = true;

        private MiniGameResult HitResult = MiniGameResult.None;
        
        Random _random = new Random();

        private int totalMGP = 0;
        
        Vector3 goldenSaucer = new Vector3(-2.129811f, 1.042503f, -8.071652f);

        List<Vector3> PlayLocations = new List<Vector3>(){
            new Vector3 (36.15812f, 0.00596046f, 28.72554f),
            new Vector3 (32.90435f, 0.006565332f, 26.9241f),
            new Vector3 (30.45348f, 0.007183194f, 27.53075f),
            new Vector3(26.95518f, 0.007746458f, 26.11792f),
            new Vector3(25.93522f, 0.008450985f, 21.96296f),
            new Vector3(23.81929f, 0.008272052f, 20.41006f)};
        
        private async Task<bool> Run()
        {
            var lang = (Language) typeof(DataManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .First(i => i.FieldType == typeof(Language)).GetValue(null);

            if (lang != Language.Eng)
            {
                TreeRoot.Stop("Only works on English Clients for now");
            }
                
            await StartOutOnLimb();
            //Logger.LogCritical("Start Done");
            await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist);
            //Logger.LogCritical("Ready");
            if (await PlayBotanist())
            {
                Logger.LogCritical("First win");
                do
                {
                    Logger.LogCritical("Loop");
                    if (!SelectYesno.IsOpen)
                    {
                        Logger.LogCritical("Waiting on window");
                        await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                        await Coroutine.Sleep(_random.Next(300,500));
                    }

                    if (SelectYesno.IsOpen && (GetDoubleDownInfo().Key <= 2 || GetDoubleDownInfo().Value < 15))
                    {
                        SelectYesno.No();
                        Logger.LogCritical("Click No");
                        await Coroutine.Sleep(_random.Next(300,500));
                        break;
                    }

                    if (SelectYesno.IsOpen && GetDoubleDownInfo().Key > 1 && GetDoubleDownInfo().Value > 15)
                    {
                        Logger.LogCritical("Click Yes");
                        SelectYesno.ClickYes();
                        await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist);
                        //await PlayBotanist();
                    }
                    else if (SelectYesno.IsOpen)
                    {
                        Logger.LogCritical("Click No");
                        SelectYesno.No();
                        await Coroutine.Sleep(_random.Next(300,500));
                        break;
                    }
                } while (await PlayBotanist());
                
                await Coroutine.Wait(5000, () => GoldSaucerReward.Instance.IsOpen);

                if (GoldSaucerReward.Instance.IsOpen)
                {
                    int gained = GoldSaucerReward.Instance.MGPReward;
                    totalMGP += gained;
                    Logger.LogCritical($"Won {gained} - Total {totalMGP}");

                    if (gained == 0)
                    {
                        Logger.LogCritical($"Won {gained}");
                        TreeRoot.Stop("Won zero...issue");
                    }
                }
                Logger.LogCritical("Starting over");
            }
            if (GoldSaucerReward.Instance.IsOpen)
                GoldSaucerReward.Instance.Close();
            
            await Coroutine.Wait(5000, () => !GoldSaucerReward.Instance.IsOpen);
            Logger.LogCritical("Done");

            //TreeRoot.Stop("Stop Requested");
            return true;
        }

        public override void Start()
        {
            GamelogManager.MessageRecevied += GamelogManagerOnMessageRecevied;
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
            _root = null;
        }

        public async Task<bool> StartOutOnLimb()
        {
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();
            
            
            uint npcid = 2005423;
            
            if (GameObjectManager.GetObjectByNPCId(npcid) == null)
            {
                Logger.LogCritical("Not Near Machine");
                if (WorldManager.ZoneId != 388)
                {
                    await GetToMinionSquare();
                    
                    var _target = new Vector3(32.61445f, 0.0005990267f, 18.66965f);
                    Navigator.PlayerMover.MoveTowards(_target);
                    while (_target.Distance2D(Core.Me.Location) >= _random.Next(1,6))
                    {
                        Navigator.PlayerMover.MoveTowards(_target);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                    
                    _target = PlayLocations[_random.Next(0, PlayLocations.Count - 1)];
                    Navigator.PlayerMover.MoveTowards(_target);
                    while (_target.Distance2D(Core.Me.Location) >= 4)
                    {
                        Navigator.PlayerMover.MoveTowards(_target);
                        await Coroutine.Sleep(100);
                    }

                    Navigator.PlayerMover.MoveStop();
                }


            }

            var station = GameObjectManager.GameObjects.Where(i => i.NpcId == 2005423).OrderBy(r=>r.Distance()).First();

            if (!station.IsWithinInteractRange)
            {
                var _target = station.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2D(Core.Me.Location) >= 4)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
            }

            station.Interact();

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot(0);

            await Coroutine.Wait(5000, () => MiniGameAimg.Instance.IsOpen);

            await Coroutine.Sleep(1000);
            
            AgentOutOnLimb.Instance.Refresh();
            
            await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyAimg);
            
            await Coroutine.Sleep(_random.Next(400,800));
            
            MiniGameAimg.Instance.PressButton();
            
            await Coroutine.Wait(5000, () => MiniGameBotanist.Instance.IsOpen);

            return MiniGameBotanist.Instance.IsOpen;
        }

        private async Task<bool> GetToMinionSquare()
        {
            if (!WorldManager.TeleportById(62))
            {
                //Logger.Error($"We can't get to {Constants.EntranceZone.CurrentLocaleAethernetName}. something is very wrong...");
                //TreeRoot.Stop();
                return false;
            }

            await Coroutine.Sleep(1000);
                    
            await Coroutine.Wait(10000, () => !Core.Me.IsCasting);
                    
            await Coroutine.Sleep(1000);
                    
            if (CommonBehaviors.IsLoading)
            {
                await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
            }

            await Coroutine.Wait(10000, () => WorldManager.ZoneId == 144);
                    
            await Coroutine.Wait(5000, () => GameObjectManager.GetObjectByNPCId(62) != null);
                    
            var unit = GameObjectManager.GetObjectByNPCId(62);

            unit.Target();
            unit.Interact();
            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            if (SelectString.IsOpen)
                SelectString.ClickLineContains("Aethernet");

            await Coroutine.Sleep(500);
                    
            await Coroutine.Wait(5000, () => SelectString.IsOpen);
            if (SelectString.IsOpen)
                SelectString.ClickLineContains("Minion");
                    
            await Coroutine.Sleep(1000);

            if (CommonBehaviors.IsLoading)
            {
                await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
            }

            await Coroutine.Sleep(3000);

            return true;
        }
        
        private void GamelogManagerOnMessageRecevied(object sender, ChatEventArgs e)
        {

            if (e.ChatLogEntry.MessageType == MessageType.SystemMessages)
            {
                if (e.ChatLogEntry.FullLine.IndexOf("hatchet", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Logger.Info("Ready");
                    Logger.Info(e.ChatLogEntry.FullLine);
                    
                }
            }

            //Hatchet Ready
            if (e.ChatLogEntry.MessageType == (MessageType) 2105)
            {
                if (e.ChatLogEntry.FullLine.IndexOf("nothing", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //Logger.Info("Not Close");
                    HitResult = MiniGameResult.NotClose;
                    GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
                }
                else if (e.ChatLogEntry.FullLine.IndexOf("something close", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //Logger.Info("Close");
                    HitResult = MiniGameResult.Close;
                    GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
                }
                else if (e.ChatLogEntry.FullLine.IndexOf("very", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //Logger.Info("Very Close");
                    HitResult = MiniGameResult.VeryClose;
                    GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
                }
                else if (e.ChatLogEntry.FullLine.IndexOf("right on", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //Logger.Info("On Top");
                    HitResult = MiniGameResult.OnTop;
                    GamelogManager.MessageRecevied -= GamelogManagerOnMessageRecevied;
                }
            }
            
        }


        private async Task<bool> PlayBotanist()
        {
            if (!MiniGameBotanist.Instance.IsOpen) return false;
            
            AgentOutOnLimb.Instance.Refresh();
            
            await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist);
            
            int lastVeryCloseLocation = -1;
            int lastCloseLocation = -1;
            int lastLocation = -1;

            //Logger.Info($"Progress {MiniGameBotanist.Instance.GetProgressLeft}");
            List<int> stops1 = new List<int>(){20, 60, 40, 80};
            stops1.Shuffle();
            foreach (var stopLoc in stops1)
            {
                if (MiniGameBotanist.Instance.IsOpen && MiniGameBotanist.Instance.GetNumberOfTriesLeft < 1)
                    return false;

                if (SelectYesno.IsOpen)
                    return true;

                var result = await StopAtLocation(stopLoc);
                bool stop = false;
                switch (result)
                {
                    case MiniGameResult.Error: Logger.LogCritical("Error"); return false;
                    case MiniGameResult.OnTop: return true;
                    case MiniGameResult.DoubleDown: return true;
                    case MiniGameResult.VeryClose: 
                        stop = true;
                        lastVeryCloseLocation = stopLoc;
                        break;
                    case MiniGameResult.Close:
                        lastCloseLocation = stopLoc;
                        stop = true;
                        break;
                }

                lastLocation = stopLoc;
                //Logger.Info($"Progress {MiniGameBotanist.Instance.GetProgressLeft}");
                await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist);
                await Coroutine.Sleep(_random.Next(100,200));
                if (stop)
                    break;
            }
            // Logger.LogCritical("endFor");
            if (lastCloseLocation > 1 && lastVeryCloseLocation < 1)
            {
                List<int> stops = new List<int>(){lastCloseLocation -12, lastCloseLocation +12, lastCloseLocation -7, lastCloseLocation +17};
                
                foreach (var stopLoc in stops)
                {
                    if (MiniGameBotanist.Instance.IsOpen && MiniGameBotanist.Instance.GetNumberOfTriesLeft < 1)
                        return false;

                    if (SelectYesno.IsOpen)
                        return true;
                
                    var result = await StopAtLocation(stopLoc);
                    bool stop = false;
                    switch (result)
                    {
                        case MiniGameResult.Error: Logger.LogCritical("Error"); return false;
                        case MiniGameResult.OnTop:      return true;
                        case MiniGameResult.DoubleDown: return true;
                        case MiniGameResult.VeryClose: 
                            stop = true;
                            lastVeryCloseLocation = stopLoc;
                            break;
                        case MiniGameResult.Close:
                            lastCloseLocation = stopLoc;
                            break;
                    }

                    lastLocation = stopLoc;
                    Logger.Info($"Progress {MiniGameBotanist.Instance.GetProgressLeft} stop {stopLoc}");
                    await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist);
                    await Coroutine.Sleep(_random.Next(100,200));
                    if (stop)
                        break;
                }
            }

            if (lastCloseLocation < 1 && lastVeryCloseLocation < 1)
            {
                List<int> stops = new List<int>(){5, 95, 50, 10, 70, 30,0};
                stops.Shuffle();
                foreach (var stopLoc in stops)
                {
                    if (MiniGameBotanist.Instance.IsOpen && MiniGameBotanist.Instance.GetNumberOfTriesLeft < 1)
                        return false;

                    if (SelectYesno.IsOpen)
                        return true;
                
                    var result = await StopAtLocation(stopLoc);
                    bool stop = false;
                    switch (result)
                    {
                        case MiniGameResult.Error: Logger.LogCritical("Error"); return false;
                        case MiniGameResult.OnTop:      return true;
                        case MiniGameResult.DoubleDown: return true;
                        case MiniGameResult.VeryClose: 
                            stop = true;
                            lastVeryCloseLocation = stopLoc;
                            break;
                        case MiniGameResult.Close:
                            lastCloseLocation = stopLoc;
                            break;
                    }

                    lastLocation = stopLoc;
                    //Logger.Info($"Progress {MiniGameBotanist.Instance.GetProgressLeft}");
                    if (MiniGameBotanist.Instance.GetProgressLeft == 0)
                        return true;
                    await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist|| SelectYesno.IsOpen);
                    await Coroutine.Sleep(_random.Next(100,200));
                    if (stop)
                        break;
                }
            }
            
            Logger.Info($"Last Location {lastLocation}");
            
            Logger.Info($"Last Close Location {lastCloseLocation}");

            if (lastVeryCloseLocation > 1)
            {
                Logger.Info($"Very Close location {lastVeryCloseLocation}");
                while (MiniGameBotanist.Instance.GetProgressLeft > 0 || !SelectYesno.IsOpen)
                {
                    var result = await StopAtLocation(_random.Next(lastVeryCloseLocation -2, lastVeryCloseLocation +2));
                    //Logger.Info($"Progress {MiniGameBotanist.Instance.GetProgressLeft} result {result}");
                    if (MiniGameBotanist.Instance.GetProgressLeft == 0)
                        return true;
                    await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist || SelectYesno.IsOpen);
                    await Coroutine.Sleep(_random.Next(100, 300));
                }
            }

            if (MiniGameBotanist.Instance.GetProgressLeft == 0)
            {
                await Coroutine.Sleep(_random.Next(100,200));
                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
            }

            return SelectYesno.IsOpen;
        }

        private async Task<MiniGameResult> StopAtLocation(int location)
        {
            if (!AgentOutOnLimb.Instance.IsReadyBotanist)
                 await Coroutine.Wait(5000, () => AgentOutOnLimb.Instance.IsReadyBotanist);
            
            if (!MiniGameBotanist.Instance.IsOpen || !AgentOutOnLimb.Instance.IsReadyBotanist || MiniGameBotanist.Instance.GetNumberOfTriesLeft < 1) return MiniGameResult.Error;

            if (!AgentOutOnLimb.Instance.CursorLocked)
            {
                
                Logger.Info("Lock Cursor");
                MiniGameBotanist.Instance.PauseCursor();
                await Coroutine.Sleep(200);
            }

            AgentOutOnLimb.Instance.CursorLocation = location;
            
            GamelogManager.MessageRecevied += GamelogManagerOnMessageRecevied;
            HitResult = MiniGameResult.None;
            await Coroutine.Sleep(100);
            MiniGameBotanist.Instance.PressButton();
            int timeleft = MiniGameBotanist.Instance.GetTimeLeft * 1000;
            await Coroutine.Wait(timeleft, () => HitResult != MiniGameResult.None || SelectYesno.IsOpen);
            
            return SelectYesno.IsOpen ? MiniGameResult.DoubleDown : HitResult;
        }
        

        public static KeyValuePair<int, int> GetDoubleDownInfo()
        {
            Regex OpportunitiesRegex = new Regex(@".* opportunities remaining: (\d)", RegexOptions.Compiled);

            Regex TimeRegex = new Regex(@"Time Remaining: (\d):(\d+).*", RegexOptions.Compiled);

            int offset0 = 458;
            int offset2 = 352;
            int count = 0;
            int sec = 0;

            AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("SelectYesno");
            if (windowByName != null)
            {
                ushort elementCount = Core.Memory.Read<ushort>(windowByName.Pointer + offset0);

                IntPtr addr = Core.Memory.Read<IntPtr>(windowByName.Pointer + offset2);
                var elements = Core.Memory.ReadArray<TwoInt>(addr, elementCount);

                var data = Core.Memory.ReadString((IntPtr) elements[0].Data, Encoding.UTF8);

                foreach (var line in data.Split('\n').Skip(5))
                {
                    if (OpportunitiesRegex.IsMatch(line))
                    {
                        count = int.Parse(OpportunitiesRegex.Match(line).Groups[1].Value.Trim());
                    }
                    else if (TimeRegex.IsMatch(line))
                    {
                        sec = int.Parse(TimeRegex.Match(line).Groups[2].Value.Trim());
                    }
                }
            }
            
            return new KeyValuePair<int, int>(count, sec);
        }
        
        
        
    }
    
    
}