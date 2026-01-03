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

namespace ToggleMarkers
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
            m_Harmony = new Harmony("com.tuusuario.togglemarkers");
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
            MarkersVisiblePatch.ForceEnabled = !MarkersVisiblePatch.ForceEnabled;

            // 2. Intentar sincronizar con EDT solo si está presente
            TrySyncWithExternalMods();

            log.Info($"[ToggleMarkers] F9 Pressed. State: {MarkersVisiblePatch.ForceEnabled}");
        }

        private void TrySyncWithExternalMods()
        {
            try
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null) return;

                // BUSCAR EL SISTEMA DE EDT DINÁMICAMENTE
                // Usamos el nombre completo para no requerir la DLL en referencias
                var edtType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == "ExtraDetailingTools.Systems.UI.UI");

                if (edtType != null)
                {
                    // Si llegamos aquí, EDT ESTÁ INSTALADO
                    // Intentamos forzar la actualización de su binding 'showMarker'
                    var bindingField = AccessTools.Field(edtType, "showMarker");
                    var bindingInstance = bindingField?.GetValue(null);

                    if (bindingInstance != null)
                    {
                        // Llamamos al método Update() del binding
                        AccessTools.Method(bindingInstance.GetType(), "Update").Invoke(bindingInstance, null);

                        // Intentamos también disparar el evento de UI si el sistema de UI existe
                        var uiSystem = world.Systems.FirstOrDefault(s => s.GetType().Name == "UISystem");
                        if (uiSystem != null)
                        {
                            var triggerMethod = AccessTools.Method(uiSystem.GetType(), "TriggerEvent", new Type[] { typeof(string), typeof(string) });
                            triggerMethod?.Invoke(uiSystem, new object[] { "edt", "updatemarkersvisible" });
                        }

                        log.Info("Sincronización con EDT completada.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Si algo falla, lo ignoramos para no romper el mod del usuario
                log.Debug("Sincronización externa omitida o fallida: " + ex.Message);
            }
        }

        public void OnDispose()
        {
            m_Harmony?.UnpatchAll("com.tuusuario.togglemarkers");
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }

    [HarmonyPatch(typeof(RenderingSystem), "markersVisible")]
    public static class MarkersVisiblePatch
    {
        public static bool ForceEnabled = false;

        [HarmonyPrefix]
        [HarmonyPatch(MethodType.Getter)]
        public static bool GetPrefix(ref bool __result)
        {
            __result = ForceEnabled;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(MethodType.Setter)]
        public static void SetPrefix(bool value)
        {
            // Sincronización inversa: si EDT (u otro mod) cambia el valor, nosotros lo adoptamos
            if (ForceEnabled != value)
            {
                ForceEnabled = value;
            }
        }
    }
}