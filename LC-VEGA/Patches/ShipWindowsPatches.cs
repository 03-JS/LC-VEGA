using HarmonyLib;
using ShipWindows;
using ShipWindows.Networking;

namespace LC_VEGA.Patches
{
    internal class ShipWindowsPatches
    {
        public static bool opened = true;

        [HarmonyPatch(typeof(NetworkHandler), "WindowSwitchUsed")]
        [HarmonyPostfix]
        static void GetShutterStateNetHandler(bool currentState)
        {
            if (WindowConfig.enableShutter.Value)
            {
                opened = currentState; 
            }
        }

        [HarmonyPatch(typeof(WindowState), "SetWindowState")]
        [HarmonyPostfix]
        static void GetShutterStateFromWindowState(bool closed)
        {
            if (WindowConfig.enableShutter.Value)
            {
                opened = !closed;
            }
        }
    }
}
