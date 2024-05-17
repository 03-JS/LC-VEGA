using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
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
                VEGA.PlayAudio("RadiationSpike");
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
                        VEGA.PlayAudio(VEGA.enemies[enemyID] + "Scan", 0.7f);
                    }
                }
            }
        }
    }
}
