using com.github.zehsteam.ToilHead.MonoBehaviours;
using LC_VEGA.Patches;
using MoreShipUpgrades.Misc.Upgrades;
using PySpeech;
using ShipWindows;
using ShipWindows.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LC_VEGA
{
    internal class VEGA
    {
        public static AudioSource audioSource;
        public static AudioSource sfxAudioSource;
        public static List<AudioClip> audioClips;
        public static bool listening;
        public static bool shouldBeInterrupted;
        public static bool warningGiven;
        public static bool facilityHasPower;
        public static float teleporterCooldownTime;
        public static TextMeshProUGUI entitiesTextComponent;
        public static TextMeshProUGUI itemsTextComponent;
        public static char creditsChar;

        // Default set of usernames and their respective colors in chat messages
        private static Dictionary<string, string> nameColorPairs = new Dictionary<string, string>()
        {
            { "JS0", "#b51b3e" }, // Opera-san Red
            { "Dorimon Pls", "#ff0000" }, // Red
            { "Lunxara", "#6700bd" }, // Lunxara Purple
            { "Mina", "#a11010" }, // BLOOD (FUEL)
            { "Sua", "#79e5cb" }, // Suachi Teal
            { "Nico", "#ffffff" }, // Literally just white
            { "xVenatoRx", "#ff8000" }, // McLaren Papaya
            { "Jowyck", "#00ffff" } // Cyan
        };

        internal static string[] htmlColors =
        {
            "", // Custom
            "<color=red>",
            "<color=#ff4d4d>", // Light Red
            "<color=#b30000>", // Dark Red
            "<color=#b51b3e>", // Opera-san Red
            "<color=#a11010>", // Blood
            "<color=#dc143c>", // Crimson
            "<color=blue>",
            "<color=#6666ff>", // Light Blue
            "<color=#0000b3>", // Dark Blue
            "<color=#0848ad>", // Lunxara Blue
            "<color=#00ffff>", // Cyan
            "<color=#79e5cb>", // Suachi Teal
            "<color=green>",
            "<color=#00cc00>", // Light Green
            "<color=#004d00>", // Dark Green
            "<color=#02F296>", // Lyra Green
            "<color=#00ff00>", // Lime
            "<color=yellow>",
            "<color=#ffff66>", // Light Yellow
            "<color=#cccc00>", // Dark Yellow
            "<color=#ffd700>", // Gold
            "<color=#ffa500>", // Orange
            "<color=#ffc966>", // Light Orange
            "<color=#cc8500>", // Dark Orange
            "<color=#ff8000>", // Papaya Orange
            "<color=#ffc0cb>", // Pink
            "<color=#ffe6ea>", // Light Pink
            "<color=#ff8095>", // Dark Pink
            "<color=#e20c96>", // Lunxara Pink
            "<color=#800080>", // Purple
            "<color=#b300b3>", // Light Purple
            "<color=#4d004d>", // Dark Purple
            "<color=#6700BD>", // Lunxara Purple
            "<color=#ff00ff>", // Magenta
            "<color=#ffffff>", // White
            "<color=#808080>", // Gray
            "<color=#b3b3b3>", // Light Gray
            "<color=#4d4d4d>", // Dark Gray
            "<color=#000000>" // Black
        };

        // Advanced Scanner
        public static bool advancedScannerActive;
        internal static string enemiesTopText;
        internal static string enemiesTextColor;
        internal static string itemsTopText;
        internal static string itemsTextColor;
        internal static string clearTextColor;
        internal static string dataUnavailableTextColor;
        internal static float scannerRange;
        private static string oneEntityText;
        private static string entitiesText;
        private static string oneItemText;
        private static string itemsText;

        // Turrets
        internal static bool turretsExist;
        internal static bool turretDisabled;
        internal static bool noVisibleTurret;
        // internal static bool noTurretNearby;
        internal static bool noTurrets;
        internal static float distanceToTurret;

        // Toils
        internal static bool toilDisabled;
        internal static bool noToils;
        internal static float distanceToToil;

        // Malfunctions
        internal static bool malfunctionPowerTriggered;
        internal static bool malfunctionTeleporterTriggered;
        internal static bool malfunctionDistortionTriggered;
        internal static bool malfunctionDoorTriggered;

        public static string[] signals;

        // Names of weather phenomena
        public static string[] weathers =
        {
            "Foggy",
            "Rainy",
            "Stormy",
            "Flooded",
            "Eclipsed"
        };

        // Names of the AudioClips paired with their respective IDs in the terminal
        public static Dictionary<int, string> enemies = new Dictionary<int, string>()
        {
            { 0, "SnareFlea" },
            { 1, "Bracken" },
            { 2, "Thumper" },
            { 3, "EyelessDog" },
            { 4, "YippeeBug" },
            { 5, "Slime" },
            { 6, "ForestKeeper" },
            { 7, "Coil-Head" },
            { 9, "Sandworm" },
            { 10, "Jester" },
            { 11, "SporeLizard" },
            { 12, "BunkerSpider" },
            { 13, "Manticoil" },
            { 14, "RedBees" },
            { 15, "Locusts" },
            { 16, "BaboonHawk" },
            { 17, "Nutcracker" },
            { 18, "OldBird" },
            { 19, "Butler" },
            { 20, "Hornets" },
            { 21, "Snakes" },
            { 22, "VainShroud" },
            { 23, "KidnapperFox" },
            { 24, "Barber" },
            { 25, "Maneater" }
        };

        // Creature file names
        public static string[] moddedEnemies =
        {
            "RedWood Giant",
            "Stalker",
            "DriftWood Giant",
            "Football",
            "Shy guy",
            "Locker",
            "Siren Head",
            "Rolling Giant",
            "Peepers",
            "Shockwave Drone",
            "Cleaning Drone",
            "Moving Turret",
            "Maggie",
            "Shrimp",
            "The Fiend"
        };

        // Names people may use for vanilla creatures
        private static readonly string[] vanillaEntityNames =
        {
            "Baboon Hawk",
            "Baboon",
            "Hawk",
            "Bunker Spider",
            "Spider",
            "Hoarding Bug",
            "Loot Bug",
            "Yippee Bug",
            "Barber",
            "Bracken",
            "Butler",
            "Coil Head",
            "Coil",
            "Forest Keeper",
            "Giant",
            "Keeper",
            "Eyeless Dog",
            "Dog",
            "Earth Leviathan",
            "Leviathan",
            "Worm",
            "Jester",
            "Roaming Locusts",
            "Locusts",
            "Manticoil",
            "Nutcracker",
            "Old Bird",
            "Bird",
            "Mech",
            "Circuit Bees",
            "Red Bees",
            "Bees",
            "Hygrodere",
            "Slime",
            "Blob",
            "Tulip Snake",
            "Tulip",
            "Snake",
            "Snare Flea",
            "Flea",
            "Centipede",
            "Spore Lizard",
            "Spore Doggy",
            "Lizard",
            "Thumper",
            "Crawler",
            "Halve",
            "Vain Shroud",
            "Vain",
            "Shroud",
            "Kidnapper Fox",
            "Kidnapper",
            "Fox",
            "Maneater",
            "Baby"
        };

        // Names people may use for modded creatures
        private static readonly string[] moddedEntityNames =
        {
            "Redwood Giant",
            "Redwood",
            "Driftwood Giant",
            "Driftwood",
            "Faceless Stalker",
            "Slenderman",
            "Slender",
            "Football",
            "Shy Guy",
            "SCP 096",
            "Locker",
            "Siren Head",
            "Rolling Giant",
            "Peepers",
            "Peeper",
            "Shockwave Drone",
            "Cleaning Drone",
            "Moving Turret",
            "Mobile Turret",
            "The Lost",
            "Maggie",
            "Shrimp",
            "The Fiend",
            "Fiend"
        };

        internal static string GetEntityAudioClipName(string entityName)
        {
            switch (entityName)
            {
                case "Baboon Hawk":
                case "Baboon":
                case "Hawk":
                    return "BaboonHawk";
                case "Bunker Spider":
                case "Spider":
                    return "BunkerSpider";
                case "Hoarding Bug":
                case "Loot Bug":
                case "Yippee Bug":
                    return "YippeeBug";
                case "Coil Head":
                case "Coil":
                    return "Coil-Head";
                case "Forest Keeper":
                case "Giant":
                case "Keeper":
                    return "ForestKeeper";
                case "Eyeless Dog":
                case "Dog":
                    return "EyelessDog";
                case "Earth Leviathan":
                case "Leviathan":
                case "Worm":
                    return "Sandworm";
                case "Roaming Locusts":
                    return "Locusts";
                case "Old Bird":
                case "Bird":
                case "Mech":
                    return "OldBird";
                case "Circuit Bees":
                case "Red Bees":
                case "Bees":
                    return "RedBees";
                case "Hygrodere":
                case "Blob":
                    return "Slime";
                case "Tulip Snake":
                case "Tulip":
                case "Snake":
                    return "Snakes";
                case "Snare Flea":
                case "Flea":
                case "Centipede":
                    return "SnareFlea";
                case "Spore Lizard":
                case "Spore Doggy":
                case "Lizard":
                    return "SporeLizard";
                case "Crawler":
                case "Halve":
                    return "Thumper";
                case "Vain Shroud":
                case "Vain":
                case "Shroud":
                    return "VainShroud";
                case "Kidnapper Fox":
                case "Kidnapper":
                case "Fox":
                    return "KidnapperFox";
                case "Redwood Giant":
                case "Redwood":
                    return "RedWood Giant";
                case "Driftwood Giant":
                case "Driftwood":
                    return "DriftWood Giant";
                case "Faceless Stalker":
                case "Slenderman":
                case "Slender":
                    return "Stalker";
                case "Shy Guy":
                case "SCP 096":
                    return "Shy guy";
                case "Peeper":
                    return "Peepers";
                case "Mobile Turret":
                    return "Moving Turret";
                case "The Lost":
                    return "Maggie";
                case "Fiend":
                    return "The Fiend";
                default:
                    return entityName;
            }
        }

        // Names of moons
        private static readonly string[] moonNames =
        {
            "Experimentation",
            "Vow",
            "Assurance",
            "Ass",
            "March",
            "Adamance",
            "Offense",
            "Rend",
            "Dine",
            "Titan",
            "Tit",
            "Artifice",
            "Embrion",
            "Mars",
            "Liquidation"
        };

        internal static string GetMoonAudioClipName(string moonName)
        {
            switch (moonName)
            {
                case "Experimentation":
                    return "41-EXP";
                case "Vow":
                    return "56-VOW";
                case "Ass":
                case "Assurance":
                    return "220-ASS";
                case "March":
                    return "61-MAR";
                case "Adamance":
                    return "20-ADA";
                case "Offense":
                    return "21-OFF";
                case "Rend":
                    return "85-REN";
                case "Dine":
                    return "7-DIN";
                case "Tit":
                case "Titan":
                    return "8-TIT";
                case "Artifice":
                    return "68-ART";
                case "Embrion":
                    return "5-EMB";
                case "Liquidation":
                    return "44-LIQ";
                case "Mars":
                    return "4-MARS";
                default:
                    return moonName;
            }
        }

        public static void PlayLine(string clipName, float delay = 0.25f, bool checkPlayerStatus = true, bool skip = false)
        {
            if (audioSource != null)
            {
                if (StartOfRound.Instance.localPlayerController == null) return;
                if (checkPlayerStatus && StartOfRound.Instance.localPlayerController.isPlayerDead) return;
                if (!skip && audioSource.isPlaying) return;

                foreach (var clip in audioClips)
                {
                    if (clip.name.Equals(clipName))
                    {
                        audioSource.clip = clip;
                    }
                }

                if (audioSource.clip != null) Plugin.LogToConsole("Playing " + audioSource.clip.name + " with a " + delay + " second delay");
                audioSource.PlayDelayed(delay);
            }
            else
            {
                Plugin.LogToConsole("Unable to play audio. The audio source for VEGA does not exist", "warn");
            }
        }

        public static void PlaySFX(string clipName, float delay = 0.25f, bool checkPlayerStatus = true, bool skip = false)
        {
            if (sfxAudioSource != null)
            {
                if (checkPlayerStatus && StartOfRound.Instance.localPlayerController.isPlayerDead) return;
                if (!skip && sfxAudioSource.isPlaying) return;

                foreach (var clip in audioClips)
                {
                    if (clip.name.Equals(clipName))
                    {
                        sfxAudioSource.clip = clip;
                    }
                }

                if (sfxAudioSource.clip != null) Plugin.LogToConsole("Playing " + sfxAudioSource.clip.name + " with a " + delay + " second delay");
                sfxAudioSource.PlayDelayed(delay);
            }
            else
            {
                Plugin.LogToConsole("Unable to play SFX. The SFX audio source for VEGA does not exist", "warn");
            }
        }

        public static void PlayRandomLine(string clipName, int range, float delay = 0.25f, bool checkPlayerStatus = true, bool skip = false)
        {
            clipName += "-" + range;
            PlayLine(clipName, delay, checkPlayerStatus, skip);
        }

        internal static bool ClientHasMoon(string moonName)
        {
            foreach (var level in StartOfRound.Instance.levels)
            {
                if (level.PlanetName.Equals(moonName))
                {
                    return true;
                }
            }
            PlayRandomLine("NoInfoOnMoon", Random.Range(1, 4));
            return false;
        }

        internal static void OpenSecureDoor()
        {
            TerminalAccessibleObject? closestDoor = GetClosestSecureDoor();
            if (!facilityHasPower)
            {
                PlayRandomLine("NoPower", Random.Range(1, 4));
                return;
            }
            if (closestDoor != null)
            {
                if (Vector3.Distance(closestDoor.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 11f)
                {
                    Plugin.LogToConsole("Opening door", "debug");
                    closestDoor.SetDoorLocalClient(true);
                    if (Plugin.vocalLevel.Value >= VocalLevels.High)
                    {
                        PlayRandomLine("DoorOpened", Random.Range(1, 4), 0.7f);
                    }
                }
                else
                {
                    if (Plugin.vocalLevel.Value >= VocalLevels.Medium) PlayLine("NoDoorNearby");
                    PlayLine("NoDoorNearbyLow");
                }
            }
        }

        internal static void OpenAllDoors()
        {
            bool doorsExist = false;
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.isBigDoor)
                {
                    item.SetDoorLocalClient(true);
                    doorsExist = true;
                }
            }
            if (doorsExist)
            {
                if (!facilityHasPower)
                {
                    PlayRandomLine("NoPower", Random.Range(1, 4));
                    return;
                }
                Plugin.LogToConsole("Opening all doors", "debug");
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayRandomLine("AllDoorsOpened", Random.Range(1, 3), 0.7f);
                }
            }
            else
            {
                PlayLine("NoDoors");
            }
        }

        internal static void CloseSecureDoor()
        {
            TerminalAccessibleObject? closestDoor = GetClosestSecureDoor();
            if (!facilityHasPower)
            {
                PlayRandomLine("NoPower", Random.Range(1, 4));
                return;
            }
            if (closestDoor != null)
            {
                if (Vector3.Distance(closestDoor.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 11f)
                {
                    Plugin.LogToConsole("Closing door", "debug");
                    closestDoor.SetDoorLocalClient(false);
                    if (Plugin.vocalLevel.Value >= VocalLevels.High)
                    {
                        PlayRandomLine("DoorClosed", Random.Range(1, 4), 0.7f);
                    }
                }
                else
                {
                    PlayLine("NoDoorNearby");
                }
            }
        }

        internal static void CloseAllDoors()
        {
            bool doorsExist = false;
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.isBigDoor)
                {
                    item.SetDoorLocalClient(false);
                    doorsExist = true;
                }
            }
            if (doorsExist)
            {
                if (!facilityHasPower)
                {
                    PlayRandomLine("NoPower", Random.Range(1, 4));
                    return;
                }
                Plugin.LogToConsole("Closing all doors", "debug");
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayRandomLine("AllDoorsClosed", Random.Range(1, 3), 0.7f);
                }
            }
            else
            {
                PlayLine("NoDoors");
            }
        }

        internal static TerminalAccessibleObject? GetClosestSecureDoor()
        {
            Plugin.LogToConsole("Getting closest secure door", "debug");

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            List<TerminalAccessibleObject> secureDoors = new List<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.isBigDoor)
                {
                    secureDoors.Add(item);
                }
            }

            if (secureDoors.Count == 0)
            {
                PlayLine("NoDoors");
                return null;
            }

            TerminalAccessibleObject closestDoor = secureDoors[0];

            List<float> distances = new List<float>();
            float distanceToPlayer;
            foreach (var door in secureDoors)
            {
                distanceToPlayer = Vector3.Distance(door.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                // Plugin.LogToConsole("DOOR DIST. TO PLAYER -> " + distanceToPlayer);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min())
                {
                    closestDoor = door;
                }
            }
            return closestDoor;
        }

        internal static TerminalAccessibleObject? GetClosestTurret()
        {
            Plugin.LogToConsole("Getting closest turret", "debug");
            List<TerminalAccessibleObject> turrets = new List<TerminalAccessibleObject>();

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Turret>())
                {
                    turrets.Add(item);
                }
            }

            if (turrets.Count == 0)
            {
                noTurrets = true;
                return null;
            }

            TerminalAccessibleObject closestTurret = turrets[0];

            List<float> distances = new List<float>();
            float distanceToPlayer;
            foreach (var turret in turrets)
            {
                distanceToPlayer = Vector3.Distance(turret.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min() && StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(turret.transform.position, 45, 10000))
                {
                    closestTurret = turret;
                }
                else if (distances.Count > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count == 0)
            {
                noVisibleTurret = true;
                return null;
            }

            distanceToTurret = distances.Min();
            return closestTurret;
        }

        internal static void DisableVisibleTurrets()
        {
            List<TerminalAccessibleObject> turrets = new List<TerminalAccessibleObject>();
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Turret>())
                {
                    turrets.Add(item);
                }
            }

            if (turrets.Count == 0)
            {
                noTurrets = true;
                return;
            }

            List<TerminalAccessibleObject> visibleTurrets = new List<TerminalAccessibleObject>();
            foreach (var turret in turrets)
            {
                if (StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(turret.transform.position, 45, 10000))
                {
                    visibleTurrets.Add(turret);
                    turret.CallFunctionFromTerminal();
                    turretDisabled = true;
                }
            }

            if (visibleTurrets.Count == 0)
            {
                noVisibleTurret = true;
            }
        }

        internal static FollowTerminalAccessibleObjectBehaviour? GetClosestToil()
        {
            Plugin.LogToConsole("Getting closest toil", "debug");
            List<FollowTerminalAccessibleObjectBehaviour> toils = new List<FollowTerminalAccessibleObjectBehaviour>();

            FollowTerminalAccessibleObjectBehaviour[] toilHeads = Object.FindObjectsOfType<FollowTerminalAccessibleObjectBehaviour>();
            foreach (var item in toilHeads)
            {
                toils.Add(item);
            }

            if (toils.Count == 0)
            {
                noToils = true;
                return null;
            }

            FollowTerminalAccessibleObjectBehaviour closestToil = toils[0];

            List<float> distances = new List<float>();
            float distanceToPlayer;
            foreach (var toil in toils)
            {
                distanceToPlayer = Vector3.Distance(toil.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min() && StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(toil.transform.position, 45, 10000))
                {
                    closestToil = toil;
                }
                else if (distances.Count > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count == 0)
            {
                noVisibleTurret = true;
                return null;
            }

            distanceToToil = distances.Min();
            return closestToil;
        }

        internal static void DisableVisibleToils()
        {
            List<FollowTerminalAccessibleObjectBehaviour> toils = new List<FollowTerminalAccessibleObjectBehaviour>();
            FollowTerminalAccessibleObjectBehaviour[] toilHeads = Object.FindObjectsOfType<FollowTerminalAccessibleObjectBehaviour>();
            foreach (var item in toilHeads)
            {
                toils.Add(item);
            }

            if (toils.Count == 0)
            {
                noToils = true;
                return;
            }

            List<FollowTerminalAccessibleObjectBehaviour> visibleToils = new List<FollowTerminalAccessibleObjectBehaviour>();
            foreach (var toil in toils)
            {
                if (StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(toil.transform.position, 45, 10000))
                {
                    visibleToils.Add(toil);
                    toil.CallFunctionFromTerminal();
                    toilDisabled = true;
                }
            }

            if (visibleToils.Count == 0)
            {
                noVisibleTurret = true;
            }
        }

        internal static void DisableTurret()
        {
            if (Plugin.enhancedHazardDisabling.Value)
            {
                DisableVisibleTurrets();
                return;
            }
            TerminalAccessibleObject? closestTurret = GetClosestTurret();
            if (closestTurret != null)
            {
                Plugin.LogToConsole("Disabling turret", "debug");
                closestTurret.CallFunctionFromTerminal();
                turretDisabled = true;
            }
        }

        internal static void DisableToil()
        {
            if (Plugin.enhancedHazardDisabling.Value)
            {
                DisableVisibleToils();
                return;
            }
            FollowTerminalAccessibleObjectBehaviour? closestToil = GetClosestToil();
            TerminalAccessibleObject? closestTurret = GetClosestTurret();
            if (closestToil == null)
            {
                toilDisabled = false;
                return;
            }
            if (closestTurret == null)
            {
                distanceToTurret = distanceToToil + 10;
            }
            if (distanceToToil < distanceToTurret)
            {
                Plugin.LogToConsole("Disabling toil turret", "debug");
                closestToil.CallFunctionFromTerminal();
                toilDisabled = true;
                turretDisabled = true;
            }
            else
            {
                toilDisabled = false;
            }
        }

        internal static void PlayTurretAudio()
        {
            if (turretDisabled)
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayRandomLine(Plugin.enhancedHazardDisabling.Value ? "TurretsDisabled" : "TurretDisabled", Random.Range(1, 4), 0.7f);
                }
            }
            else if (noVisibleTurret)
            {
                PlayLine("NoVisibleTurret");
            }
            else if (noTurrets && noToils)
            {
                PlayLine("NoTurrets");
            }
        }

        internal static void DisableAllTurrets()
        {
            Plugin.LogToConsole("Disabling all turrets", "debug");
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Turret>())
                {
                    item.CallFunctionFromTerminal();
                    turretsExist = true;
                }
            }
            if (turretsExist)
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayRandomLine("AllTurretsDisabled", Random.Range(1, 4), 0.7f);
                }
            }
            else
            {
                PlayLine("NoTurrets");
            }
        }

        internal static void DisableAllToils()
        {
            Plugin.LogToConsole("Disabling all toils", "debug");
            FollowTerminalAccessibleObjectBehaviour[] toilHeads = Object.FindObjectsOfType<FollowTerminalAccessibleObjectBehaviour>();
            foreach (var item in toilHeads)
            {
                turretsExist = true;
                item.CallFunctionFromTerminal();
            }
        }

        internal static TerminalAccessibleObject? GetClosestMine()
        {
            Plugin.LogToConsole("Getting closest landmine", "debug");

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            List<TerminalAccessibleObject> mines = new List<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Landmine>())
                {
                    mines.Add(item);
                }
            }

            if (mines.Count == 0)
            {
                PlayLine("NoMines");
                return null;
            }

            TerminalAccessibleObject closestMine = mines[0];

            List<float> distances = new List<float>();
            float distanceToPlayer;
            foreach (var mine in mines)
            {
                distanceToPlayer = Vector3.Distance(mine.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min() && StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(mine.transform.position, 45, 10000))
                {
                    closestMine = mine;
                }
                else if (distances.Count > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count == 0)
            {
                PlayLine("NoVisibleMine");
                return null;
            }

            return closestMine;
        }

        internal static void DisableVisibleMines()
        {
            List<TerminalAccessibleObject> mines = new List<TerminalAccessibleObject>();
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Landmine>())
                {
                    mines.Add(item);
                }
            }

            if (mines.Count == 0)
            {
                PlayLine("NoMines");
                return;
            }

            List<TerminalAccessibleObject> visibleMines = new List<TerminalAccessibleObject>();
            foreach (var mine in mines)
            {
                if (StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(mine.transform.position, 45, 10000))
                {
                    visibleMines.Add(mine);
                    mine.CallFunctionFromTerminal();
                }
            }

            if (visibleMines.Count == 0)
            {
                PlayLine("NoVisibleMine");
                return;
            }

            if (Plugin.vocalLevel.Value >= VocalLevels.High)
            {
                PlayRandomLine("MinesDisabled", Random.Range(1, 4));
            }
        }

        internal static void DisableMine()
        {
            if (Plugin.enhancedHazardDisabling.Value)
            {
                DisableVisibleMines();
                return;
            }
            TerminalAccessibleObject? closestMine = GetClosestMine();
            if (closestMine != null)
            {
                Plugin.LogToConsole("Disabling landmine", "debug");
                closestMine.CallFunctionFromTerminal();
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayRandomLine("MineDisabled", Random.Range(1, 4));
                }
            }
        }

        internal static void DisableAllMines()
        {
            bool minesExist = false;
            Plugin.LogToConsole("Disabling all landmines", "debug");
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Landmine>())
                {
                    item.CallFunctionFromTerminal();
                    minesExist = true;
                }
            }
            if (minesExist)
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                {
                    PlayRandomLine("AllMinesDisabled", Random.Range(1, 4));
                }
            }
            else
            {
                PlayLine("NoMines");
            }
        }

        internal static TerminalAccessibleObject? GetClosestSpikeTrap()
        {
            Plugin.LogToConsole("Getting closest spike trap", "debug");

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            List<TerminalAccessibleObject> traps = new List<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.transform.parent != null)
                {
                    if (item.gameObject.transform.parent.name.Equals("Container"))
                    {
                        traps.Add(item);
                    }
                }
            }

            if (traps.Count == 0)
            {
                PlayLine("NoTraps");
                return null;
            }

            TerminalAccessibleObject closestTrap = traps[0];

            List<float> distances = new List<float>();
            float distanceToPlayer;
            foreach (var trap in traps)
            {
                distanceToPlayer = Vector3.Distance(trap.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min() && StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(trap.transform.position, 45, 10000))
                {
                    closestTrap = trap;
                }
                else if (distances.Count > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count == 0)
            {
                PlayLine("NoVisibleTrap");
                return null;
            }

            return closestTrap;
        }

        internal static void DisableVisibleSpikeTraps()
        {
            List<TerminalAccessibleObject> traps = new List<TerminalAccessibleObject>();
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.transform.parent != null)
                {
                    if (item.gameObject.transform.parent.name.Equals("Container"))
                    {
                        traps.Add(item);
                    }
                }
            }

            if (traps.Count == 0)
            {
                PlayLine("NoTraps");
                return;
            }

            List<TerminalAccessibleObject> visibleTraps = new List<TerminalAccessibleObject>();
            foreach (var trap in traps)
            {
                if (StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(trap.transform.position, 45, 10000))
                {
                    visibleTraps.Add(trap);
                    trap.CallFunctionFromTerminal();
                }
            }

            if (visibleTraps.Count == 0)
            {
                PlayLine("NoVisibleTrap");
                return;
            }

            if (Plugin.vocalLevel.Value >= VocalLevels.High)
            {
                PlayRandomLine("TrapsDisabled", Random.Range(1, 4));
            }
        }

        internal static void DisableSpikeTrap()
        {
            if (Plugin.enhancedHazardDisabling.Value)
            {
                DisableVisibleSpikeTraps();
                return;
            }
            TerminalAccessibleObject? closestTrap = GetClosestSpikeTrap();
            if (closestTrap != null)
            {
                Plugin.LogToConsole("Disabling spike trap", "debug");
                closestTrap.CallFunctionFromTerminal();
                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                {
                    PlayRandomLine("TrapDisabled", Random.Range(1, 4));
                }
            }
        }

        internal static void DisableAllSpikeTraps()
        {
            bool trapsExist = false;
            Plugin.LogToConsole("Disabling all spike traps", "debug");
            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.transform.parent != null)
                {
                    if (item.gameObject.transform.parent.name.Equals("Container"))
                    {
                        item.CallFunctionFromTerminal();
                        trapsExist = true;
                    }
                }
            }
            if (trapsExist)
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                {
                    PlayRandomLine("AllTrapsDisabled", Random.Range(1, 4));
                }
            }
            else
            {
                PlayLine("NoTraps");
            }
        }

        internal static IEnumerator ActivateTeleporter()
        {
            if (GameObject.Find("Teleporter(Clone)"))
            {
                ShipTeleporter teleporter = GameObject.Find("Teleporter(Clone)").GetComponent<ShipTeleporter>();
                if (teleporterCooldownTime <= 0f)
                {
                    if (Plugin.vocalLevel.Value >= VocalLevels.High)
                    {
                        PlayRandomLine("Teleport", Random.Range(1, 4));
                    }
                    if (Plugin.enhancedTeleportCommands.Value && StartOfRound.Instance.mapScreen.targetedPlayer != StartOfRound.Instance.localPlayerController)
                    {
                        SwitchRadar(checkMalfunctions: false, checkCurrentTarget: false);
                        yield return new WaitForSeconds(1f);
                    }
                    teleporter.PressTeleportButtonServerRpc();
                    if (Plugin.sendTeleporterChatMessage.Value) SendChatMessage("has activated the teleporter");
                }
                else
                {
                    PlayLine("TeleporterOnCooldown");
                    Plugin.LogToConsole("The teleporter is on cooldown!", "warn");
                }
            }
            else
            {
                PlayRandomLine("NoTeleporter", Random.Range(1, 4));
                // Plugin.LogToConsole("You might need a teleporter for that", "warn");
            }
        }

        internal static void SwitchRadar(bool checkMalfunctions = true, bool checkCurrentTarget = true)
        {
            if (ModChecker.hasMalfunctions && checkMalfunctions)
            {
                if (malfunctionPowerTriggered)
                {
                    PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                    return;
                }
                if (malfunctionDistortionTriggered)
                {
                    PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                    return;
                }
            }

            if (checkCurrentTarget)
            {
                if (StartOfRound.Instance.mapScreen.targetedPlayer == StartOfRound.Instance.localPlayerController)
                {
                    PlayLine("RadarAlreadyFocused");
                    return;
                }
            }

            int index = StartOfRound.Instance.mapScreen.radarTargets.FindIndex(target => target.transform == StartOfRound.Instance.localPlayerController.transform);
            StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(index);
            if (Plugin.sendRadarSwitchChatMessage.Value) SendChatMessage("performed a radar switch");
        }

        internal static IEnumerator SwitchLights(bool on)
        {
            ShipLights shipLights = Object.FindObjectOfType<ShipLights>();
            if (shipLights != null)
            {
                if (on)
                {
                    if (shipLights.areLightsOn)
                    {
                        PlayLine("LightsAlreadyOn");
                    }
                    else
                    {
                        PlayLine("LightsOn");
                    }
                }
                else
                {
                    if (!shipLights.areLightsOn)
                    {
                        PlayLine("LightsAlreadyOff");
                    }
                    else
                    {
                        PlayLine("LightsOff");
                    }
                }

                yield return new WaitForSeconds(2f);

                shipLights.SetShipLightsServerRpc(on);
            }
        }

        public static void PerformAdvancedScan()
        {
            if (advancedScannerActive)
            {
                if (ModChecker.hasMalfunctions)
                {
                    if (malfunctionDistortionTriggered || malfunctionPowerTriggered)
                    {
                        entitiesTextComponent.SetText(enemiesTopText + dataUnavailableTextColor + "Data unavailable</color>");
                        itemsTextComponent.SetText(itemsTopText + dataUnavailableTextColor + "Data unavailable</color>");
                        return;
                    }
                }

                Vector3 playerPosition = StartOfRound.Instance.localPlayerController.transform.position;

                int enemyCount = 0;
                int itemCount = 0;
                float totalWorth = 0;
                Collider[] colliders = Physics.OverlapSphere(playerPosition, scannerRange);
                foreach (var collider in colliders)
                {
                    GrabbableObject item = collider.GetComponent<GrabbableObject>();
                    if (item != null)
                    {
                        if (item.itemProperties.isScrap && !item.isHeld)
                        {
                            itemCount++;
                            totalWorth += item.scrapValue;
                        }
                    }
                    else
                    {
                        ScanNodeProperties enemyScanNode = collider.GetComponent<ScanNodeProperties>();
                        if (enemyScanNode != null && enemyScanNode.creatureScanID != -1)
                        {
                            if (enemyScanNode.creatureScanID == 18) // Old birds
                            {
                                enemyCount++;
                            }
                            else
                            {
                                EnemyAI enemy = enemyScanNode.transform.parent.GetComponent<EnemyAI>();
                                if (enemy == null)
                                {
                                    EnemyAICollisionDetect collisionDetect = enemyScanNode.transform.parent.GetComponent<EnemyAICollisionDetect>();
                                    enemy = collisionDetect?.mainScript;
                                }
                                if (enemy != null && !enemy.isEnemyDead && enemy.GetType() != typeof(DocileLocustBeesAI) && enemy.GetType() != typeof(DoublewingAI) && enemy.GetType() != typeof(SandWormAI))
                                {
                                    enemyCount++;
                                }
                            }
                        }
                        else if (Plugin.detectMasked.Value && collider.GetComponent<MaskedPlayerEnemy>()?.isEnemyDead == false)
                        {
                            enemyCount++;
                        }
                    }
                }

                switch (enemyCount)
                {
                    case 0:
                        entitiesTextComponent.SetText(enemiesTopText + clearTextColor + "Clear</color>");
                        break;
                    case 1:
                        entitiesTextComponent.SetText(enemiesTopText + enemiesTextColor + oneEntityText);
                        break;
                    default:
                        entitiesTextComponent.SetText(enemiesTopText + enemiesTextColor + enemyCount + entitiesText);
                        break;
                }

                switch (itemCount)
                {
                    case 0:
                        itemsTextComponent.SetText(itemsTopText + clearTextColor + "Clear</color>");
                        break;
                    case 1:
                        itemsTextComponent.SetText(itemsTopText + itemsTextColor + oneItemText + creditsChar + totalWorth + "</color>");
                        break;
                    default:
                        itemsTextComponent.SetText(itemsTopText + itemsTextColor + itemCount + itemsText + creditsChar + totalWorth + "</color>");
                        break;
                }
            }
        }

        internal static void GetDayMode()
        {
            switch (TimeOfDay.Instance.dayMode)
            {
                case DayMode.None:
                    PlayLine("None");
                    Plugin.LogToConsole("No day mode found", "warn");
                    break;
                case DayMode.Dawn:
                    PlayLine("Dawn");
                    break;
                case DayMode.Noon:
                    PlayLine("Noon");
                    break;
                case DayMode.Sundown:
                    PlayLine("Sundown");
                    break;
                case DayMode.Midnight:
                    PlayLine("Midnight");
                    break;
                default:
                    PlayLine("None");
                    Plugin.LogToConsole("No day mode found", "warn");
                    break;
            }
        }

        internal static RadarBoosterItem? GetClosestRadarBooster()
        {
            RadarBoosterItem[] boosters = Object.FindObjectsOfType<RadarBoosterItem>();

            if (boosters.Count() == 0)
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.Medium) PlayLine("NoBoosters");
                PlayLine("NoBoostersLow");
                return null;
            }

            RadarBoosterItem bobbie = boosters[0];
            List<float> distances = new List<float>();
            float distanceToPlayer;
            foreach (var item in boosters)
            {
                distanceToPlayer = Vector3.Distance(item.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min())
                {
                    bobbie = item;
                }
            }
            return bobbie;
        }

        internal static void InteractWithBooster(bool ping)
        {
            RadarBoosterItem? closestBooster = GetClosestRadarBooster();
            if (closestBooster != null)
            {
                if (closestBooster.radarEnabled)
                {
                    if (Vector3.Distance(closestBooster.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 16f)
                    {
                        int index = StartOfRound.Instance.mapScreen.radarTargets.FindIndex(target => target.transform == closestBooster.transform);
                        if (ping)
                        {
                            Plugin.LogToConsole("Pinging " + closestBooster.radarBoosterName);
                            StartOfRound.Instance.mapScreen.PingRadarBooster(index);
                            return;
                        }
                        Plugin.LogToConsole("Flashing " + closestBooster.radarBoosterName);
                        StartOfRound.Instance.mapScreen.FlashRadarBooster(index);
                    }
                    else
                    {
                        if (Plugin.vocalLevel.Value >= VocalLevels.Medium) PlayLine("NoBoostersNearby");
                        PlayLine("NoBoostersNearbyLow");
                    }
                }
            }
        }

        internal static IEnumerator GetCrewStatus(float delay = 5f)
        {
            int deadPlayers = 0;
            foreach (var player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerDead)
                {
                    deadPlayers++;
                }
            }

            if (StartOfRound.Instance.livingPlayers == 1 && deadPlayers == 0)
            {
                PlayLine("CrewStatusSolo");
            }
            else
            {
                PlayLine("GettingCrewStatus");

                yield return new WaitForSeconds(delay);

                string header = "CREW STATUS REPORT:";
                string livingPlayers = StartOfRound.Instance.livingPlayers + " employees alive\n";
                string deceasedPlayers = deadPlayers + " deceased employees";
                if (deadPlayers == 1)
                {
                    deceasedPlayers = "1 deceased employee\n";
                }
                if (StartOfRound.Instance.livingPlayers == 1)
                {
                    PlayLine("GoodLuck", 2.5f);
                    livingPlayers = "1 employee alive\n";
                    HUDManager.Instance.DisplayTip(header, livingPlayers + deceasedPlayers, isWarning: true);
                }
                else
                {
                    if (Plugin.vocalLevel.Value >= VocalLevels.High)
                    {
                        PlayRandomLine("ReportComplete", Random.Range(1, 4));
                    }
                    HUDManager.Instance.DisplayTip(header, livingPlayers + deceasedPlayers);
                }
            }
        }

        internal static IEnumerator GetCrewInShip(float delay = 5f)
        {
            int deadPlayers = 0;
            List<string> playersInShip = new List<string>();
            foreach (var player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isInHangarShipRoom)
                {
                    playersInShip.Add(player.playerUsername);
                }
                if (player.isPlayerDead)
                {
                    deadPlayers++;
                }
            }

            if (StartOfRound.Instance.livingPlayers == 1 && deadPlayers == 0)
            {
                if (playersInShip.Count > 0)
                {
                    PlayLine("CrewInShipSolo");
                }
                else
                {
                    PlayLine("CrewOutsideShipSolo");
                }
            }
            else
            {
                PlayLine("GettingCrewInShip");

                yield return new WaitForSeconds(delay);

                string header = "CREWMATES IN THE SHIP:";
                string body = "";
                if (playersInShip.Count == 0)
                {
                    body = "No one is in the ship.";
                }
                else
                {
                    foreach (var playerName in playersInShip)
                    {
                        if (playersInShip.Last() == playerName)
                        {
                            body += playerName;
                        }
                        else
                        {
                            body += playerName + ", ";
                        }
                    }
                }

                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayRandomLine("ReportComplete", Random.Range(2, 4));
                }
                HUDManager.Instance.DisplayTip(header, "<size=15>" + body + "</size>");
            }
        }

        internal static IEnumerator GetScrapLeft(string message, float delay = 5f)
        {
            int scrapLeft = 0;
            int scrapInShip = 0;
            int creditsLeft = 0;
            int creditsInShip = 0;

            if (message.ToLower().Contains("item")) PlayLine("PerformingItemScan");
            PlayLine("PerformingScrapScan");

            yield return new WaitForSeconds(delay);

            string header = "SCRAP SCAN RESULTS:";
            string scrapOutsideStr;
            string scrapInShipStr;
            GrabbableObject[] items = Object.FindObjectsOfType<GrabbableObject>();

            foreach (var item in items)
            {
                if (item.itemProperties.isScrap && !item.isHeld)
                {
                    if (!item.isInShipRoom)
                    {
                        scrapLeft++;
                        creditsLeft += item.scrapValue;
                    }
                    else
                    {
                        scrapInShip++;
                        creditsInShip += item.scrapValue;
                    }
                }
            }

            if (scrapLeft == 0)
            {
                scrapOutsideStr = "There are no items outside the ship.";
            }
            else
            {
                if (scrapLeft > 1)
                {
                    scrapOutsideStr = "There are " + scrapLeft + " items left, worth " + creditsChar + creditsLeft;
                }
                else
                {
                    scrapOutsideStr = "There is 1 item left, worth " + creditsChar + creditsLeft;
                }
            }

            if (scrapInShip == 0)
            {
                scrapInShipStr = "There are no items inside the ship.";
            }
            else
            {
                if (scrapInShip > 1)
                {
                    scrapInShipStr = "The ship currently has " + scrapInShip + " items, worth " + creditsChar + creditsInShip;
                }
                else
                {
                    scrapInShipStr = "The ship currently has 1 item, worth " + creditsChar + creditsInShip;
                }
            }

            if (Plugin.vocalLevel.Value >= VocalLevels.High)
            {
                PlayRandomLine("ScrapScanComplete", Random.Range(1, 4));
            }
            HUDManager.Instance.DisplayTip(header, "<size=15>" + scrapOutsideStr + "\n" + scrapInShipStr + "</size>");
        }

        internal static IEnumerator SwitchWindowShutters(bool open)
        {
            ShipWindowShutterSwitch windowSwitch = Object.FindObjectOfType<ShipWindowShutterSwitch>();
            if (windowSwitch != null && WindowConfig.enableShutter.Value)
            {
                if (open)
                {
                    PlayLine("OpenShipShutters");
                }
                else
                {
                    PlayLine("CloseShipShutters");
                }

                yield return new WaitForSeconds(2f);

                Type shipWindowsNetworkHandlerType = Type.GetType("ShipWindows.Networking.NetworkHandler, ShipWindows");
                if (shipWindowsNetworkHandlerType != null)
                {
                    MethodInfo windowSwitchUsedMethodInfo = shipWindowsNetworkHandlerType.GetMethod("WindowSwitchUsed");
                    object[] parameters = new object[] { open };
                    windowSwitchUsedMethodInfo.Invoke(null, parameters);
                }
            }
        }

        internal static void AddNameColors()
        {
            string[] players = Plugin.playerNameColors.Value.Split(", ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in players)
            {
                string[] strings = item.Split(": ");
                string username = strings[0];
                string hex = strings[1];
                if (nameColorPairs.ContainsKey(username))
                {
                    nameColorPairs[username] = hex;
                }
                else
                {
                    nameColorPairs.Add(username, hex);
                }
            }
        }

        internal static void RestoreNameColors()
        {
            nameColorPairs = new Dictionary<string, string>()
            {
                { "JS0", "#b51b3e" }, // Opera-san Red
                { "Dorimon Pls", "#ff0000" }, // Red
                { "Lunxara", "#6700bd" }, // Lunxara Purple
                { "Mina", "#a11010" }, // BLOOD (FUEL)
                { "Sua", "#79e5cb" }, // Suachi Teal
                { "Nico", "#ffffff" }, // Literally just white
                { "xVenatoRx", "#ff8000" }, // McLaren Papaya
                { "Jowyck", "#00ffff" } // Cyan
            };
        }

        private static string GetNameColor(string name)
        {
            return nameColorPairs.ContainsKey(name) ? nameColorPairs[name] : "";
        }

        public static void SendChatMessage(string message)
        {
            string playerUsername = StartOfRound.Instance.localPlayerController.playerUsername;
            string nameColor = GetNameColor(playerUsername);
            nameColor = StringHasHexCode(nameColor) ? $"<color={nameColor}>" : "";
            string nameColorClose = nameColor != "" ? "</color>" : "";
            HUDManager.Instance.AddTextToChatOnServer($"{nameColor}{playerUsername}{nameColorClose} {message}");
        }

        private static bool StringHasHexCode(string color)
        {
            if (color.Length == 7 && color.Contains("#")) return true;
            return false;
        }

        private static string SetCustomScannerColor(int length)
        {
            string[] colorCodes = Plugin.customColorCodes.Value.Split(", ");
            if (colorCodes.Length >= length)
            {
                if (StringHasHexCode(colorCodes[length - 1]))
                {
                    return $"<color={colorCodes[length - 1]}>";
                }
            }
            return "";
        }

        public static void ResetMalfunctionValues()
        {
            Plugin.LogToConsole("Resetting malfunction values", "debug");

            malfunctionPowerTriggered = false;
            malfunctionTeleporterTriggered = false;
            malfunctionDistortionTriggered = false;
            malfunctionDoorTriggered = false;

            MalfunctionsPatches.playPowerWarning = true;
            MalfunctionsPatches.playTpWarning = true;
            MalfunctionsPatches.playCommsWarning = true;
            MalfunctionsPatches.playDoorWarning = true;
            MalfunctionsPatches.playLeverWarning = true;
        }

        public static void UpdateScannerDisplayMode()
        {
            oneEntityText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? "1 entity nearby</color>" : "1 nearby</color>";
            entitiesText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? " entities nearby</color>" : " nearby</color>";
            oneItemText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? "1 item nearby, worth " : "1 nearby, ";
            itemsText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? " items nearby, worth " : " nearby, ";
            enemiesTopText = Plugin.infoDisplayMode.Value == DisplayModes.OneLine || Plugin.infoDisplayMode.Value == DisplayModes.CompactOneLine ? "Entities: " : "Entities:\n";
            itemsTopText = Plugin.infoDisplayMode.Value == DisplayModes.OneLine || Plugin.infoDisplayMode.Value == DisplayModes.CompactOneLine ? "Items: " : "Items:\n";
        }

        public static void UpdateScannerColors()
        {
            clearTextColor = Plugin.clearTextColor.Value == Colors.Custom ? SetCustomScannerColor(1) : htmlColors[(int)Plugin.clearTextColor.Value];
            dataUnavailableTextColor = Plugin.dataUnavailableTextColor.Value == Colors.Custom ? SetCustomScannerColor(4) : htmlColors[(int)Plugin.dataUnavailableTextColor.Value];
            enemiesTextColor = Plugin.entitiesNearbyTextColor.Value == Colors.Custom ? SetCustomScannerColor(2) : htmlColors[(int)Plugin.entitiesNearbyTextColor.Value];
            itemsTextColor = Plugin.itemsNearbyTextColor.Value == Colors.Custom ? SetCustomScannerColor(3) : htmlColors[(int)Plugin.itemsNearbyTextColor.Value];
        }

        internal static void InitializeScannerVariables()
        {
            advancedScannerActive = Plugin.enableAdvancedScannerAuto.Value;
            clearTextColor = Plugin.clearTextColor.Value == Colors.Custom ? SetCustomScannerColor(1) : htmlColors[(int)Plugin.clearTextColor.Value];
            dataUnavailableTextColor = Plugin.dataUnavailableTextColor.Value == Colors.Custom ? SetCustomScannerColor(4) : htmlColors[(int)Plugin.dataUnavailableTextColor.Value];
            enemiesTopText = Plugin.infoDisplayMode.Value == DisplayModes.OneLine || Plugin.infoDisplayMode.Value == DisplayModes.CompactOneLine ? "Entities: " : "Entities:\n";
            oneEntityText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? "1 entity nearby</color>" : "1 nearby</color>";
            entitiesText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? " entities nearby</color>" : " nearby</color>";
            enemiesTextColor = Plugin.entitiesNearbyTextColor.Value == Colors.Custom ? SetCustomScannerColor(2) : htmlColors[(int)Plugin.entitiesNearbyTextColor.Value];
            itemsTopText = Plugin.infoDisplayMode.Value == DisplayModes.OneLine || Plugin.infoDisplayMode.Value == DisplayModes.CompactOneLine ? "Items: " : "Items:\n";
            oneItemText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? "1 item nearby, worth " : "1 nearby, ";
            itemsText = Plugin.infoDisplayMode.Value == DisplayModes.Default || Plugin.infoDisplayMode.Value == DisplayModes.OneLine ? " items nearby, worth " : " nearby, ";
            itemsTextColor = Plugin.itemsNearbyTextColor.Value == Colors.Custom ? SetCustomScannerColor(3) : htmlColors[(int)Plugin.itemsNearbyTextColor.Value];
            scannerRange = Plugin.scannerRange.Value; // 29m max (default)
        }

        public static void Initialize()
        {
            Plugin.LogToConsole("Initializing VEGA");
            shouldBeInterrupted = false;
            signals = Plugin.messages.Value.Split(", ");
            InitializeScannerVariables();
            AddNameColors();
            listening = false;
            if (!Plugin.useManualListening.Value || (Plugin.enableManualListeningAuto.Value && Plugin.useManualListening.Value))
            {
                listening = true;
            }
            Plugin.LogToConsole("Is VEGA listening? -> " + listening, "debug");

            RegisterAllCommands();
        }

        private static void RegisterAllCommands()
        {
            Plugin.LogToConsole("Registering voice commands");

            RegisterMiscCommands();
            RegisterMoonsInfo();
            RegisterBestiaryEntries();
            RegisterEntityInfo();
            RegisterDoorCommands();
            RegisterHazardsCommands();
            RegisterTeleportCommands();
            RegisterAdvancedScannerCommands();
            RegisterTimeOfDayCommands();
            RegisterRadarBoosterCommands();
            RegisterSignalTranslatorCommands();
            RegisterWeatherCommands();
            RegisterReportCommands();
            RegisterActivationCommands();
            RegisterModdedTerminalCommands();
        }

        internal static void RegisterActivationCommands()
        {
            if (Plugin.registerActivation.Value)
            {
                string[] phrases = Plugin.startListeningCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (Speech.IsAboveThreshold(phrases, Plugin.manualActivationConfidence.Value) && Plugin.useManualListening.Value && listening)
                    {
                        if (StartOfRound.Instance.localPlayerController == null) return;
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            if (!listening)
                            {
                                listening = true;
                                Plugin.LogToConsole("Is VEGA listening -> " + listening, "debug");
                                PlaySFX("Activate");
                            }
                            else
                            {
                                PlayLine("AlreadyActive");
                            }
                        }
                    }
                });
                string[] phrases_1 = Plugin.stopListeningCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (Speech.IsAboveThreshold(phrases_1, Plugin.manualActivationConfidence.Value) && Plugin.useManualListening.Value && listening)
                    {
                        if (StartOfRound.Instance.localPlayerController == null) return;
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            if (listening)
                            {
                                listening = false;
                                Plugin.LogToConsole("Is VEGA listening -> " + listening, "debug");
                                PlaySFX("Deactivate");
                            }
                            else
                            {
                                PlayLine("AlreadyInactive");
                            }
                        }
                    }
                });
            }
        }

        internal static void RegisterMiscCommands()
        {
            if (Plugin.registerStop.Value)
            {
                string[] phrases = Plugin.stopCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && audioSource != null && Speech.IsAboveThreshold(phrases, Plugin.stopConfidence.Value) && listening) audioSource.Stop();
                });
            }
            if (Plugin.registerThanks.Value)
            {
                string[] phrases = Plugin.gratitudeCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.gratitudeConfidence.Value) && listening) PlayRandomLine("NoProblem", Random.Range(1, 5));
                });
            }

            // Ship lights
            if (Plugin.registerInteractShipLights.Value)
            {
                string[] phrases = Plugin.lightsOnCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.shipConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        CoroutineManager.StartCoroutine(SwitchLights(on: true));
                    }
                });
                string[] phrases_1 = Plugin.lightsOffCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.shipConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        CoroutineManager.StartCoroutine(SwitchLights(on: false));
                    }
                });
            }

            // Ship shutters (ShipWindows mod)
            if (Plugin.registerInteractShipShutters.Value)
            {
                string[] phrases = Plugin.openShuttersCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.shipConfidence.Value) && listening)
                    {
                        if (ModChecker.hasShipWindows)
                        {
                            if (ShipWindowsPatches.opened)
                            {
                                PlayLine("ShuttersAlreadyOpen");
                                return;
                            }
                            CoroutineManager.StartCoroutine(SwitchWindowShutters(open: true));
                        }
                    }
                });
                string[] phrases_1 = Plugin.closeShuttersCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.shipConfidence.Value) && listening)
                    {
                        if (ModChecker.hasShipWindows)
                        {
                            if (!ShipWindowsPatches.opened)
                            {
                                PlayLine("ShuttersAlreadyClosed");
                                return;
                            }
                            CoroutineManager.StartCoroutine(SwitchWindowShutters(open: false));
                        }
                    }
                });
            }

            // Ship magnet for the CC
            if (Plugin.registerInteractShipMagnet.Value)
            {
                string[] phrases = Plugin.magnetOnCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.shipConfidence.Value) && listening)
                    {
                        if (StartOfRound.Instance.magnetOn)
                        {
                            PlayLine("MagnetAlreadyActive");
                            return;
                        }
                        StartOfRound.Instance.SetMagnetOnServerRpc(true);
                        if (Plugin.vocalLevel.Value >= VocalLevels.Low && StartOfRound.Instance.magnetOn) PlayRandomLine("ActivateMagnet", Random.Range(1, 3));
                    }
                });
                string[] phrases_1 = Plugin.magnetOffCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.shipConfidence.Value) && listening)
                    {
                        if (!StartOfRound.Instance.magnetOn)
                        {
                            PlayLine("MagnetAlreadyInactive");
                            return;
                        }
                        StartOfRound.Instance.SetMagnetOnServerRpc(false);
                        if (Plugin.vocalLevel.Value >= VocalLevels.Low && !StartOfRound.Instance.magnetOn) PlayRandomLine("DeactivateMagnet", Random.Range(1, 3));
                    }
                });
            }
        }

        internal static void RegisterModdedTerminalCommands()
        {
            // DISCOMBOBULATE!
            if (Plugin.registerDiscombobulator.Value && ModChecker.hasLGU)
            {
                string[] phrases = Plugin.discombobulatorCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.upgradesConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }

                        if (!BaseUpgrade.GetActiveUpgrade("Discombobulator"))
                        {
                            PlayRandomLine("NoDiscombobulator", Random.Range(1, 4));
                            return;
                        }
                        if (LGUPatches.flashCooldown > 0f)
                        {
                            PlayLine("DiscombobulatorUnavailable");
                            return;
                        }
                        if (!StartOfRound.Instance.localPlayerController.isInHangarShipRoom)
                        {
                            PlayLine("Discombobulate");
                        }

                        Type commandParserType = Type.GetType("MoreShipUpgrades.Misc.CommandParser, MoreShipUpgrades");
                        if (commandParserType != null)
                        {
                            MethodInfo atkMethodInfo = commandParserType.GetMethod("ExecuteDiscombobulatorAttack", BindingFlags.NonPublic | BindingFlags.Static);
                            object[] parameters = new object[] { TerminalPatch.terminalInstance };
                            atkMethodInfo.Invoke(null, parameters);
                        }

                        if (Plugin.sendDiscombobulatorChatMessage.Value) SendChatMessage("used the Discombobulator");
                    }
                });
            }
        }

        internal static void RegisterWeatherCommands()
        {
            if (Plugin.registerWeatherInfo.Value)
            {
                foreach (var condition in weathers)
                {
                    string[] phrases = new string[] { "VEGA, info about " + condition + " weather" };
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening) PlayRandomLine(condition, 2);
                    });
                }
            }
        }

        internal static void RegisterAdvancedScannerCommands()
        {
            if (Plugin.registerAdvancedScanner.Value)
            {
                string[] phrases = Plugin.activateAdvancedScannerCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.miscConfidence.Value) && listening)
                    {
                        if (advancedScannerActive)
                        {
                            PlayLine("ScannerAlreadyActive");
                            return;
                        }

                        Plugin.LogToConsole("Activating advanced scanner", "debug");
                        advancedScannerActive = true;

                        if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                        {
                            PlayRandomLine("AdvancedScannerEnabled", Random.Range(1, 4));
                        }
                    }
                });
                string[] phrases_1 = Plugin.deactivateAdvancedScannerCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.miscConfidence.Value) && listening)
                    {
                        if (!advancedScannerActive)
                        {
                            PlayLine("ScannerAlreadyInactive");
                            return;
                        }

                        Plugin.LogToConsole("Deactivating advanced scanner", "debug");
                        advancedScannerActive = false;

                        HUDManagerPatch.entities.GetComponent<TextMeshProUGUI>().SetText("");
                        HUDManagerPatch.items.GetComponent<TextMeshProUGUI>().SetText("");

                        if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                        {
                            PlayRandomLine("AdvancedScannerDisabled", Random.Range(1, 4));
                        }
                    }
                });
            }
        }

        internal static void RegisterTimeOfDayCommands()
        {
            if (Plugin.registerTime.Value)
            {
                string[] phrases = Plugin.timeCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening) GetDayMode();
                });
            }
        }

        internal static void RegisterReportCommands()
        {
            if (Plugin.registerCrewStatus.Value)
            {
                string[] phrases = Plugin.crewStatusCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.crewStatusConfidence.Value) && listening)
                    {
                        if (malfunctionPowerTriggered)
                        {
                            PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                            return;
                        }
                        CoroutineManager.StartCoroutine(GetCrewStatus());
                    }
                });
            }
            if (Plugin.registerCrewInShip.Value)
            {
                string[] phrases = Plugin.crewInShipCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.crewInShipConfidence.Value) && listening)
                    {
                        if (malfunctionPowerTriggered)
                        {
                            PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                            return;
                        }
                        CoroutineManager.StartCoroutine(GetCrewInShip());
                    }
                });
            }
            if (Plugin.registerScrapLeft.Value)
            {
                string[] phrases = Plugin.scrapLeftCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.scrapLeftConfidence.Value) && listening)
                    {
                        if (malfunctionPowerTriggered)
                        {
                            PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                            return;
                        }
                        CoroutineManager.StartCoroutine(GetScrapLeft(recognized.Text));
                    }
                });
            }
        }

        internal static void RegisterTeleportCommands()
        {
            if (Plugin.registerTeleporter.Value)
            {
                string[] phrases = Plugin.teleporterCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.teleportConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionTeleporterTriggered)
                            {
                                PlayLine("TeleporterMalfunction");
                                return;
                            }
                        }
                        CoroutineManager.StartCoroutine(ActivateTeleporter());
                    }
                });
            }
            if (Plugin.registerRadarSwitch.Value)
            {
                string[] phrases = Plugin.radarSwitchCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.teleportConfidence.Value) && listening)
                    {
                        SwitchRadar();
                        PlayRandomLine("RadarSwitch", Random.Range(1, 4));
                    }
                });
            }
        }

        internal static void RegisterDoorCommands()
        {
            // Secure doors
            if (Plugin.registerInteractSecureDoor.Value)
            {
                string[] phrases = Plugin.openSecureDoorCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.secureDoorsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        OpenSecureDoor();
                    }
                });
                string[] phrases_1 = Plugin.closeSecureDoorCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.secureDoorsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        CloseSecureDoor();
                    }
                });
            }
            if (Plugin.registerInteractAllSecureDoors.Value)
            {
                string[] phrases = Plugin.openAllSecureDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.secureDoorsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        OpenAllDoors();
                    }
                });
                string[] phrases_1 = Plugin.closeAllSecureDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.secureDoorsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        CloseAllDoors();
                    }
                });
            }

            // Ship doors
            if (Plugin.registerInteractShipDoors.Value)
            {
                string[] phrases = Plugin.openShipDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.shipConfidence.Value) && listening)
                    {
                        HangarShipDoor shipDoors = Object.FindObjectOfType<HangarShipDoor>();
                        if (shipDoors != null)
                        {
                            if (ModChecker.hasMalfunctions)
                            {
                                if (malfunctionPowerTriggered)
                                {
                                    PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                    return;
                                }
                                if (malfunctionDoorTriggered)
                                {
                                    PlayLine("DoorMalfunction");
                                    return;
                                }
                            }

                            if (!shipDoors.shipDoorsAnimator.GetBool("Closed"))
                            {
                                PlayLine("ShipDoorsAlreadyOpen");
                                return;
                            }

                            InteractTrigger component = shipDoors.transform.Find("HangarDoorButtonPanel/StartButton/Cube (2)").GetComponent<InteractTrigger>();
                            component.Interact(((Component)(object)StartOfRound.Instance.localPlayerController).transform);
                            if (!shipDoors.shipDoorsAnimator.GetBool("Closed"))
                            {
                                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                                {
                                    PlayLine("ShipDoorsOpened", 0.7f);
                                }
                            }
                        }
                    }
                });
                string[] phrases_1 = Plugin.closeShipDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.shipConfidence.Value) && listening)
                    {
                        HangarShipDoor shipDoors = Object.FindObjectOfType<HangarShipDoor>();
                        if (shipDoors != null)
                        {
                            if (ModChecker.hasMalfunctions)
                            {
                                if (malfunctionPowerTriggered)
                                {
                                    PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                    return;
                                }
                            }

                            if (shipDoors.shipDoorsAnimator.GetBool("Closed"))
                            {
                                PlayLine("ShipDoorsAlreadyClosed");
                                return;
                            }

                            InteractTrigger component = shipDoors.transform.Find("HangarDoorButtonPanel/StopButton/Cube (3)").GetComponent<InteractTrigger>();
                            component.Interact(((Component)(object)StartOfRound.Instance.localPlayerController).transform);
                            if (shipDoors.shipDoorsAnimator.GetBool("Closed"))
                            {
                                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                                {
                                    PlayLine("ShipDoorsClosed", 0.7f);
                                }
                            }
                        }
                    }
                });
            }
        }

        internal static void RegisterSignalTranslatorCommands()
        {
            if (Plugin.registerSignalTranslator.Value)
            {
                foreach (var signal in signals)
                {
                    string[] phrases = Plugin.transmitCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var phrase in phrases)
                    {
                        string fullCommand = phrase + " " + signal;
                        phrases[Array.IndexOf(phrases, phrase)] = fullCommand;
                    }
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.signalsConfidence.Value) && listening)
                        {
                            if (StartOfRound.Instance.localPlayerController == null) return;
                            if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                            {
                                if (ModChecker.hasMalfunctions)
                                {
                                    if (malfunctionPowerTriggered)
                                    {
                                        PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                        return;
                                    }
                                    if (malfunctionDistortionTriggered)
                                    {
                                        PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                        return;
                                    }
                                }
                                SignalTranslator translator = Object.FindObjectOfType<SignalTranslator>();
                                if (translator == null)
                                {
                                    if (Plugin.vocalLevel.Value >= VocalLevels.Medium) PlayLine("NoSignalTranslator");
                                    PlayLine("NoSignalTranslatorLow");
                                    return;
                                }
                                HUDManager.Instance.UseSignalTranslatorServerRpc(signal);
                                if (Plugin.sendSignalTranslatorChatMessage.Value) SendChatMessage("is transmitting a signal");
                            }
                        }
                    });
                }
            }
        }

        internal static void RegisterRadarBoosterCommands()
        {
            if (Plugin.registerRadarBoosters.Value)
            {
                string[] phrases = Plugin.radarPingCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.miscConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        InteractWithBooster(ping: true);
                    }
                });
                string[] phrases_1 = Plugin.radarFlashCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases_1);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases_1, Plugin.miscConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        InteractWithBooster(ping: false);
                    }
                });
            }
        }

        internal static void RegisterHazardsCommands()
        {
            // Turrets
            if (Plugin.registerDisableTurret.Value)
            {
                string[] phrases = Plugin.disableTurretCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.turretsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        turretDisabled = false;
                        noVisibleTurret = false;
                        // noTurretNearby = false;
                        noTurrets = false;
                        noToils = true;
                        if (ModChecker.hasToilHead)
                        {
                            noToils = false;
                            DisableToil();
                        }
                        if (!toilDisabled || !ModChecker.hasToilHead)
                        {
                            DisableTurret();
                        }
                        PlayTurretAudio();
                    }
                });
            }
            if (Plugin.registerDisableAllTurrets.Value)
            {
                string[] phrases = Plugin.disableAllTurretsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.turretsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        turretsExist = false;
                        if (ModChecker.hasToilHead)
                        {
                            DisableAllToils();
                        }
                        DisableAllTurrets();
                    }
                });
            }

            // Landmines
            if (Plugin.registerDisableMine.Value)
            {
                string[] phrases = Plugin.disableMineCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.landminesConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        DisableMine();
                    }
                });
            }
            if (Plugin.registerDisableAllMines.Value)
            {
                string[] phrases = Plugin.disableAllMinesCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.landminesConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        DisableAllMines();
                    }
                });
            }

            // Spike traps
            if (Plugin.registerDisableSpikeTrap.Value)
            {
                string[] phrases = Plugin.disableSpikeTrapCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.trapsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        DisableSpikeTrap();
                    }
                });
            }
            if (Plugin.registerDisableAllSpikeTraps.Value)
            {
                string[] phrases = Plugin.disableAllSpikeTrapsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                Speech.RegisterPhrases(phrases);
                Speech.RegisterCustomHandler((obj, recognized) =>
                {
                    if (StartOfRound.Instance.localPlayerController == null) return;
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead && Speech.IsAboveThreshold(phrases, Plugin.trapsConfidence.Value) && listening)
                    {
                        if (ModChecker.hasMalfunctions)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            if (malfunctionDistortionTriggered)
                            {
                                PlayRandomLine("CommsMalfunction", Random.Range(1, 4));
                                return;
                            }
                        }
                        DisableAllSpikeTraps();
                    }
                });
            }
        }

        internal static void RegisterMoonsInfo()
        {
            if (Plugin.registerMoonsInfo.Value)
            {
                // Vanilla
                foreach (var name in moonNames)
                {
                    string[] phrases = Plugin.moonsInfoCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var phrase in phrases)
                    {
                        string fullCommand = phrase + " " + name;
                        phrases[Array.IndexOf(phrases, phrase)] = fullCommand;
                    }
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening)
                        {
                            PlayLine(GetMoonAudioClipName(name));
                        }
                    });
                }
            }
        }

        internal static void RegisterBestiaryEntries()
        {
            if (Plugin.registerBestiaryEntries.Value)
            {
                // Vanilla
                foreach (var name in vanillaEntityNames)
                {
                    string[] phrases = Plugin.bestiaryEntriesCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var phrase in phrases)
                    {
                        string fullCommand = phrase + " " + name + " entry";
                        phrases[Array.IndexOf(phrases, phrase)] = fullCommand;
                    }
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening)
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(enemies.First(key => key.Value == GetEntityAudioClipName(name)).Key))
                            {
                                PlayLine(GetEntityAudioClipName(name));
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                    });
                }

                // Modded
                foreach (var name in moddedEntityNames)
                {
                    string[] phrases = Plugin.bestiaryEntriesCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var phrase in phrases)
                    {
                        string fullCommand = phrase + " " + name + " entry";
                        phrases[Array.IndexOf(phrases, phrase)] = fullCommand;
                    }
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening)
                        {
                            try
                            {
                                if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals(GetEntityAudioClipName(name))).creatureFileID))
                                {
                                    PlayLine(GetEntityAudioClipName(name));
                                }
                                else
                                {
                                    PlayRandomLine("NoEntityData", Random.Range(1, 5));
                                }
                            }
                            catch (Exception)
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                    });
                }
            }
        }

        internal static void RegisterEntityInfo()
        {
            if (Plugin.registerCreatureInfo.Value)
            {
                // Vanilla
                foreach (var name in vanillaEntityNames)
                {
                    string[] phrases = Plugin.creatureInfoCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var phrase in phrases)
                    {
                        string fullCommand = phrase + " " + name + "s";
                        phrases[Array.IndexOf(phrases, phrase)] = fullCommand;
                    }
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening)
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(enemies.First(key => key.Value == GetEntityAudioClipName(name)).Key))
                            {
                                PlayLine(GetEntityAudioClipName(name) + "Short");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                    });
                }

                // Modded
                foreach (var name in moddedEntityNames)
                {
                    string[] phrases = Plugin.creatureInfoCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var phrase in phrases)
                    {
                        string fullCommand = phrase + " " + name + "s";
                        phrases[Array.IndexOf(phrases, phrase)] = fullCommand;
                    }
                    Speech.RegisterPhrases(phrases);
                    Speech.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (Speech.IsAboveThreshold(phrases, Plugin.infoConfidence.Value) && listening)
                        {
                            try
                            {
                                if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals(GetEntityAudioClipName(name))).creatureFileID))
                                {
                                    PlayLine(GetEntityAudioClipName(name) + "Short");
                                }
                                else
                                {
                                    PlayRandomLine("NoEntityData", Random.Range(2, 5));
                                }
                            }
                            catch (Exception)
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                    });
                }
            }
        }
    }
}
