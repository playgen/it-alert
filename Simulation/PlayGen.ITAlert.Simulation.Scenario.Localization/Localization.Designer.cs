﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlayGen.ITAlert.Simulation.Scenario.Localization {
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
    internal class Localization {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Localization() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PlayGen.ITAlert.Simulation.Scenario.Localization.Localization", typeof(Localization).Assembly);
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
        ///   Looks up a localized string similar to [{&quot;Key&quot;:&quot;Tutorial_Label&quot;,&quot;en-gb&quot;:&quot;Tutorial&quot;,&quot;nl&quot;:&quot;Tutorial&quot;},
        ///{&quot;Key&quot;:&quot;Tutorial_Label_Continue&quot;,&quot;en-gb&quot;:&quot;Continue&quot;,&quot;nl&quot;:&quot;Doorgaan&quot;},
        ///{&quot;Key&quot;:&quot;Tutorial1_Name&quot;,&quot;en-gb&quot;:&quot;Tutorial 1\nMovement&quot;,&quot;nl&quot;:&quot;Tutorial 1\nBewegen&quot;},
        ///{&quot;Key&quot;:&quot;Tutorial1_Description&quot;,&quot;en-gb&quot;:&quot;This will introduce you to movement around the network and how to use tools.&quot;,&quot;nl&quot;:&quot;Dit scenario legt uit hoe je kunt bewegen in het netwerk en hoe je gereedschap kunt gebruiken.&quot;},
        ///{&quot;Key&quot;:&quot;Tutorial1_Frame1&quot;,&quot;en-gb&quot;:&quot;Welcome to IT Alert!\nYou are a sys [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ScenarioLocalization {
            get {
                return ResourceManager.GetString("ScenarioLocalization", resourceCulture);
            }
        }
    }
}