using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using Clio.Utilities.Helpers;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using GreyMagic;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using TreeSharp;
using Action = System.Action;

namespace LlamaLibrary
{
    public class GatherTest : BotBase
    {
        
       // public static InstanceContentDirector Director => DirectorManager.ActiveDirector as InstanceContentDirector;
       public int TimerOffset;

       public TimeSpan TimeLeftInDiadem => TimeSpan.FromSeconds(Core.Memory.Read<float>(DirectorManager.ActiveDirector.Pointer + TimerOffset)); //640
        //Wind new Vector3(greenf), BTN
        private static readonly Vector3[] umbralTempest =
        {
            new Vector3(-580.9533f, 330.8294f, 458.0585f),
            new Vector3(-608.1638f, 329.7346f, 456.2433f),
            new Vector3(-606.6253f, 331.0921f, 427.3463f),
            new Vector3(-579.6904f, 329.4015f, 424.7657f)
        };
        
        //Red Flare Mining
        private static readonly Vector3[] umbralFlare =
        {
            new Vector3(-430.0615f, 320.2874f, -572.0226f),
            new Vector3(-453.1085f, 320.7165f, -594.9066f),
            new Vector3(-431.1396f, 320.0077f, -609.3666f),
            new Vector3(-408.5051f, 319.256f, -594.2559f)
        };

        //Purple Mining Levin
        private static readonly Vector3[] umbralLevin =
        {
            new Vector3(619.6818f, 251.9896f, -423.0514f),
            new Vector3(613.7454f, 251.9896f, -396.2267f),
            new Vector3(644.903f, 251.9896f, -403.8661f),
            new Vector3(635.2657f, 252.35f, -420.4232f)
        };


        //ORange btn
        private static readonly Vector3[] umbralDuststorm =
        {
            new Vector3(408.5911f, 293.0109f, 604.5204f),
            new Vector3(409.6448f, 293.4354f, 568.1148f),
            new Vector3(379.7924f, 292.971f, 569.1271f)
        };
        
        private static readonly Vector3[] afkSpots2 =
        {
            new Vector3(60.57302f, 246.5133f, -290.3074f),
            new Vector3(168.4951f, 170.8651f, 72.97058f),
            new Vector3(-9.105042f, 158.3274f, 129.1192f)
        };
        
        private static readonly Vector3[] afkSpots =
        {
//-182.1712, 0.8445628, -298.0846
            new Vector3(-182.1712f, 0.8445628f, -298.0846f),
            
            new Vector3(214.7894f, -144.3608f, -267.1053f),
            
            new Vector3(318.1068f, -4.934579f, 408.4625f)
        };
        
        //Above orange
        private static readonly Vector3 umbralDuststormAbove = new Vector3(409.9393f, 314.985f, 560.1671f);
        
        //above green
        private static readonly Vector3 umbralTempestAbove = new Vector3(-578.7819f, 349.9304f, 462.1082f);
        
        //above purple
        private static readonly Vector3 umbralLevinAbove = new Vector3(602.6203f, 276.4863f, -394.3397f);

        //above red
        private static readonly Vector3 umbralFlareAbove = new Vector3(-396.0904f, 340.3934f, -557.02f);
        
        //above start
        private static readonly Vector3 startAbove = new Vector3(-645.4365f, 300.4301f, -151.262f);

        private static Vector3 standBy = new Vector3(-164.3966f, -1.072426f, -302.2528f);
        
        private static int[] weatherNodes = new[] {33229, 33230, 33231, 33232,33584, 33585, 33586, 33587};

        private static int lastWeather = 0;

        private static WaitTimer lastChange;

        private Composite _root;

        public GatherTest()
        {
            /*if (Translator.Language == Language.Chn)
            {
                TimerOffset = 0x640;
            }
            else
            {*/
                TimerOffset = 0x650;
          //  }
        }

        public override string Name => "DiademGather";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;


        public override void Start()
        {
            lastWeather = 0;
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            lastWeather = 0;
            _root = null;
        }

