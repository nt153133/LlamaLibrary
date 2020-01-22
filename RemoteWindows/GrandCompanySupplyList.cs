using Buddy.Coroutines;

namespace LlamaLibrary.RemoteWindows
{
    public class GrandCompanySupplyList: RemoteWindow<GrandCompanySupplyList>
    {
        private const string WindowName = "GrandCompanySupplyList";
        
        public GrandCompanySupplyList() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public void ClickItem(int index)
        {
            SendAction(2, 3, 1, 3, (ulong) index);
        }

        public async void SwitchToExpertDelivery()
        {
            SendAction(2, 3,0,3,2);
            await Coroutine.Sleep(500);
            SendAction(2, 3,5,3,2);
            await Coroutine.Sleep(500);
        }
        
        public async void SwitchToProvisioning()
        {
            SendAction(2, 3,0,3,1);
            await Coroutine.Sleep(500);
        }
        
        public async void SwitchToSupply()
        {
            SendAction(2, 3,0,3,0);
            await Coroutine.Sleep(500);
        }
        
    }
}