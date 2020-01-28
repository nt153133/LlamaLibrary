using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.RemoteWindows;
using Generate;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary
{
    public class TesterBase: BotBase
    {
        private Composite _root;

        public TesterBase()
        {

        }

        public override string Name => "Tester";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        private async Task<bool> Run()
        {
            await LeveWindow(1018997);

            TreeRoot.Stop("Stop Requested");
            return true;
        }

        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
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

            SelectString.ClickSlot((uint) (SelectString.LineCount -1));
            
            Logger.Info(output);

            return true;
        }
    }
}