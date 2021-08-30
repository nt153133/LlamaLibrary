using System.ComponentModel;
using System.IO;
using ff14bot.Helpers;

namespace LlamaLibrary
{
    public class AutoFollowSettings: JsonSettings
    {
        private static AutoFollowSettings _settings;
        private bool _followLeader;
        private string _followTargetName;
        private bool _isPaused;

        public AutoFollowSettings() : base(Path.Combine(CharacterSettingsDirectory, "AutoFollow.json"))
        {

        }

        public static AutoFollowSettings Instance => _settings ?? (_settings = new AutoFollowSettings());


        [Description("Follow the Party Leader")]
        [DefaultValue(true)] //shift +x
        public bool FollowLeader
        {
            get => _followLeader;
            set
            {
                if (_followLeader == value) return;
                _followLeader = value;
                Save();
            }
        }

        [Description("Paused")]
        [DefaultValue(false)] //shift +x
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused == value) return;
                _isPaused = value;
                Save();
            }
        } 

        [Description("Follow target name")]
        public string FollowTargetName
        {
            get => _followTargetName;
            set
            {
                if (_followTargetName == value) return;
                _followTargetName = value;
                Save();
            }
        }

    }
}