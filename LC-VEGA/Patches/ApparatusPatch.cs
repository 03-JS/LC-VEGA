using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LC_VEGA.Patches
{
    internal class ApparatusPatch
    {
        [HarmonyPatch(typeof(LungProp), "EquipItem")]
        [HarmonyPostfix]
        static void PlayApparatusPulledLine()
        {
            // VEGA.facilityHasPower = false;
            if (!Plugin.giveApparatusWarnings.Value) return;
            if (Random.Range(0, 10) == 0) VEGA.PlayRandomLine("ApparatusPulledEasterEgg", Random.Range(1, 3), 3.65f);
            VEGA.PlayLine("ApparatusPulled", 3.65f);
        }
    }
}