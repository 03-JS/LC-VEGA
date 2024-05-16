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
        // Other
        public static bool hasMalfunctions;
        
        // Moons
        public static bool hasLLL;

        public static bool CheckForMod(string modGUID)
        {
            Dictionary<string, PluginInfo> Mods = Chainloader.PluginInfos;
            foreach (PluginInfo info in Mods.Values)
            {
                if (info.Metadata.GUID.Equals(modGUID))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
