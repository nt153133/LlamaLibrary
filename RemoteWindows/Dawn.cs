using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ff14bot;

namespace LlamaLibrary.RemoteWindows
{
    public class Dawn : RemoteWindow<Dawn>
    {
        private static readonly string WindowName = "Dawn";
        private readonly List<TrustNPC> NpcList;

        public Dawn() : base(WindowName)
        {
            NpcList = new List<TrustNPC>
            {
                new TrustNPC("Alphinaud", 82061, 82081, 1),
                new TrustNPC("Alisaie", 82062, 82082, 2),
                new TrustNPC("Thancred", 82063, 82083, 3),
                new TrustNPC("Minfilia", 82064, 82084, 4),
                new TrustNPC("Urianger", 82065, 82085, 5),
                new TrustNPC("Y'shtola", 82066, 82086, 6),
                new TrustNPC("Ryne", 82067, 82087, 7),
                new TrustNPC("Lyna", 82068, 82088, 8),
                new TrustNPC("Crystal Exarch", 82069, 82089, 9),
                new TrustNPC("Crystal Exarch", 82069, 82089, 9),
                new TrustNPC("Crystal Exarch", 82069, 82089, 9)
            };

            _name = WindowName;
        }

        public int NumberOfTrustsAvailable => ___Elements()[73].TrimmedData;
        public int SelectedTrustId => ___Elements()[74].TrimmedData;
        public string SelectedTrustName => Core.Memory.ReadString((IntPtr) ___Elements()[75].Data, Encoding.UTF8);

        public TrustNPC SelectedNpc1 => GetTrustNpc(___Elements()[34].TrimmedData);
        public TrustNPC SelectedNpc2 => GetTrustNpc(___Elements()[35].TrimmedData);
        public TrustNPC SelectedNpc3 => GetTrustNpc(___Elements()[36].TrimmedData);

        public TrustNPC Npc1 => GetTrustNpc(___Elements()[10].TrimmedData);
        public TrustNPC Npc2 => GetTrustNpc(___Elements()[11].TrimmedData);
        public TrustNPC Npc3 => GetTrustNpc(___Elements()[12].TrimmedData);
        public TrustNPC Npc4 => GetTrustNpc(___Elements()[13].TrimmedData);
        public TrustNPC Npc5 => GetTrustNpc(___Elements()[14].TrimmedData);
        public TrustNPC Npc6 => GetTrustNpc(___Elements()[15].TrimmedData);

        public int Npc1Level => ___Elements()[43].TrimmedData;
        public int Npc1Leve2 => ___Elements()[44].TrimmedData;
        public int Npc1Leve3 => ___Elements()[45].TrimmedData;

        public bool CanRegister()
        {
            if (WindowByName == null) return false;
            var remoteButton = WindowByName.FindButton(36);
            return remoteButton != null && remoteButton.Clickable;
        }

        public void Register()
        {
            if (WindowByName != null) WindowByName.SendAction(1, 3, 14);
        }

        public void SetTrust(int trust)
        {
            if (WindowByName != null) WindowByName.SendAction(2, 3, 15, 4, (ulong) trust);
        }

        public void Close()
        {
            if (WindowByName != null) WindowByName.SendAction(1, 3, 0);
        }

        public void PressNpcSelection(int npc)
        {
            if (WindowByName != null && npc < 6) WindowByName.SendAction(2, 3, 12, 4, (ulong) npc);
        }

        public void ToggleScenario()
        {
            if (WindowByName != null) WindowByName.SendAction(1, 3, 17);
        }

        private TrustNPC GetTrustNpc(int id)
        {
            return NpcList.Any(i => i.Id1 == id || i.Id2 == id) ? NpcList.FirstOrDefault(i => i.Id1 == id || i.Id2 == id) : null;
        }
    }

    public class TrustNPC
    {
        public TrustNPC(string name, int id1, int id2, int classId)
        {
            Name = name;
            Id1 = id1;
            Id2 = id2;
            ClassId = classId;
        }


        public string Name { get; }
        public int Id1 { get; }
        public int Id2 { get; }
        public int ClassId { get; }
    }
}