namespace LlamaLibrary.RemoteWindows
{
    public class SatisfactionSupplyResult: RemoteWindow<SatisfactionSupplyResult>
    {
        private const string WindowName = "SatisfactionSupplyResult";

        public SatisfactionSupplyResult() : base(WindowName)
        {
            _name = WindowName;
        }

        public void Confirm()
        {
            SendAction(1,3,1);
        }
    }
}