using HarmonyLib;
using Malfunctions;
using Malfunctions.Patches;

namespace LC_VEGA.Patches
{
    internal class MalfunctionsPatches
    {
        public static bool playPowerWarning;
        public static bool playTpWarning;
        public static bool playCommsWarning;
        public static bool playDoorWarning;
        public static bool playLeverWarning;

        [HarmonyPatch(typeof(StartOfRoundPatches), "HandleLevelStart")]
        [HarmonyPostfix]
        static void NotifyPowerMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                VEGA.malfunctionPowerTriggered = State.MalfunctionPower.Triggered;
                if (State.MalfunctionPower.Triggered)
                {
                    if (!playPowerWarning) return;
                    VEGA.PlayLine("PowerMalfunctionWarning", 3.65f);
                    playPowerWarning = false;
                }
            }
        }

        [HarmonyPatch(typeof(StartOfRoundPatches), "OverwriteMapScreenInfo")]
        [HarmonyPostfix]
        static void NotifyNavigationMalfunction()
        {
            if (State.MalfunctionNavigation.Notified && Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                VEGA.PlayLine("NavigationMalfunctionWarning", 3.65f);
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionTeleporterTrigger")]
        [HarmonyPostfix]
        static void NotifyAtomicMisalignment()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                VEGA.malfunctionTeleporterTriggered = State.MalfunctionTeleporter.Triggered;
                if (State.MalfunctionTeleporter.Triggered)
                {
                    if (!playTpWarning) return;
                    VEGA.PlayLine("TeleporterMalfunctionWarning", 3.65f);
                    playTpWarning = false;
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionDistortionTrigger")]
        [HarmonyPostfix]
        static void NotifyCommsMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                VEGA.malfunctionDistortionTriggered = State.MalfunctionDistortion.Triggered;
                if (State.MalfunctionDistortion.Triggered)
                {
                    if (!playCommsWarning) return;
                    VEGA.PlayLine("CommsMalfunctionWarning", 3.65f);
                    playCommsWarning = false;
                } 
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionDoorTrigger")]
        [HarmonyPostfix]
        static void NotifyDoorMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                VEGA.malfunctionDoorTriggered = State.MalfunctionDoor.Triggered;
                if (State.MalfunctionDoor.Triggered)
                {
                    if (!playDoorWarning) return;
                    VEGA.PlayLine("DoorMalfunctionWarning", 3.65f);
                    playDoorWarning = false;
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDayPatches), "CheckMalfunctionLeverTrigger")]
        [HarmonyPostfix]
        static void NotifyHidraulicsMalfunction()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.malfunctionWarnings.Value)
            {
                if (State.MalfunctionLever.Notified)
                {
                    if (!playLeverWarning) return;
                    VEGA.PlayLine("HydraulicMalfunctionWarning", 3.65f);
                    playLeverWarning = false;
                }
            }
        }
    }
}
