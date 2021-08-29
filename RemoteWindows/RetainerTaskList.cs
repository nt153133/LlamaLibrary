namespace LlamaLibrary.RemoteWindows
{
    public class RetainerTaskList : RemoteWindow<RetainerTaskList>
    {
        private const string WindowName = "RetainerTaskList";

        public RetainerTaskList() : base(WindowName)
        {
            _name = WindowName;
        }

        public void SelectVenture(int taskId)
        {
            SendAction(2, 3, 0x0B, 03, (ulong) taskId);
        }
        
    }
}