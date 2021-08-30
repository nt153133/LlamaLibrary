using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.Enums;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.Helpers
{
    public static class FreeCompanyActions
    {
        public static async Task ActivateBuffs(int buff1, int buff2, GrandCompany grandCompany)
        {
            if (!FreeCompany.Instance.IsOpen)
            {
                AgentFreeCompany.Instance.Toggle();
                await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
            }

            var curActions = await AgentFreeCompany.Instance.GetCurrentActions();
            var fcActions = await AgentFreeCompany.Instance.GetAvailableActions();

            if (curActions.Length == 2)
            {
                if (FreeCompany.Instance.IsOpen)
                    FreeCompany.Instance.Close();
                return;
            }
            await GeneralFunctions.StopBusy(dismount: false);
            var buffs1 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff1);
            var buffs2 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff2);

            if (buffs1 == null && !curActions.Any(i=> i.id == buff1))
            {
                if (FreeCompany.Instance.IsOpen)
                    FreeCompany.Instance.Close();
                await GrandCompanyHelper.BuyFCAction(grandCompany, buff1);
                await Coroutine.Sleep(1000);
                //Logger.Info("Bought buff1");
                if (!FreeCompany.Instance.IsOpen)
                {
                    //Logger.Info("Opening window after buy");
                    AgentFreeCompany.Instance.Toggle();
                    await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
                    if (FreeCompany.Instance.IsOpen)
                    {
                        //Logger.Info("Buff 1 bought checking again");
                        FreeCompany.Instance.SelectActions();
                        await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                        fcActions = await AgentFreeCompany.Instance.GetAvailableActions();
                        buffs1 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff1);
                    }
                }

            }
            await Coroutine.Sleep(500);
            fcActions = await AgentFreeCompany.Instance.GetAvailableActions();
            buffs2 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff2);
            if (buffs2 == null && !curActions.Any(i=> i.id == buff2))
            {
                if (FreeCompany.Instance.IsOpen)
                    FreeCompany.Instance.Close();
                await GrandCompanyHelper.BuyFCAction(grandCompany, buff2);
                await Coroutine.Sleep(1000);
                //Logger.Info("Bought buff2");
                if (!FreeCompany.Instance.IsOpen)
                {
                    //Logger.Info("Opening window after buy");
                    AgentFreeCompany.Instance.Toggle();
                    await Coroutine.Wait(5000, () => FreeCompany.Instance.IsOpen);
                    if (FreeCompany.Instance.IsOpen)
                    {
                        //Logger.Info("Buff 2 bought checking again");
                        FreeCompany.Instance.SelectActions();
                        await Coroutine.Wait(5000, () => FreeCompanyAction.Instance.IsOpen);
                        fcActions = await AgentFreeCompany.Instance.GetAvailableActions();
                        buffs2 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff2);
                    }
                }

            }

            if (curActions.Length == 0)
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
                    fcActions = await AgentFreeCompany.Instance.GetAvailableActions();
                    buffs2 = fcActions.Select((n,index) => new {Action = n, Index = index}).FirstOrDefault(n => n.Action.id == buff2);
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