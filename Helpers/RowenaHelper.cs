namespace LlamaLibrary.Helpers
{
    //Class no longer needed due to rowena supply not rotating any more
    /*
    public static class RowenaHelper
    {
        public static DateTime LastUpdate;
        internal static class Offsets
        {
            [Offset("Search 40 53 48 83 EC ? B9 ? ? ? ? E8 ? ? ? ? 48 8B D8 48 85 C0 75 ? 48 83 C4 ? 5B C3 E8 ? ? ? ? 2B 03 33 D2 F7 73 ? 0F B7 C0 FF C0 0F AF 43 ? 03 03 48 83 C4 ? 5B C3 ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? 48 89 5C 24 ? 48 89 6C 24 ?")]
            internal static IntPtr RowenaGetCycleTime;
            [Offset("Search 48 8D 0D ? ? ? ? 4D 0F 44 C5 Add 3 TraceRelative")]
            internal static IntPtr RowenaItemList;
            [Offset("Search 81 FB ? ? ? ? 0F 83 ? ? ? ? 48 8D 0D ? ? ? ? Add 2 Read32")]
            internal static int RowenaItemCount;
        }

        public static async Task<bool> VerifyRowenaData()
        {
            if (Core.Memory.Read<uint>(Offsets.RowenaItemList) != 0 && LastUpdate < GetCycleEndTime())
            {
                return true;
            }
            AgentContentsInfo.Instance.Toggle();
            await Coroutine.Wait(5000, () => ContentsInfo.Instance.IsOpen);
            ContentsInfo.Instance.OpenMasterPieceSupplyWindow();
            await Coroutine.Wait(5000, () => MasterPieceSupply.Instance.IsOpen);
            await Coroutine.Wait(3000, () => Core.Memory.Read<uint>(Offsets.RowenaItemList) != 0);
            MasterPieceSupply.Instance.Close();
            await Coroutine.Wait(5000, () => !MasterPieceSupply.Instance.IsOpen);
            if (ContentsInfo.Instance.IsOpen)
                ContentsInfo.Instance.Close();
            return Core.Memory.Read<uint>(Offsets.RowenaItemList) != 0;
        }

        public static DateTime GetCycleEndTime()
        {
            IntPtr test;
            lock (Core.Memory.Executor.AssemblyLock)
                test = Core.Memory.CallInjected64<IntPtr>(Offsets.RowenaGetCycleTime, 0);
            return DateTimeOffset.FromUnixTimeSeconds(test.ToInt64()).LocalDateTime;
        }

        public static RowenaItem[] GetItems()
        {
            return VerifyRowenaData().Result ? Core.Memory.ReadArray<RowenaItem>(Offsets.RowenaItemList, Offsets.RowenaItemCount) : new RowenaItem[0];
        }
    }*/
}