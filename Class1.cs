using System.Linq;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteWindows;
using TreeSharp;

namespace LlamaLibrary
{
    public class Class1
    {

        public void test()
        {
            if (!HWDSupply.Instance.IsOpen )
            {
                foreach (var item in InventoryManager.FilledSlots.Where(i=> i.EnglishName.Contains("Mythril Ingot")))
                {
                    Log($"{item.Count} {item.Pointer.ToInt64():X}");
                }
                

            }
        }

        public void Log(string t)
        {
            
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
                                    new Action(r => RaptureAtkUnitManager.GetWindowByName("Trade").SendAction( 1, 3uL, 0))
                                )
                            )
                        )
                    )
                );
            }
        }
        
    }
}