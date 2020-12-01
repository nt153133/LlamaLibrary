using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public class FreeCompanyAction: RemoteWindow<FreeCompanyAction>
    {
        private const string WindowName = "FreeCompanyAction";
        
        public FreeCompanyAction() : base(WindowName)
        {
            _name = WindowName;
        }

        public void SelectAction(int index)
        {
            SendAction(2,3,1,4,(ulong) index);
        }

        public void SelectEnable()
        {
            RaptureAtkUnitManager.GetWindowByName("ContextMenu").SendAction(4, 3, 0, 3, 0, 4, 0, 0, 0);
        }

        public async Task EnableAction(int index)
        {
            SelectAction(index);
            await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName("ContextMenu") != null);
            if (RaptureAtkUnitManager.GetWindowByName("ContextMenu") != null)
            {
                SelectEnable();
                await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.Yes();
                }
            }
        }
    }
}