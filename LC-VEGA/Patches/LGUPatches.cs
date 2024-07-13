using HarmonyLib;
using MoreShipUpgrades.UpgradeComponents.TierUpgrades.Ship;
using System;
using System.Collections.Generic;
using System.Text;

namespace LC_VEGA.Patches
{
    internal class LGUPatches
    {
        public static float flashCooldown;

        [HarmonyPatch(typeof(Discombobulator), "Update")]
        [HarmonyPostfix]
        static void GetDiscombobulatorCooldown(ref float ___flashCooldown)
        {
            flashCooldown = ___flashCooldown;
        }
    }
}
