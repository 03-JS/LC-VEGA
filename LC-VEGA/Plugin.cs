using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_VEGA.Patches;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using SpeechRecognitionAPI;
using TMPro;
using UnityEngine;
using static BepInEx.BepInDependency;

namespace LC_VEGA
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("JS03.SpeechRecognitionAPI")]
    [BepInDependency("com.rune580.LethalCompanyInputUtils")]
    [BepInDependency("ainavt.lc.lethalconfig")]
    [BepInDependency("BMX.LobbyCompatibility", DependencyFlags.SoftDependency)]
    /*
    [BepInDependency("me.loaforc.voicerecognitionapi", DependencyFlags.SoftDependency)]
    [BepInDependency("me.loaforc.facilitymeltdown", DependencyFlags.SoftDependency)]
    [BepInDependency("com.zealsprince.malfunctions", DependencyFlags.SoftDependency)]
    [BepInDependency("com.github.zehsteam.ToilHead", DependencyFlags.SoftDependency)]
    [BepInDependency("Chaos.Diversity", DependencyFlags.SoftDependency)]
    [BepInDependency("TestAccount666.ShipWindows", DependencyFlags.SoftDependency)]
    [BepInDependency("com.malco.lethalcompany.moreshipupgrades", DependencyFlags.SoftDependency)]
     */
    [BepInDependency("me.eladnlg.customhud", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "JS03.LC-VEGA";
        private const string modName = "LC-VEGA";
        private const string modVersion = "5.0.0";

        internal static AssetBundle assetBundle;

        internal static PlayerInput PlayerInputInstance;

        // Language values
        public static ConfigEntry<Languages> voLanguage;
        public static ConfigEntry<Languages> commandLanguage;

        // Confidence values
        public static ConfigEntry<float> globalConfidence;
        public static ConfigEntry<float> gratitudeConfidence;
        public static ConfigEntry<float> stopConfidence;
        public static ConfigEntry<float> infoConfidence;
        public static ConfigEntry<float> manualActivationConfidence;
        public static ConfigEntry<float> secureDoorsConfidence;
        public static ConfigEntry<float> turretsConfidence;
        public static ConfigEntry<float> landminesConfidence;
        public static ConfigEntry<float> trapsConfidence;
        public static ConfigEntry<float> signalsConfidence;
        public static ConfigEntry<float> teleportConfidence;
        public static ConfigEntry<float> shipConfidence;
        public static ConfigEntry<float> miscConfidence;
        // public static ConfigEntry<float> upgradesConfidence;
        public static ConfigEntry<float> crewStatusConfidence;
        public static ConfigEntry<float> crewInShipConfidence;
        public static ConfigEntry<float> scrapLeftConfidence;

        // Dialogue & Interactions config values
        public static ConfigEntry<VocalLevels> vocalLevel;
        public static ConfigEntry<bool> playIntro;
        // public static ConfigEntry<bool> readBestiaryEntries;
        public static ConfigEntry<bool> remoteEntityID;
        public static ConfigEntry<bool> giveWeatherInfo;
        public static ConfigEntry<bool> giveApparatusWarnings;
        public static ConfigEntry<string> messages;

        // Mod interactions values
        // public static ConfigEntry<bool> malfunctionWarnings;
        // public static ConfigEntry<bool> diversitySpeaker;
        // public static ConfigEntry<int> diversitySpeakerReplyChance;
        // public static ConfigEntry<bool> meltdownTimer;

        // Text chat
        public static ConfigEntry<string> playerNameColors;
        public static ConfigEntry<bool> sendRadarSwitchChatMessage;
        public static ConfigEntry<bool> sendSignalTranslatorChatMessage;
        public static ConfigEntry<bool> sendTeleporterChatMessage;
        // public static ConfigEntry<bool> sendDiscombobulatorChatMessage;

        // Sound Settings values
        public static ConfigEntry<float> volume;
        public static ConfigEntry<bool> ignoreMasterVolume;

        // Advanced Scanner config values
        public static ConfigEntry<DisplayModes> infoDisplayMode;
        public static ConfigEntry<float> scannerRange;
        public static ConfigEntry<bool> enableAdvancedScannerAuto;
        public static ConfigEntry<bool> detectMasked;
        public static ConfigEntry<float> scale;
        public static ConfigEntry<float> horizontalPosition;
        public static ConfigEntry<float> verticalPosition;
        public static ConfigEntry<float> horizontalGap;
        public static ConfigEntry<float> verticalGap;
        public static ConfigEntry<float> entitiesTextLength;
        public static ConfigEntry<float> itemsTextLength;
        public static ConfigEntry<float> entitiesTilt;
        public static ConfigEntry<float> itemsTilt;
        public static ConfigEntry<TextAlignmentOptions> entitiesAlignment;
        public static ConfigEntry<TextAlignmentOptions> itemsAlignment;
        public static ConfigEntry<string> clearTextColor;
        public static ConfigEntry<string> entitiesNearbyTextColor;
        public static ConfigEntry<string> itemsNearbyTextColor;
        public static ConfigEntry<string> dataUnavailableTextColor;
        // public static ConfigEntry<string> customColorCodes;

        // Manual activation config values
        public static ConfigEntry<bool> useManualListening;
        public static ConfigEntry<bool> enableManualListeningAuto;

        // Patch values
        public static ConfigEntry<bool> patchReadInput;

        // Voice commands config values
        public static ConfigEntry<string> name;
        public static ConfigEntry<bool> enhancedTeleportCommands;
        public static ConfigEntry<bool> enhancedHazardDisabling;
        public static ConfigEntry<bool> registerActivation;
        public static ConfigEntry<string> startListeningCommands;
        public static ConfigEntry<string> stopListeningCommands;
        public static ConfigEntry<bool> registerMoonsInfo;
        public static ConfigEntry<string> moonsInfoCommands;
        // public static ConfigEntry<bool> registerBestiaryEntries;
        // public static ConfigEntry<string> bestiaryEntriesCommands;
        public static ConfigEntry<bool> registerCreatureInfo;
        public static ConfigEntry<string> creatureInfoCommands;
        public static ConfigEntry<bool> registerAdvancedScanner;
        public static ConfigEntry<string> activateAdvancedScannerCommands;
        public static ConfigEntry<string> deactivateAdvancedScannerCommands;
        public static ConfigEntry<bool> registerInteractSecureDoor;
        public static ConfigEntry<string> openSecureDoorCommands;
        public static ConfigEntry<string> closeSecureDoorCommands;
        public static ConfigEntry<bool> registerInteractAllSecureDoors;
        public static ConfigEntry<string> openAllSecureDoorsCommands;
        public static ConfigEntry<string> closeAllSecureDoorsCommands;
        public static ConfigEntry<bool> registerDisableTurret;
        public static ConfigEntry<string> disableTurretCommands;
        public static ConfigEntry<bool> registerDisableAllTurrets;
        public static ConfigEntry<string> disableAllTurretsCommands;
        public static ConfigEntry<bool> registerDisableMine;
        public static ConfigEntry<string> disableMineCommands;
        public static ConfigEntry<bool> registerDisableAllMines;
        public static ConfigEntry<string> disableAllMinesCommands;
        public static ConfigEntry<bool> registerDisableSpikeTrap;
        public static ConfigEntry<string> disableSpikeTrapCommands;
        public static ConfigEntry<bool> registerDisableAllSpikeTraps;
        public static ConfigEntry<string> disableAllSpikeTrapsCommands;
        public static ConfigEntry<bool> registerTeleporter;
        public static ConfigEntry<string> teleporterCommands;
        public static ConfigEntry<bool> registerRadarSwitch;
        public static ConfigEntry<string> radarSwitchCommands;
        // public static ConfigEntry<bool> registerDiscombobulator;
        // public static ConfigEntry<string> discombobulatorCommands;
        public static ConfigEntry<bool> registerCrewStatus;
        public static ConfigEntry<string> crewStatusCommands;
        public static ConfigEntry<bool> registerCrewInShip;
        public static ConfigEntry<string> crewInShipCommands;
        public static ConfigEntry<bool> registerScrapLeft;
        public static ConfigEntry<string> scrapLeftCommands;
        public static ConfigEntry<bool> registerRadarBoosters;
        public static ConfigEntry<string> radarPingCommands;
        public static ConfigEntry<string> radarFlashCommands;
        public static ConfigEntry<bool> registerSignalTranslator;
        public static ConfigEntry<string> transmitCommands;
        public static ConfigEntry<bool> registerTime;
        public static ConfigEntry<string> timeCommands;
        public static ConfigEntry<bool> registerInteractShipDoors;
        public static ConfigEntry<string> openShipDoorsCommands;
        public static ConfigEntry<string> closeShipDoorsCommands;
        public static ConfigEntry<bool> registerInteractShipLights;
        public static ConfigEntry<string> lightsOnCommands;
        public static ConfigEntry<string> lightsOffCommands;
        public static ConfigEntry<bool> registerInteractShipMagnet;
        public static ConfigEntry<string> magnetOnCommands;
        public static ConfigEntry<string> magnetOffCommands;
        // public static ConfigEntry<bool> registerInteractShipShutters;
        // public static ConfigEntry<string> openShuttersCommands;
        // public static ConfigEntry<string> closeShuttersCommands;
        public static ConfigEntry<bool> registerWeatherInfo;
        public static ConfigEntry<string> weatherInfoCommands;
        public static ConfigEntry<bool> registerStop;
        public static ConfigEntry<string> stopCommands;
        public static ConfigEntry<bool> registerThanks;
        public static ConfigEntry<string> gratitudeCommands;

        private readonly Harmony harmony = new Harmony(modGUID);
        public static Plugin Instance;
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

            GenerateConfigValues();
            LoadAssets();
            CheckInstalledMods();
            ManageSaveValues();

            VEGA.Initialize();
            PatchStuff();
        }

        internal void LoadAssets()
        {
            mls.LogInfo("Loading assets");
            string folderLocation = Instance.Info.Location;
            folderLocation = folderLocation.TrimEnd("LC-VEGA.dll".ToCharArray());
            assetBundle = AssetBundle.LoadFromFile(voLanguage.Value == Languages.English
                ? folderLocation + "vega_en"
                : folderLocation + "vega_es");
            if (assetBundle != null)
            {
                VEGA.audioClips = new List<AudioClip>();
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
            harmony.PatchAll(typeof(ApparatusPatch));
            harmony.PatchAll(typeof(GeneralPatches));
            if (patchReadInput.Value) harmony.PatchAll(typeof(ReadInputPatch));
            /*
            if (ModChecker.hasMalfunctions) harmony.PatchAll(typeof(MalfunctionsPatches));
            if (ModChecker.hasDiversity) harmony.PatchAll(typeof(DiversityPatches));
            if (ModChecker.hasShipWindows) harmony.PatchAll(typeof(ShipWindowsPatches));
            if (ModChecker.hasLGU) harmony.PatchAll(typeof(LGUPatches));
            if (ModChecker.hasFacilityMeltdown) harmony.PatchAll(typeof(FacilityMeltdownPatches));
            */
        }

        internal void CheckInstalledMods()
        {
            mls.LogInfo("Looking for compatible mods...");

            if (ModChecker.CheckForMod("BMX.LobbyCompatibility")) ModChecker.RegisterPlugin(modGUID, modVersion);
            /*
            ModChecker.hasToilHead = ModChecker.CheckForMod("com.github.zehsteam.ToilHead");
            ModChecker.hasMalfunctions = ModChecker.CheckForMod("com.zealsprince.malfunctions");
            ModChecker.hasFacilityMeltdown = ModChecker.CheckForMod("me.loaforc.facilitymeltdown");
            ModChecker.hasDiversity = ModChecker.CheckForMod("Chaos.Diversity");
            ModChecker.hasShipWindows = ModChecker.CheckForMod("TestAccount666.ShipWindows");
            ModChecker.hasLGU = ModChecker.CheckForMod("com.malco.lethalcompany.moreshipupgrades");
            */
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

                // This is so reinstalling / re-enabling the mod trigger the first time replies as well
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
            // Language
            voLanguage = Config.Bind(
                "Language", // Config section
                "Voice lines", // Key of this config
                Languages.English, // Default value
                "Changes the language in which VEGA speaks." // Description
            );
            voLanguage.SettingChanged += (sender, args) => { LoadAssets(); };
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<Languages>(voLanguage, requiresRestart: false));
            commandLanguage = Config.Bind(
                "Language", // Config section
                "Voice commands", // Key of this config
                Languages.English, // Default value
                "Changes the voice commands to the language you choose.\n\n" +
                "WARNING: This will erase all your custom commands and replace them with the defaults for the chosen language. If you have any custom commands and would like to keep them, DO NOT touch this!!!" +
                "\n\nRequires a restart." // Description
            );
            commandLanguage.SettingChanged += (sender, args) =>
            {
                SpeechRecognitionAPI.Plugin.language.Value = commandLanguage.Value;
                UpdateVoiceCommands();
                messages.Value = LanguageHelper.Messages[commandLanguage.Value];
            };
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<Languages>(commandLanguage));

            // Dialogue & Interactions
            vocalLevel = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Vocal Level", // Key of this config
                VocalLevels.High, // Default value
                "Changes how often VEGA speaks and the length of his answers. Interactions that have their own toggle will not be affected by this.\n" +
                "High: The default value. Will speak on every interaction. Recommended for inexperienced players.\n" +
                "Medium: Gives you useful info, doesn't speak on every interaction. Recommended for experienced players.\n" +
                "Low: Speaks as often as the Medium option, but gives you shorter answers. Recommended for players who already have experience with VEGA.\n" +
                "None: Will only speak when asked to." // Description
            );
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<VocalLevels>(vocalLevel, requiresRestart: false));
            playIntro = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Play intro", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you its introduction speech the first time you use the mod." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(playIntro, requiresRestart: false));
            // readBestiaryEntries = Config.Bind(
            //     "Dialogue & Interactions", // Config section
            //     "Read Bestiary entries", // Key of this config
            //     true, // Default value
            //     "If set to true, VEGA will read every bestiary entry you open in the terminal." // Description
            // );
            remoteEntityID = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Remote Entity Identification", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you the name of every new entity your crew scans." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(remoteEntityID, requiresRestart: false));
            giveApparatusWarnings = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Give Apparatus warnings", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you brief and informative warnings when the Apparatus is pulled." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(giveApparatusWarnings, requiresRestart: false));
            giveWeatherInfo = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Give weather info", // Key of this config
                true, // Default value
                "If set to true, VEGA will give you information on a moon's current weather upon landing if you have little to no experience." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(giveWeatherInfo, requiresRestart: false));
            messages = Config.Bind(
                "Dialogue & Interactions", // Config section
                "Signal Translator messages", // Key of this config
                LanguageHelper.Messages[commandLanguage.Value], // Default value
                "The messages VEGA can transmit using the Signal Translator.\nEach message must be separated by a comma and a white space, like so -> 'Message, Another message'\nApplies after a game restart." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(messages));

            // Mod Interactions
            /*
            malfunctionWarnings = Config.Bind(
                "Mod Interactions", // Config section
                "Malfunction Warnings", // Key of this config
                true, // Default value
                "If set to true, VEGA will warn you and give you info on a malfunction from the Malfunctions mod when it happens." // Description
            );
            meltdownTimer = Config.Bind(
                "Mod Interactions", // Config section
                "Facility Meltdown timer", // Key of this config
                true, // Default value
                "If set to true, VEGA will tell you how much time you have left before the explosion happens when there's a minute or less left." // Description
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
                new ConfigDescription(
                    "Changes how likely it is for VEGA to reply to the Diversity speaker.\n0 means it will never reply, 100 means it will always reply.",
                    new AcceptableValueRange<int>(0, 100)) // Description
            );
            */

            // Confidence
            globalConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Global", // Key of this config
                0.5f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize all voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives." +
                    "\n\nIf you'd like to change the threshold for a specific command or set of commands, simply change their values to something higher than 0.\n\nThis setting can be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(globalConfidence, requiresRestart: false));
            gratitudeConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Gratitude", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Thank you' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(gratitudeConfidence, requiresRestart: false));
            stopConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Stop talking", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Stop talking' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(stopConfidence, requiresRestart: false));
            secureDoorsConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Secure doors", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the voice commands used to interact with secure doors. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(secureDoorsConfidence, requiresRestart: false));
            turretsConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Turrets", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize voice commands used to disable turrets. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(turretsConfidence, requiresRestart: false));
            landminesConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Landmines", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize voice commands used to disable landmines. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(landminesConfidence, requiresRestart: false));
            trapsConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Spike traps", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize voice commands used to disable spike traps. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(trapsConfidence, requiresRestart: false));
            signalsConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Signal translator", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the voice commands related to the Signal Translator. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(signalsConfidence, requiresRestart: false));
            teleportConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Teleport", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Teleport' and 'Radar Switch' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(teleportConfidence, requiresRestart: false));
            shipConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Ship", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize voice commands related to the ship's lights and door. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(shipConfidence, requiresRestart: false));
            miscConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Miscellaneous", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize voice commands related to the Advanced Scanner and Radar Boosters. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(miscConfidence, requiresRestart: false));
            // upgradesConfidence = Config.Bind(
            //     "Accuracy thresholds", // Config section
            //     "Ship upgrades", // Key of this config
            //     0.5f, // Default value
            //     new ConfigDescription(
            //         "Determines how difficult it is for VEGA to recognize voice commands related to LGU. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
            //         new AcceptableValueRange<float>(0f, 1.0f)) // Description
            // );
            crewStatusConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Crew Status", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Crew status' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(crewStatusConfidence, requiresRestart: false));
            crewInShipConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Crew in ship", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Crew in ship' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(crewInShipConfidence, requiresRestart: false));
            scrapLeftConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Scrap / items left", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Scrap / items left' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(scrapLeftConfidence, requiresRestart: false));
            infoConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Info", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize voice commands related to information about Entities, Moons, Weather phenomena and the current Time of day. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(infoConfidence, requiresRestart: false));
            manualActivationConfidence = Config.Bind(
                "Accuracy thresholds", // Config section
                "Manual Activation", // Key of this config
                0f, // Default value
                new ConfigDescription(
                    "Determines how difficult it is for VEGA to recognize the 'Manual Activation' voice commands. Higher values means he needs to be more confident, lower values will activate more often, but will cause more false positives.\nCan be changed mid-game.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(manualActivationConfidence, requiresRestart: false));

            // Sound settings
            volume = Config.Bind(
                "Sound Settings", // Config section
                "Volume", // Key of this config
                1.0f, // Default value
                new ConfigDescription("Changes how loud VEGA is.",
                    new AcceptableValueRange<float>(0f, 1.0f)) // Description
            );
            volume.SettingChanged += (obj, args) =>
            {
                if (VEGA.audioSource != null) VEGA.audioSource.volume = volume.Value;
                if (VEGA.sfxAudioSource != null) VEGA.sfxAudioSource.volume = volume.Value;
            };
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(volume, requiresRestart: false));
            ignoreMasterVolume = Config.Bind(
                "Sound Settings", // Config section
                "Ignore Master Volume setting", // Key of this config
                false, // Default value
                "If set to true, VEGA will ignore the game's master volume setting and always play at the same volume level.\nThis setting only applies before joining a game." // Description
            );
            ignoreMasterVolume.SettingChanged += (obj, args) =>
            {
                if (VEGA.audioSource != null) VEGA.audioSource.ignoreListenerVolume = ignoreMasterVolume.Value;
                if (VEGA.sfxAudioSource != null) VEGA.sfxAudioSource.ignoreListenerVolume = ignoreMasterVolume.Value;
            };
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ignoreMasterVolume, requiresRestart: false));

            // Manual Listening
            useManualListening = Config.Bind(
                "Manual Listening", // Config section
                "Enabled", // Key of this config
                false, // Default value
                "Manual Listening determines if VEGA should only be able to hear you when you ask him to." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(useManualListening, requiresRestart: false));
            enableManualListeningAuto = Config.Bind(
                "Manual Listening", // Config section
                "Enable VEGA listening automatically", // Key of this config
                false, // Default value
                "Makes VEGA listen automatically when joining a game. Only works if Manual Listening is set to true." // Description
            );
            enableManualListeningAuto.SettingChanged += (obj, args) =>
            {
                if (!VEGA.listening)
                {
                    VEGA.PlaySFX("Activate");
                }
                else
                {
                    VEGA.PlaySFX("Deactivate");
                }

                VEGA.listening = enableManualListeningAuto.Value;
            };
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(enableManualListeningAuto, requiresRestart: false));

            // Advanced Scanner
            scannerRange = Config.Bind(
                "Advanced Scanner", // Config section
                "Range", // Key of this config
                29f, // Default value
                new ConfigDescription("Changes how far the Advanced Scanner can reach (in meters).",
                    new AcceptableValueRange<float>(1f, 29f)) // Description
            );
            scannerRange.SettingChanged += (obj, args) => { VEGA.scannerRange = scannerRange.Value; };
            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(scannerRange, requiresRestart: false));
            enableAdvancedScannerAuto = Config.Bind(
                "Advanced Scanner", // Config section
                "Enable the Advanced Scanner automatically", // Key of this config
                false, // Default value
                "Enables VEGA's Advanced Scanner automatically when joining a game. Useful if you always want to have it on and don't want to repeat the voice command often." // Description
            );
            enableAdvancedScannerAuto.SettingChanged += (obj, args) =>
            {
                VEGA.advancedScannerActive = enableAdvancedScannerAuto.Value;
                if (!VEGA.advancedScannerActive)
                {
                    if (HUDManagerPatch.entities != null)
                        HUDManagerPatch.entities.GetComponent<TextMeshProUGUI>().SetText("");
                    if (HUDManagerPatch.items != null)
                        HUDManagerPatch.items.GetComponent<TextMeshProUGUI>().SetText("");
                }
            };
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(enableAdvancedScannerAuto, requiresRestart: false));
            detectMasked = Config.Bind(
                "Advanced Scanner", // Config section
                "Detect Masked employees", // Key of this config
                false, // Default value
                "Determines if the Advanced Scanner should be able to count Masked employees as entities." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(detectMasked, requiresRestart: false));
            infoDisplayMode = Config.Bind(
                "Advanced Scanner", // Config section
                "Info display mode", // Key of this config
                DisplayModes.Default, // Default value
                "Changes how the Advanced Scanner displays information." // Description
            );
            infoDisplayMode.SettingChanged += (obj, args) => { VEGA.UpdateScannerDisplayMode(); };
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<DisplayModes>(infoDisplayMode, requiresRestart: false));
            scale = Config.Bind(
                "Advanced Scanner", // Config section
                "Scale", // Key of this config
                1f, // Default value
                "The size of the Advanced Scanner on the screen." // Description
            );
            scale.SettingChanged += (obj, args) => { HUDManagerPatch.UpdateScannerPosAndScale(); };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(scale, requiresRestart: false));
            horizontalPosition = Config.Bind(
                "Advanced Scanner", // Config section
                "Horizontal position", // Key of this config
                45f, // Default value
                "The horizontal position of the Advanced Scanner on the screen." // Description
            );
            horizontalPosition.SettingChanged += (obj, args) => { HUDManagerPatch.UpdateScannerPosAndScale(); };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(horizontalPosition, requiresRestart: false));
            verticalPosition = Config.Bind(
                "Advanced Scanner", // Config section
                "Vertical position", // Key of this config
                180f, // Default value
                "The vertical position of the Advanced Scanner on the screen." // Description
            );
            verticalPosition.SettingChanged += (obj, args) => { HUDManagerPatch.UpdateScannerPosAndScale(); };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(verticalPosition, requiresRestart: false));
            horizontalGap = Config.Bind(
                "Advanced Scanner", // Config section
                "Horizontal gap", // Key of this config
                0f, // Default value
                "The horizontal gap between the nearby Entities and Items on the screen." // Description
            );
            horizontalGap.SettingChanged += (obj, args) => { HUDManagerPatch.UpdateScannerPosAndScale(); };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(horizontalGap, requiresRestart: false));
            verticalGap = Config.Bind(
                "Advanced Scanner", // Config section
                "Vertical gap", // Key of this config
                50f, // Default value
                "The vertical gap between the nearby Entities and Items on the screen." // Description
            );
            verticalGap.SettingChanged += (obj, args) => { HUDManagerPatch.UpdateScannerPosAndScale(); };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(verticalGap, requiresRestart: false));
            entitiesTextLength = Config.Bind(
                "Advanced Scanner", // Config section
                "Entities text length", // Key of this config
                200f, // Default value
                "Determines how much text fits in a single line in the Entities section of the scanner." // Description
            );
            entitiesTextLength.SettingChanged += (obj, args) =>
            {
                float entitiesLength =
                    ModChecker.hasEladsHUD ? entitiesTextLength.Value + 100f : entitiesTextLength.Value;
                if (HUDManagerPatch.entities != null)
                    HUDManagerPatch.entities.GetComponent<RectTransform>().sizeDelta = new Vector2(entitiesLength,
                        HUDManagerPatch.entities.GetComponent<RectTransform>().sizeDelta.y);
            };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(entitiesTextLength, requiresRestart: false));
            itemsTextLength = Config.Bind(
                "Advanced Scanner", // Config section
                "Items text length", // Key of this config
                200f, // Default value
                "Determines how much text fits in a single line in the Items section of the scanner." // Description
            );
            itemsTextLength.SettingChanged += (obj, args) =>
            {
                float itemsLength = ModChecker.hasEladsHUD ? itemsTextLength.Value + 100f : itemsTextLength.Value;
                if (HUDManagerPatch.items != null)
                    HUDManagerPatch.items.GetComponent<RectTransform>().sizeDelta = new Vector2(itemsLength,
                        HUDManagerPatch.items.GetComponent<RectTransform>().sizeDelta.y);
            };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(itemsTextLength, requiresRestart: false));
            entitiesAlignment = Config.Bind(
                "Advanced Scanner", // Config section
                "Entities text alignment", // Key of this config
                TextAlignmentOptions.Left, // Default value
                "The alignment of the Entities section of the scanner." // Description
            );
            entitiesAlignment.SettingChanged += (obj, args) =>
            {
                if (VEGA.entitiesTextComponent != null) VEGA.entitiesTextComponent.alignment = entitiesAlignment.Value;
            };
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<TextAlignmentOptions>(entitiesAlignment, requiresRestart: false));
            itemsAlignment = Config.Bind(
                "Advanced Scanner", // Config section
                "Items text alignment", // Key of this config
                TextAlignmentOptions.Left, // Default value
                "The alignment of the Advanced Scanner on the screen." // Description
            );
            itemsAlignment.SettingChanged += (obj, args) =>
            {
                if (VEGA.itemsTextComponent != null) VEGA.itemsTextComponent.alignment = itemsAlignment.Value;
            };
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<TextAlignmentOptions>(itemsAlignment, requiresRestart: false));
            entitiesTilt = Config.Bind(
                "Advanced Scanner", // Config section
                "Entities tilt", // Key of this config
                22f, // Default value
                "The inclination of the Entities component on the screen." // Description
            );
            entitiesTilt.SettingChanged += (obj, args) =>
            {
                if (VEGA.entitiesTextComponent != null)
                {
                    RectTransform rectTransform = VEGA.entitiesTextComponent.GetComponent<RectTransform>();
                    float tilt = ModChecker.hasEladsHUD ? entitiesTilt.Value - 17f : entitiesTilt.Value;
                    rectTransform.localRotation = Quaternion.Euler(rectTransform.localRotation.x, -tilt,
                        rectTransform.localRotation.z);
                }
            };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(entitiesTilt, requiresRestart: false));
            itemsTilt = Config.Bind(
                "Advanced Scanner", // Config section
                "Items tilt", // Key of this config
                22f, // Default value
                "The inclination of the Items component on the screen." // Description
            );
            itemsTilt.SettingChanged += (obj, args) =>
            {
                if (VEGA.itemsTextComponent != null)
                {
                    RectTransform rectTransform = VEGA.itemsTextComponent.GetComponent<RectTransform>();
                    float tilt = ModChecker.hasEladsHUD ? itemsTilt.Value - 17f : itemsTilt.Value;
                    rectTransform.localRotation = Quaternion.Euler(rectTransform.localRotation.x, -tilt,
                        rectTransform.localRotation.z);
                }
            };
            LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(itemsTilt, requiresRestart: false));
            clearTextColor = Config.Bind(
                "Advanced Scanner", // Config section
                "Clear color", // Key of this config
                "#0000ff", // Default value
                "Changes the color of the text under both sections of the scanner when no entities or items are within its range." // Description
            );
            clearTextColor.SettingChanged += (obj, args) => { VEGA.UpdateScannerColors(); };
            LethalConfigManager.AddConfigItem(new HexColorInputFieldConfigItem(clearTextColor, requiresRestart: false));
            
            entitiesNearbyTextColor = Config.Bind(
                "Advanced Scanner", // Config section
                "Entities nearby color", // Key of this config
                "#ff0000", // Default value
                "Changes the color of the text under the Entities section of the scanner when entities are within the scanner's range." // Description
            );
            entitiesNearbyTextColor.SettingChanged += (obj, args) => { VEGA.UpdateScannerColors(); };
            LethalConfigManager.AddConfigItem(new HexColorInputFieldConfigItem(entitiesNearbyTextColor, requiresRestart: false));
            
            itemsNearbyTextColor = Config.Bind(
                "Advanced Scanner", // Config section
                "Items nearby color", // Key of this config
                "#008000", // Default value
                "Changes the color of the text under the Items section of the scanner when items are within the scanner's range." // Description
            );
            itemsNearbyTextColor.SettingChanged += (obj, args) => { VEGA.UpdateScannerColors(); };
            LethalConfigManager.AddConfigItem(new HexColorInputFieldConfigItem(itemsNearbyTextColor, requiresRestart: false));
            
            // dataUnavailableTextColor = Config.Bind(
            //     "Advanced Scanner", // Config section
            //     "Data unavailable color", // Key of this config
            //     "#ffff00", // Default value
            //     "Changes the color of the text under both sections of the scanner when a Communications or Power malfunction happen." // Description
            // );
            // dataUnavailableTextColor.SettingChanged += (obj, args) => { VEGA.UpdateScannerColors(); };
            // var dataUnavailableTextColorItem = new HexColorInputFieldConfigItem(dataUnavailableTextColor, requiresRestart: false);
            // LethalConfigManager.AddConfigItem(dataUnavailableTextColorItem);
            
            // customColorCodes = Config.Bind(
            //     "Advanced Scanner", // Config section
            //     "Custom color codes", // Key of this config
            //     "#0000ff, #ff0000, #008000, #ffff00", // Default value
            //     "Allows you to introduce your own custom color codes for the Clear, Entities, Items and Data unavailable color options." +
            //     "\nMake sure you separate the different values with a comma and a blank space." // Description
            // );
            // customColorCodes.SettingChanged += (obj, args) => { VEGA.UpdateScannerColors(); };

            // Text Chat
            playerNameColors = Config.Bind(
                "Text Chat", // Config section
                "Player name colors", // Key of this config
                "", // Default value
                "Allows you to change the color of people's usernames in VEGA's chat messages using hex codes. The format is:\n'username: hex code, otherusername: hex code'" // Description
            );
            playerNameColors.SettingChanged += (obj, args) =>
            {
                VEGA.RestoreNameColors();
                VEGA.AddNameColors();
            };
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(playerNameColors, requiresRestart: false));
            sendRadarSwitchChatMessage = Config.Bind(
                "Text Chat", // Config section
                "Send Radar Switch chat message", // Key of this config
                true, // Default value
                "If set to true, VEGA will send a message in the text chat that lets everyone know you used the Radar Switch command." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(sendRadarSwitchChatMessage, requiresRestart: false));
            sendSignalTranslatorChatMessage = Config.Bind(
                "Text Chat", // Config section
                "Send Signal Translator chat message", // Key of this config
                true, // Default value
                "If set to true, VEGA will send a message in the text chat that lets everyone know you used VEGA to transmit a signal." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(sendSignalTranslatorChatMessage, requiresRestart: false));
            sendTeleporterChatMessage = Config.Bind(
                "Text Chat", // Config section
                "Send Teleporter chat message", // Key of this config
                true, // Default value
                "If set to true, VEGA will send a message in the text chat that lets everyone know you asked VEGA to activate the teleporter." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(sendTeleporterChatMessage, requiresRestart: false));
            // sendDiscombobulatorChatMessage = Config.Bind(
            //     "Text Chat", // Config section
            //     "Send Discombobulator chat message", // Key of this config
            //     true, // Default value
            //     "If set to true, VEGA will send a message in the text chat that lets everyone know you made use of the Discombobulator through VEGA." // Description
            // );

            // Patches
            patchReadInput = Config.Bind(
                "Patches", // Config section
                "Patch ReadInput", // Key of this config
                true, // Default value
                "Enables / disables the code that checks for player input and allows you to make VEGA listen / stop listening by pressing a key." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(patchReadInput, requiresRestart: false));

            // Voice commands
            name = Config.Bind(
                "Voice Commands", // Config section
                "Name", // Key of this config
                "VEGA", // Default value
                "The name you'd like to address the mod by for voice commands (VEGA by default). This name will be added at the beginning of every command, except for the gratitude commands, where it will be added at the end of the command instead." +
                "\n\nYou can also leave it empty if you'd like to not use a name at all." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(name));
            enhancedTeleportCommands = Config.Bind(
                "Voice Commands", // Config section
                "Enhanced Teleport commands", // Key of this config
                true, // Default value
                "Makes VEGA perform the radar switch before activating the teleporter." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(enhancedTeleportCommands, requiresRestart: false));
            enhancedHazardDisabling = Config.Bind(
                "Voice Commands", // Config section
                "Enhanced Hazard Disabling commands", // Key of this config
                true, // Default value
                "Makes VEGA disable all visible turrets / landmines / spike traps when using their respective commands." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(enhancedHazardDisabling, requiresRestart: false));
            registerActivation = Config.Bind(
                "Voice Commands", // Config section
                "Register Manual Listening commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerActivation));
            startListeningCommands = Config.Bind(
                "Voice Commands", // Config section
                "Start listening commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["startListening"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(startListeningCommands));
            stopListeningCommands = Config.Bind(
                "Voice Commands", // Config section
                "Stop listening commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["stopListening"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(stopListeningCommands));
            registerMoonsInfo = Config.Bind(
                "Voice Commands", // Config section
                "Register Moon info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerMoonsInfo));
            moonsInfoCommands = Config.Bind(
                "Voice Commands", // Config section
                "Moon info commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["moonInfo"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'.\n" +
                "IMPORTANT: Moon info commands will always have the moon's name at the end!" // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(moonsInfoCommands));
            // registerBestiaryEntries = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Register Bestiary entries commands", // Key of this config
            //     true, // Default value
            //     "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            // );
            // bestiaryEntriesCommands = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Bestiary entries commands", // Key of this config
            //     LanguageHelper.Commands[commandLanguage.Value][
            //         "bestiaryEntries"], // Default value
            //     "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'.\n" +
            //     "IMPORTANT: Bestiary entries commands will always end with the name of the creature + entry, like so:\n" +
            //     "VEGA, read Bracken entry" // Description
            // );
            registerCreatureInfo = Config.Bind(
                "Voice Commands", // Config section
                "Register Creature info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerCreatureInfo));
            creatureInfoCommands = Config.Bind(
                "Voice Commands", // Config section
                "Creature info commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["creatureInfo"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'.\n" +
                "IMPORTANT: Creature info commands will always have the creature's name at the end!" // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(creatureInfoCommands));
            registerAdvancedScanner = Config.Bind(
                "Voice Commands", // Config section
                "Register Advanced Scanner commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerAdvancedScanner));
            activateAdvancedScannerCommands = Config.Bind(
                "Voice Commands", // Config section
                "Activate Advanced Scanner commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["activateScanner"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(activateAdvancedScannerCommands));
            deactivateAdvancedScannerCommands = Config.Bind(
                "Voice Commands", // Config section
                "Deactivate Advanced Scanner commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["deactivateScanner"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(deactivateAdvancedScannerCommands));
            registerInteractSecureDoor = Config.Bind(
                "Voice Commands", // Config section
                "Register Open / Close Secure Door commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerInteractSecureDoor));
            openSecureDoorCommands = Config.Bind(
                "Voice Commands", // Config section
                "Open secure door commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["openDoor"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(openSecureDoorCommands));
            closeSecureDoorCommands = Config.Bind(
                "Voice Commands", // Config section
                "Close secure door commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["closeDoor"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(closeSecureDoorCommands));
            registerInteractAllSecureDoors = Config.Bind(
                "Voice Commands", // Config section
                "Register Open / Close All Secure Doors command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerInteractAllSecureDoors));
            openAllSecureDoorsCommands = Config.Bind(
                "Voice Commands", // Config section
                "Open all secure door commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["openAllDoors"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(openAllSecureDoorsCommands));
            closeAllSecureDoorsCommands = Config.Bind(
                "Voice Commands", // Config section
                "Close all secure door commands", // Key of this config
                LanguageHelper.Commands[
                    commandLanguage.Value]["closeAllDoors"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(closeAllSecureDoorsCommands));
            registerDisableTurret = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable Turret command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerDisableTurret));
            disableTurretCommands = Config.Bind(
                "Voice Commands", // Config section
                "Disable turret commands", // Key of this config
                LanguageHelper.Commands[
                    commandLanguage.Value]["disableTurret"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(disableTurretCommands));
            registerDisableAllTurrets = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable All Turrets command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerDisableAllTurrets));
            disableAllTurretsCommands = Config.Bind(
                "Voice Commands", // Config section
                "Disable all turrets commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value][
                    "disableAllTurrets"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(disableAllTurretsCommands));
            registerDisableMine = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable Mine command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerDisableMine));
            disableMineCommands = Config.Bind(
                "Voice Commands", // Config section
                "Disable landmine commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["disableMine"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(disableMineCommands));
            registerDisableAllMines = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable All Mines command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerDisableAllMines));
            disableAllMinesCommands = Config.Bind(
                "Voice Commands", // Config section
                "Disable all landmines commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value][
                    "disableAllMines"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(disableAllMinesCommands));
            registerDisableSpikeTrap = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable Spike Trap command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerDisableSpikeTrap));
            disableSpikeTrapCommands = Config.Bind(
                "Voice Commands", // Config section
                "Disable spike trap commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["disableTrap"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(disableSpikeTrapCommands));
            registerDisableAllSpikeTraps = Config.Bind(
                "Voice Commands", // Config section
                "Register Disable All Spike Traps command", // Key of this config
                true, // Default value
                "Disable this if you don't want this voice command to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerDisableAllSpikeTraps));
            disableAllSpikeTrapsCommands = Config.Bind(
                "Voice Commands", // Config section
                "Disable all spike traps commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value][
                    "disableAllTraps"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(disableAllSpikeTrapsCommands));
            registerTeleporter = Config.Bind(
                "Voice Commands", // Config section
                "Register Teleporter commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerTeleporter));
            teleporterCommands = Config.Bind(
                "Voice Commands", // Config section
                "Teleporter commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["teleporter"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(teleporterCommands));
            registerRadarSwitch = Config.Bind(
                "Voice Commands", // Config section
                "Register Radar Switch commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerRadarSwitch));
            radarSwitchCommands = Config.Bind(
                "Voice Commands", // Config section
                "Radar Switch commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["mapSwitch"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(radarSwitchCommands));
            // registerDiscombobulator = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Register Discombobulator upgrade commands", // Key of this config
            //     true, // Default value
            //     "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game.\nNote: This command only works with Late Game Upgrades installed." // Description
            // );
            // discombobulatorCommands = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Discombobulator commands", // Key of this config
            //     "VEGA, attack/VEGA, stun/VEGA, shock", // Default value
            //     "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            // );
            registerCrewStatus = Config.Bind(
                "Voice Commands", // Config section
                "Register Crew Status commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerCrewStatus));
            crewStatusCommands = Config.Bind(
                "Voice Commands", // Config section
                "Crew Status commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["crewStatus"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(crewStatusCommands));
            registerCrewInShip = Config.Bind(
                "Voice Commands", // Config section
                "Register Crew in ship commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerCrewInShip));
            crewInShipCommands = Config.Bind(
                "Voice Commands", // Config section
                "Crew in ship commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["shipCrew"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(crewInShipCommands));
            registerScrapLeft = Config.Bind(
                "Voice Commands", // Config section
                "Register Scrap / items left commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerScrapLeft));
            scrapLeftCommands = Config.Bind(
                "Voice Commands", // Config section
                "Scrap / items left commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["scrap"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(scrapLeftCommands));
            registerRadarBoosters = Config.Bind(
                "Voice Commands", // Config section
                "Register Radar Booster commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerRadarBoosters));
            radarPingCommands = Config.Bind(
                "Voice Commands", // Config section
                "Radar Booster ping commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["radarBooster"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(radarPingCommands));
            radarFlashCommands = Config.Bind(
                "Voice Commands", // Config section
                "Radar Booster flash commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["radarFlash"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(radarFlashCommands));
            registerSignalTranslator = Config.Bind(
                "Voice Commands", // Config section
                "Register Signal Translator commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerSignalTranslator));
            transmitCommands = Config.Bind(
                "Voice Commands", // Config section
                "Transmit / send commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value][
                    "signalTranslator"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'.\n" +
                "IMPORTANT: Transmit / send commands will always have the message at the end!" // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(transmitCommands));
            registerTime = Config.Bind(
                "Voice Commands", // Config section
                "Register Current time of day commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerTime));
            timeCommands = Config.Bind(
                "Voice Commands", // Config section
                "Current time commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["time"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(timeCommands));
            registerInteractShipDoors = Config.Bind(
                "Voice Commands", // Config section
                "Register Ship Door commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerInteractShipDoors));
            openShipDoorsCommands = Config.Bind(
                "Voice Commands", // Config section
                "Open ship doors commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["openShip"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(openShipDoorsCommands));
            closeShipDoorsCommands = Config.Bind(
                "Voice Commands", // Config section
                "Close ship doors commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["closeShip"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(closeShipDoorsCommands));
            registerInteractShipLights = Config.Bind(
                "Voice Commands", // Config section
                "Register Ship Lights commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerInteractShipLights));
            lightsOnCommands = Config.Bind(
                "Voice Commands", // Config section
                "Ship Lights on commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["lightsOn"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(lightsOnCommands));
            lightsOffCommands = Config.Bind(
                "Voice Commands", // Config section
                "Ship Lights off commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["lightsOff"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(lightsOffCommands));
            registerInteractShipMagnet = Config.Bind(
                "Voice Commands", // Config section
                "Register Ship Magnet commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerInteractShipMagnet));
            magnetOnCommands = Config.Bind(
                "Voice Commands", // Config section
                "Activate Magnet commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["magnetOn"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(magnetOnCommands));
            magnetOffCommands = Config.Bind(
                "Voice Commands", // Config section
                "Deactivate Magnet commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["magnetOff"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(magnetOffCommands));
            // registerInteractShipShutters = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Register Ship Shutters commands", // Key of this config
            //     true, // Default value
            //     "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game.\nNote: This command only works with ShipWindows installed." // Description
            // );
            // openShuttersCommands = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Open Ship Shutters commands", // Key of this config
            //     "VEGA, open shutters/VEGA, open window shutters/VEGA, open ship shutters", // Default value
            //     "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            // );
            // closeShuttersCommands = Config.Bind(
            //     "Voice Commands", // Config section
            //     "Close Ship Shutters commands", // Key of this config
            //     "VEGA, close shutters/VEGA, close window shutters/VEGA, close ship shutters", // Default value
            //     "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            // );
            registerWeatherInfo = Config.Bind(
                "Voice Commands", // Config section
                "Register Weather info commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerWeatherInfo));
            weatherInfoCommands = Config.Bind(
                "Voice Commands", // Config section
                "Weather info commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["weatherInfo"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'.\n" +
                "IMPORTANT: Weather info commands will always have the condition's name and 'weather' at the end!" // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(weatherInfoCommands));
            registerStop = Config.Bind(
                "Voice Commands", // Config section
                "Register Stop Talking commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerStop));
            stopCommands = Config.Bind(
                "Voice Commands", // Config section
                "Stop talking commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["stopTalking"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(stopCommands));
            registerThanks = Config.Bind(
                "Voice Commands", // Config section
                "Register gratitude commands", // Key of this config
                true, // Default value
                "Disable this if you don't want these voice commands to be registered. Will apply after restarting the game." // Description
            );
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(registerThanks));
            gratitudeCommands = Config.Bind(
                "Voice Commands", // Config section
                "Gratitude commands", // Key of this config
                LanguageHelper.Commands[commandLanguage.Value]["gratitude"], // Default value
                "The voice commands that you want to get registered and picked up by VEGA. Make sure to separate different commands with a '/'." // Description
            );
            LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(gratitudeCommands));
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

        private void UpdateVoiceCommands()
        {
            startListeningCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["startListening"];
            stopListeningCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["stopListening"];
            moonsInfoCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["moonInfo"];
            // bestiaryEntriesCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["bestiaryEntries"];
            creatureInfoCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["creatureInfo"];
            activateAdvancedScannerCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["activateScanner"];
            deactivateAdvancedScannerCommands.Value =
                LanguageHelper.Commands[commandLanguage.Value]["deactivateScanner"];
            openSecureDoorCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["openDoor"];
            closeSecureDoorCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["closeDoor"];
            openAllSecureDoorsCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["openAllDoors"];
            closeAllSecureDoorsCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["closeAllDoors"];
            disableTurretCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["disableTurret"];
            disableAllTurretsCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["disableAllTurrets"];
            disableMineCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["disableMine"];
            disableAllMinesCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["disableAllMines"];
            disableSpikeTrapCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["disableTrap"];
            disableAllSpikeTrapsCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["disableAllTraps"];
            teleporterCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["teleporter"];
            radarSwitchCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["mapSwitch"];
            crewStatusCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["crewStatus"];
            crewInShipCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["shipCrew"];
            scrapLeftCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["scrap"];
            radarPingCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["radarBooster"];
            radarFlashCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["radarFlash"];
            transmitCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["signalTranslator"];
            timeCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["time"];
            openShipDoorsCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["openShip"];
            closeShipDoorsCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["closeShip"];
            lightsOnCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["lightsOn"];
            lightsOffCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["lightsOff"];
            magnetOnCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["magnetOn"];
            magnetOffCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["magnetOff"];
            weatherInfoCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["weatherInfo"];
            stopCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["stopTalking"];
            gratitudeCommands.Value = LanguageHelper.Commands[commandLanguage.Value]["gratitude"];
        }

        /*
        public static string GetPluginsPath()
        {
            return Instance.Info.Location.TrimEnd($"{modName}.dll".ToCharArray());
        }
        */
    }
}