// using HarmonyLib;
// using ShipWindows.Config;
// using ShipWindows.Networking;
//
// namespace LC_VEGA.Patches
// {
//     internal class ShipWindowsPatches
//     {
//         public static bool opened = true;
//
//         [HarmonyPatch(typeof(NetworkManager), "ToggleShutters")]
//         [HarmonyPostfix]
//         static void GetShutterStateNetHandler(bool closeShutters)
//         {
//             opened = closeShutters;
//         }
//     }
// }