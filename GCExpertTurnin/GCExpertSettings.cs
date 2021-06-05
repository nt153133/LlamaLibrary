using System.ComponentModel;
using System.IO;
using ff14bot.Helpers;

namespace LlamaLibrary.GCExpertTurnin
{
    public class GCExpertSettings : JsonSettings
    {
        
        private bool _accepted;
        
        private bool _craft;

        private int _itemId;

        private int _sealReward;
        
        private static GCExpertSettings _settings;

        public static GCExpertSettings Instance => _settings ?? (_settings = new GCExpertSettings());
        
        public GCExpertSettings() :base(Path.Combine(CharacterSettingsDirectory, "GCExpertSettings.json"))
        {
        }
        
        [Description("I understand anything in my inventory bags might be turned in")]
        [DefaultValue(false)] //shift +x
        public bool AcceptedRisk
        {
            get => _accepted;
            set
            {
                if (_accepted != value)
                {
                    _accepted = value;
                    Save();
                }
            }
        }
        
        [Description("Craft itemId until max seals")]
        [DefaultValue(false)] //shift +x
        public bool Craft
        {
            get => _craft;
            set
            {
                if (_craft != value)
                {
                    _craft = value;
                    Save();
                }
            }
        }
        
        [Description("ItemId of item to craft")]
        [DefaultValue(4380)] //shift +x
        public int ItemId
        {
            get => _itemId;
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    Save();
                }
            }
        }
        
        [Description("Number of seals you get as a reward for handing in the above item")]
        [DefaultValue(317)] //shift +x
        public int SealReward
        {
            get => _sealReward;
            set
            {
                if (_sealReward != value)
                {
                    _sealReward = value;
                    Save();
                }
            }
        }
    }
}