using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.NeoProfiles;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using TreeSharp;
using LlamaLibrary.Helpers;
using LlamaLibrary.RemoteAgents;
using LlamaLibrary.RemoteWindows;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("LLGoToHousing")]
    public class LLGoToHousing : ProfileBehavior
    {
        private bool _isDone;

        [XmlAttribute("District")] 
        [XmlAttribute("district")] 
        [DefaultValue("Mists")]
        public string District { get; set; }    

        [XmlAttribute("Ward")] 
        [XmlAttribute("ward")] 
        [DefaultValue(0)]
        public int Ward { get; set; }


        public override bool HighPriority => true;

        public override bool IsDone => _isDone;

        protected override void OnStart()
        {
        }

        protected override void OnDone()
        {
        }

        protected override void OnResetCachedDone()
        {
            _isDone = false;
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => GoToWard(Ward, District));
        }

        private async Task GoToWard(int Ward, string District)
        {

            Navigator.PlayerMover = new SlideMover();
            Navigator.NavigationProvider = new ServiceNavigationProvider();

            if (District.Equals("Lavendar Beds", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ConditionParser.IsQuestCompleted(66748))
                {
                    await LlamaLibrary.aHouseChecker.GetToResidential(2);
                    await LlamaLibrary.aHouseChecker.OpenHousingWards();
                    Log($"Traveling to ward {District} Ward - {Ward}");
                    HousingSelectBlock.Instance.SelectWard(Ward -1);
                    LlamaLibrary.RemoteWindows.HousingSelectBlock.Instance.GoToWard(Ward -1);
                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                    if (SelectYesno.IsOpen)
                        SelectYesno.Yes();

                    await Coroutine.Sleep(5000);

                    if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }
                else
                {
                    Logging.WriteDiagnostic("Please complete the quest 'Where the Heart Is (The Lavender Beds)'");
                }


            }

            if (District.Equals("Mists", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ConditionParser.IsQuestCompleted(66750))
                {
                    await LlamaLibrary.aHouseChecker.GetToResidential(8);
                    await LlamaLibrary.aHouseChecker.OpenHousingWards();
                    Log($"Traveling to ward {District} Ward - {Ward}");
                    HousingSelectBlock.Instance.SelectWard(Ward -1);
                    LlamaLibrary.RemoteWindows.HousingSelectBlock.Instance.GoToWard(Ward -1);
                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                    if (SelectYesno.IsOpen)
                        SelectYesno.Yes();

                    await Coroutine.Sleep(5000);

                    if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }
                else
                {
                    Logging.WriteDiagnostic("Please complete the quest 'Where the Heart Is (The Mists)'");
                }


            }

            if (District.Equals("Goblet", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ConditionParser.IsQuestCompleted(66749))
                {
                    await LlamaLibrary.aHouseChecker.GetToResidential(9);
                    await LlamaLibrary.aHouseChecker.OpenHousingWards();
                    Log($"Traveling to ward {District} Ward - {Ward}");
                    HousingSelectBlock.Instance.SelectWard(Ward -1);
                    LlamaLibrary.RemoteWindows.HousingSelectBlock.Instance.GoToWard(Ward -1);  
                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                    if (SelectYesno.IsOpen)
                        SelectYesno.Yes();

                    await Coroutine.Sleep(5000);

                    if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }
                else
                {
                    Logging.WriteDiagnostic("Please complete the quest 'Where the Heart Is (The Goblet)'");
                } 
            }

            if (District.Equals("Shirogane", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ConditionParser.IsQuestCompleted(68167))
                {
                    await LlamaLibrary.aHouseChecker.GetToResidential(111);
                    await LlamaLibrary.aHouseChecker.OpenHousingWards();
                    Log($"Traveling to ward {District} Ward - {Ward}");
                    HousingSelectBlock.Instance.SelectWard(Ward -1);
                    LlamaLibrary.RemoteWindows.HousingSelectBlock.Instance.GoToWard(Ward -1);  
                    await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
                    if (SelectYesno.IsOpen)
                        SelectYesno.Yes();

                    await Coroutine.Sleep(5000);

                    if (CommonBehaviors.IsLoading) await Coroutine.Wait(-1, () => !CommonBehaviors.IsLoading);
                }
                else
                {
                    Logging.WriteDiagnostic("Please complete the quest 'I Dream of Shirogane'");
                } 
            }


            _isDone = true;
        }
    }
}