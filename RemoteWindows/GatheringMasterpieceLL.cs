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
            },
            {
                "Skill1",
                30
            },
            {
                "Skill2Estimate",
                31
            },
            {
                "Skill2Max",
                32
            },
            {
                "Skill3",
                33
            }
        };

        public int Collectability => ___Elements()[Properties["Collectability"]].TrimmedData;
        public int MaxCollectability => ___Elements()[Properties["MaxCollectability"]].TrimmedData;
        public int Integrity => ___Elements()[Properties["Integrity"]].TrimmedData;
        public int MaxIntegrity => ___Elements()[Properties["MaxIntegrity"]].TrimmedData;
        public int ItemID => ___Elements()[Properties["ItemID"]].TrimmedData;
        public int Scour => ___Elements()[Properties["Skill1"]].TrimmedData;
        public int BrazenEstimate => ___Elements()[Properties["Skill2Estimate"]].TrimmedData;
        public int Brazen2Max => ___Elements()[Properties["Skill2Max"]].TrimmedData;
        public int Meticulous => ___Elements()[Properties["Skill3"]].TrimmedData;
        public int IntuitionRate => ___Elements()[Properties["IntuitionRate"]].TrimmedData;

        public void Collect()
        {
            if (IsOpen)
                SendAction(1,3,0);
        }

        public void SetScrutiny(bool value = true)
        {
            if (value)
                SendAction(3,3,0x65,0,0,2,1);
            else
            {
                SendAction(3,3,0x65,0,0,2,0);
            }
        }
        
        public void SetCollectorsIntuition(bool value = true)
        {
            if (value)
                SendAction(3,3,0x66,0,0,2,1);
            else
            {
                SendAction(3,3,0x66,0,0,2,0);
            }
        }

    }
}