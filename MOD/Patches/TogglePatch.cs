using Game.Rendering;
using HarmonyLib;

namespace ToggleMarkers.MOD.Patches
{
    internal class TogglePatch
    {
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
                // SYNC: if EDT (or any other mod) change the value, that value is set
                if (ForceEnabled != value)
                {
                    ForceEnabled = value;
                }
            }

        }
    }
}