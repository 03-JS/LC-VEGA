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
            if (!Plugin.giveApparatusWarnings.Value || !__instance.isLungDocked) return;
            VEGA.facilityHasPower = false;
            try
            {
                if (StartOfRound.Instance.localPlayerController.ItemSlots.All(item =>
                        item.GetType() != typeof(LungProp)))
                    VEGA.PlayRandomLine("RadiationSpike", Random.Range(1, 3), 7.5f);
                else VEGA.PlayRandomLine("ApparatusPulled", Random.Range(1, 4), 7.5f);
            }
            catch (System.Exception)
            {
                VEGA.PlayRandomLine("RadiationSpike", Random.Range(1, 3), 7.5f);
            }
        }
    }
}