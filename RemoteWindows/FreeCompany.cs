namespace LlamaLibrary.RemoteWindows
{
    public class FreeCompany: RemoteWindow<FreeCompany>
    {
        private const string WindowName = "FreeCompany";

        public FreeCompany() : base(WindowName)
        {
            _name = WindowName;
        }

        public void SelectActions()
        {
            SendAction(2,3,0,4,4);
        }
    }
}