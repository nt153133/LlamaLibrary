using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Retainers;
using LlamaLibrary.Structs;
using static LlamaLibrary.Retainers.HelperFunctions;
using static ff14bot.RemoteWindows.Talk;

namespace LlamaLibrary
{
    public static class RetainerRoutine
    {
        public static string Name = "RetainerRoutine";

        internal static async Task<bool> ReadRetainers(Task retainerTask)
        {
            if (!RetainerList.Instance.IsOpen)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't find open bell either you have none or not near a bell");
                return false;
            }

            var numRetainers = RetainerList.Instance.NumberOfRetainers;

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return true;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                Log($"Selecting {RetainerList.Instance.RetainerName(retainerIndex)}");
                await SelectRetainer(retainerIndex);

                await retainerTask;

                await DeSelectRetainer();
                Log($"Done with {RetainerList.Instance.RetainerName(retainerIndex)}");
            }

            RetainerList.Instance.Close();

            return true;
        }

        internal static async Task<bool> ReadRetainers(Func<Task> retainerTask)
        {
            if (!RetainerList.Instance.IsOpen)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't find open bell either you have none or not near a bell");
                return false;
            }

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i => i.Active).ToArray();
            var numRetainers = ordered.Length;

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return true;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                Log($"Selecting {ordered[retainerIndex]}");
                await SelectRetainer(retainerIndex);

                await retainerTask();

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex]}");
            }

            RetainerList.Instance.Close();

            return true;
        }

        internal static async Task<bool> ReadRetainers(Func<int, Task> retainerTask)
        {
            if (!RetainerList.Instance.IsOpen)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't find open bell either you have none or not near a bell");
                return false;
            }

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i => i.Active).ToArray();
            var numRetainers = ordered.Length;

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return true;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                Log($"Selecting {ordered[retainerIndex]}");
                await SelectRetainer(retainerIndex);

                await retainerTask(retainerIndex);

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex]}");
            }

            RetainerList.Instance.Close();

            return true;
        }

        internal static async Task<bool> ReadRetainers(Func<RetainerInfo, int, Task> retainerTask)
        {
            if (!RetainerList.Instance.IsOpen)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't find open bell either you have none or not near a bell");
                return false;
            }

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i => i.Active).ToArray();
            var numRetainers = ordered.Length;

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return true;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                Log($"Selecting {ordered[retainerIndex]}");
                await SelectRetainer(retainerIndex);

                await retainerTask(ordered[retainerIndex], retainerIndex);

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex]}");
            }

            RetainerList.Instance.Close();

            return true;
        }

        internal static async Task<List<CompleteRetainer>> ReadRetainers(Func<RetainerInfo, int, Task<CompleteRetainer>> retainerTask)
        {
            List<CompleteRetainer> Retainers = new List<CompleteRetainer>();
            if (!RetainerList.Instance.IsOpen)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't find open bell either you have none or not near a bell");
                return Retainers;
            }

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i => i.Active).ToArray();
            var numRetainers = ordered.Length;

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return Retainers;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                Log($"Selecting {ordered[retainerIndex].Name}");
                await SelectRetainer(retainerIndex);

                Retainers.Add(await retainerTask(ordered[retainerIndex], retainerIndex));

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex].Name}");
            }

            RetainerList.Instance.Close();

            return Retainers;
        }


        public static async Task<CompleteRetainer> RetainerCheck(RetainerInfo retainer, int Index)
        {
            //Log($"Selected Retainer ({Index}): {retainer.Name}");
            if (SelectString.IsOpen)
            {
                SelectString.ClickLineContains(Translator.SellInventory);
                await Coroutine.Wait(9000, () =>RaptureAtkUnitManager.GetWindowByName("RetainerSellList") != null);
                await Coroutine.Sleep(2000);
                RaptureAtkUnitManager.GetWindowByName("RetainerSellList").SendAction(1, 3uL, 4294967295uL);
                await Coroutine.Wait(9000, () =>SelectString.IsOpen);
                
            }

            var ItemsForSale = InventoryManager.GetBagByInventoryBagId(InventoryBagId.Retainer_Market).FilledSlots.ToList();

            foreach (var item in ItemsForSale)
            {
                Log($"{item}");
            }
            
            var Inventory = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).ToList();

            var completeRetainer = new CompleteRetainer(retainer, Index, ItemsForSale, Inventory);

            return completeRetainer;
        }

        internal static async Task<bool> ReadRetainers(Func<RetainerInfo, Task> retainerTask)
        {
            if (!RetainerList.Instance.IsOpen)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
            }

            if (!RetainerList.Instance.IsOpen)
            {
                LogCritical("Can't find open bell either you have none or not near a bell");
                return false;
            }

            var count = await GetNumberOfRetainers();
            var rets = Core.Memory.ReadArray<RetainerInfo>(Offsets.RetainerData, count);

            var ordered = RetainerList.Instance.OrderedRetainerList(rets).Where(i => i.Active).ToArray();
            var numRetainers = ordered.Length;
            //LogCritical($"Ordered length {numRetainers}");

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return true;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                Log($"Selecting {ordered[retainerIndex].Name}");
                await SelectRetainer(retainerIndex);

                await retainerTask(ordered[retainerIndex]);

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex].Name}");
            }

            RetainerList.Instance.Close();

            return true;
        }

        internal static async Task DumpItems()
        {
            var playerItems = InventoryManager.GetBagsByInventoryBagId(PlayerInventoryBagIds).Select(i => i.FilledSlots).SelectMany(x => x).AsParallel()
                .Where(FilterStackable);

            var retItems = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).Select(i => i.FilledSlots).SelectMany(x => x).AsParallel()
                .Where(FilterStackable);

            var sameItems = playerItems.Intersect(retItems, new BagSlotComparer());
            foreach (var slot in sameItems)
            {
                LogLoud($"Want to move {slot}");
                slot.RetainerEntrustQuantity((int) slot.Count);
                await Coroutine.Sleep(100);
            }
        }

        internal static async Task<bool> SelectRetainer(int retainerIndex)
        {
            if (RetainerList.Instance.IsOpen) return await RetainerList.Instance.SelectRetainer(retainerIndex);

            if (RetainerTasks.IsOpen)
            {
                RetainerTasks.CloseTasks();
                await Coroutine.Wait(1500, () => DialogOpen || RetainerList.Instance.IsOpen);
                await Coroutine.Sleep(200);
                if (DialogOpen) Next();
                //await Coroutine.Sleep(200);
                await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);
                return await RetainerList.Instance.SelectRetainer(retainerIndex);
            }

            if (!RetainerList.Instance.IsOpen && NearestSummoningBell() != null)
            {
                await UseSummoningBell();
                await Coroutine.Wait(5000, () => RetainerList.Instance.IsOpen);
                return await RetainerList.Instance.SelectRetainer(retainerIndex);
            }

            return false;
        }

        internal static async Task<bool> DeSelectRetainer()
        {
            if (!RetainerTasks.IsOpen) return true;
            RetainerTasks.CloseTasks();

            await Coroutine.Wait(1500, () => DialogOpen || SelectYesno.IsOpen);
            if (SelectYesno.IsOpen)
            {
                SelectYesno.Yes();
                await Coroutine.Wait(1500, () => DialogOpen || RetainerList.Instance.IsOpen);
            }

            await Coroutine.Sleep(200);
            if (DialogOpen) Next();
            //await Coroutine.Sleep(200);
            return await Coroutine.Wait(3000, () => RetainerList.Instance.IsOpen);
        }

        public static async Task<bool> RetainerVentureCheck(RetainerInfo retainer)
        {
            if (retainer.Job != ClassJobType.Adventurer)
            {
                if (retainer.VentureTask != 0)
                {
                    var now = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    var timeLeft = retainer.VentureEndTimestamp - now;

                    if (timeLeft <= 0 && SpecialCurrencyManager.GetCurrencyCount(SpecialCurrency.Venture) > 2)
                    {
                        await RetainerHandleVentures();
                    }
                    else
                    {
                        Log($"Venture will be done at {RetainerInfo.UnixTimeStampToDateTime(retainer.VentureEndTimestamp)}");
                    }
                }
            }

            return true;
        }

        public static async Task<bool> RetainerHandleVentures()
        {
            if (!SelectString.IsOpen)
            {
                return false;
            }

            if (SelectString.Lines().Contains(Translator.VentureCompleteText))
            {
                //Log("Venture Done");
                SelectString.ClickLineEquals(Translator.VentureCompleteText);

                await Coroutine.Wait(5000, () => RetainerTaskResult.IsOpen);

                if (!RetainerTaskResult.IsOpen)
                {
                    Log("RetainerTaskResult didn't open");
                    return false;
                }

                var taskId = AgentRetainerVenture.Instance.RetainerTask;

                RetainerTaskResult.Reassign();

                await Coroutine.Wait(5000, () => RetainerTaskAsk.IsOpen);
                if (!RetainerTaskAsk.IsOpen)
                {
                    Log("RetainerTaskAsk didn't open");
                    return false;
                }

                await Coroutine.Wait(2000, RetainerTaskAskExtensions.CanAssign);
                if (RetainerTaskAskExtensions.CanAssign())
                {
                    RetainerTaskAsk.Confirm();
                }
                else
                {
                    Log($"RetainerTaskAsk Error: {RetainerTaskAskExtensions.GetErrorReason()}");
                    RetainerTaskAsk.Close();
                }

                await Coroutine.Wait(1500, () => DialogOpen || SelectString.IsOpen);
                await Coroutine.Sleep(200);
                if (DialogOpen) Next();
                await Coroutine.Sleep(200);
                await Coroutine.Wait(5000, () => SelectString.IsOpen);
            }
            else
            {
                Log("Venture Not Done");
            }


            return true;
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.DodgerBlue, msg);
        }

        public static void LogLoud(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Goldenrod, msg);
        }

        private static void LogCritical(string text)
        {
            Logging.Write(Colors.OrangeRed, "[" + Name + "] " + text);
        }

        private class BagSlotComparer : IEqualityComparer<BagSlot>
        {
            public bool Equals(BagSlot x, BagSlot y)
            {
                return x.RawItemId == y.RawItemId && x.Count + y.Count < x.Item.StackSize;
            }

            public int GetHashCode(BagSlot obj)
            {
                return obj.Item.GetHashCode();
            }
        }
    }
}