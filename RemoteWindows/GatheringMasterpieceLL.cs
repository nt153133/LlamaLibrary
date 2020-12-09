using System.Collections.Generic;

namespace LlamaLibrary.RemoteWindows
{
    public class GatheringMasterpieceLL: RemoteWindow<GatheringMasterpieceLL>
    {
        private const string WindowName = "GatheringMasterpiece";
        public GatheringMasterpieceLL() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public static readonly Dictionary<string, int> Properties = new Dictionary<string, int>
        {
            {
                "Collectability",
                4
            },
            {
                "MaxCollectability",
                5
            },
            {
                "Integrity",
                40
            },
            {
                "MaxIntegrity",
                41
            },
            {
                "ItemID",
                10
            },
            {
                "IntuitionRate",
                37
            }
        };

        public int Collectability => ___Elements()[Properties["Collectability"]].TrimmedData;
        public int MaxCollectability => ___Elements()[Properties["MaxCollectability"]].TrimmedData;
        public int Integrity => ___Elements()[Properties["Integrity"]].TrimmedData;
        public int MaxIntegrity => ___Elements()[Properties["MaxIntegrity"]].TrimmedData;
        public int ItemID => ___Elements()[Properties["ItemID"]].TrimmedData;
        public int IntuitionRate => ___Elements()[Properties["IntuitionRate"]].TrimmedData;

        public void Collect()
        {
            if (IsOpen)
                SendAction(1,3,0);
        }
        
    }
}