using System.Collections.Generic;
using System.Linq;
using Clio.Utilities;
using ff14bot.Enums;
using ff14bot.Managers;

namespace LlamaLibrary.Structs
{
    public class GatheringNodeData
    {
        public uint NodeID;
        public byte Type;
        public int Level;
        public KeyValuePair<byte,byte>[] TimeSlots;
        public uint[] ItemIds;
        public ushort ZoneId;
        public Vector3[] Locations;
		
        public GatheringNodeData()
        {
        }

        public override string ToString()
        {
            List<string> times = TimeSlots.Select(slot => $"{slot.Key:D2}:00-{slot.Value:D2}:00").ToList();
            return $"{NodeID} ({(GatheringType)Type}) {string.Join(" , ", ItemIds.Select(i=> DataManager.GetItem(i).CurrentLocaleName))} \n{string.Join("\n", times)}\n{DataManager.ZoneNameResults[ZoneId].CurrentLocaleName}\n\t{string.Join("\n\t",Locations)}";
        }
    }
}