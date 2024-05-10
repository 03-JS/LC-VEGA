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
        private const string modVersion = "1.1.1";

        internal static AssetBundle assetBundle;

        public static bool gameOpened;
        
        // Config values
        public static ConfigEntry<bool> enableAdvancedScannerAuto;
        public static ConfigEntry<bool> playIntro;
        public static ConfigEntry<bool> readBestiaryEntries;
        public static ConfigEntry<VocalLevels> vocalLevel;

        // Voice commands config values
        public static ConfigEntry<bool> registerMoonsInfo;
        public static ConfigEntry<bool> registerBestiaryEntries;
        public static ConfigEntry<bool> registerCreatureInfo;
        public static ConfigEntry<bool> registerAdvancedScanner;
        public static ConfigEntry<bool> registerInteractSecureDoor;
        public static ConfigEntry<bool> registerInteractAllSecureDoors;
        public static ConfigEntry<bool> registerDisableTurret;
        public static ConfigEntry<bool> registerDisableAllTurrets;
        public static ConfigEntry<bool> registerDisableMine;
        public static ConfigEntry<bool> registerDisableAllMines;
        public static ConfigEntry<bool> registerDisableSpikeTrap;
        public static ConfigEntry<bool> registerDisableAllSpikeTraps;
        public static ConfigEntry<bool> registerTeleporter;
        public static ConfigEntry<bool> registerRadarSwitch;
        public static ConfigEntry<bool> registerCrewStatus;
        public static ConfigEntry<bool> registerRadarBoosters;
        public static ConfigEntry<bool> registerSignalTranslator;
        public static ConfigEntry<bool> registerTime;
        public static ConfigEntry<bool> registerInteractShipDoors;
        public static ConfigEntry<bool> registerInteractShipLights;
        public static ConfigEntry<bool> registerWeatherInfo;
        public static ConfigEntry<bool> registerStop;
        public static ConfigEntry<bool> registerThanks;

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
            readBestiaryEntries = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Read Bestiary entries", // Key of this config
                true, // Default value
                "If set to true, VEGA will read every bestiary entry you open in the terminal." // Description
            );
            enableAdvancedScannerAuto = Config.Bind(
                "Advanced Scanner", // Config section
                "Enable the Advanced Scanner automatically", // Key of this config
                false, // Default value
                "Enables VEGA's Advanced Scanner automatically when joining a game. Useful if you always want to have it on and don't want to repeat the voice command often." // Description
            );
            
            // Voice commands
            registerMoonsInfo = Config.Bind(
                "Voice Recognition", // Config section
                "Register Moon info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerBestiaryEntries = Config.Bind(
                "Voice Recognition", // Config section
                "Register Bestiary entries commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerCreatureInfo = Config.Bind(
                "Voice Recognition", // Config section
                "Register Creature info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerAdvancedScanner = Config.Bind(
                "Voice Recognition", // Config section
                "Register Advanced Scanner commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractSecureDoor = Config.Bind(
                "Voice Recognition", // Config section
                "Register Open / Close Secure Door commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractAllSecureDoors = Config.Bind(
                "Voice Recognition", // Config section
                "Register Open / Close All Secure Doors command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableTurret = Config.Bind(
                "Voice Recognition", // Config section
                "Register Disable Turret command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableAllTurrets = Config.Bind(
                "Voice Recognition", // Config section
                "Register Disable All Turrets command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableMine = Config.Bind(
                "Voice Recognition", // Config section
                "Register Disable Mine command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableAllMines = Config.Bind(
                "Voice Recognition", // Config section
                "Register Disable All Mines command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableSpikeTrap = Config.Bind(
                "Voice Recognition", // Config section
                "Register Disable Spike Trap command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableAllSpikeTraps = Config.Bind(
                "Voice Recognition", // Config section
                "Register Disable All Spike Traps command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerTeleporter = Config.Bind(
                "Voice Recognition", // Config section
                "Register Teleporter commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerRadarSwitch = Config.Bind(
                "Voice Recognition", // Config section
                "Register Radar Switch commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerCrewStatus = Config.Bind(
                "Voice Recognition", // Config section
                "Register Crew Status commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerRadarBoosters = Config.Bind(
                "Voice Recognition", // Config section
                "Register Radar Booster commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerSignalTranslator = Config.Bind(
                "Voice Recognition", // Config section
                "Register Signal Translator commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerTime = Config.Bind(
                "Voice Recognition", // Config section
                "Register Current time of day commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractShipDoors = Config.Bind(
                "Voice Recognition", // Config section
                "Register Ship Door commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractShipLights = Config.Bind(
                "Voice Recognition", // Config section
                "Register Ship Lights commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerWeatherInfo = Config.Bind(
                "Voice Recognition", // Config section
                "Register Weather info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerStop = Config.Bind(
                "Voice Recognition", // Config section
                "Register Stop Talking commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerThanks = Config.Bind(
                "Voice Recognition", // Config section
                "Register Thank you commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
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
