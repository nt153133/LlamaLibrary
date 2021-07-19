using ff14bot.Managers;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.Helpers;

namespace LlamaLibrary.RemoteWindows
{
    public class InventoryBuddy: RemoteWindow<InventoryBuddy>
    {
        private const string WindowName = "InventoryBuddy";


        public InventoryBuddy() : base(WindowName, AgentInventoryBuddy.Instance)
        {
            _name = WindowName;
        }

    }
}