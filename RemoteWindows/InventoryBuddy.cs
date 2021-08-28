using LlamaLibrary.RemoteAgents;

namespace LlamaLibrary.RemoteWindows
{
    public class InventoryBuddy: RemoteWindow<InventoryBuddy>
    {
        private const string WindowName = "InventoryBuddy";


        public InventoryBuddy() : base(WindowName, AgentInventoryBuddy.Instance)
        {
            _name = WindowName;
            //_agent = AgentInventoryBuddy.Instance;
        }

    }
}