using System.ComponentModel;
using System.Configuration;
using System.IO;
using ff14bot.Helpers;
using Newtonsoft.Json;

namespace LlamaLibrary.Reduce
{
    public class ReduceSettings : JsonSettings
    {
        private static string Name => "Reduce";
        private static ReduceSettings _settings;
        private bool _initialized;
        private bool _includeArmory;
        private bool _includeDe10000;
        private bool _stayRunning;
        private int _AEZone;
        private bool _AEZoneCheck;
        private bool _openCoffers;

        public ReduceSettings() : base(Path.Combine(CharacterSettingsDirectory, "Reduce.json"))
        {
        }
        
        public static ReduceSettings Instance
        {
            get
            {
                if (_settings != null)
                    return _settings;
                _settings = new ReduceSettings {_initialized = true};

                return _settings;
            }
        }

        [Setting]
        [Description("Include Armory")]
        [DefaultValue(false)]
        [JsonProperty("IncludeArmory")]
        [Category("Bags")]
        public bool IncludeArmory
        {
            get => _includeArmory;
            set { 
                _includeArmory = value;
                Save();
            }
        }

        [Setting]
        [Description("Include low level objects")]
        [DefaultValue(false)]
        [JsonProperty("IncludeDE10000")]
        [Category("Extra")]
        public bool IncludeDE10000
        {
            get => _includeDe10000;
            set { 
                    _includeDe10000 = value;
                    Save();
                }
        }
        
        [Setting]
        [Description("Stay constantly running")]
        [DefaultValue(false)]
        [JsonProperty("StayRunning")]
        [Category("Extra")]
        public bool StayRunning
        {
            get => _stayRunning;
            set { 
                _stayRunning = value;
                Save();
            }
        }
        
        [Setting]
        [Description("Open Coffers")]
        [DefaultValue(false)]
        [JsonProperty("OpenCoffers")]
        public bool OpenCoffers
        {
            get => _openCoffers;
            set { 
                _openCoffers = value;
                Save();
            }
        }
        
        [Setting]
        [Description("ZoneID to check for AE reduction")]
        [DefaultValue(0)]
        [JsonProperty("AEZone")]
        [Category("AE")]
        public int AEZone
        {
            get => _AEZone;
            set { 
                _AEZone = value;
                Save();
            }
        }
        
        [Setting]
        [Description("Only Reduce in set zone")]
        [DefaultValue(false)]
        [JsonProperty("AEZoneCheck")]
        [Category("AE")]
        public bool AEZoneCheck
        {
            get => _AEZoneCheck;
            set { 
                _AEZoneCheck = value;
                Save();
            }
        }

    }
}