        private async Task<bool> Run()
        {
            //await LeveWindow(1018997);
            //await HousingWards();
           Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            await GatherWeatherNodes();
            //Navigator.PlayerMover = new SlideMover();
            //Navigator.NavigationProvider = new ServiceNavigationProvider();


            //TreeRoot.Stop("Stop Requested");
            return true;
        }

        public async Task GatherWeatherNodes()
        {

            if (WorldManager.RawZoneId != 901)
            {
                await EnterDiadem();
                Log($"Waiting for instance time");
                await Coroutine.Wait(5000, () => TimeLeftInDiadem.TotalMinutes > 1);
                Log($"Time left {TimeLeftInDiadem:hh\\:mm\\:ss}");
            }

            lastChange = new WaitTimer(new TimeSpan(0,7,0));
            Log($"Current Weather: {WorldManager.CurrentWeather}  {WorldManager.CurrentWeatherId}");
            
            Random time = new Random();

            int minutes = time.Next(10, 20);
            int sec = time.Next(0, 59);
            standBy = afkSpots[time.Next(0, afkSpots.Length)];
            
            while (TimeLeftInDiadem > new TimeSpan(0,minutes,sec))
            {
                switch (WorldManager.CurrentWeatherId)
                {
                    case 133:
                        if (lastWeather == 133) break;
                        //await FlyTo(new Vector3(-295.9257f, 268.4518f, -370.327f)); 
                        await MineWeather(ClassJobType.Miner, umbralFlareAbove, umbralFlare);
                        standBy = afkSpots[time.Next(0, afkSpots.Length)];
                        await StandBy();
                        break;
                    case 135: if (lastWeather == 135) break; await MineWeather(ClassJobType.Miner, umbralLevinAbove, umbralLevin); 
                        standBy = afkSpots[time.Next(0, afkSpots.Length)];
                        await StandBy();
                        break;
                    case 134: if (lastWeather == 134) break; await MineWeather(ClassJobType.Botanist, umbralDuststormAbove, umbralDuststorm);
                        standBy = afkSpots[time.Next(0, afkSpots.Length)];
                        await StandBy();
                        break;
                    case 136: if (lastWeather == 136) break; await MineWeather(ClassJobType.Botanist, umbralTempestAbove, umbralTempest);
                        standBy = afkSpots[time.Next(0, afkSpots.Length)];
                        await StandBy();
                        break;
                    default: await Coroutine.Sleep(1000);
                        if (lastChange.IsFinished)
                        {
                            lastChange.Reset();
                            lastWeather = 0;
                        }

                        await StandBy(); break;
                } 
                
                await Coroutine.Sleep(1000);
            }

            if (DutyManager.InInstance)
            {
                Log($"Out of time: {TimeLeftInDiadem:hh\\:mm\\:ss} Left");
                DutyManager.LeaveActiveDuty();

                if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
                {
                    await Coroutine.Yield();
                    await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
                    await Coroutine.Sleep(10000);
                }
            }
          
           
        }
        
        public async Task UseCordial()
        {
            if (Core.Me.CurrentGP < 500)
            {
                await Coroutine.Sleep(2000);
                var cordial = InventoryManager.FilledSlots.FirstOrDefault(i => i.RawItemId == 12669);

                if (cordial != default(BagSlot))
                {
                    cordial.UseItem();
                    await Coroutine.Sleep(2000);
                }
            }
        }

        public void GetNodes()
        {
            foreach (var node in NodeList)
            {
                if (weatherNodes.Contains(node.Base()))
                    Log($"Weather Node Found: {node.Name} Visible: {node.CanGather}");
                else
                {
                    Log($"Node Found: {node.Name} Base: {node.Base()} Visible {node.CanGather}");
                }
            }
        }

        public async Task StandBy()
        {
            if (Core.Me.DistanceSqr(standBy) > 10f)
                await FlyTo(standBy);
        }

