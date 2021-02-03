using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.Enums;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.Helpers
{
    public static class GrandCompanyHelper
    {
        internal static Dictionary<GrandCompany, KeyValuePair<uint, Vector3>> BaseLocations = new Dictionary<GrandCompany, KeyValuePair<uint, Vector3>>
        {
            {GrandCompany.Immortal_Flames, new KeyValuePair<uint, Vector3>(130, new Vector3(-139.3435f, 4.1f, -100.8658f))},
            {GrandCompany.Order_Of_The_Twin_Adder, new KeyValuePair<uint, Vector3>(132, new Vector3(-67.49361f, -0.5035391f, -2.149932f))},
            {GrandCompany.Maelstrom, new KeyValuePair<uint, Vector3>(128, new Vector3(88.8576f, 40.24876f, 71.6758f))}
        };

        internal static Dictionary<GCNpc, uint> MaelstromNPCs = new Dictionary<GCNpc, uint>
        {
            {GCNpc.Flyer, 1011820},
            {GCNpc.Mage, 1003248},
            {GCNpc.OIC_Administrator, 1003247},
            {GCNpc.Personnel_Officer, 1002388},
            {GCNpc.OIC_Quartermaster, 1002389},
            {GCNpc.OIC_Officer_of_Arms, 1005183},
            {GCNpc.Quartermaster, 1002387},
            {GCNpc.Company_Chest, 2000470},
            {GCNpc.Hunt_Board, 2004438},
            {GCNpc.Entrance_to_the_Barracks, 2007527},
            {GCNpc.Commander, 1003281},
            {GCNpc.Hunt_Billmaster, 1009552}
        };

        internal static Dictionary<GCNpc, uint> FlameNPCs = new Dictionary<GCNpc, uint>
        {
            {GCNpc.Flyer, 1011818},
            {GCNpc.Mage, 1004380},
            {GCNpc.Personnel_Officer, 1002391},
            {GCNpc.OIC_Administrator, 1002392},
            {GCNpc.OIC_Quartermaster, 1003925},
            {GCNpc.Quartermaster, 1002390},
            {GCNpc.OIC_Officer_of_Arms, 1004513},
            {GCNpc.Company_Chest, 2000470},
            {GCNpc.Commander, 1004576},
            {GCNpc.Entrance_to_the_Barracks, 2007529},
            {GCNpc.Hunt_Board, 2004440},
            {GCNpc.Hunt_Billmaster, 1001379}
        };

        internal static Dictionary<GCNpc, uint> TwinAdderNPCs = new Dictionary<GCNpc, uint>
        {
            {GCNpc.Flyer, 1011819},
            {GCNpc.Mage, 1004381},
            {GCNpc.OIC_Administrator, 1002395},
            {GCNpc.Personnel_Officer, 1002394},
            {GCNpc.OIC_Quartermaster, 1000165},
            {GCNpc.Commander, 1000168},
            {GCNpc.Quartermaster, 1002393},
            {GCNpc.Hunt_Billmaster, 1009152},
            {GCNpc.Company_Chest, 2000470},
            {GCNpc.Hunt_Board, 2004439},
            {GCNpc.OIC_Officer_of_Arms, 1004401},
            {GCNpc.Entrance_to_the_Barracks, 2006962}
        };

        internal static Dictionary<GrandCompany, Dictionary<GCNpc, uint>> NpcList = new Dictionary<GrandCompany, Dictionary<GCNpc, uint>>
        {
            {GrandCompany.Immortal_Flames, FlameNPCs},
            {GrandCompany.Order_Of_The_Twin_Adder, TwinAdderNPCs},
            {GrandCompany.Maelstrom, MaelstromNPCs}
        };

        public static async Task GetToGCBase()
        {
            if (Core.Me.GrandCompany == 0) return;
            var GcBase = BaseLocations[Core.Me.GrandCompany];
            Logger.Info($"{Core.Me.GrandCompany} {GcBase.Key} {GcBase.Value}");
            await Navigation.GetTo(GcBase.Key, GcBase.Value);
        }

        public static uint GetNpcByType(GCNpc npc)
        {
            return NpcList[Core.Me.GrandCompany][npc];
        }

        public static async Task InteractWithNpc(GCNpc npc)
        {
            if (Core.Me.GrandCompany == 0) return;
            var targetNpc = GameObjectManager.GetObjectByNPCId(NpcList[Core.Me.GrandCompany][npc]);
            if (targetNpc == null || !targetNpc.IsWithinInteractRange)
            {
                await GetToGCBase();
                targetNpc = GameObjectManager.GetObjectByNPCId(NpcList[Core.Me.GrandCompany][npc]);
            }

            if (targetNpc == null)
                return;
            if (!targetNpc.IsWithinInteractRange)
                await Navigation.OffMeshMoveInteract(targetNpc);
            if (targetNpc.IsWithinInteractRange)
                targetNpc.Interact();
        }

        public static async Task GetToGCBase(GrandCompany grandCompany)
        {
            var GcBase = BaseLocations[grandCompany];
            Logger.Info($"{grandCompany} {GcBase.Key} {GcBase.Value}");
            await Navigation.GetTo(GcBase.Key, GcBase.Value);
        }

        public static uint GetNpcByType(GCNpc npc, GrandCompany grandCompany)
        {
            return NpcList[grandCompany][npc];
        }

        public static async Task InteractWithNpc(GCNpc npc, GrandCompany grandCompany)
        {
            var targetNpc = GameObjectManager.GetObjectByNPCId(NpcList[grandCompany][npc]);
            if (targetNpc == null || !targetNpc.IsWithinInteractRange)
            {
                await GetToGCBase(grandCompany);
                targetNpc = GameObjectManager.GetObjectByNPCId(NpcList[grandCompany][npc]);
            }

            if (targetNpc == null)
                return;
            if (!targetNpc.IsWithinInteractRange)
                await Navigation.OffMeshMoveInteract(targetNpc);
            if (targetNpc.IsWithinInteractRange)
                targetNpc.Interact();
        }

        public static async Task BuyFCAction(GrandCompany grandCompany, int actionId)
        {
            await InteractWithNpc(GCNpc.OIC_Quartermaster, grandCompany);
            await Coroutine.Wait(5000, () => Talk.DialogOpen || Conversation.IsOpen);

            if (!Talk.DialogOpen)
            {
                await InteractWithNpc(GCNpc.OIC_Quartermaster, grandCompany);
                await Coroutine.Wait(5000, () => Talk.DialogOpen);
            }

            if (Talk.DialogOpen || Conversation.IsOpen)
            {
                if (Talk.DialogOpen)
                {
                    Talk.Next();
                    await Coroutine.Wait(5000, () => Conversation.IsOpen);
                }

                if (Conversation.IsOpen)
                {
                    Conversation.SelectLine(0);
                    await Coroutine.Wait(10000, () => FreeCompanyExchange.Instance.IsOpen);
                    if (FreeCompanyExchange.Instance.IsOpen)
                    {
                        await Coroutine.Sleep(500);
                        await FreeCompanyExchange.Instance.BuyAction(actionId);
                        FreeCompanyExchange.Instance.Close();
                    }
                }
            }


        }
    }
}

