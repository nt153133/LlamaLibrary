using System;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Directors;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using LlamaLibrary.Extensions;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory;
using LlamaLibrary.RemoteAgents;
using Character = LlamaLibrary.RemoteWindows.Character;

namespace LlamaLibrary.ScriptConditions
{
    public static class Helpers
    {
        private static readonly uint[] idList =
        {
            28725,28733,28741,28749,28757,29792,29800,29808,29816,29824,29832,31184,31192,31200,31208,31216,31224,31913,31921,31929,31937,31945,31953,28726,28734,28742,28750,28758,29793,29801,29809,29817,29825,29833,31185,31193,31201,31209,31217,31225,31914,31922,31930,31938,31946,31954,28727,28735,28743,28751,28759,29794,29802,29810,29818,29826,29834,31186,31194,31202,31210,31218,31226,31915,31923,31931,31939,31947,31955,28728,28736,28744,28752,28760,29795,29803,29811,29819,29827,29835,31187,31195,31203,31211,31219,31227,31916,31924,31932,31940,31948,31956,28729,28737,28745,28753,28761,29796,29804,29812,29820,29828,29836,31188,31196,31204,31212,31220,31228,31917,31925,31933,31941,31949,31957,28730,28738,28746,28754,28762,29797,29805,29813,29821,29829,29837,31189,31197,31205,31213,31221,31229,31918,31926,31934,31942,31950,31958,28731,28739,28747,28755,28763,29798,29806,29814,29822,29830,29838,31190,31198,31206,31214,31222,31230,31919,31927,31935,31943,31951,31959,28732,28740,28748,28756,28764,29799,29807,29815,29823,29831,29839,31191,31199,31207,31215,31223,31231,31920,31928,31936,31944,31952,31960
        };
        
        private static readonly uint[] idList0 = { 29896,29897,29901,29902,29903,29909,29910,29911,29912,29913,29919,29920,29921,29922,29923,29929,29930,29931,29932,29933,29939,29940,29941,29942,29943,29946,29947,31278,31279,31283,31284,31285,31291,31292,31293,31294,31295,31301,31302,31303,31304,31305,31311,31312,31313,31314,31315,31318,31319,32007,32008,32012,32013,32014,32020,32021,32022,32023,32024,32030,32031,32032,32033,32034,32040,32041,32042,32043,32044,32047,32048 };
        private static readonly uint[] idList1 = { 29894,29895,29898,29899,29900,29904,29905,29906,29907,29908,29914,29915,29916,29917,29918,29924,29925,29926,29927,29928,29934,29935,29936,29937,29938,29944,29945,31276,31277,31280,31281,31282,31286,31287,31288,31289,31290,31296,31297,31298,31299,31300,31306,31307,31308,31309,31310,31316,31317,32005,32006,32009,32010,32011,32015,32016,32017,32018,32019,32025,32026,32027,32028,32029,32035,32036,32037,32038,32039,32045,32046 };
        private static readonly uint[] idList2 = { 29994,29995,29996,29997,29998,29999,30000,30001,30002,30003,30004,30005,30008,30009,30006,30007,30010,30011,30012,30013,31578,31579,31580,31581,31582,31583,31584,31585,31586,31587,31588,31589,31590,31591,31592,31593,31594,31595,31596,31597,31598,31599,31600,31601,31602,31603,32882,32883,32884,32885,32886,32887,32888,32889,32890,32891,32892,32893,32894,32895,32896,32897,32898,32899,32900,32901,32902,32903,32904,32905,32906,32907 };



        public static int HasIshgardItem()
        {
            return InventoryManager.FilledSlots.Count(i => idList.Contains(i.RawItemId) && i.IsCollectable && i.Collectability > 50);
        }
        
        public static bool IsCNClient()
        {
            return Translator.Language == Language.Chn;
        }
        
        public static bool HasIshgardGatheringMining()
        {
            return InventoryManager.FilledSlots.Any(i => idList0.Contains(i.RawItemId) && i.Count >= 10);
        }
        
        public static bool HasIshgardGatheringBotanist()
        {
            return InventoryManager.FilledSlots.Any(i => idList1.Contains(i.RawItemId) && i.Count >= 10);
        }
		
		public static bool HasIshgardGatheringFisher()
        {
            return InventoryManager.FilledSlots.Any(i => idList2.Contains(i.RawItemId) && i.Count >= 1);
        }

        public static bool LLHasItemNQ(int itemID)
        {
            return InventoryManager.FilledSlots.Count(i => i.RawItemId == itemID && i.IsHighQuality == false) >= 1;
        }

        public static bool LLHasItemHQ(int itemID)
        {
            return InventoryManager.FilledSlots.Count(i => i.RawItemId == itemID && i.IsHighQuality) >= 1;
        }
        
        public static bool LLHasItemCollectable(int itemID)
        {
            return InventoryManager.FilledSlots.Count(i => i.RawItemId == itemID && i.IsCollectable) >= 1;
        }

