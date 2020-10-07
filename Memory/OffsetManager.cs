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
using System.Text;
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
        private static string Name => "LLOffsetManager";
        private static bool initDone = false;
        private static StringBuilder sb= new StringBuilder();
        //public static Dictionary<string, string> patterns = new Dictionary<string, string>();

        private static readonly bool _debug = false;
        internal static void Init()
        {
            if (initDone)
                return;
            
            var q1 = from t in Assembly.GetExecutingAssembly().GetTypes()
                     where t.Namespace != null && (t.IsClass && t.Namespace.Contains("LlamaLibrary") && t.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Static).Any(i => i.Name == "Offsets"))
                     select t.GetNestedType("Offsets", BindingFlags.NonPublic | BindingFlags.Static);
            
            var types = typeof(Offsets).GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Concat(q1.SelectMany(j => j.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)));
            using (var pf = new PatternFinder(Core.Memory))
                Parallel.ForEach(types, type =>
                                 {
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

            Dictionary<IntPtr, int> vtables = new Dictionary<IntPtr, int>();
            for (var index = 0; index < AgentModule.AgentVtables.Count; index++)
            {
                vtables.Add(AgentModule.AgentVtables[index], index);
            }

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == "LlamaLibrary.RemoteAgents"
                    select t;

            foreach (var MyType in q.Where(i => typeof(IAgent).IsAssignableFrom(i)))
            {
                //Log(MyType.Name);

                var test = (((IAgent) Activator.CreateInstance(MyType,
                                                               BindingFlags.Instance | BindingFlags.NonPublic,
                                                               null,
                                                               new object[]
                                                               {
                                                                   IntPtr.Zero
                                                               },
                                                               null))).RegisteredVtable;
                if (vtables.ContainsKey(test))
                {
                    Log($"\tTrying to add {MyType.Name} {AgentModule.TryAddAgent(vtables[test], MyType)}");
                }
                else
                    Log($"\tFound one {test.ToString("X")} but no agent");
            }
            AddNamespacesToScriptManager(new[] {"LlamaLibrary", "LlamaLibrary.ScriptConditions", "LlamaLibrary.ScriptConditions.Helpers","LlamaLibrary.ScriptConditions.Extras"});//
            ScriptManager.Init(typeof(ScriptConditions.Helpers));
            initDone = true;
            if (_debug)
                Log($"\n {sb}");
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

                //var b1 = true;
                try
                {
                    result = pf.Find(offset.PatternCN);
                }
                catch (Exception e)
                {

                }
                
            }
            else
            {
                if (valna != null)
                    return (IntPtr) valna.Value;
                if (offset == null) return IntPtr.Zero;

                try
                {
                    result = pf.Find(offset.PatternCN);
                }
                catch (Exception e)
                {

                }
            }

            if (result == IntPtr.Zero)
            {
                if(field.DeclaringType != null && field.DeclaringType.IsNested)
                    Log($"[{field.DeclaringType.DeclaringType.Name}:{field.Name:,27}] Not Found");
                else
                {
                    Log($"[{field.DeclaringType.Name}:{field.Name:,27}] Not Found");
                }
            }

            if (!_debug)
            {
                return result;
            }
            
            if (offset!=null)
                if (field.DeclaringType != null && field.DeclaringType.IsNested && field.FieldType != typeof(int))
                {
                    sb.AppendLine($"{field.DeclaringType.DeclaringType.Name}_{field.Name}, {offset.Pattern}");
                    //patterns.Add($"{field.DeclaringType.DeclaringType.Name}_{field.Name}", offset.Pattern);
                }
                else if (field.FieldType != typeof(int))
                {
                    sb.AppendLine($"{field.Name}, {offset.Pattern}");
                    //patterns.Add($"{field.Name}", offset.Pattern);
                }

            if (valna != null)
                sb.AppendLine($"{field.DeclaringType.Name},{field.Name},{valna}");
            
            if(field.DeclaringType != null && field.DeclaringType.IsNested)
                Log($"[{field.DeclaringType.DeclaringType.Name}:{field.Name:,27}] {result.ToInt64():X}");
            else
            {
                Log($"[{field.DeclaringType.Name}:{field.Name:,27}] {result.ToInt64():X}");
            }


            return result;
        }

        private static void Log(string text)
        {
            Logger.External(Name, text, Colors.RosyBrown);
        }
    }
}