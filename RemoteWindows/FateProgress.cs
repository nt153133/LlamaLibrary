namespace LlamaLibrary.RemoteWindows
{
    public class FateProgress: RemoteWindow<FateProgress>
    {
        private const string WindowName = "FateProgress";
        
        public FateProgress() : base(WindowName)
        {
            _name = WindowName;
        }
    }
}