namespace LlamaLibrary.RemoteWindows
{
    public class HWDSupply : RemoteWindow<HWDSupply>
    {
        private const string WindowName = "HWDSupply";

        public HWDSupply() : base(WindowName)
        {
            _name = WindowName;
        }
        public int ClassSelected
        {
            get => ___Elements()[29].TrimmedData;
            set
            {
                if (WindowByName != null && ___Elements()[29].TrimmedData != value)
                    SendAction(2, 0, 1, 1, (ulong) value);
            }
        }
        public void ClickItem(int index)
        {
            SendAction(2, 3, 1, 3, (ulong) index);
        }
    }
}