        public async Task MineWeather(ClassJobType jobType, Vector3 above, Vector3[] safeSpots)
        {
            lastWeather = WorldManager.CurrentWeatherId;
            lastChange.Reset();
            await SwitchToJob(jobType);
            
            //await FlyTo(above);
            var safeSpot = safeSpots.ToList().OrderBy(i => i.DistanceSqr(Core.Me.Location)).First();
            await FlyTo(safeSpot);
            Log("Near Node Location");
            Log($"Current Weather: {WorldManager.CurrentWeather}  {WorldManager.CurrentWeatherId}");
            GetNodes();
            var node = WeatherNodeList.FirstOrDefault();

            if (node != null)
            {
                //safeSpot = safeSpots.ToList().OrderBy(i => i.DistanceSqr(node.Location)).First();
                //Log("Node Found");
                await FlyTo(node.Location);

                /*if (MovementManager.IsFlying)
                {
                    MovementManager.StartDescending();
                    await Coroutine.Wait(2000, () => !MovementManager.IsFlying);
                    MovementManager.StopDescending();
                }*/
                
                var _target = node.Location;
                Navigator.PlayerMover.MoveTowards(_target);
                while (_target.Distance2DSqr(Core.Me.Location) >= 3)
                {
                    Navigator.PlayerMover.MoveTowards(_target);
                    await Coroutine.Sleep(100);
                }

                Navigator.PlayerMover.MoveStop();
                
                if (Core.Player.IsMounted)
                {
                    ActionManager.Dismount();
                    await Coroutine.Sleep(2000);
                }
                
                //await UseCordial();
                
                node = WeatherNodeList.FirstOrDefault();
                node.Interact();
                
                await Coroutine.Sleep(200);
                await Coroutine.Wait(2000, () => GatheringManager.WindowOpen);
                await Coroutine.Sleep(500);

                if (Core.Me.CurrentJob == ClassJobType.Miner)
                    await testMine();
                else if (Core.Me.CurrentJob == ClassJobType.Botanist)
                    await testBtn();
                
                //await UseCordial();
                Log("Done Test Gather");
                /*DutyManager.LeaveActiveDuty();
                if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
                {
                    await Coroutine.Yield();
                    await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
                    await Coroutine.Sleep(10000);
                }*/
            }
        }

        public static async Task SwitchToJob(ClassJobType job)
        {
            if (Core.Me.CurrentJob == job) return;
            
            var gearSets = GearsetManager.GearSets.Where(i => i.InUse);
            
            if (gearSets.Any(gs => gs.Class == job))
            {
                Logging.Write(Colors.Fuchsia, $"[ChangeJob] Found GearSet");
                gearSets.First(gs => gs.Class == job).Activate();
                await Coroutine.Sleep(1000);
            }
        }
        
        public static List<GatheringPointObject> NodeList => GameObjectManager.GetObjectsOfType<GatheringPointObject>().OrderBy(r=>r.Distance()).ToList();
        
        public static List<GatheringPointObject> WeatherNodeList => GameObjectManager.GetObjectsOfType<GatheringPointObject>().Where(i => weatherNodes.Contains(i.Base()) && i.IsVisible).OrderBy(r=>r.Distance()).ToList();
        
