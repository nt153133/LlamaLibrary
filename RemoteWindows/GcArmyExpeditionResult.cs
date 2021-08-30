namespace LlamaLibrary.RemoteWindows
{
    public class GcArmyExpeditionResult: RemoteWindow<GcArmyExpeditionResult>
    {
        private const string WindowName = "GcArmyExpeditionResult";

        public GcArmyExpeditionResult() : base(WindowName)
        {
            _name = WindowName;
        }


        public override void Close()
        {
            SendAction(1,3,0);
        }

    }
}