using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_VEGA.Patches;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace LC_VEGA
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("me.loaforc.voicerecognitionapi", DependencyFlags.HardDependency)]
    [BepInDependency("BMX.LobbyCompatibility", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.LC-VEGA";
        private const string modName = "LC-VEGA";
        private const string modVersion = "1.1.0";

        internal static AssetBundle assetBundle;

        public static bool gameOpened;
        public static ConfigEntry<bool> enableAdvancedScannerAuto;
        public static ConfigEntry<bool> playIntro;
        public static ConfigEntry<VocalLevels> vocalLevel;

        private readonly Harmony harmony = new Harmony(modGUID);
        private static Plugin Instance;
        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("The installation of VEGA has begun");

            LoadAssets();
            GenerateConfigValues();
            VEGA.Initialize();
            gameOpened = true;

            PatchStuff();
            CheckForBMXLC();
        }

        internal void LoadAssets()
        {
            mls.LogInfo("Loading assets");
            VEGA.voiceLines = new List<AudioClip>();
            string folderLocation = Instance.Info.Location;
            folderLocation = folderLocation.TrimEnd("LC-VEGA.dll".ToCharArray());
            assetBundle = AssetBundle.LoadFromFile(folderLocation + "lcvegavoicelines");
            if (assetBundle != null)
            {
                VEGA.voiceLines = assetBundle.LoadAllAssets<AudioClip>().ToList();
                mls.LogInfo("Assets loaded successfully");
            }
            else
            {
                mls.LogWarning("Unable to load assets");
            }
        }

        internal void PatchStuff()
        {
            mls.LogInfo("Patching things");
            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(TerminalPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(GeneralPatches));
        }

        internal void CheckForBMXLC()
        {
            Dictionary<string, PluginInfo> Mods = Chainloader.PluginInfos;
            foreach (PluginInfo info in Mods.Values)
            {
                if (info.Metadata.GUID.Contains("BMX.LobbyCompatibility"))
                {
                    PluginHelper.RegisterPlugin(modGUID, Version.Parse(modVersion), CompatibilityLevel.ClientOnly, VersionStrictness.None);
                }
            }
        }

        internal void GenerateConfigValues()
        {
            vocalLevel = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Vocal Level", // Key of this config
                VocalLevels.High, // Default value
                "Changes how often VEGA speaks.\n" +
                "None: Will only speak when asked to.\n" +
                "Low: Gives you useful info, doesn't talk on most interactions and new enemy scans. Recommended for more experienced players.\n" +
                "Medium: Gives you useful info, doesn't talk on most interactions. Recommended for intermediate level players.\n" +
                "High: The default value. Will speak on every interaction. Recommended for inexperienced players." // Description
            );
            playIntro = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Play intro", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you its introduction speech every time you open the game." // Description
            );
            enableAdvancedScannerAuto = Config.Bind(
                "Advanced Scanner", // Config section
                "Enable the Advanced Scanner automatically", // Key of this config
                false, // Default value
                "Enables VEGA's Advanced Scanner automatically when joining a game. Useful if you always want to have it on and don't want to repeat the voice command often." // Description
            );
        }

        public static void LogToConsole(string message, string logType = "")
        {
            switch (logType.ToLower())
            {
                case "warn":
                    mls.LogWarning(message);
                    break;
                case "error":
                    mls.LogError(message);
                    break;
                default:
                    mls.LogInfo(message);
                    break;
            }
        }
    }
}
