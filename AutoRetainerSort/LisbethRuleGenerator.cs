using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LlamaLibrary.AutoRetainerSort.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LlamaLibrary.AutoRetainerSort
{
    public static class LisbethRuleGenerator
    {
        public static string GetSettingsPath()
        {
            TypeInfo botType = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(i => i.FullName.Contains("Lisbeth.Reborn"))?.DefinedTypes.FirstOrDefault(i => i.Name == "Directories")
                               ?? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(i => i.FullName.Contains("Lisbeth"))?.DefinedTypes.FirstOrDefault(i => i.Name == "Directories");

            if (botType == null)
            {
                AutoRetainerSort.LogCritical($"Couldn't find our Lisbeth install, but we're supposed to generate rules for it...?");
                return string.Empty;
            }

            AutoRetainerSort.Log($"Lisbeth Type {botType.FullName}");

            string assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? string.Empty);
            string settingsPath = botType.GetProperty("SettingsPath")?.GetValue(null) as string ?? string.Empty;

            if (string.IsNullOrEmpty(assemblyLocation) || string.IsNullOrEmpty(settingsPath))
            {
                return string.Empty;
            }

            string settingsFilePath = Path.Combine(assemblyLocation, settingsPath);
            return settingsFilePath;
        }

        private static JObject GetJObject(string settingsFilePath)
        {
            using (StreamReader reader = File.OpenText(settingsFilePath))
            {
                return (JObject) JToken.ReadFrom(new JsonTextReader(reader));
            }
        }

        public static void PopulateSettings(string settingsPath)
        {
            if (string.IsNullOrEmpty(settingsPath))
            {
                AutoRetainerSort.LogCritical("Provided Lisbeth settings path is invalid!");
                return;
            }

            JObject settingsJObject = GetJObject(settingsPath);
            LisbethRetainerRules knownRules = new LisbethRetainerRules(settingsJObject);

            foreach (var indexInfoPair in AutoRetainerSortSettings.Instance.InventoryOptions)
            {
                if (!knownRules.RulesByIndex.ContainsKey(indexInfoPair.Key)) continue;
                var ruleList = knownRules.RulesByIndex[indexInfoPair.Key];
                foreach (uint itemId in indexInfoPair.Value.SpecificItems.Select(x => x.RawItemId).Distinct())
                {
                    ruleList.Add(new LisbethRetainerRules.ItemRule(itemId));
                }
            }

            foreach (CachedInventory cachedInventory in ItemSortStatus.GetAllInventories())
            {
                if (!knownRules.RulesByIndex.ContainsKey(cachedInventory.Index)) continue;
                var ruleList = knownRules.RulesByIndex[cachedInventory.Index];
                foreach (ItemSortInfo sortInfo in cachedInventory.ItemCounts.Select(x => ItemSortStatus.GetSortInfo(x.Key)).Distinct())
                {
                    if (sortInfo.ItemInfo.Unique || sortInfo.ItemInfo.StackSize <= 1) continue;
                    ruleList.Add(new LisbethRetainerRules.ItemRule(sortInfo.RawItemId));
                }
            }

            SetRules(settingsJObject, knownRules);

            using (StreamWriter outputFile = new StreamWriter(settingsPath, false))
            {
                outputFile.Write(JsonConvert.SerializeObject(settingsJObject, Formatting.None));
            }
        }

        private static void SetRules(JObject settingsObject, LisbethRetainerRules rules)
        {
            foreach (JToken retainerToken in settingsObject["Retainers"])
            {
                int retainerIndex = retainerToken["Index"]?.ToObject<int>() ?? 0;
                retainerToken["Rules"] = JToken.FromObject(rules.RulesByIndex[retainerIndex]);
            }
        }
    }

    public class LisbethRetainerRules
    {
        public Dictionary<int, HashSet<ItemRule>> RulesByIndex;

        [JsonObject(MemberSerialization.OptIn)]
        public class ItemRule : IEquatable<ItemRule>
        {
            [JsonProperty("Item")] public readonly uint ItemId;

            [JsonProperty("LowerQuality")] public readonly bool LowerQuality;

            public bool Equals(ItemRule other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return ItemId == other.ItemId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ItemRule)obj);
            }

            public override int GetHashCode() => (int)ItemId;

            public ItemRule(uint itemId, bool lowerQuality = false)
            {
                ItemId = itemId;
                LowerQuality = lowerQuality;
            }
        }

        public LisbethRetainerRules(JObject lisbethSettings)
        {
            RulesByIndex = new Dictionary<int, HashSet<ItemRule>>();
            JToken retainerSettings = lisbethSettings["Retainers"];
            if (retainerSettings == null)
            {
                AutoRetainerSort.LogCritical("No retainers found in Lisbeth's settings!");
                return;
            }

            foreach (JToken retainerInfo in retainerSettings)
            {
                int index = retainerInfo["Index"]?.ToObject<int>() ?? 0;
                RulesByIndex.Add(index, new HashSet<ItemRule>());
                var ruleSet = RulesByIndex[index];
                JToken currentRules = retainerInfo["Rules"];
                if (currentRules == null)
                {
                    AutoRetainerSort.LogCritical("RetainerInfo didn't contain any rules array!");
                    return;
                }

                foreach (JToken rule in currentRules)
                {
                    ItemRule itemRule = rule.ToObject<ItemRule>();
                    if (itemRule == null)
                    {
                        JToken itemIdToken = rule["Item"];
                        AutoRetainerSort.LogCritical(itemIdToken == null ? "Couldn't parse rule! ID is null?" : $"Couldn't parse rule for ID {itemIdToken.ToObject<uint>().ToString()}");
                        continue;
                    }

                    ruleSet.Add(itemRule);
                }
            }
        }
    }
}