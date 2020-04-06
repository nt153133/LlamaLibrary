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
    internal static partial class Offsets
    {
        //[Offset("Search 48 85 D2 0F 84 ? ? ? ? 55 56 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 80 7A ? ? 41 8B E8 48 8B FA 48 8B F1 74 ? 48 8B CA E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? EB ? 0F B6 42 ? A8 ? 74 ? 8B 42 ? 05 ? ? ? ? EB ? A8 ? 8B 42 ? 74 ? 05 ? ? ? ? 85 C0 0F 84 ? ? ? ? 48 89 9C 24 ? ? ? ? 48 8B CE 4C 89 B4 24 ? ? ? ? E8 ? ? ? ? 8B 9E ? ? ? ?")] //0x1BEA
        [Offset("Search 48 85 D2 0F 84 ? ? ? ? 55 56 57 41 56")]
        [OffsetCN("Search 48 85 D2 0F 84 ? ? ? ? 55 56 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 80 7A ? ? 41 8B E8 48 8B FA 48 8B F1 74 ? 48 8B CA E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? EB ? 0F B6 42 ? A8 ? 74 ? 8B 42 ? 05 ? ? ? ? EB ? A8 ? 8B 42 ? 74 ? 05 ? ? ? ? 85 C0 0F 84 ? ? ? ? 48 89 9C 24 ? ? ? ? 48 8B CE 4C 89 B4 24 ? ? ? ? E8 ? ? ? ? 8B 9E ? ? ? ?")] //0x1BEA
        internal static IntPtr SalvageAgent;

        [Offset("Search 41 88 46 ? 0F B6 42 ? Add 3 Read8")]
        internal static int DawnTrustId;
        
        [Offset("Search 48 8D 05 ? ? ? ? 48 C7 43 ? ? ? ? ? 48 8D 4B ? 48 89 03 66 C7 43 ? ? ? Add 3 TraceRelative")]
        internal static IntPtr DawnVtable;
        
        [Offset("Search 41 88 46 ? E8 ? ? ? ? C6 43 ? ? Add 3 Read8")]
        internal static int DawnIsScenario;

        [Offset("Search 4C 8D 0D ? ? ? ? 45 33 C0 33 D2 48 8B C8 E8 ? ? ? ? Add 3 TraceRelative")]
        internal static IntPtr RepairVendor;

        [Offset("Search 48 89 5C 24 ? 57 48 83 EC ? 88 51 ? 49 8B F9")] 
        internal static IntPtr RepairWindowOpen;
        
        [Offset("Search 48 8B 0D ? ? ? ? 4C 8B C0 33 D2 Add 3 TraceRelative")] 
        internal static IntPtr SearchResultPtr;
        
        [Offset("Search 48 8D 05 ? ? ? ? 48 89 06 48 8D 4E ? 48 8D 05 ? ? ? ? 48 89 46 ? E8 ? ? ? ? 33 ED Add 3 TraceRelative")]
        internal static IntPtr AgentRetainerCharacterVtable;
            
        [Offset("Search 48 8D 05 ? ? ? ? 48 89 03 B9 ? ? ? ? 89 53 ? Add 3 TraceRelative")]
        internal static IntPtr AgentRetainerAskVtable;
            
        [Offset("Search 4C 8D 2D ? ? ? ? 66 0F 7F 44 24 ? 41 8B DF Add 3 TraceRelative")]
        internal static IntPtr RetainerStats;
        
        [Offset("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 74 ? 4C 63 78 ? Add 3 TraceRelative")]
        internal static IntPtr ItemFuncParam;
        
        [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 56 48 83 EC ? 8B DA 41 0F B7 E8")]
        internal static IntPtr ItemDiscardFunc;
        
        [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 48 89 7C 24 ? 41 56 48 83 EC ? 8B FA 33 DB")]
        internal static IntPtr ItemLowerQualityFunc;
        
        [Offset("Search 40 55 53 56 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 45 ? 8D 82 ? ? ? ?")]
        internal static IntPtr ItemSplitFunc;
        
        [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 45 33 DB 41 8B F9 45 8B D3 41 0F B7 F0 8B EA 48 8B D9 48 8B C1 0F 1F 80 ? ? ? ? 80 38 ? 75 ? 41 FF C3 49 FF C2 48 83 C0 ? 49 81 FA ? ? ? ? 7C ? EB ? 49 63 C3 48 6B D0 ? 48 03 D3 C6 02 ? 74 ? C7 42 ? ? ? ? ? 44 8B C7 89 6A ? 66 89 72 ? 89 7A ? 8B 81 ? ? ? ? 89 42 ? 0F B7 D6 44 8B 89 ? ? ? ? 8B CD E8 ? ? ? ? 8B 8B ? ? ? ? B8 ? ? ? ? FF C1 F7 E1 8B C1 2B C2 ? ? 03 C2 C1 E8 ? 69 C0 ? ? ? ? 2B C8 0F BA E9 ? 89 8B ? ? ? ? 48 8B 5C 24 ? 48 8B 6C 24 ? 48 8B 74 24 ? 48 83 C4 ? 5F C3 ? ? ? ? ? ? ? ? 66 83 FA ?")]
        internal static IntPtr RetainerRetrieveQuantity;
        
        //[Offset("Search 40 53 48 83 EC ? 8B DA 48 8B 15 ? ? ? ? 48 85 D2 0F 84 ? ? ? ? 48 89 6C 24 ? 4C 8D 0D ? ? ? ? 33 ED 0F B7 CD 0F 1F 80 ? ? ? ? 0F B7 C1 41 39 1C 81 74 ? 66 FF C1 66 83 F9 ? 72 ? 48 8B 6C 24 ? 48 83 C4 ? 5B C3 66 83 F9 ? 0F 83 ? ? ? ? 0F B7 C1 48 8D 0C 40 48 39 2C CA 48 8D 0C CA 0F 84 ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 48 89 74 24 ? 48 89 7C 24 ? 41 0F BF F8 8B D7 E8 ? ? ? ? 48 8B F0 48 85 C0 0F 84 ? ? ? ? 45 33 C0 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? 84 C0 75 ? 45 33 C0 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? E9 ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 75 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 48 8B 10 48 8B C8 FF 52 ? BA ? ? ? ? E9 ? ? ? ? 40 38 2D ? ? ? ? 76 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 48 8B 10 48 8B C8 FF 52 ? BA ? ? ? ? E9 ? ? ? ? 48 8B CE E8 ? ? ? ? 44 8B C8 89 6C 24 ? 44 8B C7 8B D3 B9 ? ? ? ? E8 ? ? ? ? 48 8B 0D ? ? ? ? 48 8B 01 FF 50 ? 89 6C 24 ? 48 8D 0D ? ? ? ? 4C 8B C8 89 6C 24 ? BA ? ? ? ? 89 6C 24 ? 41 B8 ? ? ? ? E8 ? ? ? ? 84 C0 74 ? 41 B0 ? 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? EB ? 48 8B 0D ? ? ? ?")]
        [Offset("Search E8 ? ? ? ? 8B 77 ? 33 DB Add 1 TraceRelative")]
        [OffsetCN("Search 40 53 48 83 EC ? 8B DA 48 8B 15 ? ? ? ? 48 85 D2 0F 84 ? ? ? ? 48 89 6C 24 ? 4C 8D 0D ? ? ? ? 33 ED 0F B7 CD 0F 1F 80 ? ? ? ? 0F B7 C1 41 39 1C 81 74 ? 66 FF C1 66 83 F9 ? 72 ? 48 8B 6C 24 ? 48 83 C4 ? 5B C3 66 83 F9 ? 0F 83 ? ? ? ? 0F B7 C1 48 8D 0C 40 48 39 2C CA 48 8D 0C CA 0F 84 ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 48 89 74 24 ? 48 89 7C 24 ? 41 0F BF F8 8B D7 E8 ? ? ? ? 48 8B F0 48 85 C0 0F 84 ? ? ? ? 45 33 C0 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? 84 C0 75 ? 45 33 C0 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? E9 ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 75 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 48 8B 10 48 8B C8 FF 52 ? BA ? ? ? ? E9 ? ? ? ? 40 38 2D ? ? ? ? 76 ? 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? 48 8B 10 48 8B C8 FF 52 ? BA ? ? ? ? E9 ? ? ? ? 48 8B CE E8 ? ? ? ? 44 8B C8 89 6C 24 ? 44 8B C7 8B D3 B9 ? ? ? ? E8 ? ? ? ? 48 8B 0D ? ? ? ? 48 8B 01 FF 10 89 6C 24 ? 48 8D 0D ? ? ? ? 4C 8B C8 89 6C 24 ? BA ? ? ? ? 89 6C 24 ? 41 B8 ? ? ? ? E8 ? ? ? ? 84 C0 74 ? 41 B0 ? 48 8D 0D ? ? ? ? BA ? ? ? ? E8 ? ? ? ? EB ? 48 8B 0D ? ? ? ?")]
        internal static IntPtr DesynthNoWindow;

        [Offset("Search 48 89 5C 24 ? 57 48 83 EC ? 41 0F B7 F8 48 8D 0D ? ? ? ?")]
        internal static IntPtr ConvertToMateria;
        
        [Offset("Search 48 8D 05 ? ? ? ? 33 D2 48 89 03 48 8D 4B ? 33 C0 Add 3 TraceRelative")]
        internal static IntPtr ItemAppraisalAgentVtable;

        [Offset("Search 66 C7 87 ? ? ? ? ? ? 48 8D 4D ? Add 3 Read8")]
        internal static int ItemAppraisalReady;

        [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 0F B6 DA 48 8B F9")]
        internal static IntPtr EntrustRetainerFunc;
        
        [Offset("Search 48 8D 05 ? ? ? ? 48 89 6F ? 48 89 07 48 8D 9F ? ? ? ? Add 3 TraceRelative")]
        internal static IntPtr AgentRetainerInventoryVtable;
        
        [Offset("Search 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 80 B9 ? ? ? ? ? 41 8B F0")]
        internal static IntPtr SellFunc;
        
        [Offset("Search 40 56 48 83 EC ? 8B F2 48 8B 15 ? ? ? ?")]
        internal static IntPtr RemoveMateriaFunc;
        
        [Offset("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? EB ? 44 0F B7 41 ? Add 3 TraceRelative")]
        internal static IntPtr MateriaParam;
        
        [Offset("Search 48 8D 05 ? ? ? ? 33 FF 48 89 03 48 8D 4B ? Add 3 TraceRelative")]
        [OffsetCN("Search 48 8D 05 ? ? ? ? 48 8D 4B ? 48 89 03 E8 ? ? ? ? 48 8D 4B ? E8 ? ? ? ? 33 C9 Add 3 TraceRelative")]
        internal static IntPtr AgentMeldVtable;
        
        [Offset("Search 48 89 91 ? ? ? ? 33 D2 C7 81 ? ? ? ? ? ? ? ?")]
        internal static IntPtr MeldWindowFunc;
        
        [Offset("Search 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 49 8B E8 48 8B F2 48 8B F9 48 85 D2 0F 84 ? ? ? ?")]
        internal static IntPtr AffixMateriaFunc;
        
        [Offset("Search 48 8D 0D ? ? ? ? 74 ? E8 ? ? ? ? 85 C0 Add 3 TraceRelative")]
        internal static IntPtr AffixMateriaParam;
        
        [Offset("Search 48 89 5C 24 ? 57 48 83 EC ? 41 0F B7 F8 48 8D 0D ? ? ? ?")]
        internal static IntPtr ExtractMateriaFunc;
        
        [Offset("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? 39 73 ? Add 3 TraceRelative")]
        internal static IntPtr ExtractMateriaParam;
        
        [Offset("Search 48 8D 05 ? ? ? ? 48 8D 4F ? 48 89 07 E8 ? ? ? ? 33 C9 Add 3 TraceRelative")]
        internal static IntPtr AgentOutOnLimbVtable;
        
        [Offset("Search 48 8D 05 ? ? ? ? 48 89 01 48 8D 05 ? ? ? ? 48 89 41 ? 48 8B D9 Add 3 TraceRelative")]
        internal static IntPtr AgentOutHandIn;
        
        [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC ? 48 8B 41 ? 48 8B E9 48 83 C1 ?")]
        internal static IntPtr HandInFunc;
        
        [Offset("Search 4C 8D 2D ? ? ? ? 48 89 74 24 ? Add 3 TraceRelative")]
        internal static IntPtr AgentHousingSelectBlock;
        
        [Offset("Search 48 8D 05 ? ? ? ? C6 43 ? ? 48 89 03 48 8B C3 C7 43 ? ? ? ? ? Add 3 TraceRelative")]
        internal static IntPtr AgentRecommendEquip;
        
        [Offset("Search 48 8D 05 ? ? ? ? 89 77 ? Add 3 TraceRelative")]
        internal static IntPtr AgentCharacter;
        
        [Offset("Search 48 8D 05 ? ? ? ? BF ? ? ? ? 48 89 03 48 8D 73 ? Add 3 TraceRelative")]
        internal static IntPtr AgentContentsInfo;
    }

    internal static partial class Offsets
    {
        [Offset("Search 48 8D 05 ? ? ? ? 48 89 06 48 8D 5E ? Add 3 TraceRelative")]
        internal static IntPtr AgentRetainerList;
        [Offset("Search 48 8D 8E ? ? ? ? 33 D2 41 B8 ? ? ? ? E8 ? ? ? ? 48 8B 5C 24 ? 48 8B C6 48 8B 74 24 ? 48 83 C4 ? 5F C3 ? ? ? ? ? ? 48 89 5C 24 ? Add 3 Read32")]
        internal static int AgentRetainerOffset;
        
        [Offset("Search 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 8B F0 48 85 C0 74 ? 48 83 38 ? Add 3 TraceRelative")]
        internal static IntPtr RetainerData;
        [Offset("Search 83 FA ? 73 ? 8B C2 0F B6 94 08 ? ? ? ? 80 FA ?")]
        internal static IntPtr GetRetainerPointer; 
        [Offset("Search 48 83 39 ? 4C 8B C9")]
        internal static IntPtr GetNumberOfRetainers; 
        [Offset("Search 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 8B E9 41 8B D9 48 8B 0D ? ? ? ? 41 8B F8 8B F2")]
        internal static IntPtr RequestRetainerData; 
        [Offset("Search 48 8D 56 ? EB ? ? ? ? ? ? ? ? ? ? ? 40 53 Add 3 Read8")]
        internal static int RetainerName;
        [Offset("Search 83 FB ? 72 ? 33 D2 48 8D 4C 24 ? E8 ? ? ? ? 48 8D 15 ? ? ? ? Add 2 Read8")]
        internal static int MaxRetainers;
        [Offset("Search 66 83 78 ? ? 74 ? 8B 78 ? E8 ? ? ? ? Add 3 Read8")]
        internal static int VentureTask;
        [Offset("Search 8B 78 ? E8 ? ? ? ? 3B F8 Add 2 Read8")]
        internal static int VentureFinishTime;
        [Offset("Search B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 5C 24 ?? C6 85 ?? ?? ?? ?? ?? Add 1 Read32")]
        internal static int RetainerNetworkPacket;
        
    }
#pragma warning restore CS0649
}