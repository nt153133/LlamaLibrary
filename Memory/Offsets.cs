﻿/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

 using System;
 using LlamaLibrary.Memory.Attributes;

 namespace LlamaLibrary.Memory
{
#pragma warning disable CS0649
    internal static class Offsets
    {
        [Offset("Search 48 85 D2 0F 84 ? ? ? ? 55 56 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 80 7A ? ? 41 8B E8 48 8B FA 48 8B F1 74 ? 48 8B CA E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? EB ? 0F B6 42 ? A8 ? 74 ? 8B 42 ? 05 ? ? ? ? EB ? A8 ? 8B 42 ? 74 ? 05 ? ? ? ? 85 C0 0F 84 ? ? ? ? 48 89 9C 24 ? ? ? ? 48 8B CE 4C 89 B4 24 ? ? ? ? E8 ? ? ? ? 8B 9E ? ? ? ?")] //0x1BEA
        internal static IntPtr SalvageAgent;

        [Offset("Search 41 88 46 ? 0F B6 42 ? Add 3 Read8")]
        internal static int DawnTrustId;
        
        [Offset("Search 41 88 46 ? E8 ? ? ? ? C6 43 ? ? Add 3 Read8")]
        internal static int DawnIsScenario;

        [Offset("Search 4C 8D 0D ? ? ? ? 45 33 C0 33 D2 48 8B C8 E8 ? ? ? ? Add 3 TraceRelative")]
        internal static IntPtr RepairVendor;

        [Offset("Search 48 89 5C 24 ? 57 48 83 EC ? 88 51 ? 49 8B F9")] 
        internal static IntPtr RepairWindowOpen;

    //    [Offset("Search 0F B7 82 ? ? ? ? 2B C8 48 8D 0C 89 Add 3 Read32")] // 0x332
    //    internal static int WallStartingPoint;

     //   [Offset("Search 49 8D BE ? ? ? ? 0F 1F 44 00 ? 8B 17 Add 3 Read32")] //0x140
     //   internal static int Starting;

      //  [Offset("Search 41 8B 96 ? ? ? ? 84 C9 Add 3 Read32")] //0x13c
       // internal static int UNK_StartingCircle;

        //[Offset("Search 42 0F B6 8C 28 ? ? ? ? 84 0E Add 5 Read32")] //0x1860
        //internal static int WallGroupEnabled;
    }
#pragma warning restore CS0649
}