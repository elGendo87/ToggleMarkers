using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using UnityEngine; // Needed to open the links Application.OpenURL

namespace ToggleMarkers.MOD
{
    [FileLocation(nameof(ToggleMarkers))]
    [SettingsUIGroupOrder(kKeybindingGroup, kInfoGroup)]
    [SettingsUIShowGroupName(kKeybindingGroup, kInfoGroup)]
    [SettingsUIKeyboardAction(Mod.kButtonActionName, ActionType.Button, usages: new string[] { "InGame" })]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kKeybindingGroup = "Key Binding";
        public const string kInfoGroup = "Mod Info and Support";

        public Setting(IMod mod) : base(mod) { }

        [SettingsUIKeyboardBinding(BindingKeyboard.F8, Mod.kButtonActionName)]
        [SettingsUISection(kSection, kKeybindingGroup)]
        public ProxyBinding KeyboardBinding { get; set; }

        public override void SetDefaults()
        {
            // Default values
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kKeybindingGroup)]
        public bool ResetBindings
        {
            set { ResetKeyBindings(); }
        }

        // --- INFORMATION SECTION (Read Only) ---

        [SettingsUISection(kSection, kInfoGroup)]
        public string ModInfo => "elGendo87";

        [SettingsUISection(kSection, kInfoGroup)]
        public string ModVersion => "1.1.0";

        // --- SUPPORT BUTTONS ---

        [SettingsUIButton]
        [SettingsUISection(kSection, kInfoGroup)]
        public bool SupportKofi
        {
            set { Application.OpenURL("https://ko-fi.com/elgendo87"); }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kInfoGroup)]
        public bool SupportPaypal
        {
            set { Application.OpenURL("https://paypal.me/elGendo87"); }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kInfoGroup)]
        public bool JoinDiscord
        {
            set { Application.OpenURL("https://discord.gg/yN2xb93NMm"); }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kInfoGroup)]
        public bool FollowX
        {
            set { Application.OpenURL("https://x.com/elGendo87"); }
        }
    }
}