using System.ComponentModel;
using System.IO;
using ff14bot.Enums;
using ff14bot.Helpers;
using LlamaLibrary.Enums;

namespace LlamaLibrary.Retainers
{
    public class RetainerSettings : JsonSettings
    {
        private static RetainerSettings _settings;

        private bool _deposit;
        
        private bool _depositSaddle;

        private bool _debug;

        private bool _gil;

        private bool _merge;
        private bool _role;

        private bool _category;

        private int _numOfRetainers;
        private MyItemRole _itemRole;
        private ItemUiCategory _itemCategory;
        private bool _ventures;
        private bool _loop;

        public RetainerSettings() : base(Path.Combine(CharacterSettingsDirectory, "RetainerSettings.json"))
        {
        }

        public static RetainerSettings Instance => _settings ?? (_settings = new RetainerSettings());

        [Description("Entrust items to retainer if the have the same item?")]
        [DefaultValue(true)] //shift +x
        public bool DepositFromPlayer
        {
            get => _deposit;
            set
            {
                if (_deposit != value)
                {
                    _deposit = value;
                    Save();
                }
            }
        }
        
        [Description("Entrust same items from saddle bags")]
        [DefaultValue(false)] //shift +x
        public bool DepositFromSaddleBags
        {
            get => _depositSaddle;
            set
            {
                if (_depositSaddle != value)
                {
                    _depositSaddle = value;
                    Save();
                }
            }
        }

        [Description("Print verbose debug info")]
        [DefaultValue(false)] //shift +x
        public bool DebugLogging
        {
            get => _debug;
            set
            {
                if (_debug != value)
                {
                    _debug = value;
                    Save();
                }
            }
        }

        [Description("Withdrawal Gil from each retainer")]
        [DefaultValue(true)] //shift +x
        public bool GetGil
        {
            get => _gil;
            set
            {
                if (_gil != value)
                {
                    _gil = value;
                    Save();
                }
            }
        }

        [Description("Reassign Ventures")]
        [DefaultValue(true)] //shift +x
        public bool ReassignVentures
        {
            get => _ventures;
            set
            {
                if (_ventures != value)
                {
                    _ventures = value;
                    Save();
                }
            }
        }

        [Description("Don't try and merge duplicate item stacks between retainers")]
        [DefaultValue(false)] //shift +x
        public bool DontOrganizeRetainers
        {
            get => _merge;
            set
            {
                if (_merge != value)
                {
                    _merge = value;
                    Save();
                }
            }
        }

        [Description("Loop to continue ventures")]
        [DefaultValue(true)] //shift +x
        public bool Loop
        {
            get => _loop;
            set
            {
                if (_loop != value)
                {
                    _loop = value;
                    Save();
                }
            }
        }
    }

}