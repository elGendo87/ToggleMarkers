using Game.Rendering;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // Sincronización inversa: si EDT (u otro mod) cambia el valor, nosotros lo adoptamos
                if (ForceEnabled != value)
                {
                    ForceEnabled = value;
                }
            }
        }
    }
}
