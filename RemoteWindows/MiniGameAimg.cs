namespace LlamaLibrary.RemoteWindows
{
    public class MiniGameAimg: RemoteWindow<MiniGameAimg>
    {
        private const string WindowName = "MiniGameAimg";

        public MiniGameAimg() : base(WindowName)
        {
            _name = WindowName;
        }

        public void PressButton()
        {
            SendAction(1,3,0xB);
        }
        
        public void PauseCursor()
        {
            SendAction(1,3,0xF);
        }
        
        public void ResumeCursor()
        {
            SendAction(1,3,0xF);
        }
    }
}