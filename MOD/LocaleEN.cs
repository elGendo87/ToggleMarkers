using Colossal;
using System.Collections.Generic;

namespace ToggleMarkers.MOD
{
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
                    { m_Setting.GetOptionDescLocaleID(nameof(Setting.KeyboardBinding)), "Shows/hides markers and invisible roads." },
                    { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset Key Binding" },
                    { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Reset key binding to default key F8." },
                    { m_Setting.GetBindingKeyLocaleID(Mod.kButtonActionName), "Toggle" },
                    { m_Setting.GetBindingMapLocaleID(), "Toggle Markers" }
                };
            }
                public void Unload() { }
        }
    }

