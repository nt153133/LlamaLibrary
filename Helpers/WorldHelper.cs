using System.Collections.Generic;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;

namespace LlamaLibrary.Helpers
{
    public static class WorldHelper
    {
        private static class Offsets
        {
            [Offset("Search 48 8D 4B ? E8 ? ? ? ? 84 C0 74 ? 48 8B 74 24 ? Add 3 Read8")]
            internal static int Offset1;

            [Offset("Search 41 89 8F ? ? ? ? 49 39 6E ? Add 3 Read32")]
            internal static int DCOffset;

            [Offset("Search 0F B7 98 ? ? ? ? 66 85 FF Add 3 Read32")]
            internal static int CurrentWorld;

            [Offset("Search 0F B7 81 ? ? ? ? 66 89 44 24 ? 48 8D 4C 24 ? Add 3 Read32")]
            internal static int HomeWorld;
        }

        public static byte DataCenterId
        {
            get
            {
                var agentPointer = AgentModule.AgentPointers[0];
                var offset1 = agentPointer + Offsets.Offset1;
                return Core.Memory.Read<byte>(offset1 + Offsets.DCOffset);
            }
        }

        public static readonly Dictionary<byte, string> DataCenterNamesDictionary = new Dictionary<byte, string>
        {
            { 0, "INVALID" },
            { 1, "Elemental" },
            { 2, "Gaia" },
            { 3, "Mana" },
            { 4, "Aether" },
            { 5, "Primal" },
            { 6, "Chaos" },
            { 7, "Light" },
            { 8, "Crystal" },
            { 99, "Beta" },
        };

        public static ushort CurrentWorldId => Core.Memory.NoCacheRead<ushort>(Core.Me.Pointer + Offsets.CurrentWorld);

        public static ushort HomeWorldId => Core.Memory.NoCacheRead<ushort>(Core.Me.Pointer + Offsets.HomeWorld);

        public static readonly Dictionary<ushort, string> WorldNamesDictionary = new Dictionary<ushort, string>
        {
            { 0, "INVALID" },
            { 23, "Asura" },
            { 24, "Belias" },
            { 25, "Chaos" },
            { 26, "Hecatoncheir" },
            { 27, "Moomba" },
            { 28, "Pandaemonium" },
            { 29, "Shinryu" },
            { 30, "Unicorn" },
            { 31, "Yojimbo" },
            { 32, "Zeromus" },
            { 33, "Twintania" },
            { 34, "Brynhildr" },
            { 35, "Famfrit" },
            { 36, "Lich" },
            { 37, "Mateus" },
            { 38, "Shemhazai" },
            { 39, "Omega" },
            { 40, "Jenova" },
            { 41, "Zalera" },
            { 42, "Zodiark" },
            { 43, "Alexander" },
            { 44, "Anima" },
            { 45, "Carbuncle" },
            { 46, "Fenrir" },
            { 47, "Hades" },
            { 48, "Ixion" },
            { 49, "Kujata" },
            { 50, "Typhon" },
            { 51, "Ultima" },
            { 52, "Valefor" },
            { 53, "Exodus" },
            { 54, "Faerie" },
            { 55, "Lamia" },
            { 56, "Phoenix" },
            { 57, "Siren" },
            { 58, "Garuda" },
            { 59, "Ifrit" },
            { 60, "Ramuh" },
            { 61, "Titan" },
            { 62, "Diabolos" },
            { 63, "Gilgamesh" },
            { 64, "Leviathan" },
            { 65, "Midgardsormr" },
            { 66, "Odin" },
            { 67, "Shiva" },
            { 68, "Atomos" },
            { 69, "Bahamut" },
            { 70, "Chocobo" },
            { 71, "Moogle" },
            { 72, "Tonberry" },
            { 73, "Adamantoise" },
            { 74, "Coeurl" },
            { 75, "Malboro" },
            { 76, "Tiamat" },
            { 77, "Ultros" },
            { 78, "Behemoth" },
            { 79, "Cactuar" },
            { 80, "Cerberus" },
            { 81, "Goblin" },
            { 82, "Mandragora" },
            { 83, "Louisoix" },
            { 84, "Syldra" },
            { 85, "Spriggan" },
            { 90, "Aegis" },
            { 91, "Balmung" },
            { 92, "Durandal" },
            { 93, "Excalibur" },
            { 94, "Gungnir" },
            { 95, "Hyperion" },
            { 96, "Masamune" },
            { 97, "Ragnarok" },
            { 98, "Ridill" },
            { 99, "Sargatanas" },
        };

        public static string DataCenterName => DataCenterNamesDictionary[DataCenterId];

        public static string HomeWorldName => WorldNamesDictionary[HomeWorldId];
    }
}