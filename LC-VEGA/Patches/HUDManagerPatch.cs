﻿using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace LC_VEGA.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        public static GameObject? enemies;
        public static GameObject? items;

        [HarmonyPatch("RadiationWarningHUD")]
        [HarmonyPostfix]
        static void PlayRadiationWarning()
        {
            VEGA.facilityHasPower = false;
            if (Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                if (ModChecker.hasFacilityMeltdown)
                {
                    if (StartOfRound.Instance.localPlayerController.isInsideFactory)
                    {
                        VEGA.PlayLine("FacilityMeltdownInside", 3.65f);
                    }
                    else
                    {
                        VEGA.PlayLine("FacilityMeltdownOutside", 3.65f);
                    }
                }
                else
                {
                    try
                    {
                        if (StartOfRound.Instance.localPlayerController.ItemSlots.Any(item => item.GetType() == typeof(LungProp)))
                        {
                            if (Random.Range(0, 10) == 0) VEGA.PlayRandomLine("ApparatusPulledEasterEgg", Random.Range(1, 3), 3.65f);
                            VEGA.PlayLine("ApparatusPulled", 3.65f);
                        }
                        else
                        {
                            VEGA.PlayRandomLine("RadiationSpike", Random.Range(1, 3), 3.65f);
                        }
                    }
                    catch (System.Exception)
                    {
                        VEGA.PlayRandomLine("RadiationSpike", Random.Range(1, 3), 3.65f);
                    }
                }
            }
        }

        [HarmonyPatch("AttemptScanNewCreature")]
        [HarmonyPrefix]
        static void PlayNewEnemyLine(int enemyID, ref Terminal ___terminalScript)
        {
            if (!___terminalScript.scannedEnemyIDs.Contains(enemyID))
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.Medium)
                {
                    if (VEGA.enemies.ContainsKey(enemyID))
                    {
                        VEGA.PlayLine(VEGA.enemies[enemyID] + "Scan", 0.7f);
                    }
                    else
                    {
                        foreach (var file in ___terminalScript.enemyFiles)
                        {
                            if (file.creatureFileID == enemyID && VEGA.moddedEnemies.Contains(file.creatureName) && !file.creatureName.Equals("Football") && !file.creatureName.Equals("Locker") && !file.creatureName.Equals("Boombas") && !file.creatureName.Contains("Drone") && !file.creatureName.Contains("Turret"))
                            {
                                VEGA.PlayLine(file.creatureName + "Scan", 0.7f);
                            }
                        }
                    }
                }
            }
        }
    }
}
