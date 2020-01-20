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
using Clio.Utilities;
using Clio.XmlEngine;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using ff14bot.RemoteWindows;
using TreeSharp;

namespace ff14bot.NeoProfiles
{
    [XmlElement("LLTurnIn")]
    // ReSharper disable once InconsistentNaming
    public class LLTurnInTag : ProfileBehavior
    {
        private readonly Queue<int> _selectStringIndex = new Queue<int>();
        private bool _dialogwasopen;
        private bool _doneEmote;
        private bool _doneTalking;
        public bool hasrewards;
        protected bool IsDoneOverride;
        public Vector3 Position = Vector3.Zero;
        private QuestResult _questdata;

        private string _questGiver;

        private HashSet<BagSlot> _usedSlots;

        [XmlAttribute("RewardSlot")]
        [DefaultValue(-1)]
        public int RewardSlot { get; set; }

        [XmlAttribute("ItemIds")]
        [XmlAttribute("ItemId")]
        public int[] ItemIds { get; set; }

        [XmlAttribute("RequiresHq")] public bool[] RequiresHq { get; set; }

        [DefaultValue(new int[0])]
        [XmlAttribute("DialogOption")]
        public int[] DialogOption { get; set; }

        [DefaultValue("")]
        [XmlAttribute("Emote")]
        public string Emote { get; set; }

        #region Overrides of ProfileBehavior

        public override bool IsDone
        {
            get
            {
                if (IsQuestComplete)
                    return true;
                return IsStepComplete;
            }
        }

        #endregion

        [XmlAttribute("NpcId")] public int NpcId { get; set; }

        [XmlAttribute("InteractDistance")]
        [DefaultValue(5f)]
        public float InteractDistance { get; set; }

        [XmlAttribute("XYZ")]
        public Vector3 XYZ
        {
            get => Position;
            set => Position = value;
        }


        public override string StatusText => "Talking to " + _questGiver;


        public GameObject NPC => GameObjectManager.GetObjectByNPCId((uint) NpcId);

        protected override void OnStart()
        {
            if (DialogOption.Length > 0)
                foreach (var i in DialogOption)
                {
                    _selectStringIndex.Enqueue(i);
                }

            _usedSlots = new HashSet<BagSlot>();
            _questGiver = DataManager.GetLocalizedNPCName(NpcId);
            if (RewardSlot == -1)
            {
                if (QuestId > 65535)
                    DataManager.QuestCache.TryGetValue((uint) QuestId, out _questdata);
                else
                    DataManager.QuestCache.TryGetValue((ushort) QuestId, out _questdata);

                if (_questdata != null && _questdata.Rewards.Any())
                {
                    var values = _questdata.Rewards.Select(r => new score(r)).OrderByDescending(r => r.Value).ToArray();

                    //If everything is valued the same cause its items that are not equipment most likely
                    if (values.Select(r => r.Value).Distinct().Count() == 1) values = values.OrderByDescending(r => r.Reward.Worth).ToArray();
                    //Now in heavensward the rewardlist doesnt start at 0 for some reason
                    RewardSlot = _questdata.Rewards.IndexOf(values[0].Reward) + 5;
                    hasrewards = true;
                    //AsmManager.JournalResult_SelectItem(window, );
                }
            }
            else
            {
                RewardSlot += 5;
                hasrewards = true;
            }


            if (RequiresHq == null)
            {
                if (ItemIds != null) RequiresHq = new bool[ItemIds.Length];
            }
            else
            {
                if (RequiresHq.Length != ItemIds.Length) LogError("RequiresHq must have the same number of items as ItemIds");
            }


            Log("Turning in quest {0}({1}) from {2} at {3}", QuestName, QuestId, _questGiver, Position);
        }


        protected override void OnResetCachedDone()
        {
            _doneTalking = false;
            _dialogwasopen = false;
            _doneEmote = false;
        }

        protected override Composite CreateBehavior()
        {
            return new PrioritySelector(
                ctx => NPC,
                new Decorator(ret => !_doneEmote && !string.IsNullOrWhiteSpace(Emote),
                    new Action(r =>
                    {
                        GameObjectManager.GetObjectByNPCId((uint) NpcId).Target();
                        ChatManager.SendChat("/" + Emote);
                        _doneEmote = true;
                    })
                ),
                new Decorator(ret => SelectYesno.IsOpen,
                    new Action(r => { SelectYesno.ClickYes(); })
                ),
                new Decorator(ret => SelectString.IsOpen,
                    new Action(r =>
                    {
                        if (_selectStringIndex.Count > 0)
                            SelectString.ClickSlot((uint) _selectStringIndex.Dequeue());
                        else
                            SelectString.ClickSlot(0);
                    })
                ),
                new Decorator(r => SelectString.IsOpen, new Action(r =>
                {
                    SelectString.ClickSlot(0);
                    return RunStatus.Success;
                })),
                new Decorator(r => _dialogwasopen && !Talk.ConvoLock, new Action(r =>
                {
                    _doneTalking = true;
                    return RunStatus.Failure;
                })),
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
                new Decorator(r => JournalResult.IsOpen && JournalResult.ButtonClickable, new Action(r => JournalResult.Complete())),
                new Decorator(r => JournalResult.IsOpen && hasrewards, new Action(r => JournalResult.SelectSlot(RewardSlot))),

                //new Decorator(r => !Talk.DialogOpen && dialogwasopen && !SelectIconString.IsOpen, new Action(r => { DoneTalking = true; return RunStatus.Success; })),
                new Decorator(r => SelectIconString.IsOpen, new Action(r =>
                {
                    SelectIconString.ClickLineEquals(QuestName);
                    return RunStatus.Success;
                })),
                // If we're in interact range, and the NPC/Placeable isn't here... wait 30s.
                new Decorator(r => QuestLogManager.InCutscene, new ActionAlwaysSucceed()),
                CommonBehaviors.MoveAndStop(ret => XYZ, ret => InteractDistance, true, ret => $"[{GetType().Name}] Moving to {XYZ} so we can turnin {QuestName} to {_questGiver}"),
                // If we're in interact range, and the NPC/Placeable isn't here... wait 30s.
                new Decorator(ret => NPC == null, new Sequence(new SucceedLogger(r => $"Waiting at {Core.Player.Location} for {_questGiver} to spawn"), new WaitContinue(5, ret => NPC != null, new Action(ret => RunStatus.Failure)))),
                new Decorator(ret => !Talk.ConvoLock && !SelectIconString.IsOpen, new Action(ret => NPC.Interact()))
            );
        }

        protected override void OnDone()
        {
            _doneEmote = false;
            base.OnDone();
        }

        private struct score
        {
            public score(Reward reward)
            {
                Reward = reward;
                Value = ItemWeightsManager.GetItemWeight(reward);
            }

            public readonly Reward Reward;
            public readonly float Value;
        }
    }
}