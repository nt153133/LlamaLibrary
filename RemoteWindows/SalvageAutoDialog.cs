using System;
using System.Text;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteWindows
{
    public class SalvageAutoDialog: RemoteWindow<SalvageAutoDialog>
    {
        private const string WindowName = "SalvageAutoDialog";

        public SalvageAutoDialog() : base(WindowName)
        {
            _name = WindowName;
        }
    }
}