        public async Task testGather()
        {
            var GatherLock = Core.Memory.Read<uint>(Offsets.Conditions + 0x2A);
            Log("in Test Gather");
            if (GatheringManager.WindowOpen)
            {
                GatheringItem items = GatheringManager.GatheringWindowItems.FirstOrDefault(i => i.IsFilled && i.CanGather);

                Log($"Gathering: {items}");

                while (GatheringManager.SwingsRemaining > 0)
                {
                    items.GatherItem();
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) != 0);
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) == 0);
                }
            }
        }
        
        public async Task testMine()
        {
            //Log("in Test Gather");
            if (GatheringManager.WindowOpen)
            {
                GatheringItem items = GatheringManager.GatheringWindowItems.Where(i => i.IsFilled && i.CanGather).OrderByDescending(s => s.Stars).FirstOrDefault();

                if (Core.Me.CurrentGP >= 500)
                {
                    await Coroutine.Wait(5000, () => ActionManager.CanCast(241, Core.Me));
                    ActionManager.DoAction(241, Core.Me);
                    await Coroutine.Sleep(2500);
                }
                
                /*
                if (Core.Me.CurrentGP >= 250)
                {
                    await Coroutine.Wait(5000, () => ActionManager.CanCast(4587, Core.Me));
                    ActionManager.DoAction(4587, Core.Me);
                    await Coroutine.Sleep(2500);
                }
                */

                Log($"Gathering: {items}");

                while (GatheringManager.SwingsRemaining > 0 && GatheringManager.WindowOpen)
                {
                    if (Core.Me.CurrentGP >= 100)
                    {
                        await Coroutine.Wait(5000, () => ActionManager.CanCast(272, Core.Me));
                        ActionManager.DoAction(272, Core.Me);
                        await Coroutine.Sleep(2500);
                    }
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) == 0);
                    items?.GatherItem();
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) != 0);
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) == 0);
                }
            }
        }
        
        public async Task testBtn()
        {
            //Log("in Test Gather");
            if (GatheringManager.WindowOpen)
            {
                GatheringItem items = GatheringManager.GatheringWindowItems.Where(i => i.IsFilled && i.CanGather).OrderByDescending(s => s.Stars).FirstOrDefault();
                if (Core.Me.CurrentGP >= 500)
                {
                    await Coroutine.Wait(5000, () => ActionManager.CanCast(224, Core.Me));
                    ActionManager.DoAction(224, Core.Me);
                    await Coroutine.Sleep(2500);
                }
                
                /*
                if (Core.Me.CurrentGP >= 250)
                {
                    await Coroutine.Wait(5000, () => ActionManager.CanCast(4588, Core.Me));
                    ActionManager.DoAction(4588, Core.Me);
                    await Coroutine.Sleep(2500);
                }
                */
                
                Log($"Gathering: {items}");

                while (GatheringManager.SwingsRemaining > 0)
                {
                    if (Core.Me.CurrentGP >= 100)
                    {
                        await Coroutine.Wait(5000, () => ActionManager.CanCast(273, Core.Me));
                        ActionManager.DoAction(273, Core.Me);
                        await Coroutine.Sleep(2500);
                    }
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) == 0);
                    items?.GatherItem();
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) != 0);
                    await Coroutine.Wait(20000, () => Core.Memory.Read<uint>(Offsets.Conditions + 0x2A) == 0);
                }
            }
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Pink, msg);
        }

        private static async Task EnterDiadem()
        {
            await Navigation.GetTo(886, new Vector3(-17.82406f, -16f, 141.3146f));

            var Aurvael = GameObjectManager.GetObjectByNPCId(1031694);

            if (Aurvael != null)
            {
                Aurvael.Interact();
                if (await Coroutine.Wait(3000, () => Talk.DialogOpen))
                {
                    Talk.Next();
                }

                await Coroutine.Wait(3000, () => SelectString.IsOpen);

                if (SelectString.IsOpen)
                {
                    SelectString.ClickSlot(0);
                    await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
                    SelectYesno.Yes();
                    
                    await Coroutine.Wait(30000, () => ContentsFinderConfirm.IsOpen);

                    await Coroutine.Yield();
                    while (ContentsFinderConfirm.IsOpen)
                    {
                        DutyManager.Commence();
                        await Coroutine.Yield();
                        if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
                        {
                            await Coroutine.Yield();
                            await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
                        }
                    }
                    await Coroutine.Sleep(2500);
                }
            }
        }
        
        
        internal static async Task<bool> FlyTo2(Vector3 loc)
        {
            var moving = MoveResult.GeneratingPath;
            while (!(moving == MoveResult.Done ||
                     moving == MoveResult.ReachedDestination ||
                     moving == MoveResult.Failed ||
                     moving == MoveResult.Failure ||
                     moving == MoveResult.PathGenerationFailed))
            {
                moving = Flightor.MoveTo(loc, 100f,false);

                await Coroutine.Yield();
            }
            Flightor.Clear();
            MovementManager.MoveStop();

            return true;
        }
        
        internal static async Task<bool> FlyTo(Vector3 loc)
        {
            return await Lisbeth.TravelTo("The Diadem", loc);
        }
    }
}