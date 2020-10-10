using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.NeoProfiles;
using ff14bot.RemoteWindows;
using TreeSharp;

namespace ff14bot
{
    [XmlElement("ChangeClass")]
    public class ChangeClass : ProfileBehavior
    {
        private bool _isDone;

        public override bool IsDone => _isDone;

        public override bool HighPriority => true;

        [XmlAttribute("Job")] public string job { get; set; }
        
        [XmlAttribute("Force")] 
        [XmlAttribute("force")] 
        [DefaultValue(false)]
        public bool force { get; set; }


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

        private async Task ChangeJob()
        {
            var gearSets = GearsetManager.GearSets.Where(i => i.InUse);
            ClassJobType newjob;
            var foundJob = Enum.TryParse(job.Trim(), true, out newjob);

            if (Core.Me.CurrentJob == newjob && !force)
            {
                _isDone = true;
                return;
            }
            Logging.Write(Colors.Fuchsia, $"[ChangeJobTag] Started");
            Logging.Write(Colors.Fuchsia, $"[ChangeJobTag] Found job: {foundJob} Job:{newjob}");
            if (foundJob && gearSets.Any(gs => gs.Class == newjob))
            {
                Logging.Write(Colors.Fuchsia, $"[ChangeJobTag] Found GearSet");
                gearSets.First(gs => gs.Class == newjob).Activate();
                
                await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
                if (SelectYesno.IsOpen)
                {
                    SelectYesno.Yes();
                    await Coroutine.Sleep(3000);
                }
                
                // await Coroutine.Sleep(1000);
            }
            
            else if (foundJob)
            {
                job = job.Trim() + ("s_Primary_Tool");

                ItemUiCategory category;
                var categoryFound = Enum.TryParse(job, true, out category);
                
                if (categoryFound)
                {
                    Logging.Write(Colors.Fuchsia, $"[ChangeJobTag] Found Item Category: {categoryFound} Category:{category}");
                    var item = InventoryManager.FilledInventoryAndArmory.Where(i => i.Item.EquipmentCatagory == category).OrderByDescending(i=> i.Item.ItemLevel).FirstOrDefault();
                    BagSlot EquipSlot = InventoryManager.GetBagByInventoryBagId(InventoryBagId.EquippedItems)[EquipmentSlot.MainHand];
                    
                    Logging.Write(Colors.Fuchsia, $"[ChangeJobTag] Found Item {item}");
                    if (item != null)
                    {
                        item.Move(EquipSlot);
                    }
                    
                    await Coroutine.Sleep(1000);
                    
                    ChatManager.SendChat("/gs save");
                    
                    await Coroutine.Sleep(1000);
                }
                else
                {
                    Logging.Write(Colors.Fuchsia, $"[ChangeJobTag] Couldn't find item category'");
                }
            }

            _isDone = true;
        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(r => ChangeJob());
        }
    }
}