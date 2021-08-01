using System.Collections.Generic;

namespace LlamaLibrary.RemoteWindows
{
    public class AetherialWheel: RemoteWindow<AetherialWheel>
    {

        private const string WindowName = "AetherialWheel";
        
        public static readonly Dictionary<string, int> Properties = new Dictionary<string, int>
        {
            {
                "MaxSlots",
                0
            }
        };
        
        public AetherialWheel() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public int MaxSlots => ___Elements()[Properties["MaxSlots"]].TrimmedData;
    }
}