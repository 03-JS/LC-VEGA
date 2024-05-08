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

        [HarmonyPatch("ScanNewCreatureClientRpc")]
        [HarmonyPostfix]
        static void PlayNewEnemyLine(int enemyID, ref float ___playerPingingScan)
        {
            if (___playerPingingScan >= 0f)
            {
                if (enemyID < VEGA.enemyList.Length)
                {
                    VEGA.PlayAudio(VEGA.enemyList[enemyID] + "Scan", 0.7f);
                }
            }
        }
    }
}
