namespace LlamaLibrary.RemoteWindows
{
    public class RecommendEquip: RemoteWindow<RecommendEquip>
    {

        private const string WindowName = "RecommendEquip";
        
        public RecommendEquip() : base(WindowName)
        {
            _name = WindowName;
        }

        public void Confirm()
        {
            SendAction(1,3,0);
        }
    }
}