using BepInEx;
using BepInEx.Bootstrap;
using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace LC_VEGA
{
    internal class ModChecker
    {   
        public static bool hasToilHead;
        public static bool hasMalfunctions;
        public static bool hasDiveristy;
        public static bool hasCodeRebirth;

        public static bool CheckForMod(string modGUID)
        {
            Dictionary<string, PluginInfo> Mods = Chainloader.PluginInfos;
            foreach (PluginInfo info in Mods.Values)
            {
                if (info.Metadata.GUID.Equals(modGUID))
                {
                    Plugin.LogToConsole("Found mod: " + info.Metadata.Name);
                    return true;
                }
            }
            return false;
        }
    }
}
