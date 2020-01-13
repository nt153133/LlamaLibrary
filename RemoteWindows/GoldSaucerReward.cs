namespace LlamaLibrary.RemoteWindows
{
    public class GoldSaucerReward: RemoteWindow<GoldSaucerReward>
    {
        private const string WindowName = "GoldSaucerReward";
        
        public GoldSaucerReward() : base(WindowName)
        {
            _name = WindowName;
        }

        
        public int MGPReward => ___Elements()[1].TrimmedData;
    }
}