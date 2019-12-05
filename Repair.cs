/*
AutoRepair is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
Original work done by Kayla D'orden
                                                                                 */

using System;
using System.Linq;
using System.Windows.Media;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using GreyMagic;
using TreeSharp;
using Action = TreeSharp.Action;

namespace AutoRepair
{
    public class AutoRepair : BotPlugin
    {
        public static int RepairThreshold = 30;
        public static int AgentId = 36;

        private static IntPtr off;
        private static IntPtr func;
        private static IntPtr vtable;

        private static ActionRunCoroutine s_hook;

        private Composite _root;
        public override string Author => "Kayla D'orden";
        public override Version Version => new Version(1, 7);
        public override string Name => _Name;
        public override bool WantButton => false;
        public static string _Name => "AutoRepair";

        public Composite CreateVendorBehavior
        {
            get
            {
                return new Decorator(r => !Core.Me.InCombat && !MovementManager.IsOccupied && !Repairing,
                    new PrioritySelector(
                        new Decorator(r => InventoryManager.EquippedItems.Any(item => item.Item != null && item.Item.RepairItemId != 0 && item.Condition < RepairThreshold),
                            new Sequence(
                                new Action(r => Log("Start")),
                                new Action(r => TreeRoot.StatusText = "Should be repairing"),
                                //new Action(r => Run()),
                                new Action(r => Repairing = true),
                                new Action(r => Log("Stop")),
                                new Action(r => TreeRoot.StatusText = "Should be done repairing")
                            )
                        )
                    )
                );
            }
        }

        public Composite RepairBehavior
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(r => Repairing,
                        new PrioritySelector(
                            new Decorator(r => SelectYesno.IsOpen,
                                new Sequence(
                                    new Action(r => Log("At Select Yes/No")),
                                    new Sleep(500),
                                    //new Action(r => Thread.Sleep(1000)),
                                    new Action(r => SelectYesno.ClickYes()),
                                    new Action(r => Repairing = false),
                                    new Action(r => Repair.Close())
                                )
                            ),
                            new Decorator(r => Repair.IsOpen,
                                new Sequence(
                                    new Action(r => Log("Window open so repairing")),
                                    new Sleep(500),
                                    new Action(r => Repair.RepairAll())
                                )
                            ),
                            new Decorator(r => !Repair.IsOpen,
                                new Sequence(
                                    new Action(r => Log("Window not open so opening")),
                                    new Action(r => OpenRepair())
                                )
                            )
                        )
                    ),
                    new Decorator(r => !Repairing && Repair.IsOpen, new Action(r => Repair.Close()))
                );
            }
        }

        public static bool Repairing { get; set; }

        public override void OnInitialize()
        {
            TreeRoot.OnStart += OnBotStart;
            TreeRoot.OnStop += OnBotStop;
            Repairing = false;
            var patternFinder = new PatternFinder(Core.Memory);
            var off = patternFinder.Find("4C 8D 0D ? ? ? ? 45 33 C0 33 D2 48 8B C8 E8 ? ? ? ? Add 3 TraceRelative");
            var func = patternFinder.Find("48 89 5C 24 ? 57 48 83 EC ? 88 51 ? 49 8B F9");
            var vtable = patternFinder.Find("48 8D 05 ? ? ? ? 48 89 03 B9 ? ? ? ? 4C 89 43 ? Add 3 TraceRelative");

            var repairAgent = AgentModule.FindAgentIdByVtable(vtable);


            Log($"AgentId {AgentId} Offset {off.ToInt64():X} Func {func.ToInt64():X}");

            AgentId = repairAgent;

            _root = new Decorator(r => Repairing, RepairBehavior);
            //_root = new Decorator(r => Repairing, new ActionAlwaysSucceed());
        }

        private void OnBotStop(BotBase bot)
        {
            RemoveHooks();
        }

        private void OnBotStart(BotBase bot)
        {
            AddHooks();
        }

        public override void OnShutdown()
        {
            TreeRoot.OnStart -= OnBotStart;
            TreeRoot.OnStop -= OnBotStop;
            RemoveHooks();
        }

        private void AddHooks()
        {
            Log("Adding Hook");
            Repairing = false;
            TreeHooks.Instance.AddHook("TreeStart", _root);
            TreeHooks.Instance.AddHook("TreeStart", CreateVendorBehavior);
        }

        private void RemoveHooks()
        {
            Log("Removing Hook");
            Repairing = false;
            TreeHooks.Instance.RemoveHook("TreeStart", CreateVendorBehavior);
            TreeHooks.Instance.RemoveHook("TreeStart", _root);
        }

        private void OnHooksCleared(object sender, EventArgs e)
        {
            AddHooks();
        }

        public override void OnEnabled()
        {
            Repairing = false;
            TreeRoot.OnStart += OnBotStart;
            TreeRoot.OnStop += OnBotStop;
            TreeHooks.Instance.OnHooksCleared += OnHooksCleared;

            if (TreeRoot.IsRunning) AddHooks();
        }

        public override void OnDisabled()
        {
            TreeRoot.OnStart -= OnBotStart;
            TreeRoot.OnStop -= OnBotStop;
            RemoveHooks();
        }

        public static void OpenRepair()
        {
            Core.Memory.CallInjected64<IntPtr>(func, AgentModule.GetAgentInterfaceById(AgentId).Pointer, 0, 0, off);
        }

        public static void Log(string text)
        {
            Logging.Write(Colors.OrangeRed, string.Format("[{1}] {0}", text, _Name));
        }
    }
}