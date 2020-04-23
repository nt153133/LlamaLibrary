using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using Generate;
using LlamaLibrary.RemoteWindows;
using Newtonsoft.Json;
using TreeSharp;

//using UI_Checker;

namespace MasterPieceSupplyTest
{
    public class MasterPieceSupplyTester : BotBase
    {
        private Composite _root;
        public override string Name => "GCSupplyOrderLisbeth";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        private async Task<bool> Run()
        {
            //await PrintMasterPieceList();
            await PrintGCSupplyList();

            TreeRoot.Stop("Stop Requested");
            return true;
        }

        public override void Start()
        {
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
        }
        
        public async Task<bool> PrintGCSupplyList()
        {
            
            if (!ContentsInfoDetail.Instance.IsOpen)
            {
                Logging.Write($"Trying to open window");
                
                if (!ContentsInfo.Instance.IsOpen)
                {
                    if (await ContentsInfo.Instance.Open())
                        ContentsInfo.Instance.OpenGCSupplyWindow();
                }
                
                await Coroutine.Wait(5000, () => ContentsInfoDetail.Instance.IsOpen);

                if (!ContentsInfoDetail.Instance.IsOpen)
                {
                    Logging.Write($"Nope failed opening GC Supply window");
                    return false;
                }

            }

            if (!ContentsInfoDetail.Instance.IsOpen)
            {
                Logging.Write($"Nope failed");
                return false;
            }
            List<LisbethOrder> outList = new List<LisbethOrder>();
            int id = 0;
            foreach (var item in ContentsInfoDetail.Instance.GetCraftingTurninItems())
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                var order = new LisbethOrder(id, 1, (int) item.Key.Id, item.Value.Key, item.Value.Value);
                outList.Add(order);
                
                id++;
            }

            foreach (var item in ContentsInfoDetail.Instance.GetGatheringTurninItems())
            {
                Logging.Write($"{item.Key} Qty: {item.Value.Key} Class: {item.Value.Value}");
                var type = "Gather";
                if (item.Value.Value.Equals("Fisher"))
                    type = "Fisher";
                var order = new LisbethOrder(id, 1, (int) item.Key.Id, item.Value.Key, type);
                
                outList.Add(order);
                id++;
            }
            
            ContentsInfoDetail.Instance.Close();
            ContentsInfo.Instance.Close();

            foreach (var order in outList)
            {
                Logging.Write($"{order}");
            }
            
            

            using (StreamWriter outputFile = new StreamWriter("GCSupply.json", false))
            {
                await outputFile.WriteAsync(JsonConvert.SerializeObject(outList, Formatting.None));
            }
            
            return true;
        }

        public async Task<bool> PrintMasterPieceList()
        {
            Dictionary<ClassJobType, int> Classes = new Dictionary<ClassJobType, int>
            {
                {ClassJobType.Carpenter,0},
                {ClassJobType.Blacksmith,1},
                {ClassJobType.Armorer,2},
                {ClassJobType.Goldsmith,3},
                {ClassJobType.Leatherworker,4},
                {ClassJobType.Weaver,5},
                {ClassJobType.Alchemist,6},
                {ClassJobType.Culinarian,7},
                {ClassJobType.Miner,8},
                {ClassJobType.Botanist,9},
                {ClassJobType.Fisher,10},
            };

            if (!MasterPieceSupply.Instance.IsOpen)
            {
                Logging.Write($"Trying to open window");

                AgentModule.ToggleAgentInterfaceById(95);
                await Coroutine.Wait(5000, () => RaptureAtkUnitManager.GetWindowByName("ContentsInfo") != null);
                await Coroutine.Sleep(500);

                if (RaptureAtkUnitManager.GetWindowByName("ContentsInfo") == null)
                {
                    Logging.Write($"Nope failed opening timer window");
                    return false;
                }

                await Coroutine.Sleep(500);
                RaptureAtkUnitManager.GetWindowByName("ContentsInfo").SendAction(2, 3, 0xC, 3, 6);
                await Coroutine.Wait(5000, () => MasterPieceSupply.Instance.IsOpen);
                await Coroutine.Sleep(500);
            }

            if (!MasterPieceSupply.Instance.IsOpen)
            {
                Logging.Write($"Nope failed");
                return false;
            }

            foreach (var job in Classes)
            {
                Logging.Write($"{job.Key}:");

                MasterPieceSupply.Instance.ClassSelected = job.Value;
                await Coroutine.Sleep(1000);

                //Can also use MasterPieceSupply.GetTurninItems() if you don't wanted starred info
                foreach (var item in MasterPieceSupply.Instance.GetTurninItemsStarred())
                {
                    Logging.Write($"{item.Key} Starred: {item.Value}");
                }
            }
            
            MasterPieceSupply.Instance.Close();

            return true;
        }
    }
}