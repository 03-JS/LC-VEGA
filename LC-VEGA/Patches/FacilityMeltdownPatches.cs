using FacilityMeltdown.MeltdownSequence.Behaviours;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LC_VEGA.Patches
{
    internal class FacilityMeltdownPatches
    {
        // Time requirements in seconds
        private static Dictionary<int, string> timeClipPairs = new Dictionary<int, string>()
        {
            { 60, "1MinLeft" },
            { 30, "30SecLeft" },
            { 10, "Countdown" }
        };
        private static int index;
        private static int condition;

        [HarmonyPatch(typeof(MeltdownHandler), "Update")]
        [HarmonyPostfix]
        static void PlayTimerAudios(ref float ___meltdownTimer, ref bool ___meltdownStarted, ref GameObject ___explosion)
        {
            if (!___meltdownStarted || ___explosion != null) return;
            if (___meltdownTimer <= condition)
            {
                condition = timeClipPairs.Keys.ToArray()[index];
                VEGA.PlayLine(timeClipPairs[index]);
                index++;
            }
        }
    }
}
