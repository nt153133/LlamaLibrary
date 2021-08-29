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
        
        private static readonly InventoryBagId[] SaddlebagIds =
        {
            (InventoryBagId) 0xFA0,(InventoryBagId) 0xFA1//, (InventoryBagId) 0x1004,(InventoryBagId) 0x1005 
        };

        internal static async Task<bool> ReadRetainers(Task retainerTask)
        {
            if (!await OpenRetainerList())
            {
                return false;
            }
            
            foreach (var retainer in RetainerList.Instance.OrderedRetainerList.Where(i=> i.Active))
            {
                Log($"Selecting {retainer.Name}");
                await SelectRetainer(retainer.Unique);

                await retainerTask;

                await DeSelectRetainer();
                Log($"Done with {retainer.Name}");
            }
            if (!await CloseRetainerList())
            {
                LogCritical("Could not close retainer list");
                return false;
            }

            return true;
        }

        internal static async Task<bool> ReadRetainers(Func<Task> retainerTask)
        {
            if (!await OpenRetainerList())
            {
                return false;
            }

            foreach (var retainer in RetainerList.Instance.OrderedRetainerList.Where(i=> i.Active))
            {
                Log($"Selecting {retainer.Name}");
                await SelectRetainer(retainer.Unique);

                await retainerTask();

                await DeSelectRetainer();
                Log($"Done with {retainer.Name}");
            }

            if (!await CloseRetainerList())
            {
                LogCritical("Could not close retainer list");
                return false;
            }

            return true;
        }

        internal static async Task<bool> ReadRetainers(Func<int, Task> retainerTask)
        {
            if (!await OpenRetainerList())
            {
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return false;
            }

            var ordered = RetainerList.Instance.OrderedRetainerList.ToArray();
            var numRetainers = await GetNumberOfRetainers();

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                if (!ordered[retainerIndex].Active) continue;
                Log($"Selecting {ordered[retainerIndex].Name}");
                await SelectRetainer(retainerIndex);

                await retainerTask(retainerIndex);

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex].Name}");
            }

            if (!await CloseRetainerList())
            {
                LogCritical("Could not close retainer list");
                return false;
            }

            return true;
        }

        internal static async Task<List<CompleteRetainer>> ReadRetainers(Func<RetainerInfo, int, Task<CompleteRetainer>> retainerTask)
        {
            List<CompleteRetainer> retainers = new List<CompleteRetainer>();
            if (!await OpenRetainerList())
            {
                return retainers;
            }

            var count = await GetNumberOfRetainers();

            var ordered = RetainerList.Instance.OrderedRetainerList.ToArray();
            var numRetainers = count;

            if (numRetainers <= 0)
            {
                LogCritical("Can't find number of retainers either you have none or not near a bell");
                RetainerList.Instance.Close();
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return retainers;
            }

            for (var retainerIndex = 0; retainerIndex < numRetainers; retainerIndex++)
            {
                if (!ordered[retainerIndex].Active) continue;
                Log($"Selecting {ordered[retainerIndex].Name}");
                await SelectRetainer(retainerIndex);

                retainers.Add(await retainerTask(ordered[retainerIndex], retainerIndex));

                await DeSelectRetainer();
                Log($"Done with {ordered[retainerIndex].Name}");
            }

            if (!await CloseRetainerList())
            {
                LogCritical("Could not close retainer list");
            }

            return retainers;
        }

        internal static async Task<RetainerInfo[]> OpenAndCountRetainers()
        {
            if (!await OpenRetainerList())
            {
                return null;
            }

            var ordered = RetainerList.Instance.OrderedRetainerList.ToArray();
            return ordered;
        }

        public static async Task CloseRetainers()
        {
            if (RetainerList.Instance.IsOpen)
            {
                RetainerList.Instance.Close();
                await Coroutine.Wait(5000, () => !RetainerList.Instance.IsOpen);
            }
        }

        public static async Task<bool> ReadRetainers(Func<RetainerInfo, Task> retainerTask)
        {
            if (!await OpenRetainerList())
            {
                //Move this to botbase based on return value
                TreeRoot.Stop("Failed: Find a bell or some retainers");
                return false;
            }

            foreach (var retainer in RetainerList.Instance.OrderedRetainerList.Where(i=> i.Active))
            {
                Log($"Selecting {retainer.Name}");
                await SelectRetainer(retainer.Unique);

                await retainerTask(retainer);

                await DeSelectRetainer();
                Log($"Done with {retainer.Name}");
            }

            if (!await CloseRetainerList())
            {
                LogCritical("Could not close retainer list");
                return false;
            }

            return true;
        }

        internal static async Task DumpItems(bool includeSaddle = false)
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

            if (includeSaddle)
            {
                await RetrieveFromSaddleBagsRetainer();
            }
        }

        internal static async Task RetrieveFromSaddleBagsRetainer()
        {
            if (await InventoryBuddy.Instance.Open())
            {
                Log("Saddlebags window open");

                var saddleInventory = InventoryManager.GetBagsByInventoryBagId(SaddlebagIds).SelectMany(i => i.FilledSlots);

                var overlap = saddleInventory.Where(i => InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).SelectMany(k => k.FilledSlots).Any(j => j.TrueItemId == i.TrueItemId && j.Item.StackSize > 1 && j.Count < j.Item.StackSize));
                if (overlap.Any())
                {
                    foreach (var slot in overlap)
                    {
                        var haveSlot = InventoryManager.GetBagsByInventoryBagId(RetainerBagIds).SelectMany(k => k.FilledSlots).FirstOrDefault(j => j.TrueItemId == slot.TrueItemId && j.Item.StackSize > 1 && j.Count < j.Item.StackSize);

                        if (haveSlot == default(BagSlot)) break;

                        slot.RetainerEntrustQuantity(Math.Min(haveSlot.Item.StackSize - haveSlot.Count, slot.Count));

                        Log($"(Saddlebag) Entrust {slot.Item.CurrentLocaleName}");

                        await Coroutine.Sleep(500);
                    }
                }

                InventoryBuddy.Instance.Close();
                await Coroutine.Wait(5000, () => !InventoryBuddy.Instance.IsOpen);
                Log("Saddlebags window closed");
            }
        }

        public static async Task<bool> SelectRetainer(int retainerIndex)
        {
            var list = await GetOrderedRetainerArray();

            return await SelectRetainer(list[retainerIndex].Unique);
        }

        public static async Task<bool> SelectRetainer(ulong retainerContentId)
        {
            if (RetainerList.Instance.IsOpen) return await RetainerList.Instance.SelectRetainer(retainerContentId);
            
            if (RetainerTasks.IsOpen)
            {
                if (CurrentRetainer == retainerContentId) return true;

                if (await DeSelectRetainer())
                {
                    return await RetainerList.Instance.SelectRetainer(retainerContentId);
                }
            }

            if (await OpenRetainerList())
            {
                return await RetainerList.Instance.SelectRetainer(retainerContentId);                    
            }

            return false;
        }

        public static async Task<bool> DeSelectRetainer()
        {
            if (!RetainerTasks.IsOpen) return true;
            RetainerTasks.CloseTasks();

            await Coroutine.Wait(3000, () => DialogOpen || SelectYesno.IsOpen);
            
            if (SelectYesno.IsOpen)
            {
                SelectYesno.Yes();
                await Coroutine.Wait(3000, () => DialogOpen || RetainerList.Instance.IsOpen);
            }
            
            while (!RetainerList.Instance.IsOpen)
            {
                if (DialogOpen)
                {
                    Next();
                    await Coroutine.Sleep(100);
                }
                await Coroutine.Wait(3000, () => DialogOpen || RetainerList.Instance.IsOpen);
            }

            return RetainerList.Instance.IsOpen;
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

        public static void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.DodgerBlue, msg);
        }

        public static void LogLoud(string text, params object[] args)
        {
            var msg = string.Format("[" + Name + "] " + text, args);
            Logging.Write(Colors.Goldenrod, msg);
        }

        public static void LogCritical(string text)
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