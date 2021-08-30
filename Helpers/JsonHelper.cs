using System.IO;
using ff14bot;
using ff14bot.Helpers;

namespace LlamaLibrary.Helpers
{
    public static class JsonHelper
    {
        public static string UniqueCharacterSettingsDirectory => Path.Combine(JsonSettings.SettingsPath, $"{Core.Me.Name}_World{WorldHelper.HomeWorldId}");

        public static string HomeWorldSettingsDirectory => Path.Combine(JsonSettings.SettingsPath, $"World{WorldHelper.HomeWorldId}");

        public static string DataCenterSettingsDirectory => Path.Combine(JsonSettings.SettingsPath, $"DataCenter{WorldHelper.DataCenterId}");
    }
}