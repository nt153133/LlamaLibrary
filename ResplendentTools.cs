using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Pathing.Service_Navigation;
using Generate;
using LlamaLibrary.Helpers;
using Newtonsoft.Json;
using TreeSharp;

//using UI_Checker;

namespace ResplendentTools
{
    public class ResplendentToolsCrafter : BotBase
    {
        private Composite _root;
        public override string Name => _name;
        
        public static string _name => "ResplendentTools";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = false;

        public static bool Carpenter => true;
        public static bool Blacksmith => true;
        public static bool Armorer => true;
        public static bool Goldsmith => true;
        public static bool Leatherworker => true;
        public static bool Weaver => true;
        public static bool Alchemist => true;
        public static bool Culinarian => true;


        private async Task<bool> Run()
        {

            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();


            while (true)
            {
                await TurninResplendentCrafting();
                await Coroutine.Sleep(2000);
                await DoResplendentCrafting();
                await Coroutine.Sleep(2000);
            }

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

        public async static Task<String> CalculateLisbethResplendentOrder(String job, int finalItemId, int cMaterialId, int bMaterialId, int cComponentId, int bComponentId, int aComponentId)
        {
            List<LisbethOrder> outList = new List<LisbethOrder>();

            var item = InventoryManager.FilledSlots.FirstOrDefault(i => i.RawItemId == finalItemId);
            int finalItemCount = (int)(item == null ? 0 : item.Count);

            LisbethOrder order;
            if (ConditionParser.HasAtLeast((uint)cMaterialId, 2))
                order = new LisbethOrder(1, 1, cComponentId, NumberToCraft(finalItemCount, cMaterialId), job);
            else if (ConditionParser.HasAtLeast((uint)bMaterialId, 2))
                order = new LisbethOrder(1, 1, bComponentId, NumberToCraft(finalItemCount, bMaterialId), job);
            else
                order = new LisbethOrder(1, 1, aComponentId, 30 - finalItemCount / 2, job);
            outList.Add(order);
            return JsonConvert.SerializeObject(outList, Formatting.None);
        }

        public async static Task<String> GetLisbethResplendentOrder()
        {

            if (Carpenter && ConditionParser.HasAtLeast(33210, 60) == false && ConditionParser.HasAtLeast(33154, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Carpenter", 33210, 33202, 33194, 33178, 33170, 33162);
            }
            if (Blacksmith && ConditionParser.HasAtLeast(33211, 60) == false && ConditionParser.HasAtLeast(33155, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Blacksmith", 33211, 33203, 33195, 33179, 33171, 33163);
            }
            if (Armorer && ConditionParser.HasAtLeast(33212, 60) == false && ConditionParser.HasAtLeast(33156, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Armorer", 33212, 33204, 33196, 33180, 33172, 33164);
            }
            if (Goldsmith && ConditionParser.HasAtLeast(33213, 60) == false && ConditionParser.HasAtLeast(33157, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Goldsmith", 33213, 33205, 33197, 33181, 33173, 33165);
            }
            if (Leatherworker && ConditionParser.HasAtLeast(33214, 60) == false && ConditionParser.HasAtLeast(33158, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Leatherworker", 33214, 33206, 33198, 33182, 33174, 33166);
            }
            if (Weaver && ConditionParser.HasAtLeast(33215, 60) == false && ConditionParser.HasAtLeast(33159, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Weaver", 33215, 33207, 33199, 33183, 33175, 33167);
            }
            if (Alchemist && ConditionParser.HasAtLeast(33216, 60) == false && ConditionParser.HasAtLeast(33160, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Alchemist", 33216, 33208, 33200, 33184, 33176, 33168);
            }
            if (Culinarian && ConditionParser.HasAtLeast(33217, 60) == false && ConditionParser.HasAtLeast(33161, 1) == false)
            {
                return await CalculateLisbethResplendentOrder("Culinarian", 33217, 33209, 33201, 33185, 33177, 33169);
            }

            return "";

        }

        private static int NumberToCraft(int finalItemCount, int currentIngredientId)
        {
            var currentIngredient = InventoryManager.FilledSlots.FirstOrDefault(i => i.RawItemId == currentIngredientId);
            int currentIngredientCount = (int)(currentIngredient == null ? 0 : currentIngredient.Count);

            return (int)Math.Min(30 - finalItemCount / 2, currentIngredientCount / 2);
        }

        public async static Task DoResplendentCrafting()
        {
            string lisbethOrder = await GetLisbethResplendentOrder();

            if (lisbethOrder == "")
            {
                Log("Not Calling lisbeth.");
            }
            else
            {
                Log("Calling lisbeth");
                if (!await Lisbeth.ExecuteOrders(lisbethOrder))
                {
                    Log("Lisbeth order failed, Dumping order to GCSupply.json");
                    using (StreamWriter outputFile = new StreamWriter("GCSupply.json", false))
                    {
                        await outputFile.WriteAsync(lisbethOrder);
                    }
                }
                else
                    Log("Lisbeth order should be done");
            }

        }

        public async static Task TurninResplendentCrafting()
        {
            await GeneralFunctions.TurninResplendentCrafting();
        }

        private static void Log(string text, params object[] args)
        {
            var msg = string.Format("[" + _name + "] " + text, args);
            Logging.Write(Colors.Gold, msg);
        }

    }
}