using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.RemoteAgents;

namespace LlamaLibrary.RemoteWindows
{
    public class ContentsInfo : RemoteWindow<ContentsInfo>
    {
        private const string WindowName = "ContentsInfo";

        public ContentsInfo() : base(WindowName)
        {
            _name = WindowName;
        }


        public async Task<bool> Open()
        {
            if (IsOpen)
                return true;
            
            AgentInterface<AgentContentsInfo>.Instance.Toggle();
            await Coroutine.Wait(5000, () => IsOpen);

            return IsOpen;
        }

        public void OpenGCSupplyWindow()
        {
            SendAction(2, 3, 0xC, 3, 1);
        }
        
        public void OpenMasterPieceSupplyWindow()
        {
            SendAction(2, 3, 0xC, 3, 6);
        }
    }
}