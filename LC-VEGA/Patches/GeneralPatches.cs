using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

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

        [HarmonyPatch(typeof(LungProp), "DisconnectFromMachinery")]
        [HarmonyPostfix]
        static void RadiationWarning()
        {
            VEGA.facilityHasPower = false;
            VEGA.PlayAudio("RadiationSpike", 6.5f);
        }

        //[HarmonyPatch(typeof(RoundManager), "PowerSwitchOnClientRpc")]
        //[HarmonyPostfix]
        //static void PowerOnClient()
        //{
        //    VEGA.facilityHasPower = true;
        //}

        //[HarmonyPatch(typeof(RoundManager), "PowerSwitchOffClientRpc")]
        //[HarmonyPostfix]
        //static void PowerOffClient()
        //{
        //    VEGA.facilityHasPower = false;
        //}

        [HarmonyPatch(typeof(RoundManager), "TurnOnAllLights")]
        [HarmonyPostfix]
        static void AllLights(bool on)
        {
            Plugin.LogToConsole("POWER -> " + on);
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
        static void DisableVega()
        {
            VEGA.audioSource.Stop();
        }

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingLevel")]
        [HarmonyPostfix]
        static void GiveWeatherInfo()
        {
            if (HUDManager.Instance.localPlayerXP <= 150)
            {
                float delay = 8f;
                switch (TimeOfDay.Instance.currentLevelWeather)
                {
                    case LevelWeatherType.Rainy:
                        if (!rainyWeatherWarned)
                        {
                            VEGA.PlayAudioWithVariant("Rainy", 1, delay);
                            rainyWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Stormy:
                        if (!stormyWeatherWarned)
                        {
                            VEGA.PlayAudioWithVariant("Stormy", 1, delay);
                            stormyWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Foggy:
                        if (!foggyWeatherWarned)
                        {
                            VEGA.PlayAudioWithVariant("Foggy", 1, delay);
                            foggyWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Flooded:
                        if (!floodedWeatherWarned)
                        {
                            VEGA.PlayAudioWithVariant("Flooded", 1, delay);
                            floodedWeatherWarned = true;
                        }
                        break;
                    case LevelWeatherType.Eclipsed:
                        if (!eclipsedWeatherWarned)
                        {
                            VEGA.PlayAudioWithVariant("Eclipsed", 1, delay);
                            eclipsedWeatherWarned = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
