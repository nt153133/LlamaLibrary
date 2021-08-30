using System.ComponentModel;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.XmlEngine;
using ff14bot.NeoProfiles;
using LlamaLibrary.Helpers;
using TreeSharp;

namespace LlamaLibrary.OrderbotTags
{
    [XmlElement("SetBluActive")]
    public class SetBluSpells: ProfileBehavior
    {
        private bool _isDone;
        [XmlAttribute("Spells")] public int[] Spells { get; set; }

        [XmlAttribute("Clear")]
        [DefaultValue(false)]
        public bool Clear { get; set; }

        public override bool HighPriority => true;

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
            return new ActionRunCoroutine(r => SetBlueActiveSpells(Spells));
        }

        private async Task SetBlueActiveSpells(int[] spells)
        {
            uint[] newSpells = new uint[spells.Length];
            for (int i = 0; i < spells.Length; i++)
            {
                newSpells[i] = (uint) spells[i];
            }
            if (Clear)
                await BlueMageSpellBook.SetAllSpells(newSpells);
            else
            {
                await BlueMageSpellBook.SetSpells(newSpells);
            }
            await Coroutine.Sleep(100);
            _isDone = true;
        }

        public override bool IsDone => _isDone;
    }
}