using System;
using System.Threading.Tasks;
using ff14bot.Managers;

namespace LlamaLibrary.Helpers
{
    public static class SyncRoutines
    {
        public static bool WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1, bool checkWindows = false)
        {
            var t = Task.Run(async delegate
            {
                var waitTask = Task.Run(async () =>
                {
                    while (!condition())
                    {
                        if (checkWindows) RaptureAtkUnitManager.Update();
                        await Task.Delay(frequency);
                        await Task.Yield();
                    }
                });

                if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                {
                    throw new TimeoutException();
                }

                return condition();
            });

            try
            {
                t.Wait();
            }
            catch (AggregateException ae)
            {
            }

            return condition();
        }
    }
}