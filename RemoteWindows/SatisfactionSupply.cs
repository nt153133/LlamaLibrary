namespace LlamaLibrary.RemoteWindows
{
    public class SatisfactionSupply : RemoteWindow<SatisfactionSupply>
    {
        private const string WindowName = "SatisfactionSupply";

        public SatisfactionSupply() : base(WindowName)
        {
            _name = WindowName;
        }

        public void ClickItem(int index)
        {
            SendAction(2,3,1,3,(ulong) index);
        } 
    }
}