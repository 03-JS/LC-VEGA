using FacilityMeltdown.MeltdownSequence.Behaviours;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
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
        public static int index;
        public static int condition = 60;
        public static bool playedCountdown;

        [HarmonyPatch(typeof(MeltdownHandler), "Update")]
        [HarmonyPostfix]
        static void PlayTimerAudios(ref float ___meltdownTimer, ref bool ___meltdownStarted, ref GameObject ___explosion)
        {
            if (!___meltdownStarted || ___explosion != null || !Plugin.meltdownTimer.Value) return;
            if (___meltdownTimer <= condition && !playedCountdown)
            {
                VEGA.PlayLine(timeClipPairs[condition], 0f);
                if (condition == 10)
                {
                    playedCountdown = true;
                    return;
                }
                index++;
                condition = timeClipPairs.Keys.ToList()[index];
            }
        }
    }
}
