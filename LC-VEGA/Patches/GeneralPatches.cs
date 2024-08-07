﻿using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LC_VEGA.Patches
{
    internal class GeneralPatches
    {
        internal static bool rainyWeatherWarned = false;
        internal static bool stormyWeatherWarned = false;
        internal static bool foggyWeatherWarned = false;
        internal static bool floodedWeatherWarned = false;
        internal static bool eclipsedWeatherWarned = false;
        internal static bool roundStart;

        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        [HarmonyPostfix]
        static void SaveValues()
        {
            Plugin.LogToConsole("Saving...", "debug");
            if (ModChecker.hasDiversity)
            {
                if (!DiversityPatches.firstTimeWelcome || !DiversityPatches.firstTimeReply)
                {
                    SaveManager.firstTimeDiversity = false;
                }
            }
            SaveManager.hadDiversity = ModChecker.hasDiversity;
            SaveManager.SaveToFile();
        }

        [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
        [HarmonyPostfix]
        static void ResetModValuesOnDisconnect()
        {
            if (ModChecker.hasMalfunctions)
            {
                VEGA.ResetMalfunctionValues();
            }
            if (ModChecker.hasFacilityMeltdown)
            {
                FacilityMeltdownPatches.index = 0;
                FacilityMeltdownPatches.playedCountdown = false;
                FacilityMeltdownPatches.condition = 60;
            }
        }

        [HarmonyPatch(typeof(RoundManager), "Update")]
        [HarmonyPostfix]
        static void AdvancedScanner()
        {
            if (VEGA.advancedScannerActive)
            {
                VEGA.PerformAdvancedScan();
            }
        }

        [HarmonyPatch(typeof(RoundManager), "TurnOnAllLights")]
        [HarmonyPostfix]
        static void AllLights(bool on)
        {
            Plugin.LogToConsole("POWER -> " + on, "debug");
            VEGA.facilityHasPower = on;
        }

        [HarmonyPatch(typeof(ShipTeleporter), "Update")]
        [HarmonyPostfix]
        static void GetTeleporterCooldownTime(ShipTeleporter __instance, ref float ___cooldownTime)
        {
            if (!__instance.isInverseTeleporter)
            {
                VEGA.teleporterCooldownTime = ___cooldownTime;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
        [HarmonyPostfix]
        static void DisableVEGAOnDeath()
        {
            VEGA.audioSource.Stop();
            VEGA.sfxAudioSource.Stop();
        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
        [HarmonyPostfix]
        static void GiveWeatherInfo()
        {
            if (HUDManager.Instance.localPlayerXP <= 150 && Plugin.giveWeatherInfo.Value)
            {
                float delay = 8f;
                switch (TimeOfDay.Instance.currentLevelWeather)
                {
                    case LevelWeatherType.Rainy:
                        if (!rainyWeatherWarned)
                        {
                            VEGA.PlayRandomLine("Rainy", 1, delay);
                            rainyWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Stormy:
                        if (!stormyWeatherWarned)
                        {
                            VEGA.PlayRandomLine("Stormy", 1, delay);
                            stormyWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Foggy:
                        if (!foggyWeatherWarned)
                        {
                            VEGA.PlayRandomLine("Foggy", 1, delay);
                            foggyWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Flooded:
                        if (!floodedWeatherWarned)
                        {
                            VEGA.PlayRandomLine("Flooded", 1, delay);
                            floodedWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Eclipsed:
                        if (!eclipsedWeatherWarned)
                        {
                            VEGA.PlayRandomLine("Eclipsed", 1, delay);
                            eclipsedWeatherWarned = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
        [HarmonyPostfix]
        static void ResetCountdown()
        {
            if (ModChecker.hasFacilityMeltdown)
            {
                FacilityMeltdownPatches.playedCountdown = false;
            }
        }
    }

    internal class ReadInputPatch
    {
        [HarmonyPatch(typeof(RoundManager), "Update")]
        [HarmonyPostfix]
        public static void ReadInput()
        {
            if (StartOfRound.Instance.localPlayerController.inTerminalMenu || StartOfRound.Instance.localPlayerController.isTypingChat) return;
            if ((VEGA.sfxAudioSource.clip.name.Equals("Activate") || VEGA.sfxAudioSource.clip.name.Equals("Deactivate")) && VEGA.sfxAudioSource.isPlaying) return;
            if (Plugin.registerActivation.Value && Plugin.useManualListening.Value && Plugin.PlayerInputInstance.Activation.triggered)
            {
                if (!VEGA.listening)
                {
                    VEGA.PlaySFX("Activate");
                }
                else
                {
                    VEGA.PlaySFX("Deactivate");
                }
                VEGA.listening = !VEGA.listening;
                Plugin.LogToConsole("Is VEGA listening -> " + VEGA.listening, "debug");
            }
        }
    }
}
