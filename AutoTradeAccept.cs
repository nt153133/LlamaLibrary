using System.Windows.Media;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Memory;
using TreeSharp;

namespace LlamaLibrary
{
    public class AutoTradeAccept : BotBase
    {
        private Composite _root;
        public override string Name => "AutoTrade";
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;
        public override Composite Root => _root;
        public override bool WantButton { get; } = false;
        public override void Initialize()
        {
            OffsetManager.Init();
        }

        public Composite TradeAcceptBehavior
        {
            get
            {
                return new PrioritySelector(
                    new Decorator(r => Trade.IsOpen,
                        new PrioritySelector(
                            new Decorator(r => SelectYesno.IsOpen && Trade.TradeStage == 5,
                                new Sequence(
                                    new Action(r => Log("At Select Yes/No")),
                                    new Sleep(200),
                                    new Action(r => SelectYesno.ClickYes())
                                )
                            ),
                            new Decorator(r => Trade.IsOpen && Trade.TradeStage == 3,
                                new Sequence(
                                    new Action(r => Log($"Window open accepting from {Trade.Trader}")),
                                    new Sleep(500),
                                    new Action(r => RaptureAtkUnitManager.GetWindowByName("Trade").SendAction(1, 3uL, 0))
                                )
                            )
                        )
                    )
                );
            }
        }


        public override void Start()
        {
            _root = TradeAcceptBehavior;
        }

        public override void Stop()
        {
            _root = null;
        }

        public void Log(string text)
        {
            Logging.Write(Colors.Orange, $"[{Name}] " + text);
        }
    }
}