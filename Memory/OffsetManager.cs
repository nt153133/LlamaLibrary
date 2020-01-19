﻿/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using Clio.Utilities;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using GreyMagic;
using LlamaLibrary.Helpers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteAgents;

 namespace LlamaLibrary.Memory
{
    internal class OffsetManager
    {
        private static string Name => "OffsetManager";
        private static bool initDone = false;


        internal static void Init()
        {
            if (initDone)
                return;
            
            var types = typeof(Offsets).GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            Parallel.ForEach(types, type =>
                {
                    var pf = new PatternFinder(Core.Memory);
                    if (type.FieldType.IsClass)
                    {
                        var instance = Activator.CreateInstance(type.FieldType);


                        foreach (var field in type.FieldType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            var res = ParseField(field, pf);
                            if (field.FieldType == typeof(IntPtr))
                                field.SetValue(instance, res);
                            else
                                field.SetValue(instance, (int) res);
                        }

                        //set the value
                        type.SetValue(null, instance);
                    }
                    else
                    {
                        var res = ParseField(type, pf);
                        if (type.FieldType == typeof(IntPtr))
                            type.SetValue(null, res);
                        else
                            type.SetValue(null, (int) res);
                    }
                }
            );

            bool retaineragent = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.AgentRetainerAskVtable), typeof(AgentRetainerVenture));
            bool retainerchar = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.AgentRetainerCharacterVtable), typeof(AgentRetainerCharacter));
            bool dawnAgent = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.DawnVtable), typeof(AgentDawn));
            bool retainerInventory = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.AgentRetainerInventoryVtable), typeof(AgentRetainerInventory)); //60
            bool MateriaMelding = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.AgentMeldVtable), typeof(AgentMeld)); //63
            bool OutOnLimb = AgentModule.TryAddAgent(AgentModule.FindAgentIdByVtable(Offsets.AgentOutOnLimbVtable), typeof(AgentOutOnLimb)); //159
            

            Log($"Added Venture Agent: {retaineragent}");
            Log($"Added RetainerChar Agent: {retainerchar}");
            Log($"Added Dawn(Trust) Agent: {dawnAgent}");
            Log($"Added RetainerInventory Agent: {retainerInventory}");
            Log($"Added MateriaMelding Agent: {MateriaMelding}");
            Log($"Added OutOnLimb Agent: {OutOnLimb} {AgentModule.FindAgentIdByVtable(Offsets.AgentOutOnLimbVtable)}");
            AddNamespacesToScriptManager(new[] {"LlamaLibrary", "LlamaLibrary.ScriptConditions"});//
            initDone = true;
            
            
        }
        
        internal static void AddNamespacesToScriptManager(params string[] param)
        {
            var field =
                typeof(ScriptManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                    .FirstOrDefault(f => f.FieldType == typeof(List<string>));

            if (field == null)
            {
                return;
            }

            try
            {
                var list = field.GetValue(null) as List<string>;
                if (list == null)
                {
                    return;
                }

                foreach (var ns in param)
                {
                    if (!list.Contains(ns))
                    {
                        list.Add(ns);
                        Log($"Added namespace '{ns}' to ScriptManager");
                    }
                }
            }
            catch
            {
                Log("Failed to add namespaces to ScriptManager, this can cause issues with some profiles.");
            }
        }


        private static IntPtr ParseField(FieldInfo field, PatternFinder pf)
        {
            var offset = (OffsetAttribute) Attribute.GetCustomAttributes(field, typeof(OffsetAttribute))
                .FirstOrDefault();
            var valcn = (OffsetValueCN) Attribute.GetCustomAttributes(field, typeof(OffsetValueCN))
                .FirstOrDefault();
            var valna = (OffsetValueNA) Attribute.GetCustomAttributes(field, typeof(OffsetValueNA))
                .FirstOrDefault();

            var result = IntPtr.Zero;
            var lang = (Language) typeof(DataManager).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                .First(i => i.FieldType == typeof(Language)).GetValue(null);

            if (lang == Language.Chn)
            {
                if (valcn != null)
                    return (IntPtr) valcn.Value;
                if (offset == null) return IntPtr.Zero;

                var b1 = true;
                var results = pf.FindMany(offset.PatternCN, ref b1);
                if (results != null)
                    result = results[0];
            }
            else
            {
                if (valna != null)
                    return (IntPtr) valna.Value;
                if (offset == null) return IntPtr.Zero;

                var b1 = true;
                var results = pf.FindMany(offset.Pattern, ref b1);
                if (results != null)
                    result = results[0];
            }

            Log($"[{field.Name:,27}] {result.ToInt64():X}");

            return result;
        }

        private static void Log(string text)
        {
            Logger.External(Name, text, Colors.RosyBrown);
        }
    }
}