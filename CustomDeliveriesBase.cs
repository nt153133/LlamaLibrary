using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Objects;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteAgents;
using ff14bot.RemoteWindows;
using Generate;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;
using Newtonsoft.Json;
using TreeSharp;

namespace LlamaLibrary
{
    public class CustomDeliveriesBase : BotBase
    {
        private Composite _root;

        public override string Name => NameStatic;

        public static string NameStatic => "Custom Deliveries";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;


        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
        }

        private async Task<bool> Run()
        {
            await CustomDeliveries();
            TreeRoot.Stop("Stop Requested");
            return true;
        }

        private async Task<bool> CustomDeliveries()
        {
            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();
            var DeliveryNpcs = new Dictionary<uint, (uint Zone, Vector3 location, string name, int requiredQuest, uint index)>
            {
                { 1019615, (478, new Vector3(-71.68203f, 206.5714f, 29.38501f), "Zhloe Aliapoh", 67087, 1) }, //(Zhloe Aliapoh) Idyllshire(Dravania) 
                { 1020337, (635, new Vector3(171.312988f, 13.02367f, -89.951965f), "M'naago", 68541, 2) }, //(M'naago) Rhalgr's Reach(Gyr Abania) 
                { 1025878, (613, new Vector3(343.984009f, -120.329468f, -306.019714f), "Kurenai", 68675, 3) }, //(Kurenai) The Ruby Sea(Othard) 
                { 1018393, (478, new Vector3(-62.3016f, 206.6002f, 23.893f), "Adkiragh", 68713, 4) }, //(Adkiragh) Idyllshire(Dravania) 
                { 1031801, (820, new Vector3(52.811401f, 82.993774f, -65.384949f), "Kai-Shirr", 69265, 5) }, //(Kai-Shirr) Eulmore(Eulmore) 
                { 1033543, (886, new Vector3(113.389771f, -20.004639f, -0.961365f), "Ehll Tou", 69425, 6) }, //(Ehll Tou) The Firmament(Ishgard) 
                { 1035211, (886, new Vector3(-115.1127f, 0f, -134.8367f), "Charlemend", 69615, 7) }
            };

            foreach (var npc in DeliveryNpcs.Where(i => ConditionParser.IsQuestCompleted(i.Value.requiredQuest)).OrderByDescending(i => i.Value.index))
            {
                await AgentSatisfactionSupply.Instance.LoadWindow(npc.Value.index);
                List<uint> items = new List<uint>();
                if (!DeliveryNpcs.ContainsKey(AgentSatisfactionSupply.Instance.NpcId))
                {
                    Log($"Bad Npc ID: {AgentSatisfactionSupply.Instance.NpcId}");
                    break;
                }
                Log($"{DeliveryNpcs[AgentSatisfactionSupply.Instance.NpcId].name}");
                Log($"\tHeartLevel:{AgentSatisfactionSupply.Instance.HeartLevel}");
                Log($"\tRep:{AgentSatisfactionSupply.Instance.CurrentRep}/{AgentSatisfactionSupply.Instance.MaxRep}");
                Log($"\tDeliveries Remaining:{AgentSatisfactionSupply.Instance.DeliveriesRemaining}");
                Log($"\tDoH: {DataManager.GetItem(AgentSatisfactionSupply.Instance.DoHItemId)}");
                items.Add(AgentSatisfactionSupply.Instance.DoHItemId);
                Log($"\tDoL: {DataManager.GetItem(AgentSatisfactionSupply.Instance.DoLItemId)}");
                items.Add(AgentSatisfactionSupply.Instance.DoLItemId);
                Log($"\tFsh: {DataManager.GetItem(AgentSatisfactionSupply.Instance.FshItemId)}");
                items.Add(AgentSatisfactionSupply.Instance.FshItemId);

                if (AgentSatisfactionSupply.Instance.HeartLevel == 5 || AgentSatisfactionSupply.Instance.DeliveriesRemaining == 0)
                {
                    Log($"{DeliveryNpcs[AgentSatisfactionSupply.Instance.NpcId].name} Satisfaction Level is Maxed or out of deliveries, skipping");
                    continue;
                }

                List<LisbethOrder> outList = new List<LisbethOrder>();

                if (npc.Key == 1025878)
                {
                    outList.Add(new LisbethOrder(0, 1, (int) AgentSatisfactionSupply.Instance.DoLItemId, Math.Min(3, (int) AgentSatisfactionSupply.Instance.DeliveriesRemaining), "Gather", true));
                }
                else
                {
                    outList.Add(new LisbethOrder(0, 1, (int) AgentSatisfactionSupply.Instance.DoHItemId, Math.Min(3, (int) AgentSatisfactionSupply.Instance.DeliveriesRemaining), "Carpenter", true));
                }

                var order = JsonConvert.SerializeObject(outList, Formatting.None).Replace("Hq", "Collectable");

                if (!InventoryManager.FilledSlots.Any(i => items.Contains(i.RawItemId)))
                {
                    if (order != "" && !InventoryManager.FilledSlots.Any(i => items.Contains(i.RawItemId)))
                    {
                        await GeneralFunctions.StopBusy();
                        Log($"Calling Lisbeth with {order}");
                        try
                        {
                            await Lisbeth.ExecuteOrdersIgnoreHome(order);
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                }

                if (InventoryManager.FilledSlots.Any(i => items.Contains(i.RawItemId)) && AgentSatisfactionSupply.Instance.DeliveriesRemaining > 0)
                {
                    Log("Have items to turn in");
                    await HandInCustomNpc(npc.Key, (npc.Value.Zone, npc.Value.location));
                }

                /*if (AgentSatisfactionSupply.Instance.DeliveriesRemaining == 0)
                {
                    Log("Out of delivery allowances");
                    break;
                }*/
            }

            TreeRoot.Stop("Stop Requested");
            return true;
        }

        private async Task<bool> HandInCustomNpc(uint npcID, (uint Zone, Vector3 location) npcLocation)
        {
            var npc = GameObjectManager.GetObjectByNPCId(npcID);

            if (npc == default(GameObject) || !npc.IsWithinInteractRange)
            {
                await Navigation.GetTo(npcLocation.Zone, npcLocation.location);
                npc = GameObjectManager.GetObjectByNPCId(npcID);
            }

            if (npc == default(GameObject)) return false;

            npc.Interact();

            await Coroutine.Wait(10000, () => Talk.DialogOpen);

            if (!Talk.DialogOpen)
            {
                npc.Interact();

                await Coroutine.Wait(10000, () => Talk.DialogOpen);
            }

            while (Talk.DialogOpen)
            {
                Talk.Next();
                await Buddy.Coroutines.Coroutine.Sleep(200);
                await Coroutine.Yield();
            }

            await Coroutine.Wait(10000, () => Conversation.IsOpen);
            await Buddy.Coroutines.Coroutine.Sleep(500);

            Logging.WriteDiagnostic("Choosing 'Make a delivery.'");
            Conversation.SelectLine(0);
            await Buddy.Coroutines.Coroutine.Wait(1000, () => Talk.DialogOpen);

            if (Talk.DialogOpen)
                while (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Buddy.Coroutines.Coroutine.Sleep(200);
                    await Coroutine.Yield();
                }

            await Coroutine.Wait(10000, () => SatisfactionSupply.Instance.IsOpen);

            if (SatisfactionSupply.Instance.IsOpen)
            {
                do
                {
                    Logging.WriteDiagnostic("Turning in items");

                    if (AgentSatisfactionSupply.Instance.DeliveriesRemaining < 1) break;

                    if (AgentSatisfactionSupply.Instance.HasDoHTurnin)
                        SatisfactionSupply.Instance.ClickItem(0);
                    else if (AgentSatisfactionSupply.Instance.HasDoLTurnin)
                        SatisfactionSupply.Instance.ClickItem(1);
                    else if (AgentSatisfactionSupply.Instance.HasFshTurnin)
                        SatisfactionSupply.Instance.ClickItem(2);

                    await Coroutine.Wait(10000, () => Request.IsOpen);

                    Logging.WriteDiagnostic("Selecting items.");
                    await CommonTasks.HandOverRequestedItems();
                    await Buddy.Coroutines.Coroutine.Sleep(500);

                    if (SelectYesno.IsOpen)
                    {
                        SelectYesno.Yes();
                        await Coroutine.Wait(5000, () => !SelectYesno.IsOpen);
                    }

                    while (!SatisfactionSupply.Instance.IsOpen && !QuestLogManager.InCutscene)
                    {
                        if (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                        }

                        await Buddy.Coroutines.Coroutine.Sleep(500);
                    }

                    if (QuestLogManager.InCutscene)
                    {
                        while (!SatisfactionSupplyResult.Instance.IsOpen && QuestLogManager.InCutscene)
                        {
                            Logging.WriteDiagnostic("Dealing with cutscene.");
                            if (QuestLogManager.InCutscene && AgentCutScene.Instance.CanSkip)
                            {
                                AgentCutScene.Instance.PromptSkip();
                                await Coroutine.Wait(5000, () => SelectString.IsOpen);
                                if (SelectString.IsOpen) SelectString.ClickSlot(0);
                            }

                            if (Talk.DialogOpen)
                            {
                                Talk.Next();
                                await Buddy.Coroutines.Coroutine.Sleep(200);
                            }

                            await Buddy.Coroutines.Coroutine.Sleep(500);
                        }

                        if (SatisfactionSupplyResult.Instance.IsOpen)
                        {
                            Logging.WriteDiagnostic("Clicking Accept.");
                            SatisfactionSupplyResult.Instance.Confirm();
                        }

                        await Coroutine.Wait(5000, () => Talk.DialogOpen);
                        while (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Coroutine.Wait(200, () => !Talk.DialogOpen);
                            await Coroutine.Wait(500, () => Talk.DialogOpen);
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                            await Coroutine.Yield();
                        }

                        await Buddy.Coroutines.Coroutine.Sleep(500);
                        await Coroutine.Wait(5000, () => Talk.DialogOpen);
                        while (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Coroutine.Wait(200, () => !Talk.DialogOpen);
                            await Coroutine.Wait(500, () => Talk.DialogOpen);
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                            await Coroutine.Yield();
                        }

                        await Buddy.Coroutines.Coroutine.Sleep(500);
                        await Coroutine.Wait(5000, () => Talk.DialogOpen);
                        while (Talk.DialogOpen)
                        {
                            Talk.Next();
                            await Coroutine.Wait(200, () => !Talk.DialogOpen);
                            await Coroutine.Wait(500, () => Talk.DialogOpen);
                            await Buddy.Coroutines.Coroutine.Sleep(200);
                            await Coroutine.Yield();
                        }

                        break;
                    }

                    await Coroutine.Wait(5000, () => SatisfactionSupply.Instance.IsOpen);
                    if (!SatisfactionSupply.Instance.IsOpen) break;
                }
                while (AgentSatisfactionSupply.Instance.DeliveriesRemaining > 0 && AgentSatisfactionSupply.Instance.HasAnyTurnin);
            }

            if (SatisfactionSupply.Instance.IsOpen)
            {
                SatisfactionSupply.Instance.Close();
                await Coroutine.Wait(10000, () => !SatisfactionSupply.Instance.IsOpen);
            }

            await Coroutine.Wait(1000, () => Conversation.IsOpen);
            if (Conversation.IsOpen)
            {
                Conversation.SelectLine((uint) (Conversation.GetConversationList.Count - 1));
            }

            return true;
        }

        private static void Log(string text)
        {
            Logging.Write(Colors.Gold, text);
        }
    }
}