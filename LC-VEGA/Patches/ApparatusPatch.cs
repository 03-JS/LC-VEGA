using System.Linq;
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
            if (Random.Range(0, 10) == 0) VEGA.PlayRandomLine("ApparatusPulledEasterEgg", Random.Range(1, 3), 3.65f);
            VEGA.PlayLine("ApparatusPulled", 3.65f);
        }
    }
}