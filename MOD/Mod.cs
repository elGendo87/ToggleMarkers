using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Input;
using Unity.Entities;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using System.Linq;
using System;
using ToggleMarkers.MOD.Patches;
using UnityEngine.InputSystem;

namespace ToggleMarkers.MOD
{
    /// <summary>
    /// Main mod entry point.
    /// Handles lifecycle, input binding, marker toggling logic
    /// and optional synchronization with external mods.
    /// </summary>
    public class Mod : IMod
    {
        // Logger instance for this mod
        public static ILog log = LogManager
            .GetLogger($"{nameof(ToggleMarkers)}")
            .SetShowsErrorsInUI(false);

        // Mod settings instance
        private Setting m_Setting;

        // Input action used to toggle markers visibility
        public static ProxyAction m_ToggleAction;

        // Action name as defined in the settings
        public const string kButtonActionName = "ToggleMarkersAction";

        // Harmony instance used for patching
        private Harmony m_Harmony;

        // Cached input handler delegate to allow proper unregistration on dispose
        private Action<ProxyAction, InputActionPhase> _toggleHandler;

        // Cached EDT UI type (reflection)
        // Used to avoid scanning assemblies on every key press
        private static Type _edtUIType;
        private static bool _edtScanned;

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Initialize and apply Harmony patches
            m_Harmony = new Harmony("com.g87.togglemarkers");
            m_Harmony.PatchAll();

            // Create and register mod settings
            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();

            // Register localization sources
            var lm = GameManager.instance.localizationManager;
            foreach (var locale in lm.GetSupportedLocales())
            {
                lm.AddSource(locale, new LocaleEN(m_Setting));
            }

            // Load persisted settings or apply defaults
            AssetDatabase.global.LoadSettings(
                nameof(ToggleMarkers),
                m_Setting,
                new Setting(this)
            );

            // Register key bindings defined in settings
            m_Setting.RegisterKeyBindings();

            // Retrieve the toggle action
            m_ToggleAction = m_Setting.GetAction(kButtonActionName);
            m_ToggleAction.shouldBeEnabled = true;

            // Store delegate reference to properly unregister later
            _toggleHandler = (_, phase) =>
            {
                if (phase == InputActionPhase.Performed)
                {
                    ToggleMarkersLogic();
                }
            };

            // Subscribe to input interaction
            m_ToggleAction.onInteraction += _toggleHandler;
        }

        /// <summary>
        /// Toggles marker visibility state and attempts to synchronize
        /// with external mods if present.
        /// </summary>
        private void ToggleMarkersLogic()
        {
            // Toggle internal marker visibility state
            TogglePatch.MarkersVisiblePatch.ForceEnabled =
                !TogglePatch.MarkersVisiblePatch.ForceEnabled;

            // Attempt best-effort synchronization with external mods
            TrySyncWithExternalMods();

            // Log input event at debug level to avoid log spam
            log.Debug(
                $"F8 pressed → MarkersVisible={TogglePatch.MarkersVisiblePatch.ForceEnabled}"
            );
        }

        /// <summary>
        /// Attempts to synchronize marker visibility state with
        /// Extra Detailing Tools (EDT), if installed.
        /// This method must never break gameplay if EDT is absent or changes.
        /// </summary>
        private void TrySyncWithExternalMods()
        {
            try
            {
                // Access the default ECS world
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                    return;

                // Scan assemblies only once to locate EDT UI type
                if (!_edtScanned)
                {
                    _edtUIType = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t =>
                            t.FullName == "ExtraDetailingTools.Systems.UI.UI");

                    _edtScanned = true;
                }

                // EDT is not installed or not detected
                if (_edtUIType == null)
                    return;

                // Log EDT detection (debug only)
                var bindingField = AccessTools.Field(_edtUIType, "showMarker");
                var bindingInstance = bindingField?.GetValue(null);

                if (bindingInstance != null)
                {
                    // Update EDT binding to reflect current state: new method
                    var updateMethod = AccessTools.Method(bindingInstance.GetType(), "Update");
                    updateMethod?.Invoke(bindingInstance, null);

                    log.Debug("EDT Binding updated silently.");
                }


                // Successful synchronization (debug only)
                log.Debug("EDT sync completed.");
            }
            catch (Exception ex)
            {
                // External mod synchronization is best-effort and must never break gameplay
                log.Debug("External sync skipped or failed: " + ex.Message);
            }
        }

        public void OnDispose()
        {
            // Remove all Harmony patches applied by this mod
            m_Harmony?.UnpatchAll("com.g87.togglemarkers");

            // Unregister input handler to avoid duplicated callbacks
            if (m_ToggleAction != null && _toggleHandler != null)
            {
                m_ToggleAction.onInteraction -= _toggleHandler;
                _toggleHandler = null;
            }

            // Unregister settings UI
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
