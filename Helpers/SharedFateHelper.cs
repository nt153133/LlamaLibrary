using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using LlamaLibrary.Structs;

namespace LlamaLibrary.Helpers
{
    public static class SharedFateHelper
    {
        private static DateTime LastWindowCheck;

        private static SharedFateProgress[] CachedProgress;

        private static TimeSpan CachePeriod = new TimeSpan(0, 1, 0);

        static SharedFateHelper()
        {
            LastWindowCheck = new DateTime(1970, 1, 1);
        }

        public static async Task<SharedFateProgress> GetSharedFateProgress(uint zoneId)
        {
            return (await CachedRead()).FirstOrDefault(i => i.Zone == zoneId);
        }

        public static async Task<SharedFateProgress[]> CachedRead()
        {
            if (DateTime.Now - LastWindowCheck < CachePeriod)
            {
                return CachedProgress;
            }

            CachedProgress = await OpenWindowGetFateProgresses();
            
            LastWindowCheck = DateTime.Now;

            return CachedProgress;
        }

        public static async Task<SharedFateProgress[]> OpenWindowGetFateProgresses()
        {
            if (FateProgress.Instance.IsOpen)
            {
                FateProgress.Instance.Close();
                await Coroutine.Wait(10000, () => !FateProgress.Instance.IsOpen);
            }
            
            AgentFateProgress.Instance.Toggle();

            await Coroutine.Wait(10000, () => FateProgress.Instance.IsOpen);

            if (!FateProgress.Instance.IsOpen)
            {
                return Array.Empty<SharedFateProgress>();
            }

            await Coroutine.Wait(20000, () => AgentFateProgress.Instance.NumberOfLoadedZones == 6);
            
            if (AgentFateProgress.Instance.NumberOfLoadedZones == 0)
            {
                return Array.Empty<SharedFateProgress>();
            }

            var result = AgentFateProgress.Instance.ProgressArray;
            
            if (FateProgress.Instance.IsOpen)
            {
                FateProgress.Instance.Close();
            }
            
            await Coroutine.Wait(10000, () => !FateProgress.Instance.IsOpen);

            return result;
        }
    }
}