        public static int GetSkybuilderScrips()
        {
            return (int) SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency) 28063);
        }

        public static bool HasMap()
        {
            return ActionHelper.HasMap();
        }

        public static int AverageItemLevel()
        {
            return InventoryManager.EquippedItems.Where(k => k.IsFilled).Sum(i => i.Item.ItemLevel) / InventoryManager.EquippedItems.Count(k => k.IsFilled);
        }

        public static int NovusLightLevel()
        {
            return (int) (InventoryManager.EquippedItems.First().SpiritBond * 100);
        }
        
        public static int ZodiacLightLevel()
        {
            return (int) (InventoryManager.EquippedItems.First().SpiritBond * 100);
        }
        
        public static int ZodiacCompletedMahatma()
        {
            return (int)(ZodiacLightLevel() / 500) ;
        }
        
        public static bool ZodiacMahatmaIsDone()
        {
            return ((ZodiacLightLevel() % 500) == 80) ;
        }   
        
        public static int GetInstanceTodo(int objective)
        {
            if (DirectorManager.ActiveDirector is InstanceContentDirector activeAsInstance)
            {
                return activeAsInstance.GetTodoArgs(objective).Item1;
            }

            return -1;
        }

        public static bool IsPartyInCombat()
        {
            return PartyManager.VisibleMembers.Any(x => !x.IsMe && x.BattleCharacter.IsAlive && x.BattleCharacter.InCombat);
        }


        public static int CurrentGCSeals()
        {
            uint[] sealTypes = {20, 21, 22};
            var bagslot = InventoryManager.GetBagByInventoryBagId(InventoryBagId.Currency).FirstOrDefault(i => i.RawItemId == sealTypes[(int)Core.Me.GrandCompany -1]);
            return (int) (bagslot?.Count ?? (uint) 0);
        }
        
        public static int MaxGCSeals()
        {
            return (int)Core.Me.MaxGCSeals();
        }
        
        public static int GetNPCIconId(int npcID)
        {
            var npc = GameObjectManager.GetObjectByNPCId((uint) npcID);
            if (npc != null)
            {
                return (int) npc.IconId();
            }

            return 0;
        }
        
        public static int GilCount()
        {
            return (int)InventoryManager.GetBagByInventoryBagId(InventoryBagId.Currency).Where(r => r.IsFilled).FirstOrDefault(item => item.RawItemId == DataManager.GetItem("Gil").Id).Count;
        }
        
        public static bool MsLeftInDungeonGt(long time)
        {
            if (DirectorManager.ActiveDirector == null) return false;
            
            if (DirectorManager.ActiveDirector is InstanceContentDirector activeAsInstance)
            {
                return activeAsInstance.TimeLeftInDungeon.Milliseconds > time;
            }

            return false;
        }
        
        public static int CurrentMettle()
        {
            if (DirectorManager.ActiveDirector == null) return 0;
            
            return (int)Core.Memory.Read<uint>(DirectorManager.ActiveDirector.Pointer + Offsets.CurrentMettle);
        }
        
        public static int NextResistanceRank()
        {
            if (DirectorManager.ActiveDirector == null) return 0;
            
            return (int)Core.Memory.Read<uint>(DirectorManager.ActiveDirector.Pointer + Offsets.NextReistanceRank);
        }

        public static bool IsDutyAvailable(int duty)
        {
            return DutyManager.AvailableContent.Keys.Contains((uint)duty);
        }

        /*public static int GetLeveTodoArgsItem1(int index)
        {
            if (DirectorManager.ActiveDirector == null) return -1;
            var type = DirectorManager.ActiveDirector.GetType();

            if (type == typeof(ff14bot.Directors.BattleLeveConciliate))
                return (DirectorManager.ActiveDirector as BattleLeveConciliate).GetTodoArgs(index).Item1;
            if (type == typeof(ff14bot.Directors.BattleLeveSweep))
                return (DirectorManager.ActiveDirector as BattleLeveSweep).GetTodoArgs(index).Item1;
            if (type == typeof(ff14bot.Directors.BattleLeveDetect))
                return (DirectorManager.ActiveDirector as BattleLeveDetect).GetTodoArgs(index).Item1;
            if (type == typeof(ff14bot.Directors.BattleLeveGuide))
                return (DirectorManager.ActiveDirector as BattleLeveGuide).GetTodoArgs(index).Item1;
            
            DirectorManager.ActiveDirector.GetTodoArgs(index)
        }*/

        public static async Task<bool> UpdateGearSet()
        {
            
            if (!Character.Instance.IsOpen)
            {
                //Logger.Info("Character window not open");
                AgentCharacter.Instance.Toggle();
                //Logger.Info("Toggled");
                try
                {
                    await WaitUntil(() => Character.Instance.IsOpen, timeout:10000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (!Character.Instance.IsOpen)
            {
                return false;
            }
            //Logger.Info($"Can click {Character.Instance.CanUpdateGearSet()}");
            if (!Character.Instance.CanUpdateGearSet())
            {
                Character.Instance.Close();
                return false;
            }
            
            Character.Instance.UpdateGearSet();

            try
            {
                await WaitUntil(() => SelectYesno.IsOpen, timeout:1500);
            }
            catch (Exception e)
            {
                if (Character.Instance.IsOpen)
                {
                    Character.Instance.Close();
                }
                return true;
            }

            if (SelectYesno.IsOpen)
                SelectYesno.Yes();

            try
            {
                await WaitUntil(() => !SelectYesno.IsOpen, timeout:10000);
            }
            catch (Exception e)
            {
                return true;
            }
			
            //await Coroutine.Sleep(200);

            if (Character.Instance.IsOpen)
            {
                Character.Instance.Close();
            }

            return true;
        }
        
        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition())
                {
                    await Task.Delay(frequency);
                }
            });

            if (waitTask != await Task.WhenAny(waitTask,
                                               Task.Delay(timeout)))
                throw new TimeoutException();
        }
        
        
    }
}