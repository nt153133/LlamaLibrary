//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/3.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Clio.Utilities;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using TreeSharp;

namespace ff14bot.NeoProfiles.Tags
{
    [XmlElement("LLHandOver")]
    // ReSharper disable once InconsistentNaming
    public class LLHandOverTag : ProfileBehavior
    {
        private bool _dialogwasopen;

        private bool _doneTalking;

        protected bool IsDoneOverride;
        private string _itemNames;
        public Vector3 Position = Vector3.Zero;

        private string _questGiver;
        private HashSet<BagSlot> _usedSlots;

        [XmlAttribute("ItemIds")]
        [XmlAttribute("ItemId")]
        public int[] ItemIds { get; set; }

        [DefaultValue(0)]
        [XmlAttribute("DialogOption")]
        public int DialogOption { get; set; }

        [XmlAttribute("RequiresHq")] 
        public bool[] RequiresHq { get; set; }

        [XmlAttribute("Amount")] 
        public int[] Amount { get; set; }

        [XmlAttribute("InteractDistance")]
        [DefaultValue(5f)]
        public float InteractDistance { get; set; }

        [XmlAttribute("XYZ")]
        public Vector3 XYZ
        {
            get => Position;
            set => Position = value;
        }

        [XmlAttribute("NpcId")] public int NpcId { get; set; }

        #region Overrides of ProfileBehavior

        public override bool IsDone
        {
            get
            {
                if (IsQuestComplete)
                    return true;
                if (IsStepComplete)
                    return true;

                if (_doneTalking) return true;

                return false;
            }
        }

        #endregion


        public override string StatusText => "Talking to " + _questGiver;

        public GameObject NPC => GameObjectManager.GameObjects.FirstOrDefault(i=> i.NpcId == (uint) NpcId && i.IsVisible && i.IsTargetable);

        protected override void OnStart()
        {
            _questGiver = DataManager.GetLocalizedNPCName(NpcId);
            _usedSlots = new HashSet<BagSlot>();
            if (RequiresHq == null)
            {
                if (ItemIds != null) RequiresHq = new bool[ItemIds.Length];
            }
            else
            {
                if (RequiresHq.Length != ItemIds.Length) LogError("RequiresHq must have the same number of items as ItemIds");
                return;
            }

            if (Amount != null)
            {
                if (Amount.Length != ItemIds.Length) LogError("Amount must have the same number of items as ItemIds");
                return;
            }

            var sb = new StringBuilder();
            if (ItemIds != null)
                for (var i = 0; i < ItemIds.Length; i++)
                {
                    var item = DataManager.GetItem((uint) ItemIds[i], RequiresHq[i]);

                    if (i == ItemIds.Length - 1)
                        sb.Append($"{item.CurrentLocaleName}");
                    else
                        sb.Append($"{item.CurrentLocaleName},");
                }

            _itemNames = sb.ToString();
        }

        protected override void OnResetCachedDone()
        {
            _doneTalking = false;
            _dialogwasopen = false;
        }

        protected override Composite CreateBehavior()
        {
            return new PrioritySelector(
                ctx => NPC,
                new Decorator(ret => SelectYesno.IsOpen,
                    new Action(r => { SelectYesno.ClickYes(); })
                ),
                new Decorator(ret => SelectString.IsOpen,
                    new Action(r => { SelectString.ClickSlot((uint) DialogOption); })
                ),
                new Decorator(r => Talk.DialogOpen, new Action(r =>
                {
                    _dialogwasopen = true;
                    Talk.Next();
                    return RunStatus.Failure;
                })),
                new Decorator(r => Request.IsOpen, new Action(r =>
                {
                    var items = InventoryManager.FilledInventoryAndArmory.Where(i => i.BagId != InventoryBagId.EquippedItems).OrderByDescending(i => i.Count).ToArray();
                    for (var i = 0; i < ItemIds.Length; i++)
                    {
                        BagSlot item;
                        if (RequiresHq[i])
                            item = items.FirstOrDefault(z => z.RawItemId == ItemIds[i] && z.IsHighQuality && !_usedSlots.Contains(z));
                        else
                            item = items.FirstOrDefault(z => z.RawItemId == ItemIds[i] && !_usedSlots.Contains(z));


                        if (item == null)
                        {
                            if (RequiresHq[i])
                                LogError("We don't have any high quality items with an id of {0}", ItemIds[i]);
                            else
                                LogError("We don't have any items with an id of {0}", ItemIds[i]);
                        }
                        else
                        {
                            item.Handover();
                            _usedSlots.Add(item);
                        }
                    }

                    _usedSlots.Clear();
                    Request.HandOver();
                })),
                new Decorator(r => _dialogwasopen && !Core.Player.HasTarget, new Action(r =>
                {
                    _doneTalking = true;
                    return RunStatus.Success;
                })),

                //new Decorator(r => !Talk.DialogOpen && dialogwasopen && !SelectIconString.IsOpen, new Action(r => { DoneTalking = true; return RunStatus.Success; })),
                new Decorator(r => SelectIconString.IsOpen, new Action(r =>
                {
                    SelectIconString.ClickLineEquals(QuestName);
                    return RunStatus.Success;
                })),
                // If we're in interact range, and the NPC/Placeable isn't here... wait 30s.
                CommonBehaviors.MoveAndStop(ret => XYZ, ret => InteractDistance, true, ret => $"[{GetType().Name}] Moving to {XYZ} so we can give {_itemNames} to {_questGiver} for {QuestName}"),
                // If we're in interact range, and the NPC/Placeable isn't here... wait 30s.
                new Decorator(ret => NPC == null, new Sequence(new SucceedLogger(r => $"Waiting at {Core.Player.Location} for {_questGiver} to spawn"), new WaitContinue(5, ret => NPC != null, new Action(ret => RunStatus.Failure)))),
                new Action(ret => NPC.Interact())
            );
        }
    }
}