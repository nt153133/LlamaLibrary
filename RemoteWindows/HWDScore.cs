namespace LlamaLibrary.RemoteWindows
{
    public class HWDScore : RemoteWindow<HWDScore>
    {
        private const string WindowName = "HWDScore";

        public HWDScore() : base(WindowName)
        {
            _name = WindowName;
        }
    }
}