using BepInEx;
using BepInEx.AssemblyPublicizer;
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
    [BepInDependency("com.rune580.LethalCompanyInputUtils", DependencyFlags.HardDependency)]
    [BepInDependency("BMX.LobbyCompatibility", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.LC-VEGA";
        private const string modName = "LC-VEGA";
        private const string modVersion = "2.0.0";

        internal static AssetBundle assetBundle;

        internal static PlayerInput PlayerInputInstance;

        // Dialogue & Interactions config values
        public static ConfigEntry<VocalLevels> vocalLevel;
        public static ConfigEntry<bool> playIntro;
        public static ConfigEntry<bool> readBestiaryEntries;
        public static ConfigEntry<bool> giveWeatherInfo;
        public static ConfigEntry<bool> ignoreMasterVolume;
        public static ConfigEntry<string> messages;

        // Advanced Scanner config values
        public static ConfigEntry<bool> enableAdvancedScannerAuto;
        public static ConfigEntry<bool> detectMasked;

        // Manual activation config values
        public static ConfigEntry<bool> useManualListening;
        public static ConfigEntry<bool> enableManualListeningAuto;

        // Voice commands config values
        public static ConfigEntry<float> confidence;
        public static ConfigEntry<bool> registerActivation;
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
        public static ConfigEntry<bool> registerCrewInShip;
        public static ConfigEntry<bool> registerScrapLeft;
        public static ConfigEntry<bool> registerRadarBoosters;
        public static ConfigEntry<bool> registerSignalTranslator;
        public static ConfigEntry<bool> registerTime;
        public static ConfigEntry<bool> registerLeverPull;
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

            PlayerInputInstance = new PlayerInput();

            LoadAssets();
            GenerateConfigValues();
            CheckInstalledMods();
            ManageSaveValues();
            
            VEGA.Initialize();
            PatchStuff();
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
            if (ModChecker.hasMalfunctions)
            {
                harmony.PatchAll(typeof(MalfunctionsPatches)); 
            }
            if (ModChecker.hasDiveristy)
            {
                harmony.PatchAll(typeof(DiversityPatches));
            }
        }

        internal void CheckInstalledMods()
        {
            mls.LogInfo("Looking for compatible mods...");

            // BMX Lobby compat
            if (ModChecker.CheckForMod("BMX.LobbyCompatibility"))
            {
                PluginHelper.RegisterPlugin(modGUID, Version.Parse(modVersion), CompatibilityLevel.ClientOnly, VersionStrictness.None);
            }

            // Other mods
            ModChecker.hasToilHead = ModChecker.CheckForMod("com.github.zehsteam.ToilHead");
            ModChecker.hasMalfunctions = ModChecker.CheckForMod("com.zealsprince.malfunctions");
            ModChecker.hasDiveristy = ModChecker.CheckForMod("Chaos.Diversity");
            ModChecker.hasFacilityMeltdown = ModChecker.CheckForMod("me.loaforc.facilitymeltdown");
        }

        internal void ManageSaveValues()
        {
            mls.LogDebug("Looking for: " + Application.persistentDataPath + SaveManager.fileName);
            SaveManager.playedIntro = false;
            SaveManager.firstTimeDiversity = true;
            if (File.Exists(Application.persistentDataPath + SaveManager.fileName))
            {
                mls.LogDebug("File found. Loading values...");
                SaveManager.playedIntro = SaveManager.LoadFromFile(0);
                SaveManager.firstTimeDiversity = SaveManager.LoadFromFile(1);
                
                // This is so reinstalls / reenables of the mod trigger the first time replies as well
                if (!ModChecker.hasDiveristy && !SaveManager.firstTimeDiversity)
                {
                    SaveManager.firstTimeDiversity = true;
                }
            }
            else
            {
                mls.LogDebug("File not found. Creating it now!");
                SaveManager.SaveToFile();
            }
        }

        internal void GenerateConfigValues()
        {
            // Dialogue & Interactions
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
            ignoreMasterVolume = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Ignore Master Volume setting", // Key of this config
                false, // Default value
                "If set to true, VEGA will ignore the game's master volume setting and always play at the same volume level.\nThis setting only applies before joining a game." // Description
            );
            playIntro = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Play intro", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you its introduction speech the first time you use the mod." // Description
            );
            readBestiaryEntries = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Read Bestiary entries", // Key of this config
                true, // Default value
                "If set to true, VEGA will read every bestiary entry you open in the terminal." // Description
            );
            giveWeatherInfo = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Give Weather info", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you information on a moon's current weather upon landing if you have little to no experience." // Description
            );
            messages = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Signal Translator messages", // Key of this config
                "YES, NO, OKAY, HELP, THANKS, ITEMS, MAIN, FIRE, GIANT, GIANTS, DOG, DOGS, WORM, WORMS, BABOONS, HAWKS, DANGER, GIRL, GHOST, BRACKEN, BUTLER, BUTLERS, BUG, BUGS, YIPPEE, SNARE, FLEA, COIL, JESTER, SLIME, THUMPER, MIMIC, MIMICS, MASKED, SPIDER, SNAKES, OLD BIRD, HEROBRINE, FOOTBALL, FIEND, SLENDER, LOCKER, SHY GUY, SIRENHEAD, DRIFTWOOD, WALKER, WATCHER, INSIDE, TRAPPED, LEAVE, GOLD, APPARATUS", // Default value
                "The messages VEGA can transmit using the Signal Translator.\nEach message must be separated by a comma and a white space, like so -> 'Message, Another message'\nApplies after a game restart." // Description
            );

            // Advanced Scanner
            enableAdvancedScannerAuto = Config.Bind(
                "Advanced Scanner", // Config section
                "Enable the Advanced Scanner automatically", // Key of this config
                false, // Default value
                "Enables VEGA's Advanced Scanner automatically when joining a game. Useful if you always want to have it on and don't want to repeat the voice command often. Applies after restarting the game." // Description
            );
            detectMasked = Config.Bind(
                "Advanced Scanner", // Config section
                "Detect Masked employees", // Key of this config
                false, // Default value
                "Determines if the Advanced Scanner should be able to count Masked employees as entities." // Description
            );

            // Voice commands
            confidence = Config.Bind(
                "Voice Recognition", // Config section
                "Confidence", // Key of this config
                0.7f, // Default value
                new ConfigDescription("Determines how easy / hard it is for VEGA to recognize voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives. If VEGA doesn't pick you up, try lowering this value.\nCan be changed mid-game.", new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            useManualListening = Config.Bind(
                "Voice Recognition", // Config section
                "Manual Listening", // Key of this config
                false, // Default value
                "Determines if VEGA should only be able to hear you when you ask him to." // Description
            );
            enableManualListeningAuto = Config.Bind(
                "Voice Recognition", // Config section
                "Enable VEGA listening automatically", // Key of this config
                false, // Default value
                "Makes VEGA listen automatically when joining a game. Only works if Manual Listening is set to true. Applies after restarting the game." // Description
            );
            registerActivation = Config.Bind(
                "Voice Recognition", // Config section
                "Register Manual Listening commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
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
            registerCrewInShip = Config.Bind(
                "Voice Recognition", // Config section
                "Register Crew in ship commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerScrapLeft = Config.Bind(
                "Voice Recognition", // Config section
                "Register Scrap / items left commands", // Key of this config
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
            /* 
            registerLeverPull = Config.Bind(
                "Voice Recognition", // Config section
                "Register Ship lever pull commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            */
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
                case "debug":
                    mls.LogDebug(message);
                    break;
                default:
                    mls.LogInfo(message);
                    break;
            }
        }
    }
}
