using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;

namespace LlamaLibrary.RemoteWindows
{
    public class ContentsInfo : RemoteWindow<ContentsInfo>
    {
        private const string WindowName = "ContentsInfo";

        private int _agentId;
        public ContentsInfo() : base(WindowName)
        {
            _name = WindowName;
            
            var patternFinder = new GreyMagic.PatternFinder(Core.Memory);
            IntPtr agentVtable = patternFinder.Find("48 8D 05 ? ? ? ? BF ? ? ? ? 48 89 03 48 8D 73 ? Add 3 TraceRelative");
            _agentId = AgentModule.FindAgentIdByVtable(agentVtable);
        }


        public async Task<bool> Open()
        {
            if (IsOpen)
                return true;
            
            AgentModule.ToggleAgentInterfaceById(_agentId);
            await Coroutine.Wait(5000, () => IsOpen);

            return IsOpen;
        }

        public void OpenGCSupplyWindow()
        {
            SendAction(2, 3, 0xC, 3, 1);
        }
    }
}