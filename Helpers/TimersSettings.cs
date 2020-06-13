using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using ff14bot.Helpers;
using Newtonsoft.Json;

namespace LlamaLibrary.Helpers
{
    public class TimersSettings: JsonSettings
    {
        private static TimersSettings _settings;

        private Dictionary<int,SavedTimer> _savedTimers = new Dictionary<int, SavedTimer>();

        private bool _debug;

        private bool _gil;

        private bool _merge;
        private bool _role;

        private bool _category;

        private int _numOfRetainers;

        public static TimersSettings Instance => _settings ?? (_settings = new TimersSettings());
        public TimersSettings() : base(Path.Combine(CharacterSettingsDirectory, "TimersSettings.json"))
        {
        }

        public Dictionary<int,SavedTimer> SavedTimers
        {
            get => _savedTimers;
            set
            {
                if (_savedTimers != value)
                {
                    _savedTimers = value;
                    Save();
                }
            }
        }

        public void SetTimer(int cycle, DateTimeOffset time)
        {
            if (_savedTimers.ContainsKey(cycle))
            {
                _savedTimers[cycle] = new SavedTimer(time, Timers.CurrentTime);
            }
            else
            {
                _savedTimers.Add(cycle, new SavedTimer(time, Timers.CurrentTime));
            }
            Save();
        }

        public DateTimeOffset GetTimer(int cycle)
        {
            if (_savedTimers.ContainsKey(cycle))
            {
                if (_savedTimers[cycle].IsValid)
                    return _savedTimers[cycle].ResetTime;
                
                Log($"Timer Invalid getting new one for cycle: {cycle}");
                _savedTimers[cycle] = new SavedTimer(DateTimeOffset.FromUnixTimeSeconds(Timers.GetNextCycle(cycle)).LocalDateTime, Timers.CurrentTime);
            }
            else
            {
                Log($"No Timer saved for cycle: {cycle}");
                _savedTimers.Add(cycle, new SavedTimer(DateTimeOffset.FromUnixTimeSeconds(Timers.GetNextCycle(cycle)).LocalDateTime, Timers.CurrentTime));
            }
            Save();
            
            return _savedTimers[cycle].ResetTime;
        }
        
        private void Log(string text)
        {
            Logging.Write(Colors.Tomato, $"[{GetType().Name}] {text}");
        }
    }
    
    public struct SavedTimer
    {
        public DateTimeOffset ResetTime;
        public DateTimeOffset LastChecked;
        
        public SavedTimer(DateTimeOffset resetTime, DateTimeOffset lastChecked)
        {
            ResetTime = resetTime;
            LastChecked = lastChecked;
        }

        [JsonIgnore]
        public bool IsValid => ((ResetTime - LastChecked).TotalSeconds > 0) && !((Timers.CurrentTime - ResetTime).TotalSeconds > 0);
    }
}