﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LlamaLibrary.AutoRetainerSort {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LlamaLibrary.AutoRetainerSort.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} already contains {1}!
        ///Do you want to remove it from {2} and add it to {3}?.
        /// </summary>
        internal static string AddNewItem_AlreadyExists_Warning {
            get {
                return ResourceManager.GetString("AddNewItem_AlreadyExists_Warning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Make sure you&apos;ve looked at your saddlebag &amp; retainer inventories recently, and that you were the last one to do so if you&apos;re sharing your account!.
        /// </summary>
        internal static string AutoSetup_CacheAdvice {
            get {
                return ResourceManager.GetString("AutoSetup_CacheAdvice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If two different inventories have similar items, should that item type be unsorted, or should it belong to the inventory with the most of those types?
        ///Yes for unsorted, No for most types..
        /// </summary>
        internal static string AutoSetup_ConflictQuestion {
            get {
                return ResourceManager.GetString("AutoSetup_ConflictQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This will overwrite all your current SortType settings! Are you sure?.
        /// </summary>
        internal static string AutoSetup_OverwriteWarning {
            get {
                return ResourceManager.GetString("AutoSetup_OverwriteWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You should restart RebornBuddy now before opening any Lisbeth windows, or else the changed Lisbeth settings won&apos;t apply!.
        /// </summary>
        internal static string LisbethRules_RestartRB {
            get {
                return ResourceManager.GetString("LisbethRules_RestartRB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [AutoRetainerSort] .
        /// </summary>
        internal static string LogPrefix {
            get {
                return ResourceManager.GetString("LogPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning.
        /// </summary>
        internal static string WarningCaption {
            get {
                return ResourceManager.GetString("WarningCaption", resourceCulture);
            }
        }
    }
}
