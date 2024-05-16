using GameNetcodeStuff;
using LC_VEGA.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using VoiceRecognitionAPI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LC_VEGA
{
    internal class VEGA
    {
        public static AudioSource audioSource;
        public static List<AudioClip> voiceLines;
        public static bool shouldBeInterrupted;
        public static bool warningGiven;
        public static bool facilityHasPower;
        public static bool performAdvancedScan;
        public static float teleporterCooldownTime;
        public static TextMeshProUGUI enemiesText;
        public static TextMeshProUGUI itemsText;
        public static char creditsChar;

        internal static string enemiesTopText;
        internal static string itemsTopText;
        internal static float scannerRange;

        public static string[] weathers =
        {
            "Foggy",
            "Rainy",
            "Stormy",
            "Flooded",
            "Eclipsed"
        };

        public static string[] enemyList = {
            "SnareFlea",
            "Bracken",
            "Thumper",
            "EyelessDog",
            "YippeeBug",
            "Slime",
            "ForestKeeper",
            "Coil-Head",
            "",
            "Sandworm",
            "Jester",
            "SporeLizard",
            "BunkerSpider",
            "Manticoil",
            "RedBees",
            "Locusts",
            "BaboonHawk",
            "Nutcracker",
            "OldBird",
            "Butler",
            "",
            "Snakes"
        };

        public static string[] signals = {
            "YES",
            "NO",
            "OKAY",
            "HELP",
            "THANKS",
            "ITEMS",
            "MAIN",
            "FIRE",
            "GIANT",
            "GIANTS",
            "DOG",
            "DOGS",
            "WORM",
            "WORMS",
            "BABOONS",
            "HAWKS",
            "DANGER",
            "GIRL",
            "GHOST",
            "BRACKEN",
            "BUTLER",
            "BUTLERS",
            "BUG",
            "BUGS",
            "YIPPEE",
            "SNARE",
            "FLEA",
            "COIL",
            "SLIME",
            "THUMPER",
            "MIMIC",
            "MIMICS",
            "MASKED",
            "SPIDER",
            "SNAKES",
            "OLD BIRD",
            "HEROBRINE",
            "FOOTBALL",
            "FURBO",
            "FIEND",
            "SLENDER",
            "LOCKER",
            "SHY GUY",
            "SIRENHEAD",
            "DRIFTWOOD",
            "WATCHER",
            "INSIDE",
            "TRAPPED",
            "LEAVE",
            "GOLD",
            "APPARATUS"
        };

        public static void PlayIntro()
        {
            if (audioSource != null)
            {
                if (!audioSource.isPlaying)
                {
                    Plugin.LogToConsole("Playing intro audio");
                    foreach (var clip in voiceLines)
                    {
                        if (clip.name.Equals("Intro"))
                        {
                            audioSource.clip = clip;
                        }
                    }
                    audioSource.PlayDelayed(4.5f);
                }
            }
            else
            {
                Plugin.LogToConsole("Unable to play intro audio. The audio source for VEGA does not exist", "error");
            }
        }

        public static void PlayAudio(string clipName, float delay = 0.25f)
        {
            if (audioSource != null)
            {
                if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                {
                    if (!audioSource.isPlaying)
                    {
                        Plugin.LogToConsole("Playing audio");
                        foreach (var clip in voiceLines)
                        {
                            if (clip.name.Equals(clipName))
                            {
                                audioSource.clip = clip;
                            }
                        }
                        audioSource.PlayDelayed(delay);
                    }
                }
            }
            else
            {
                Plugin.LogToConsole("Unable to play audio. The audio source for VEGA does not exist", "error");
            }
        }

        public static void PlayAudioWithVariant(string clipName, int range, float delay = 0.25f)
        {
            clipName += "-" + range;
            PlayAudio(clipName, delay);
        }

        internal static void OpenSecureDoor()
        {
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
                if (!facilityHasPower)
                {
                    PlayAudioWithVariant("NoPower", Random.Range(1, 4));
                    return;
                }
                TerminalAccessibleObject? closestDoor = GetClosestSecureDoor();
                if (closestDoor != null)
                {
                    if (Vector3.Distance(closestDoor.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 11f)
                    {
                        Plugin.LogToConsole("Opening door");
                        closestDoor.SetDoorLocalClient(true);
                        if (Plugin.vocalLevel.Value >= VocalLevels.High)
                        {
                            PlayAudioWithVariant("DoorOpened", Random.Range(1, 4), 0.7f);
                        }
                    }
                    else
                    {
                        PlayAudio("NoDoorNearby");
                    }
                }
            }
            else
            {
                PlayAudioWithVariant("IndoorsOnly", Random.Range(1, 4));
            }
        }

        internal static void OpenAllDoors()
        {
            bool doorsExist = false;
            if (!facilityHasPower)
            {
                PlayAudioWithVariant("NoPower", Random.Range(1, 4));
                return;
            }
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
                Plugin.LogToConsole("Opening all doors");
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayAudioWithVariant("AllDoorsOpened", Random.Range(1, 3), 0.7f);
                }
            }
            else
            {
                PlayAudio("NoDoors");
            }
        }

        internal static void CloseSecureDoor()
        {
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
                if (!facilityHasPower)
                {
                    PlayAudioWithVariant("NoPower", Random.Range(1, 4));
                    return;
                }
                TerminalAccessibleObject? closestDoor = GetClosestSecureDoor();
                if (closestDoor != null)
                {
                    if (Vector3.Distance(closestDoor.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 11f)
                    {
                        Plugin.LogToConsole("Closing door");
                        closestDoor.SetDoorLocalClient(false);
                        if (Plugin.vocalLevel.Value >= VocalLevels.High)
                        {
                            PlayAudioWithVariant("DoorClosed", Random.Range(1, 4), 0.7f);
                        }
                    }
                    else
                    {
                        PlayAudio("NoDoorNearby");
                    }
                }
            }
            else
            {
                PlayAudioWithVariant("IndoorsOnly", Random.Range(1, 4));
            }
        }

        internal static void CloseAllDoors()
        {
            bool doorsExist = false;
            if (!facilityHasPower)
            {
                PlayAudioWithVariant("NoPower", Random.Range(1, 4));
                return;
            }
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
                Plugin.LogToConsole("Closing all doors");
                if (Plugin.vocalLevel.Value >= VocalLevels.High)
                {
                    PlayAudioWithVariant("AllDoorsClosed", Random.Range(1, 3), 0.7f);
                }
            }
            else
            {
                PlayAudio("NoDoors");
            }
        }

        internal static TerminalAccessibleObject? GetClosestSecureDoor()
        {
            Plugin.LogToConsole("Getting closest secure door");

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            List<TerminalAccessibleObject> secureDoors = new List<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.isBigDoor)
                {
                    secureDoors.Add(item);
                }
            }

            if (secureDoors.Count() == 0)
            {
                PlayAudio("NoDoors");
                return null;
            }

            TerminalAccessibleObject closestDoor = secureDoors[0];

            List<float> distances = new List<float>();
            float distanceToPlayer = 0f;
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
            Plugin.LogToConsole("Getting closest turret");

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            List<TerminalAccessibleObject> turrets = new List<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Turret>())
                {
                    turrets.Add(item);
                }
            }

            if (turrets.Count() == 0)
            {
                PlayAudio("NoTurrets");
                return null;
            }

            TerminalAccessibleObject closestTurret = turrets[0];

            List<float> distances = new List<float>();
            float distanceToPlayer = 0f;
            foreach (var turret in turrets)
            {
                distanceToPlayer = Vector3.Distance(turret.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min())
                {
                    closestTurret = turret;
                }
            }
            return closestTurret;
        }

        internal static void DisableTurret()
        {
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
                TerminalAccessibleObject? closestTurret = GetClosestTurret();
                if (closestTurret != null)
                {
                    if (Vector3.Distance(closestTurret.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 51f)
                    {
                        if (StartOfRound.Instance.localPlayerController.HasLineOfSightToPosition(closestTurret.transform.position, 45, 240))
                        {
                            Plugin.LogToConsole("Disabling turret");
                            closestTurret.CallFunctionFromTerminal();
                            if (Plugin.vocalLevel.Value >= VocalLevels.High)
                            {
                                PlayAudioWithVariant("TurretDisabled", Random.Range(1, 4), 0.7f);
                            }
                        }
                        else
                        {
                            PlayAudio("NoVisibleTurret");
                        }
                    }
                    else
                    {
                        PlayAudio("NoTurretNearby");
                    }
                }
            }
            else
            {
                PlayAudioWithVariant("IndoorsOnly", Random.Range(1, 4));
            }
        }

        internal static void DisableAllTurrets()
        {
            bool turretsExist = false;
            Plugin.LogToConsole("Disabling all turrets");
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
                    PlayAudioWithVariant("AllTurretsDisabled", Random.Range(1, 4), 0.7f);
                }
            }
            else
            {
                PlayAudio("NoTurrets");
            }
        }

        internal static TerminalAccessibleObject? GetClosestMine()
        {
            Plugin.LogToConsole("Getting closest landmine");

            TerminalAccessibleObject[] terminalObjects = Object.FindObjectsOfType<TerminalAccessibleObject>();
            List<TerminalAccessibleObject> mines = new List<TerminalAccessibleObject>();
            foreach (var item in terminalObjects)
            {
                if (item.gameObject.GetComponent<Landmine>())
                {
                    mines.Add(item);
                }
            }

            if (mines.Count() == 0)
            {
                PlayAudio("NoMines");
                return null;
            }

            TerminalAccessibleObject closestMine = mines[0];

            List<float> distances = new List<float>();
            float distanceToPlayer = 0f;
            foreach (var mine in mines)
            {
                distanceToPlayer = Vector3.Distance(mine.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min())
                {
                    closestMine = mine;
                }
            }
            return closestMine;
        }

        internal static void DisableMine()
        {
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
                TerminalAccessibleObject? closestMine = GetClosestMine();
                if (closestMine != null)
                {
                    if (Vector3.Distance(closestMine.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 11f)
                    {
                        Plugin.LogToConsole("Disabling landmine");
                        closestMine.CallFunctionFromTerminal();
                        if (Plugin.vocalLevel.Value >= VocalLevels.High)
                        {
                            PlayAudioWithVariant("MineDisabled", Random.Range(1, 4));
                        }
                    }
                    else
                    {
                        PlayAudio("NoMineNearby");
                    }
                }
            }
            else
            {
                PlayAudioWithVariant("IndoorsOnly", Random.Range(1, 4));
            }
        }

        internal static void DisableAllMines()
        {
            bool minesExist = false;
            Plugin.LogToConsole("Disabling all landmines");
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
                    PlayAudioWithVariant("AllMinesDisabled", Random.Range(1, 4));
                }
            }
            else
            {
                PlayAudio("NoMines");
            }
        }

        internal static TerminalAccessibleObject? GetClosestSpikeTrap()
        {
            Plugin.LogToConsole("Getting closest spike trap");

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

            if (traps.Count() == 0)
            {
                PlayAudio("NoTraps");
                return null;
            }

            TerminalAccessibleObject closestTrap = traps[0];

            List<float> distances = new List<float>();
            float distanceToPlayer = 0f;
            foreach (var trap in traps)
            {
                distanceToPlayer = Vector3.Distance(trap.transform.position, StartOfRound.Instance.localPlayerController.transform.position);
                distances.Add(distanceToPlayer);
                if (distanceToPlayer <= distances.Min())
                {
                    closestTrap = trap;
                }
            }
            return closestTrap;
        }

        internal static void DisableSpikeTrap()
        {
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
                TerminalAccessibleObject? closestTrap = GetClosestSpikeTrap();
                if (closestTrap != null)
                {
                    if (Vector3.Distance(closestTrap.transform.position, StartOfRound.Instance.localPlayerController.transform.position) < 11f)
                    {
                        Plugin.LogToConsole("Disabling spike trap");
                        closestTrap.CallFunctionFromTerminal();
                        if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                        {
                            PlayAudioWithVariant("TrapDisabled", Random.Range(1, 4));
                        }
                    }
                    else
                    {
                        PlayAudio("NoTrapNearby");
                    }
                }
            }
            else
            {
                PlayAudioWithVariant("IndoorsOnly", Random.Range(1, 4));
            }
        }

        internal static void DisableAllSpikeTraps()
        {
            bool trapsExist = false;
            Plugin.LogToConsole("Disabling all spike traps");
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
                    PlayAudioWithVariant("AllTrapsDisabled", Random.Range(1, 4));
                }
            }
            else
            {
                PlayAudio("NoTraps");
            }
        }

        internal static void ActivateTeleporter()
        {
            if (GameObject.Find("Teleporter(Clone)"))
            {
                ShipTeleporter teleporter = GameObject.Find("Teleporter(Clone)").GetComponent<ShipTeleporter>();
                if (teleporterCooldownTime <= 0f)
                {
                    if (Plugin.vocalLevel.Value >= VocalLevels.High)
                    {
                        PlayAudioWithVariant("Teleport", Random.Range(1, 4));
                    }
                    teleporter.PressTeleportButtonServerRpc();
                }
                else
                {
                    PlayAudio("TeleporterOnCooldown");
                    Plugin.LogToConsole("The teleporter is on cooldown!", "warn");
                }
            }
            else
            {
                PlayAudioWithVariant("NoTeleporter", Random.Range(1, 4));
                // Plugin.LogToConsole("You might need a teleporter for that", "warn");
            }
        }

        internal static IEnumerator SwitchLights(bool on)
        {
            ShipLights shipLights = Object.FindObjectOfType<ShipLights>();
            if (shipLights != null)
            {
                if (shipLights.areLightsOn)
                {
                    PlayAudio("LightsOff");
                }
                else
                {
                    PlayAudio("LightsOn");
                }

                yield return new WaitForSeconds(2f);

                shipLights.SetShipLightsServerRpc(on);
            }
        }

        public static void PerformAdvancedScan()
        {
            if (performAdvancedScan)
            {
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
                        else if (collider.GetComponent<MaskedPlayerEnemy>()?.isEnemyDead == false)
                        {
                            enemyCount++;
                        }
                    }
                }

                switch (enemyCount)
                {
                    case 0:
                        enemiesText.SetText(enemiesTopText + "<color=blue>Clear</color>");
                        break;
                    case 1:
                        enemiesText.SetText(enemiesTopText + "<color=red>1 entity nearby</color>");
                        break;
                    default:
                        enemiesText.SetText(enemiesTopText + "<color=red>" + enemyCount + " entities nearby</color>");
                        break;
                }

                switch (itemCount)
                {
                    case 0:
                        itemsText.SetText(itemsTopText + "<color=blue>Clear</color>");
                        break;
                    case 1:
                        itemsText.SetText(itemsTopText + "<color=green>1 item nearby, worth " + creditsChar + totalWorth + "</color>");
                        break;
                    default:
                        itemsText.SetText(itemsTopText + "<color=green>" + itemCount + " items nearby, worth " + creditsChar + totalWorth + "</color>");
                        break;
                }
            }
        }

        internal static void GetDayMode()
        {
            switch (TimeOfDay.Instance.dayMode)
            {
                case DayMode.None:
                    PlayAudio("None");
                    Plugin.LogToConsole("No day mode found", "warn");
                    break;
                case DayMode.Dawn:
                    PlayAudio("Dawn");
                    break;
                case DayMode.Noon:
                    PlayAudio("Noon");
                    break;
                case DayMode.Sundown:
                    PlayAudio("Sundown");
                    break;
                case DayMode.Midnight:
                    PlayAudio("Midnight");
                    break;
                default:
                    PlayAudio("None");
                    Plugin.LogToConsole("No day mode found", "warn");
                    break;
            }
        }

        internal static RadarBoosterItem? GetClosestRadarBooster()
        {
            RadarBoosterItem[] boosters = Object.FindObjectsOfType<RadarBoosterItem>();

            if (boosters.Count() == 0)
            {
                PlayAudio("NoBoosters");
                return null;
            }

            RadarBoosterItem bobbie = boosters[0];
            List<float> distances = new List<float>();
            float distanceToPlayer = 0f;
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
                        PlayAudio("NoBoostersNearby");
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
                PlayAudio("SoloCrew");
            }
            else
            {
                PlayAudio("GettingCrewStatus");

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
                    PlayAudio("GoodLuck", 2.5f);
                    livingPlayers = "1 employee alive\n";
                    HUDManager.Instance.DisplayTip(header, livingPlayers + deceasedPlayers, isWarning: true);
                }
                else
                {
                    if (Plugin.vocalLevel.Value >= VocalLevels.High)
                    {
                        PlayAudioWithVariant("ReportComplete", Random.Range(1, 4));
                    }
                    HUDManager.Instance.DisplayTip(header, livingPlayers + deceasedPlayers);
                }
            }
        }

        internal static void InitializeScannerVariables()
        {
            performAdvancedScan = Plugin.enableAdvancedScannerAuto.Value;
            enemiesTopText = "Entities:\n";
            itemsTopText = "Items:\n";
            scannerRange = 29f;
        }

        public static void Initialize()
        {
            Plugin.LogToConsole("Initializing VEGA");
            shouldBeInterrupted = false;
            InitializeScannerVariables();

            Plugin.LogToConsole("Registering voice commands");
            RegisterMiscCommands();
            RegisterMoonsInfo();
            RegisterBestiaryEntries();
            RegisterEntityInfo();
            RegisterDoorCommands();
            RegisterHazardsCommands();
            RegisterTeleportCommands();
            RegisterScanCommands();
            RegisterTimeOfDayCommands();
            RegisterRadarBoosterCommands();
            RegisterSignalTranslatorCommands();
            RegisterWeatherCommands();
            RegisterCrewStatusCommands();
        }

        internal static void RegisterMiscCommands()
        {
            if (Plugin.registerStop.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, shut up", "VEGA, stop", "VEGA, stop talking" }, (message) =>
                    {
                        audioSource.Stop();
                    }, Plugin.confidence.Value);
            }
            if (Plugin.registerThanks.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, thank you", "VEGA, thanks", "Thank you, VEGA", "Thanks, VEGA" }, (message) =>
                    {
                        PlayAudioWithVariant("NoProblem", Random.Range(1, 5));
                    }, Plugin.confidence.Value);
            }

            // Ship's lights
            if (Plugin.registerInteractShipLights.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, lights on", "VEGA, turn the lights on" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            CoroutineManager.StartCoroutine(SwitchLights(on: true));
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, lights out", "VEGA, lights off", "VEGA, turn the lights off" }, (message) =>
                {
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                    {
                        CoroutineManager.StartCoroutine(SwitchLights(on: false));
                    }
                }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterWeatherCommands()
        {
            if (Plugin.registerWeatherInfo.Value)
            {
                foreach (var condition in weathers)
                {
                    Voice.ListenForPhrases(new string[] { "VEGA, info about " + condition + " weather" }, (message) =>
                    {
                        PlayAudioWithVariant(condition, 2);
                    }, Plugin.confidence.Value);
                }
            }
        }

        internal static void RegisterScanCommands()
        {
            if (Plugin.registerAdvancedScanner.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, activate scanner", "VEGA, activate advanced scanner", "VEGA, turn on scanner", "VEGA, turn on advanced scanner", "VEGA, scan", "VEGA, enable scanner", "VEGA, enable advanced scanner" }, (message) =>
                    {
                        if (performAdvancedScan)
                        {
                            PlayAudio("ScannerAlreadyActive");
                            return;
                        }

                        Plugin.LogToConsole("Activating advanced scanner");
                        performAdvancedScan = true;

                        HUDManagerPatch.enemies.GetComponent<TextMeshProUGUI>().SetText("Enemies:");
                        HUDManagerPatch.items.GetComponent<TextMeshProUGUI>().SetText("Items:");

                        if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                        {
                            PlayAudioWithVariant("AdvancedScannerEnabled", Random.Range(1, 4));
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, disable scanner", "VEGA, disable advanced scanner", "VEGA, turn off scanner", "VEGA, turn off advanced scanner", "VEGA, disable scan" }, (message) =>
                {
                    if (!performAdvancedScan)
                    {
                        PlayAudio("ScannerAlreadyInactive");
                        return;
                    }

                    Plugin.LogToConsole("Deactivating advanced scanner");
                    performAdvancedScan = false;

                    HUDManagerPatch.enemies.GetComponent<TextMeshProUGUI>().SetText("");
                    HUDManagerPatch.items.GetComponent<TextMeshProUGUI>().SetText("");

                    if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                    {
                        PlayAudioWithVariant("AdvancedScannerDisabled", Random.Range(1, 4));
                    }
                }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterTimeOfDayCommands()
        {
            if (Plugin.registerTime.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, what's the current time of day?", "VEGA, current time of day", "VEGA, time of day", "VEGA, current time", "VEGA, time" }, (message) =>
                    {
                        GetDayMode();
                    }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterCrewStatusCommands()
        {
            if (Plugin.registerCrewStatus.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, crew status", "VEGA, team status", "VEGA, crew info", "VEGA, team info", "VEGA, crew report", "VEGA, team report" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            CoroutineManager.StartCoroutine(GetCrewStatus());
                        }
                    }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterTeleportCommands()
        {
            if (Plugin.registerTeleporter.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, tp", "VEGA, activate tp", "VEGA, teleport", "VEGA, activate teleporter" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            ActivateTeleporter();
                        }
                    }, 0.93f);
            }
            if (Plugin.registerRadarSwitch.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, switch to me", "VEGA, switch radar", "VEGA, switch radar to me", "VEGA, focus", "VEGA, focus on me" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            if (StartOfRound.Instance.mapScreen.targetedPlayer == StartOfRound.Instance.localPlayerController)
                            {
                                PlayAudio("RadarAlreadyFocused");
                                return;
                            }
                            int index = StartOfRound.Instance.mapScreen.radarTargets.FindIndex(target => target.transform == StartOfRound.Instance.localPlayerController.transform);
                            StartOfRound.Instance.mapScreen.SwitchRadarTargetAndSync(index);
                        }
                        PlayAudioWithVariant("RadarSwitch", Random.Range(1, 4));
                    }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterDoorCommands()
        {
            // Secure doors
            if (Plugin.registerInteractSecureDoor.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, open secure door", "VEGA, open door", "VEGA, open the door", "VEGA, open the secure door" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            OpenSecureDoor();
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, close secure door", "VEGA, close door", "VEGA, close the door", "VEGA, close the secure door" }, (message) =>
                {
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                    {
                        CloseSecureDoor();
                    }
                }, Plugin.confidence.Value);
            }
            if (Plugin.registerInteractAllSecureDoors.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, open all secure doors", "VEGA, open all doors" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            OpenAllDoors();
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, close all secure doors", "VEGA, close all doors" }, (message) =>
                {
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                    {
                        CloseAllDoors();
                    }
                }, Plugin.confidence.Value);
            }

            // Ship doors
            if (Plugin.registerInteractShipDoors.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, open ship doors", "VEGA, open the ship's doors", "VEGA, open hangar doors" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            HangarShipDoor shipDoors = Object.FindObjectOfType<HangarShipDoor>();
                            if (shipDoors != null)
                            {
                                if (shipDoors.shipDoorsAnimator.GetBool("Closed"))
                                {
                                    if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                                    {
                                        PlayAudio("ShipDoorsOpened", 0.7f);
                                    }
                                }
                                InteractTrigger component = shipDoors.transform.Find("HangarDoorButtonPanel/StartButton/Cube (2)").GetComponent<InteractTrigger>();
                                component.Interact(((Component)(object)StartOfRound.Instance.localPlayerController).transform);
                            }
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, close ship doors", "VEGA, close the ship's doors", "VEGA, close hangar doors" }, (message) =>
                {
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                    {
                        HangarShipDoor shipDoors = Object.FindObjectOfType<HangarShipDoor>();
                        if (shipDoors != null)
                        {
                            if (!shipDoors.shipDoorsAnimator.GetBool("Closed"))
                            {
                                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                                {
                                    PlayAudio("ShipDoorsClosed", 0.7f);
                                }
                            }
                            InteractTrigger component = shipDoors.transform.Find("HangarDoorButtonPanel/StopButton/Cube (3)").GetComponent<InteractTrigger>();
                            component.Interact(((Component)(object)StartOfRound.Instance.localPlayerController).transform);
                        }
                    }
                }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterSignalTranslatorCommands()
        {
            if (Plugin.registerSignalTranslator.Value)
            {
                foreach (var signal in signals)
                {
                    Voice.ListenForPhrases(new string[] { "VEGA, transmit " + signal, "VEGA, send " + signal }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            SignalTranslator translator = Object.FindObjectOfType<SignalTranslator>();
                            if (translator == null)
                            {
                                PlayAudio("NoSignalTranslator");
                                return;
                            }
                            HUDManager.Instance.UseSignalTranslatorServerRpc(signal);
                        }
                    }, Plugin.confidence.Value);
                }
            }
        }

        internal static void RegisterRadarBoosterCommands()
        {
            if (Plugin.registerRadarBoosters.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, ping" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            InteractWithBooster(ping: true);
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, flash" }, (message) =>
                {
                    if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                    {
                        InteractWithBooster(ping: false);
                    }
                }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterHazardsCommands()
        {
            // Turrets
            if (Plugin.registerDisableTurret.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, disable the turret", "VEGA, disable turret" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            DisableTurret();
                        }
                    }, Plugin.confidence.Value);
            }
            if (Plugin.registerDisableAllTurrets.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, disable all turrets" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            DisableAllTurrets();
                        }
                    }, Plugin.confidence.Value);
            }

            // Landmines
            if (Plugin.registerDisableMine.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, disable the mine", "VEGA, disable mine", "VEGA, disable the landmine", "VEGA, disable landmine" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            DisableMine();
                        }
                    }, Plugin.confidence.Value);
            }
            if (Plugin.registerDisableAllMines.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, disable all mines", "VEGA, disable all landmines" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            DisableAllMines();
                        }
                    }, Plugin.confidence.Value);
            }

            // Spike traps
            if (Plugin.registerDisableSpikeTrap.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, disable the trap", "VEGA, disable trap", "VEGA, disable the spike trap", "VEGA, disable spike trap" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            DisableSpikeTrap();
                        }
                    }, Plugin.confidence.Value);
            }
            if (Plugin.registerDisableAllSpikeTraps.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, disable all traps", "VEGA, disable all spike traps" }, (message) =>
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            DisableAllSpikeTraps();
                        }
                    }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterMoonsInfo()
        {
            if (Plugin.registerMoonsInfo.Value)
            {
                Voice.ListenForPhrase("VEGA, info about experimentation", (message) =>
                    {
                        PlayAudio("41-EXP");
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about assurance", (message) =>
                {
                    PlayAudio("220-ASS");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about vow", (message) =>
                {
                    PlayAudio("56-VOW");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about offense", (message) =>
                {
                    PlayAudio("21-OFF");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about march", (message) =>
                {
                    PlayAudio("61-MAR");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about adamance", (message) =>
                {
                    PlayAudio("20-ADA");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about rend", (message) =>
                {
                    PlayAudio("85-REN");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about dine", (message) =>
                {
                    PlayAudio("7-DIN");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about titan", (message) =>
                {
                    PlayAudio("8-TIT");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about artifice", (message) =>
                {
                    PlayAudio("68-ART");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about embrion", (message) =>
                {
                    PlayAudio("5-EMB");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about liquidation", (message) =>
                {
                    PlayAudio("44-LIQ");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrase("VEGA, info about mars", (message) =>
                {
                    PlayAudio("4-MARS");
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about the Company", "VEGA, info about the Company building", "VEGA, info about Gordion" }, (message) =>
                {
                    PlayAudio("71-GOR");
                }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterBestiaryEntries()
        {
            if (Plugin.registerBestiaryEntries.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, read Hawk entry", "VEGA, read Baboon entry", "VEGA, read Baboon hawk entry" }, (message) =>
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(16))
                        {
                            PlayAudio("BaboonHawk");
                        }
                        else
                        {
                            PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Bunker spider entry", "VEGA, read Spider entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(12))
                    {
                        PlayAudio("BunkerSpider");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Hoarding bug entry", "VEGA, read Loot bug entry", "VEGA, read Yippee bug entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(4))
                    {
                        PlayAudio("YippeeBug");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Bracken entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(1))
                    {
                        PlayAudio("Bracken");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Butler entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(19))
                    {
                        PlayAudio("Butler");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Coil head entry", "VEGA, read Coil entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(7))
                    {
                        PlayAudio("Coil-Head");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Forest Keeper entry", "VEGA, read Giant entry", "VEGA, read Keeper entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(6))
                    {
                        PlayAudio("ForestKeeper");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Eyeless dog entry", "VEGA, read dog entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(3))
                    {
                        PlayAudio("EyelessDog");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Earth Leviathan entry", "VEGA, read Leviathan entry", "VEGA, read Worm entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(9))
                    {
                        PlayAudio("Sandworm");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Jester entry", "VEGA, read Jack in the box entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(10))
                    {
                        PlayAudio("Jester");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Roaming locusts entry", "VEGA, read Locusts entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(15))
                    {
                        PlayAudio("Locusts");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Manticoil entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(13))
                    {
                        PlayAudio("Manticoil");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Nutcracker entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(17))
                    {
                        PlayAudio("Nutcracker");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Old bird entry", "VEGA, read Bird entry", "VEGA, read Mech entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(18))
                    {
                        PlayAudio("OldBird");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Circuit bees entry", "VEGA, read Bees entry", "VEGA, read Red Bees entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(14))
                    {
                        PlayAudio("RedBees");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Hygrodere entry", "VEGA, read Slime entry", "VEGA, read Blob entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(5))
                    {
                        PlayAudio("Slime");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Tulip snake entry", "VEGA, read Tulip entry", "VEGA, read Snake entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(21))
                    {
                        PlayAudio("Snakes");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Snare flea entry", "VEGA, read Flea entry", "VEGA, read Centipede entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(0))
                    {
                        PlayAudio("SnareFlea");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Spore lizard entry", "VEGA, read Lizard entry", "VEGA, read Spore doggy entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(11))
                    {
                        PlayAudio("SporeLizard");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, read Thumper entry", "VEGA, read Crawler entry", "VEGA, read Halve entry" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(2))
                    {
                        PlayAudio("Thumper");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(1, 5));
                    }
                }, Plugin.confidence.Value);
            }
        }

        internal static void RegisterEntityInfo()
        {
            if (Plugin.registerCreatureInfo.Value)
            {
                Voice.ListenForPhrases(new string[] { "VEGA, info about Hawks", "VEGA, info about Baboons", "VEGA, info about Baboon hawks" }, (message) =>
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(16))
                        {
                            PlayAudio("BaboonHawkShort");
                        }
                        else
                        {
                            PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                        }
                    }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Bunker spiders", "VEGA, info about Spiders" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(12))
                    {
                        PlayAudio("BunkerSpiderShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Hoarding bugs", "VEGA, info about Loot bugs", "VEGA, info about Yippee bugs" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(4))
                    {
                        PlayAudio("YippeeBugShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Brackens", "VEGA, info about the Bracken" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(1))
                    {
                        PlayAudio("BrackenShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Butlers" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(19))
                    {
                        PlayAudio("ButlerShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Coil heads", "VEGA, info about Coils" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(7))
                    {
                        PlayAudio("Coil-HeadShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Forest Keepers", "VEGA, info about Giants", "VEGA, info about Keepers" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(6))
                    {
                        PlayAudio("ForestKeeperShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Eyeless dogs", "VEGA, info about dogs" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(3))
                    {
                        PlayAudio("EyelessDogShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Earth Leviathans", "VEGA, info about Leviathans", "VEGA, info about Worms" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(9))
                    {
                        PlayAudio("SandwormShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Jesters", "VEGA, info about the Jack in the box" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(10))
                    {
                        PlayAudio("JesterShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Roaming locusts", "VEGA, info about Locusts" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(15))
                    {
                        PlayAudio("LocustsShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Manticoils" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(13))
                    {
                        PlayAudio("ManticoilShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Nutcrackers" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(17))
                    {
                        PlayAudio("NutcrackerShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Old birds", "VEGA, info about Birds", "VEGA, info about Mechs" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(18))
                    {
                        PlayAudio("OldBirdShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Circuit bees", "VEGA, info about Bees", "VEGA, info about Red Bees" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(14))
                    {
                        PlayAudio("RedBeesShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Hygroderes", "VEGA, info about Slimes", "VEGA, info about Blobs" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(5))
                    {
                        PlayAudio("SlimeShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Tulip snakes", "VEGA, info about Tulips", "VEGA, info about Snakes" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(21))
                    {
                        PlayAudio("SnakesShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Snare fleas", "VEGA, info about Fleas", "VEGA, info about Centipedes" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(0))
                    {
                        PlayAudio("SnareFleaShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Spore lizards", "VEGA, info about Lizards", "VEGA, info about Spore doggies" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(11))
                    {
                        PlayAudio("SporeLizardShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
                Voice.ListenForPhrases(new string[] { "VEGA, info about Thumpers", "VEGA, info about Crawlers", "VEGA, info about Halves" }, (message) =>
                {
                    if (TerminalPatch.scannedEnemyIDs.Contains(2))
                    {
                        PlayAudio("ThumperShort");
                    }
                    else
                    {
                        PlayAudioWithVariant("NoEntityData", Random.Range(2, 5));
                    }
                }, Plugin.confidence.Value);
            }
        }
    }
}
