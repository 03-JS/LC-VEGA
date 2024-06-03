using HarmonyLib;
using Malfunctions;
using Malfunctions.Patches;

namespace LC_VEGA.Patches
{
    internal class MalfunctionsPatches
    {
        [HarmonyPatch(typeof(StartOfRoundPatches), "HandleLevelStart")]
        [HarmonyPrefix]
        static void NotifyPowerMalfunction()
        {
            if (State.MalfunctionPower.Active && Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("PowerMalfunctionWarning");
            }
        }

        [HarmonyPatch(typeof(StartOfRoundPatches), "OverwriteMapScreenInfo")]
        [HarmonyPrefix]
        static void NotifyNavigationMalfunction()
        {
            if (State.MalfunctionNavigation.Active && Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("NavigationMalfunctionWarning");
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionTeleporterTrigger")]
        [HarmonyPostfix]
        static void NotifyAtomicMisalignment()
        {
            if (State.MalfunctionTeleporter.Active && Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("TeleporterMalfunctionWarning");
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionDistortionTrigger")]
        [HarmonyPostfix]
        static void NotifyCommsMalfunction()
        {
            if (State.MalfunctionDistortion.Active && Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("CommsMalfunctionWarning");
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionDoorTrigger")]
        [HarmonyPrefix]
        static void NotifyDoorMalfunction()
        {
            if (State.MalfunctionDoor.Active && Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("DoorMalfunctionWarning");
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionLeverTrigger")]
        [HarmonyPrefix]
        static void NotifyHidraulicsMalfunction()
        {
            if (State.MalfunctionLever.Active && Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("HydraulicMalfunctionWarning");
            }
        }
    }
}
