using HarmonyLib;
using ShipWindows.Networking;

namespace LC_VEGA.Patches
{
    internal class ShipWindowsPatches
    {
        public static bool opened = true;

        [HarmonyPatch(typeof(NetworkHandler), "WindowSwitchUsed")]
        [HarmonyPostfix]
        static void GetShutterState(bool currentState)
        {
            opened = currentState;
        }
    }
}
