namespace LlamaLibrary.RemoteWindows
{
    public class GcArmyCapture: RemoteWindow<GcArmyCapture>
    {
        private const string WindowName = "GcArmyCapture";

        public GcArmyCapture() : base(WindowName)
        {
            _name = WindowName;
        }

        public void Commence()
        {
            SendAction(1, 3, 0xd);
        }

        public void SelectDuty(int index)
        {
            SendAction(2, 3, 0xB, 4, (ulong) index);
        }
    }
}