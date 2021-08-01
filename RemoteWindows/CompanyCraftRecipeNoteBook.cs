using System.Collections.Generic;
using System.Linq;

namespace LlamaLibrary.RemoteWindows
{
    public class CompanyCraftRecipeNoteBook: RemoteWindow<CompanyCraftRecipeNoteBook>
    {
        private const string WindowName = "CompanyCraftRecipeNoteBoo";
        
        public CompanyCraftRecipeNoteBook() : base(WindowName)
        {
            _name = WindowName;
        }
        
        public void SelectWheelsCategory()
        {
            SendAction( 4, 3,2,0,0,4,0,4,2);
        }
        
        public void SelectWheel(uint ItemId)
        {
            if (WheelIndexes.Any(i=> i.Value == ItemId))
                SendAction(8, 3,1,0,0,0,0,0,0,4,WheelIndexes.First(i=> i.Value == ItemId).Key,0,0,0,0,0,0);
        }
        
        public static Dictionary<ulong, uint> WheelIndexes = new Dictionary<ulong, uint>
        {
            {6, 9653}, //Grade 2 Wheel of Confrontation
            {7, 9654}, //Grade 2 Wheel of Productivity
            {8, 9655}, //Grade 2 Wheel of Industry
            {9, 9656}, //Grade 2 Wheel of Longeing
            {10, 9657}, //Grade 2 Wheel of Rivalry
            {11, 9658}, //Grade 2 Wheel of Company
            {12, 12215}, //Grade 2 Wheel of Recreation
            {13, 9659}, //Grade 2 Wheel of Initiation
            {14, 9660}, //Grade 2 Wheel of Capacity
            {15, 9661}, //Grade 2 Wheel of Observation
            {16, 9662}, //Grade 2 Wheel of Efficiency
            {17, 9663}, //Grade 2 Wheel of Precision
            {18, 9664}, //Grade 2 Wheel of Dedication
            {19, 9665}, //Grade 2 Wheel of Nutriment
            {20, 9666}, //Grade 2 Wheel of Permanence
            {21, 9667}, //Grade 2 Wheel of Revival
            {22, 9668}, //Grade 2 Wheel of Pilgrimage
            {23, 9669}, //Grade 3 Wheel of Confrontation
            {24, 9670}, //Grade 3 Wheel of Productivity
            {25, 9671}, //Grade 3 Wheel of Industry
            {26, 9672}, //Grade 3 Wheel of Longeing
            {27, 9673}, //Grade 3 Wheel of Rivalry
            {28, 9674}, //Grade 3 Wheel of Company
            {29, 12216}, //Grade 3 Wheel of Recreation
            {31, 9676}, //Grade 3 Wheel of Capacity
            {32, 9677}, //Grade 3 Wheel of Observation
            {33, 9678}, //Grade 3 Wheel of Efficiency
            {34, 9679}, //Grade 3 Wheel of Precision
            {35, 9680}, //Grade 3 Wheel of Dedication
            {36, 9681}, //Grade 3 Wheel of Nutriment
            {37, 9682}, //Grade 3 Wheel of Permanence
            {39, 9684}, //Grade 3 Wheel of Pilgrimage
        };
    }
}