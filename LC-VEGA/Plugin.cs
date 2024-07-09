using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_VEGA.Patches;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace LC_VEGA
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
    [BepInDependency("BMX.LobbyCompatibility", DependencyFlags.HardDependency)]
    [BepInDependency("me.loaforc.voicerecognitionapi", DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", DependencyFlags.HardDependency)]
    [BepInDependency("me.loaforc.facilitymeltdown", DependencyFlags.SoftDependency)]
    [BepInDependency("com.zealsprince.malfunctions", DependencyFlags.SoftDependency)]
    [BepInDependency("com.github.zehsteam.ToilHead", DependencyFlags.SoftDependency)]
    [BepInDependency("Chaos.Diversity", DependencyFlags.SoftDependency)]
    [BepInDependency("TestAccount666.ShipWindows", DependencyFlags.SoftDependency)]
    [BepInDependency("com.malco.lethalcompany.moreshipupgrades", DependencyFlags.SoftDependency)]
    [BepInDependency("me.eladnlg.customhud", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.LC-VEGA";
        private const string modName = "LC-VEGA";
        private const string modVersion = "2.1.0";

        internal static AssetBundle assetBundle;

        internal static PlayerInput PlayerInputInstance;

        // Confidence values

        // Dialogue & Interactions config values
        public static ConfigEntry<VocalLevels> vocalLevel;
        public static ConfigEntry<bool> playIntro;
        public static ConfigEntry<bool> readBestiaryEntries;
        public static ConfigEntry<bool> giveWeatherInfo;
        public static ConfigEntry<string> messages;

        // Mod interactions values
        public static ConfigEntry<bool> malfunctionWarnings;
        public static ConfigEntry<bool> diversitySpeaker;
        public static ConfigEntry<int> diversitySpeakerReplyChance;

        // Sound Settings values
        public static ConfigEntry<float> volume;
        public static ConfigEntry<bool> ignoreMasterVolume;

        // Advanced Scanner config values
        public static ConfigEntry<float> scannerRange;
        public static ConfigEntry<bool> enableAdvancedScannerAuto;
        public static ConfigEntry<bool> detectMasked;
        public static ConfigEntry<bool> scanItems;
        public static ConfigEntry<bool> scanEntities;

        // Manual activation config values
        public static ConfigEntry<bool> useManualListening;
        public static ConfigEntry<bool> enableManualListeningAuto;

        // Voice commands config values
        public static ConfigEntry<bool> enhancedTeleportCommands;
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
        public static ConfigEntry<bool> registerDiscombobulator;
        public static ConfigEntry<bool> registerCrewStatus;
        public static ConfigEntry<bool> registerCrewInShip;
        public static ConfigEntry<bool> registerScrapLeft;
        public static ConfigEntry<bool> registerRadarBoosters;
        public static ConfigEntry<bool> registerSignalTranslator;
        public static ConfigEntry<bool> registerTime;
        public static ConfigEntry<bool> registerInteractShipDoors;
        public static ConfigEntry<bool> registerInteractShipLights;
        public static ConfigEntry<bool> registerInteractShipShutters;
        public static ConfigEntry<bool> registerWeatherInfo;
        public static ConfigEntry<bool> registerStop;
        public static ConfigEntry<bool> registerThanks;

        // Patch values
        public static ConfigEntry<bool> patchReadInput;

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
            VEGA.audioClips = new List<AudioClip>();
            string folderLocation = Instance.Info.Location;
            folderLocation = folderLocation.TrimEnd("LC-VEGA.dll".ToCharArray());
            assetBundle = AssetBundle.LoadFromFile(folderLocation + "lcvegavoicelines");
            if (assetBundle != null)
            {
                VEGA.audioClips = assetBundle.LoadAllAssets<AudioClip>().ToList();
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
            if (patchReadInput.Value) harmony.PatchAll(typeof(ReadInputPatch));
            if (ModChecker.hasMalfunctions) harmony.PatchAll(typeof(MalfunctionsPatches));
            if (ModChecker.hasDiversity) harmony.PatchAll(typeof(DiversityPatches));
            if (ModChecker.hasShipWindows) harmony.PatchAll(typeof(ShipWindowsPatches));
            if (ModChecker.hasLGU) harmony.PatchAll(typeof(LGUPatches));
        }

        internal void CheckInstalledMods()
        {
            mls.LogInfo("Looking for compatible mods...");

            ModChecker.hasToilHead = ModChecker.CheckForMod("com.github.zehsteam.ToilHead");
            ModChecker.hasMalfunctions = ModChecker.CheckForMod("com.zealsprince.malfunctions");
            ModChecker.hasFacilityMeltdown = ModChecker.CheckForMod("me.loaforc.facilitymeltdown");
            ModChecker.hasDiversity = ModChecker.CheckForMod("Chaos.Diversity");
            ModChecker.hasShipWindows = ModChecker.CheckForMod("TestAccount666.ShipWindows");
            ModChecker.hasLGU = ModChecker.CheckForMod("com.malco.lethalcompany.moreshipupgrades");
            ModChecker.hasEladsHUD = ModChecker.CheckForMod("me.eladnlg.customhud");
        }

        internal void ManageSaveValues()
        {
            mls.LogDebug("Looking for: " + Application.persistentDataPath + SaveManager.fileName);
            SaveManager.playedIntro = false;
            SaveManager.firstTimeDiversity = true;
            SaveManager.hadDiversity = false;
            if (File.Exists(Application.persistentDataPath + SaveManager.fileName))
            {
                mls.LogDebug("File found. Reading values...");
                SaveManager.playedIntro = SaveManager.GetValueFromIndex(0);
                mls.LogDebug("Has played the intro? -> " + SaveManager.playedIntro);
                SaveManager.firstTimeDiversity = SaveManager.GetValueFromIndex(1);
                mls.LogDebug("First time using Diversity? -> " + SaveManager.firstTimeDiversity);
                SaveManager.hadDiversity = SaveManager.GetValueFromIndex(2);
                mls.LogDebug("Had Diversity installed? -> " + SaveManager.hadDiversity);

                // This is so reinstalls / reenables of the mod trigger the first time replies as well
                if (!SaveManager.firstTimeDiversity && !SaveManager.hadDiversity)
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
                "Give weather info", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you information on a moon's current weather upon landing if you have little to no experience." // Description
            );
            messages = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Signal Translator messages", // Key of this config
                "YES, NO, OKAY, HELP, THANKS, ITEMS, MAIN, FIRE, GIANT, GIANTS, DOG, DOGS, WORM, WORMS, BABOONS, HAWKS, DANGER, GIRL, GHOST, BRACKEN, BUTLER, BUTLERS, BUG, BUGS, YIPPEE, SNARE, FLEA, COIL, JESTER, SLIME, THUMPER, MIMIC, MIMICS, MASKED, SPIDER, SNAKES, OLD BIRD, HEROBRINE, FOOTBALL, FIEND, SLENDER, LOCKER, SHY GUY, SIRENHEAD, DRIFTWOOD, WALKER, WATCHER, LOST, INSIDE, TRAPPED, LEAVE, GOLD, APPARATUS", // Default value
                "The messages VEGA can transmit using the Signal Translator.\nEach message must be separated by a comma and a white space, like so -> 'Message, Another message'\nApplies after a game restart." // Description
            );

            // Mod Interactions
            malfunctionWarnings = Config.Bind(
                "Mod Interactions", // Config section
                "Malfunction Warnings", // Key of this config
                true, // Default value
                "If set to true, VEGA will warn you and give you info on a malfunction from the Malfunctions mod when it happens." // Description
            );
            diversitySpeaker = Config.Bind(
                "Mod Interactions", // Config section
                "Diversity Speaker", // Key of this config
                true, // Default value
                "If set to true, VEGA will reply to Diversity's speaker." // Description
            );
            diversitySpeakerReplyChance = Config.Bind(
                "Mod Interactions", // Config section
                "Reply chance", // Key of this config
                40, // Default value
                new ConfigDescription("Changes how likely it is for VEGA to reply to the Diversity speaker.\n0 means it will never reply, 100 means it will always reply.", new AcceptableValueRange<int>(0, 100)) // Description
            );

            // Sound settings
            volume = Config.Bind(
                "Sound Settings", // Config section
                "Volume", // Key of this config
                1.0f, // Default value
                new ConfigDescription("Changes how loud VEGA is.", new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            ignoreMasterVolume = Config.Bind(
                "Sound Settings", // Config section
                "Ignore Master Volume setting", // Key of this config
                false, // Default value
                "If set to true, VEGA will ignore the game's master volume setting and always play at the same volume level.\nThis setting only applies before joining a game." // Description
            );

            // Advanced Scanner
            scannerRange = Config.Bind(
                "Advanced Scanner", // Config section
                "Range", // Key of this config
                29f, // Default value
                new ConfigDescription("Changes how far the Advanced Scanner can reach (in meters). Requires a restart.", new AcceptableValueRange<float>(1f, 29f)) // Description
            );
            scanEntities = Config.Bind(
                "Advanced Scanner", // Config section
                "Scan entities", // Key of this config
                true, // Default value
                "Whether the Advanced Scanner scans nearby entities or not." // Description
            );
            scanItems = Config.Bind(
                "Advanced Scanner", // Config section
                "Scan items", // Key of this config
                true, // Default value
                "Whether the Advanced Scanner scans nearby items or not." // Description
            );
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
            enhancedTeleportCommands = Config.Bind(
                "Voice Commands", // Config section
                "Enhanced Teleport commands", // Key of this config
                true, // Default value
                "Makes VEGA perform the radar switch before activating the teleporter." // Description
            );
            // confidence = Config.Bind(
            //     "Voice Recognition", // Config section
            //     "Confidence", // Key of this config
            //     0.7f, // Default value
            //     new ConfigDescription("Determines how easy / hard it is for VEGA to recognize voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives. If VEGA doesn't pick you up, try lowering this value.\nCan be changed mid-game.", new AcceptableValueRange<float>(0f, 1.0f)) // Description
            // );
            useManualListening = Config.Bind(
                "Voice Commands", // Config section
                "Manual Listening", // Key of this config
                false, // Default value
                "Determines if VEGA should only be able to hear you when you ask him to." // Description
            );
            enableManualListeningAuto = Config.Bind(
                "Voice Commands", // Config section
                "Enable VEGA listening automatically", // Key of this config
                false, // Default value
                "Makes VEGA listen automatically when joining a game. Only works if Manual Listening is set to true. Applies after restarting the game." // Description
            );
            registerActivation = Config.Bind(
                "Voice Commands", // Config section
                "Register Manual Listening commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerMoonsInfo = Config.Bind(
                "Voice Commands", // Config section
                "Register Moon info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerBestiaryEntries = Config.Bind(
                "Voice Commands", // Config section
                "Register Bestiary entries commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerCreatureInfo = Config.Bind(
                "Voice Commands", // Config section
                "Register Creature info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerAdvancedScanner = Config.Bind(
                "Voice Commands", // Config section
                "Register Advanced Scanner commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractSecureDoor = Config.Bind(
                "Voice Commands", // Config section
                "Register Open / Close Secure Door commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractAllSecureDoors = Config.Bind(
                "Voice Commands", // Config section
                "Register Open / Close All Secure Doors command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableTurret = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable Turret command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableAllTurrets = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable All Turrets command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableMine = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable Mine command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableAllMines = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable All Mines command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableSpikeTrap = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable Spike Trap command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerDisableAllSpikeTraps = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable All Spike Traps command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            registerTeleporter = Config.Bind(
                "Voice Commands", // Config section
                "Register Teleporter commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerRadarSwitch = Config.Bind(
                "Voice Commands", // Config section
                "Register Radar Switch commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerDiscombobulator = Config.Bind(
                "Voice Commands", // Config section
                "Register Discombobulator upgrade commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game.\nNote: This command only works with Late Game Upgrades installed." // Description
            );
            registerCrewStatus = Config.Bind(
                "Voice Commands", // Config section
                "Register Crew Status commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerCrewInShip = Config.Bind(
                "Voice Commands", // Config section
                "Register Crew in ship commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerScrapLeft = Config.Bind(
                "Voice Commands", // Config section
                "Register Scrap / items left commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerRadarBoosters = Config.Bind(
                "Voice Commands", // Config section
                "Register Radar Booster commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerSignalTranslator = Config.Bind(
                "Voice Commands", // Config section
                "Register Signal Translator commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerTime = Config.Bind(
                "Voice Commands", // Config section
                "Register Current time of day commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractShipDoors = Config.Bind(
                "Voice Commands", // Config section
                "Register Ship Door commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractShipLights = Config.Bind(
                "Voice Commands", // Config section
                "Register Ship Lights commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerInteractShipShutters = Config.Bind(
                "Voice Commands", // Config section
                "Register Ship Shutters commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game.\nNote: This command only works with ShipWindows installed." // Description
            );
            registerWeatherInfo = Config.Bind(
                "Voice Commands", // Config section
                "Register Weather info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerStop = Config.Bind(
                "Voice Commands", // Config section
                "Register Stop Talking commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            registerThanks = Config.Bind(
                "Voice Commands", // Config section
                "Register Thank you commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );

            // Patches
            patchReadInput = Config.Bind(
                "Patches", // Config section
                "Patch ReadInput", // Key of this config
                true, // Default value
                "Enables / disables the code that checks for player input and allows you to make VEGA listen / stop listening by pressing a key." // Description
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
