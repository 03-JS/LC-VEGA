using HarmonyLib;
using Malfunctions;
using Malfunctions.Patches;
using UnityEngine.Rendering;

namespace LC_VEGA.Patches
{
    internal class MalfunctionsPatches
    {
        public static bool playPowerWarning;
        public static bool playTpWarning;
        public static bool playCommsWarning;
        public static bool playDoorWarning;
        public static bool playLeverWarning;

        [HarmonyPatch(typeof(StartOfRoundPatches), "HandleRollPower")]
        [HarmonyPrefix]
        static void NotifyPowerMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                if (State.MalfunctionPower.Triggered)
                {
                    if (!playPowerWarning) return;
                    VEGA.PlayLine("PowerMalfunctionWarning");
                    playPowerWarning = false;
                }
            }
        }

        [HarmonyPatch(typeof(StartOfRoundPatches), "OverwriteMapScreenInfo")]
        [HarmonyPrefix]
        static void NotifyNavigationMalfunction()
        {
            if (State.MalfunctionNavigation.Notified && Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                VEGA.PlayLine("NavigationMalfunctionWarning");
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionTeleporterTrigger")]
        [HarmonyPrefix]
        static void NotifyAtomicMisalignment()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                if (State.MalfunctionTeleporter.Triggered)
                {
                    if (!playTpWarning) return;
                    VEGA.PlayLine("TeleporterMalfunctionWarning");
                    playTpWarning = false;
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionDistortionTrigger")]
        [HarmonyPrefix]
        static void NotifyCommsMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                if (State.MalfunctionDistortion.Triggered)
                {
                    if (!playCommsWarning) return;
                    VEGA.PlayLine("CommsMalfunctionWarning");
                    playCommsWarning = false;
                } 
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionDoorTrigger")]
        [HarmonyPrefix]
        static void NotifyDoorMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                if (State.MalfunctionDoor.Triggered)
                {
                    if (!playDoorWarning) return;
                    VEGA.PlayLine("DoorMalfunctionWarning");
                    playDoorWarning = false;
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionLeverTrigger")]
        [HarmonyPrefix]
        static void NotifyHidraulicsMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                if (State.MalfunctionLever.Notified)
                {
                    if (!playLeverWarning) return;
                    VEGA.PlayLine("HydraulicMalfunctionWarning");
                    playLeverWarning = false;
                }
            }
        }
    }
}
