using HarmonyLib;
using UnityEngine;

namespace LC_VEGA.Patches
{
    internal class ApparatusPatch
    {
        [HarmonyPatch(typeof(LungProp), "EquipItem")]
        [HarmonyPrefix]
        static void PlayApparatusPulledLine(LungProp __instance)
        {
            // VEGA.facilityHasPower = false;
            if (!Plugin.giveApparatusWarnings.Value || !__instance.isLungDocked) return;
            VEGA.PlayRandomLine("ApparatusPulled", Random.Range(1, 4), 3.65f);
        }
    }
}