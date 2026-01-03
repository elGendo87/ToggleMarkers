using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using UnityEngine; // Necesario para Application.OpenURL

namespace ToggleMarkers
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
            // Valores iniciales si son necesarios
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kKeybindingGroup)]
        public bool ResetBindings
        {
            set { ResetKeyBindings(); }
        }

        // --- SECCIÓN DE INFORMACIÓN (Solo lectura) ---

        [SettingsUISection(kSection, kInfoGroup)]
        public string ModInfo => "elGendo87"; // Al no tener 'set', es de solo lectura

        [SettingsUISection(kSection, kInfoGroup)]
        public string ModVersion => "1.0.0";

        // --- BOTONES DE SOPORTE Y REDES ---

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

    // --- LOCALIZACIÓN ---
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting) { m_Setting = setting; }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Toggle Markers" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "General" },
                
                // Info Section
                { m_Setting.GetOptionGroupLocaleID(Setting.kInfoGroup), "MOD INFO AND SUPPORT" },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModInfo)), "Mod Author" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModInfo)), "The author of the mod." },
                
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModVersion)), "Current internal mod version." },
                
                // Support Buttons Labels
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SupportKofi)), "Support on Ko-fi" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SupportPaypal)), "Support on PayPal" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.JoinDiscord)), "Join Discord Server" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FollowX)), "Follow on X" },

                // Descriptions/Tooltips
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SupportKofi)), "Buy me a coffee!" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SupportPaypal)), "Direct support via PayPal." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.JoinDiscord)), "Join my Discord server!" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.FollowX)), "Stay updated with my latest mods." },

                // Keybindings
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "KEY BINDING" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.KeyboardBinding)), "Toggle markers visibility" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.KeyboardBinding)), "Shows/hides markers and invisible roads. This will work also with Extra Detailing Tools UI toggle." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset Key Binding" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Reset key binding to default key F8." },
                { m_Setting.GetBindingKeyLocaleID(Mod.kButtonActionName), "Toggle" },
                { m_Setting.GetBindingMapLocaleID(), "Toggle Markers" }
            };
        }
        public void Unload() { }
    }
}