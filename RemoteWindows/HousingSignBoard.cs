using System;
using System.Text;
using ff14bot;

namespace LlamaLibrary.RemoteWindows
{
    public class HousingSignBoard: RemoteWindow<HousingSignBoard>
    {
        private const string WindowName = "HousingSignBoard";

        public HousingSignBoard() : base(WindowName)
        {
            _name = WindowName;
        }

        public bool IsForSale
        {
            get => Core.Memory.ReadString((IntPtr) ___Elements()[1].Data, Encoding.UTF8).Contains("Sale");
        }

        public void ClickBuy()
        {
            SendAction(1,3,1);
        }
    }
}