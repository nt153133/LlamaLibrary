using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteWindows
{
    public class FishGuide: RemoteWindow<FishGuide>
    {
        private AtkAddonControl _windowByName;
        private const string WindowName = "FishGuide";

        public FishGuide() : base(WindowName)
        {
            _name = WindowName;
            
        }

        public void ClickTab(int index)
        {
            SendAction(2,3,8,3,(ulong) index);
        }



    }
    

}