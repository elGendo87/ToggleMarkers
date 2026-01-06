using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Input;
using Unity.Entities;
using Game.Modding;
using Game.SceneFlow;
using Game.Rendering;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using System;
using ToggleMarkers.MOD.Patches;

namespace ToggleMarkers.MOD
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(ToggleMarkers)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;
        public static ProxyAction m_ToggleAction;
        public const string kButtonActionName = "ToggleMarkersAction";
        private Harmony m_Harmony;

        public void OnLoad(UpdateSystem updateSystem)
        {
            m_Harmony = new Harmony("com.g87.togglemarkers");
            m_Harmony.PatchAll();

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();

            var lm = GameManager.instance.localizationManager;

            foreach (var locale in lm.GetSupportedLocales())
            {
                lm.AddSource(locale, new LocaleEN(m_Setting));
            }

            AssetDatabase.global.LoadSettings(nameof(ToggleMarkers), m_Setting, new Setting(this));

            m_Setting.RegisterKeyBindings();
            m_ToggleAction = m_Setting.GetAction(kButtonActionName);
            m_ToggleAction.shouldBeEnabled = true;

            m_ToggleAction.onInteraction += (_, phase) =>
            {
                if (phase == UnityEngine.InputSystem.InputActionPhase.Performed)
                {
                    ToggleMarkersLogic();
                }
            };
        }

        private void ToggleMarkersLogic()
        {
            // 1. Cambiar estado interno
            TogglePatch.MarkersVisiblePatch.ForceEnabled = !TogglePatch.MarkersVisiblePatch.ForceEnabled;

            // 2. Intentar sincronizar con EDT solo si está presente
            TrySyncWithExternalMods();

            log.Info($"[ToggleMarkers] F9 Pressed. State: {TogglePatch.MarkersVisiblePatch.ForceEnabled}");
        }

        private void TrySyncWithExternalMods()
        {
            try
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null) return;

                // Dynamic search of EDT UI type
                // Using full name to not require reference DLL
                var edtType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == "ExtraDetailingTools.Systems.UI.UI");

                if (edtType != null)
                {
                    // EDT IS INSTALLED
                    // Trying to force EDT 'showMarker' binding to our value
                    var bindingField = AccessTools.Field(edtType, "showMarker");
                    var bindingInstance = bindingField?.GetValue(null);

                    if (bindingInstance != null)
                    {
                        // Call binding Update()
                        AccessTools.Method(bindingInstance.GetType(), "Update").Invoke(bindingInstance, null);

                        // Try to trigger EDT UI event if exist
                        var edtUISystem = world.Systems.FirstOrDefault(s => s.GetType().FullName == "ExtraDetailingTools.Systems.UI.UI");

                        if (edtUISystem != null)
                        {
                            var triggerMethod = AccessTools.Method(edtUISystem.GetType(), "TriggerEvent", new Type[] { typeof(string), typeof(string) });
                            triggerMethod?.Invoke(edtUISystem, new object[] { "edt", "updatemarkersvisible" });
                        }

                        log.Info("EDT synched finished.");
                    }
                }
            }
            catch (Exception ex)
            {
                // If someting fails, is ignored to no break EDT
                log.Debug("External synch not needed or failed: " + ex.Message);
            }
        }

        public void OnDispose()
        {
            m_Harmony?.UnpatchAll("com.g87.togglemarkers");
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }

   
}