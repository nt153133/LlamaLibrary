using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.Helpers
{
    public static class FreeCompanyActions
    {
        public static async Task ActivateBuffs(int buff1, int buff2)
        {
            if (!FreeCompany.Instance.IsOpen)
            {
                AgentFreeCompany.Instance.Toggle();
                await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
            }
            
            var curActions = await AgentFreeCompany.Instance.GetCurrentActions();
            var fcActions = await AgentFreeCompany.Instance.GetAvailableActions();

            if (curActions.Length == 2) return;
            
            var buffs1 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff1);
            var buffs2 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff2);
            
            if (fcActions.Length == 0)
            {
                //Log($"No Buffs: Activating");
                if (!FreeCompanyAction.Instance.IsOpen)
                {
                    FreeCompany.Instance.SelectActions();
                    await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                }

                if (FreeCompanyAction.Instance.IsOpen)
                {
                    if (buffs1 != null)
                    {
                        await FreeCompanyAction.Instance.EnableAction(buffs1.Index);
                    }

                    await Coroutine.Sleep(500);
                    
                    if (buffs2 != null)
                    {
                        await FreeCompanyAction.Instance.EnableAction(buffs2.Index);
                    }
                }
            }
            else
            {
                if (!curActions.Any(i=> i.id == buff1))
                {
                    Logger.Info("Buff 1 not active");
                    if (!FreeCompanyAction.Instance.IsOpen)
                    {
                        FreeCompany.Instance.SelectActions();
                        await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                    }

                    if (FreeCompanyAction.Instance.IsOpen)
                    {
                        if (buffs1 != null)
                        {
                            await FreeCompanyAction.Instance.EnableAction(buffs1.Index);
                        }
                    }
                }
                else
                {
                    Logger.Info("Buff 2 not active");
                    if (!FreeCompanyAction.Instance.IsOpen)
                    {
                        FreeCompany.Instance.SelectActions();
                        await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                    }

                    if (FreeCompanyAction.Instance.IsOpen)
                    {
                        if (buffs2 != null)
                        {
                            await FreeCompanyAction.Instance.EnableAction(buffs2.Index);
                        }
                    }
                }
            }
            
            if (FreeCompany.Instance.IsOpen)
                FreeCompany.Instance.Close();
        }
    }
}