﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TGC.Group {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Game : global::System.Configuration.ApplicationSettingsBase {
        
        private static Game defaultInstance = ((Game)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Game())));
        
        public static Game Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Alumnos")]
        public string Category {
            get {
                return ((string)(this["Category"]));
            }
            set {
                this["Category"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Back Buffer")]
        public string Name {
            get {
                return ((string)(this["Name"]));
            }
            set {
                this["Name"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Copia de subnautica")]
        public string Description {
            get {
                return ((string)(this["Description"]));
            }
            set {
                this["Description"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\Shaders\\")]
        public string ShadersDirectory {
            get {
                return ((string)(this["ShadersDirectory"]));
            }
            set {
                this["ShadersDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("..\\..\\Media\\")]
        public string MediaDirectory {
            get {
                return ((string)(this["MediaDirectory"]));
            }
            set {
                this["MediaDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("cajaMadera4.jpg")]
        public string TexturaCaja {
            get {
                return ((string)(this["TexturaCaja"]));
            }
            set {
                this["TexturaCaja"] = value;
            }
        }
    }
}
