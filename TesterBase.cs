using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Buddy.Service.Core;
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
using Generate;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.Properties;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
using Newtonsoft.Json;
using TreeSharp;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary
{
    public class TesterBase : BotBase
    {
        private Composite _root;

        public TesterBase()
        {
            Task.Factory.StartNew(() =>
            {
                init();
                _init = true;
                Log("INIT DONE");
            });
        }

        public override string Name => "Tester";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;
        internal static List<RetainerTaskData> VentureData;
        private volatile bool _init;
        internal void init()
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
            //await LeveWindow(1018997);
            //await HousingWards();
            await testVentures();
            //Navigator.PlayerMover = new SlideMover();
            //Navigator.NavigationProvider = new ServiceNavigationProvider();


           
            TreeRoot.Stop("Stop Requested");
            return true;
        }
        
        public async Task testVentures()
        {
            //var ishgard = new IshgardHandin();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();

            /*Log($"Number of Retainers: {await HelperFunctions.GetNumberOfRetainers()}");

            var ventures = await HelperFunctions.GetVentureFinishTimes();

            foreach (var venture in ventures)
            {
                var timeleft = (venture.Value - HelperFunctions.UnixTimestamp) / 60;
                string result = "Complete";
                if (timeleft > 0)
                    result = $"{timeleft}";
                Log($"{HelperFunctions.GetRetainerName(venture.Key)} : {result}");
            }*/

            var count = await HelperFunctions.GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            foreach (var ret in rets)
            {
                Log(ret.ToString());
            }
            //await RetainerRoutine.ReadRetainers(CheckVentures);


        }

        public async Task testKupoTickets()
        {
            var ishgard = new IshgardHandin();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            Navigator.PlayerMover = new SlideMover();

            await ishgard.HandInKupoTicket(1);
        }
        
        public async Task<bool> CheckVentures()
        {
            if (!SelectString.IsOpen)
            {
                return false;
            }

            if (SelectString.LineCount > 9)
            {
                if (SelectString.Lines().Contains(Translator.VentureCompleteText))
                {
                    Log("Venture Done");
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
            }
            else
            {
                Log("Venture Not Done");
            }
            return true;
        }
        
        public async Task<bool> testFacetCheck()
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);

            var result = patternFinder.Find("44 89 BF ?? ?? ?? ?? 83 BF ?? ?? ?? ?? ?? Add 3 Read32");
            //Log(result);
            uint[] npcs = {1027233, 1027234, 1027235, 1027236, 1027237};
     
            var units = GameObjectManager.GameObjects;
            foreach(var unit in units.Where(i => npcs.Contains(i.NpcId)))
            {
                Log("Name:{0}, Type:{3}, ID:{1}, Obj:{2}",unit,unit.NpcId,unit.ObjectId,unit.GetType());
            }

            return false;
        }

        public async Task testGather()
        {
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            IntPtr AnimationLocked = patternFinder.Find("48 8D 0D ?? ?? ?? ?? BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 80 8B ?? ?? ?? ?? ?? 45 33 C9 44 8B C7 89 BB ?? ?? ?? ?? Add 3 TraceRelative");
            
            var GatherLock = Core.Memory.Read<uint>(AnimationLocked + 0x2A);

            if (GatheringManager.WindowOpen)
            {
                GatheringItem items = GatheringManager.GatheringWindowItems.FirstOrDefault(i=> i.IsFilled && !i.IsUnknown && !i.ItemData.Unique && i.CanGather);
                
                Log($"Gathering: {items}");

                while (GatheringManager.SwingsRemaining > 0)
                {
                    items.GatherItem();
                    await Coroutine.Wait(20000,() => Core.Memory.Read<uint>(AnimationLocked + 0x2A) != 0);
                    await Coroutine.Wait(20000,() => Core.Memory.Read<uint>(AnimationLocked + 0x2A) == 0);
                }
            }
        }

        private void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Pink, msg);
        }

        public async Task<bool> testExtract()
        {
           // var item = InventoryManager.FilledInventoryAndArmory.Where(i => i.Item.EngName.Contains("Voeburtite Ring of Slaying")).FirstOrDefault();
            

          //  if (item != null)
          //      item.ExtractMateria();
          var a = InventoryManager.FilledSlots.First(i=> i.RawItemId == 27712 );

          Log( ($"{a} {a.BagId} {a.Slot}"));
          Log($"Inventory Pointer: {Offsets.ItemFuncParam.ToInt64():X}  Function: {Offsets.ItemSplitFunc.ToInt64():X}");
          a.Split(1);
          
          AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("SelectString");

          if (windowByName != null)
          {
              List<string> list = new List<string>();
              IntPtr pointer = Core.Memory.Read<IntPtr>(windowByName.Pointer + 0x238 + 0x38);

              if (pointer != IntPtr.Zero)
              {
                  int count = Core.Memory.Read<int>(pointer + 0x118);
                  for (int i = 0; i < count ; i++)
                  {
                      IntPtr addr = Core.Memory.Read<IntPtr>(pointer + 0xF0) + 24 * i + 8;
                      IntPtr pointer2 = Core.Memory.Read<IntPtr>(addr) + 8;
                      var short1 = Core.Memory.Read<ushort>(pointer2 + 0x42);
                      IntPtr addr2 = Core.Memory.Read<IntPtr>(pointer2 + 0x50)+ 8 * (short1 - 1);
                      IntPtr pointer3 = Core.Memory.Read<IntPtr>(addr2);
                      string item = Core.Memory.ReadString(Core.Memory.Read<IntPtr>(pointer3 + 0xB8), Encoding.UTF8);
                      list.Add(item);
                  }
              }
          }

            return true;
        }

        public async Task<bool> LeveWindow(uint NpcId)
        {
            var npcId = GameObjectManager.GetObjectByNPCId(NpcId);

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

            await Coroutine.Wait(5000, () => Talk.DialogOpen);

            while (Talk.DialogOpen)
            {
                Talk.Next();
                await Coroutine.Sleep(1000);
            }

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot(0);

            await Coroutine.Wait(5000, () => GuildLeve.Instance.IsOpen);

            if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Battle)
            {
                Logger.Info("Battle");
                Logger.Info(GuildLeve.Instance.PrintWindow());

                GuildLeve.Instance.SwitchType(1);
                await Coroutine.Sleep(500);
                Logger.Info("Gathering");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Gathering)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }

                GuildLeve.Instance.SwitchType(2);
                await Coroutine.Sleep(500);
                Logger.Info("Crafting");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Crafting)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }
            }
            else if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Gathering)
            {
                Logger.Info("Gathering");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Gathering)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }

                GuildLeve.Instance.SwitchType(1);
                await Coroutine.Sleep(500);
                Logger.Info("Crafting");
                if (GuildLeve.Instance.Window == RemoteWindows.LeveWindow.Crafting)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        GuildLeve.Instance.SwitchClass(i);
                        await Coroutine.Sleep(500);
                        Logger.Info(GuildLeve.Instance.PrintWindow());
                    }
                }
            }

            var output = GuildLeve.Instance.PrintWindow();

            await Coroutine.Sleep(500);

            GuildLeve.Instance.Close();

            await Coroutine.Wait(5000, () => SelectString.IsOpen);

            SelectString.ClickSlot((uint) (SelectString.LineCount - 1));

            Logger.Info(output);

            return true;
        }
    }
}