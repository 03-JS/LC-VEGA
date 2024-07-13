using com.github.zehsteam.ToilHead.MonoBehaviours;
using LC_VEGA.Patches;
using Malfunctions;
using MoreShipUpgrades.Misc;
using MoreShipUpgrades.Misc.Upgrades;
using MoreShipUpgrades.UpgradeComponents.TierUpgrades;
using ShipWindows;
using ShipWindows.Components;
using ShipWindows.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using VoiceRecognitionAPI;
using static UnityEngine.GraphicsBuffer;
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
        public static bool performAdvancedScan;
        public static float teleporterCooldownTime;
        public static TextMeshProUGUI enemiesText;
        public static TextMeshProUGUI itemsText;
        public static char creditsChar;

        internal static string enemiesTopText;
        internal static string itemsTopText;
        internal static float scannerRange;

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
        public static string[] weathers =
        {
            "Foggy",
            "Rainy",
            "Stormy",
            "Flooded",
            "Eclipsed"
        };

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
            { 21, "Snakes" }
        };

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
            "Shrimp"
        };

        public static void PlayLine(string clipName, float delay = 0.25f, bool checkPlayerStatus = true, bool skip = false)
        {
            if (audioSource != null)
            {
                if (checkPlayerStatus && StartOfRound.Instance.localPlayerController.isPlayerDead) return;
                if (!skip && audioSource.isPlaying) return;

                Plugin.LogToConsole("Playing audio");
                foreach (var clip in audioClips)
                {
                    if (clip.name.Equals(clipName))
                    {
                        audioSource.clip = clip;
                    }
                }
                audioSource.PlayDelayed(delay);
            }
            else
            {
                Plugin.LogToConsole("Unable to play audio. The audio source for VEGA does not exist", "error");
            }
        }

        public static void PlaySFX(string clipName, float delay = 0.25f, bool checkPlayerStatus = true, bool skip = false)
        {
            if (sfxAudioSource != null)
            {
                if (checkPlayerStatus && StartOfRound.Instance.localPlayerController.isPlayerDead) return;
                if (!skip && sfxAudioSource.isPlaying) return;

                Plugin.LogToConsole("Playing SFX");
                foreach (var clip in audioClips)
                {
                    if (clip.name.Equals(clipName))
                    {
                        sfxAudioSource.clip = clip;
                    }
                }
                sfxAudioSource.PlayDelayed(delay);
            }
            else
            {
                Plugin.LogToConsole("Unable to play SFX. The SFX audio source for VEGA does not exist", "error");
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
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
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
            else
            {
                PlayRandomLine("IndoorsOnly", Random.Range(1, 4));
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
            if (StartOfRound.Instance.localPlayerController.isInsideFactory)
            {
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
            else
            {
                PlayRandomLine("IndoorsOnly", Random.Range(1, 4));
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

            if (secureDoors.Count() == 0)
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

            if (turrets.Count() == 0)
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
                else if (distances.Count() > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count() == 0)
            {
                noVisibleTurret = true;
                return null;
            }

            distanceToTurret = distances.Min();
            return closestTurret;
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

            if (toils.Count() == 0)
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
                else if (distances.Count() > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count() == 0)
            {
                noVisibleTurret = true;
                return null;
            }

            distanceToToil = distances.Min();
            return closestToil;
        }

        internal static void DisableTurret()
        {
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
                    PlayRandomLine("TurretDisabled", Random.Range(1, 4), 0.7f);
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

            if (mines.Count() == 0)
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
                else if (distances.Count() > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count() == 0)
            {
                PlayLine("NoVisibleMine");
                return null;
            }

            return closestMine;
        }

        internal static void DisableMine()
        {
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

            if (traps.Count() == 0)
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
                else if (distances.Count() > 0)
                {
                    distances.Remove(distanceToPlayer);
                }
            }

            if (distances.Count() == 0)
            {
                PlayLine("NoVisibleTrap");
                return null;
            }

            return closestTrap;
        }

        internal static void DisableSpikeTrap()
        {
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
            if (performAdvancedScan)
            {
                if (ModChecker.hasMalfunctions)
                {
                    if (malfunctionDistortionTriggered || malfunctionPowerTriggered)
                    {
                        enemiesText.SetText(enemiesTopText + "<color=yellow>Data unavailable</color>");
                        itemsText.SetText(itemsTopText + "<color=yellow>Data unavailable</color>");
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

        internal static void InitializeScannerVariables()
        {
            performAdvancedScan = Plugin.enableAdvancedScannerAuto.Value;
            enemiesTopText = "Entities:\n";
            itemsTopText = "Items:\n";
            scannerRange = Plugin.scannerRange.Value; // 29m max (default)
        }

        public static void Initialize()
        {
            Plugin.LogToConsole("Initializing VEGA");
            shouldBeInterrupted = false;
            signals = Plugin.messages.Value.Split(", ");
            InitializeScannerVariables();
            listening = false;
            if (!Plugin.useManualListening.Value || (Plugin.enableManualListeningAuto.Value && Plugin.useManualListening.Value))
            {
                listening = true;
            }
            Plugin.LogToConsole("Is VEGA listening -> " + listening, "debug");

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
                // Voice.RegisterPhrases(new string[] { "VEGA, activate" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.manualActivationConfidence.Value && Plugin.useManualListening.Value)
                    {
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
                // Voice.RegisterPhrases(new string[] { "VEGA, deactivate" });
                Voice.RegisterPhrases(phrases_1);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, deactivate") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.manualActivationConfidence.Value && Plugin.useManualListening.Value)
                    {
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
                // Voice.RegisterPhrases(new string[] { "VEGA, shut up", "VEGA, stop", "VEGA, stop talking", "Shut up, VEGA", "Stop, VEGA", "Stop talking, VEGA" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, shut up" && recognized.Message != "VEGA, stop" && recognized.Message != "VEGA, stop talking" && recognized.Message != "Shut up, VEGA" && recognized.Message != "Stop, VEGA" && recognized.Message != "Stop talking, VEGA") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.stopConfidence.Value)
                    {
                        audioSource.Stop();
                    }
                });
            }
            if (Plugin.registerThanks.Value)
            {
                string[] phrases = Plugin.gratitudeCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, thank you", "VEGA, thanks", "Thank you, VEGA", "Thanks, VEGA" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, thank you" && recognized.Message != "VEGA, thanks" && recognized.Message != "Thank you, VEGA" && recognized.Message != "Thanks, VEGA") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.gratitudeConfidence.Value && listening)
                    {
                        PlayRandomLine("NoProblem", Random.Range(1, 5));
                    }
                });
            }

            // Ship lights
            if (Plugin.registerInteractShipLights.Value)
            {
                string[] phrases = Plugin.lightsOnCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, lights on", "VEGA, turn the lights on" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, lights on" && recognized.Message != "VEGA, turn the lights on") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.shipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                    }
                });
                string[] phrases_1 = Plugin.lightsOffCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, lights out", "VEGA, lights off", "VEGA, turn the lights off" });
                Voice.RegisterPhrases(phrases_1);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, lights out" && recognized.Message != "VEGA, lights off" && recognized.Message != "VEGA, turn the lights off") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.shipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                    }
                });
            }

            // Ship shutters (ShipWindows mod)
            if (Plugin.registerInteractShipShutters.Value)
            {
                string[] phrases = Plugin.openShuttersCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, open shutters", "VEGA, open window shutters", "VEGA, open ship shutters" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, open shutters" && recognized.Message != "VEGA, open window shutters" && recognized.Message != "VEGA, open ship shutters") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.shipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                    }
                });
                string[] phrases_1 = Plugin.closeShuttersCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, close shutters", "VEGA, close window shutters", "VEGA, close ship shutters" });
                Voice.RegisterPhrases(new string[] { "VEGA, close shutters", "VEGA, close window shutters", "VEGA, close ship shutters" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, close shutters" && recognized.Message != "VEGA, close window shutters" && recognized.Message != "VEGA, close ship shutters") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.shipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                // Voice.RegisterPhrases(new string[] { "VEGA, attack", "VEGA, stun", "VEGA, shock" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, attack" && recognized.Message != "VEGA, stun" && recognized.Message != "VEGA, shock") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.upgradesConfidence.Value && listening)
                    {
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
                        }
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
                    Voice.RegisterPhrases(new string[] { "VEGA, info about " + condition + " weather" });
                    Voice.RegisterCustomHandler((obj, recognized) =>
                    {
                        if (recognized.Message != "VEGA, info about " + condition + " weather") return;
                        if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                        {
                            PlayRandomLine(condition, 2);
                        }
                    });
                }
            }
        }

        internal static void RegisterAdvancedScannerCommands()
        {
            if (Plugin.registerAdvancedScanner.Value)
            {
                string[] phrases = Plugin.activateAdvancedScannerCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, activate scanner", "VEGA, activate advanced scanner", "VEGA, turn on scanner", "VEGA, turn on advanced scanner", "VEGA, scan", "VEGA, enable scanner", "VEGA, enable advanced scanner" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, activate scanner" && recognized.Message != "VEGA, activate advanced scanner" && recognized.Message != "VEGA, turn on scanner" && recognized.Message != "VEGA, turn on advanced scanner" && recognized.Message != "VEGA, scan" && recognized.Message != "VEGA, enable scanner" && recognized.Message != "VEGA, enable advanced scanner") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.miscConfidence.Value && listening)
                    {
                        if (performAdvancedScan)
                        {
                            PlayLine("ScannerAlreadyActive");
                            return;
                        }

                        Plugin.LogToConsole("Activating advanced scanner", "debug");
                        performAdvancedScan = true;

                        //if (Plugin.scanEntities.Value) HUDManagerPatch.enemies.GetComponent<TextMeshProUGUI>().SetText("Enemies:");
                        //if (Plugin.scanItems.Value) HUDManagerPatch.items.GetComponent<TextMeshProUGUI>().SetText("Items:");

                        if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                        {
                            PlayRandomLine("AdvancedScannerEnabled", Random.Range(1, 4));
                        }
                    }
                });
                string[] phrases_1 = Plugin.deactivateAdvancedScannerCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, disable scanner", "VEGA, disable advanced scanner", "VEGA, turn off scanner", "VEGA, turn off advanced scanner", "VEGA, disable scan" });
                Voice.RegisterPhrases(phrases_1);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable scanner" && recognized.Message != "VEGA, disable advanced scanner" && recognized.Message != "VEGA, turn off scanner" && recognized.Message != "VEGA, turn off advanced scanner" && recognized.Message != "VEGA, disable scan") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.miscConfidence.Value && listening)
                    {
                        if (!performAdvancedScan)
                        {
                            PlayLine("ScannerAlreadyInactive");
                            return;
                        }

                        Plugin.LogToConsole("Deactivating advanced scanner", "debug");
                        performAdvancedScan = false;

                        HUDManagerPatch.enemies.GetComponent<TextMeshProUGUI>().SetText("");
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
                // Voice.RegisterPhrases(new string[] { "VEGA, what's the current time of day?", "VEGA, current time of day", "VEGA, time of day", "VEGA, current time", "VEGA, time", "VEGA, what time is it?" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, what's the current time of day?" && recognized.Message != "VEGA, current time of day" && recognized.Message != "VEGA, time of day" && recognized.Message != "VEGA, current time" && recognized.Message != "VEGA, time" && recognized.Message != "VEGA, what time is it?") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        GetDayMode();
                    }
                });
            }
        }

        internal static void RegisterReportCommands()
        {
            if (Plugin.registerCrewStatus.Value)
            {
                string[] phrases = Plugin.crewStatusCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, crew status", "VEGA, team status", "VEGA, crew info", "VEGA, team info", "VEGA, crew report", "VEGA, team report" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, crew status" && recognized.Message != "VEGA, team status" && recognized.Message != "VEGA, crew info" && recognized.Message != "VEGA, team info" && recognized.Message != "VEGA, crew report" && recognized.Message != "VEGA, team report") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.crewStatusConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            CoroutineManager.StartCoroutine(GetCrewStatus());
                        }
                    }
                });
            }
            if (Plugin.registerCrewInShip.Value)
            {
                string[] phrases = Plugin.crewInShipCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, crew in ship", "VEGA, people in ship", "VEGA, get crew in ship", "VEGA, get people in ship", "VEGA, how many people are in the ship?", "VEGA, is anyone in the ship?", "VEGA, is anybody in the ship?" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, crew in ship" && recognized.Message != "VEGA, people in ship" && recognized.Message != "VEGA, get crew in ship" && recognized.Message != "VEGA, get people in ship" && recognized.Message != "VEGA, how many people are in the ship?" && recognized.Message != "VEGA, is anyone in the ship?" && recognized.Message != "VEGA, is anybody in the ship?") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.crewInShipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            CoroutineManager.StartCoroutine(GetCrewInShip());
                        }
                    }
                });
            }
            if (Plugin.registerScrapLeft.Value)
            {
                string[] phrases = Plugin.scrapLeftCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, scrap left", "VEGA, items left", "VEGA, scan for scrap", "VEGA, scan for items" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, scrap left" && recognized.Message != "VEGA, items left" && recognized.Message != "VEGA, scan for scrap" && recognized.Message != "VEGA, scan for items") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.scrapLeftConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            if (malfunctionPowerTriggered)
                            {
                                PlayRandomLine("PowerMalfunction", Random.Range(1, 4));
                                return;
                            }
                            CoroutineManager.StartCoroutine(GetScrapLeft(recognized.Message));
                        }
                    }
                });
            }
        }

        internal static void RegisterTeleportCommands()
        {
            if (Plugin.registerTeleporter.Value)
            {
                string[] phrases = Plugin.teleporterCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, tp", "VEGA, activate tp", "VEGA, teleport", "VEGA, activate teleporter" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, tp" && recognized.Message != "VEGA, activate tp" && recognized.Message != "VEGA, teleport" && recognized.Message != "VEGA, activate teleporter") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.teleportConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                    }
                });
            }
            if (Plugin.registerRadarSwitch.Value)
            {
                string[] phrases = Plugin.radarSwitchCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, switch to me", "VEGA, switch radar", "VEGA, switch radar to me", "VEGA, focus", "VEGA, focus on me" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, switch to me" && recognized.Message != "VEGA, switch radar" && recognized.Message != "VEGA, switch radar to me" && recognized.Message != "VEGA, focus" && recognized.Message != "VEGA, focus on me") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.teleportConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
                        {
                            SwitchRadar();
                        }
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
                // Voice.RegisterPhrases(new string[] { "VEGA, open secure door", "VEGA, open door", "VEGA, open the door", "VEGA, open the secure door" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, open secure door" && recognized.Message != "VEGA, open door" && recognized.Message != "VEGA, open the door" && recognized.Message != "VEGA, open the secure door") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.secureDoorsConfidence.Value && listening)
                    {
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
                            OpenSecureDoor();
                        }
                    }
                });
                string[] phrases_1 = Plugin.closeSecureDoorCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, close secure door", "VEGA, close door", "VEGA, close the door", "VEGA, close the secure door" });
                Voice.RegisterPhrases(phrases_1);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, close secure door" && recognized.Message != "VEGA, close door" && recognized.Message != "VEGA, close the door" && recognized.Message != "VEGA, close the secure door") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.secureDoorsConfidence.Value && listening)
                    {
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
                            CloseSecureDoor();
                        }
                    }
                });
            }
            if (Plugin.registerInteractAllSecureDoors.Value)
            {
                string[] phrases = Plugin.openAllSecureDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, open all secure doors", "VEGA, open all doors" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, open all secure doors" && recognized.Message != "VEGA, open all doors") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.secureDoorsConfidence.Value && listening)
                    {
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
                            OpenAllDoors();
                        }
                    }
                });
                string[] phrases_1 = Plugin.closeAllSecureDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, close all secure doors", "VEGA, close all doors" });
                Voice.RegisterPhrases(new string[] { "VEGA, close all secure doors", "VEGA, close all doors" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, close all secure doors" && recognized.Message != "VEGA, close all doors") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.secureDoorsConfidence.Value && listening)
                    {
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
                            CloseAllDoors();
                        }
                    }
                });
            }

            // Ship doors
            if (Plugin.registerInteractShipDoors.Value)
            {
                string[] phrases = Plugin.openShipDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, open ship doors", "VEGA, open the ship's doors", "VEGA, open hangar doors" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, open ship doors" && recognized.Message != "VEGA, open the ship's doors" && recognized.Message != "VEGA, open hangar doors") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.shipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                    }
                });
                string[] phrases_1 = Plugin.closeShipDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, close ship doors", "VEGA, close the ship's doors", "VEGA, close hangar doors" });
                Voice.RegisterPhrases(phrases_1);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, close ship doors" && recognized.Message != "VEGA, close the ship's doors" && recognized.Message != "VEGA, close hangar doors") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.shipConfidence.Value && listening)
                    {
                        if (!StartOfRound.Instance.localPlayerController.isPlayerDead)
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
                    string[] phrases = Plugin.openShipDoorsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    // Voice.RegisterPhrases(new string[] { "VEGA, transmit " + signal, "VEGA, send " + signal });
                    for (int i = 0; i < phrases.Length; i++)
                    {
                        phrases[i] = phrases[i] + " " +  signal;
                    }
                    Voice.RegisterPhrases(phrases);
                    Voice.RegisterCustomHandler((obj, recognized) =>
                    {
                        // if (recognized.Message != "VEGA, transmit " + signal && recognized.Message != "VEGA, send " + signal) return;
                        if (!phrases.Contains(recognized.Message)) return;
                        if (recognized.Confidence > Plugin.signalsConfidence.Value && listening)
                        {
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
                // Voice.RegisterPhrases(new string[] { "VEGA, ping" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, ping") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.miscConfidence.Value && listening)
                    {
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
                            InteractWithBooster(ping: true);
                        }
                    }
                });
                string[] phrases_1 = Plugin.radarFlashCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, flash" });
                Voice.RegisterPhrases(phrases_1);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, flash") return;
                    if (!phrases_1.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.miscConfidence.Value && listening)
                    {
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
                            InteractWithBooster(ping: false);
                        }
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
                // Voice.RegisterPhrases(new string[] { "VEGA, disable the turret", "VEGA, disable turret" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable the turret" && recognized.Message != "VEGA, disable turret") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.turretsConfidence.Value && listening)
                    {
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
                    }
                });
            }
            if (Plugin.registerDisableAllTurrets.Value)
            {
                string[] phrases = Plugin.disableAllTurretsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, disable all turrets" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable all turrets") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.turretsConfidence.Value && listening)
                    {
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
                            turretsExist = false;
                            if (ModChecker.hasToilHead)
                            {
                                DisableAllToils();
                            }
                            DisableAllTurrets();
                        }
                    }
                });
            }

            // Landmines
            if (Plugin.registerDisableMine.Value)
            {
                string[] phrases = Plugin.disableMineCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, disable the mine", "VEGA, disable mine", "VEGA, disable the landmine", "VEGA, disable landmine" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable the mine" && recognized.Message != "VEGA, disable mine" && recognized.Message != "VEGA, disable the landmine" && recognized.Message != "VEGA, disable landmine") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.landminesConfidence.Value && listening)
                    {
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
                            DisableMine();
                        }
                    }
                });
            }
            if (Plugin.registerDisableAllMines.Value)
            {
                string[] phrases = Plugin.disableAllMinesCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, disable all mines", "VEGA, disable all landmines" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable all mines" && recognized.Message != "VEGA, disable all landmines") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.landminesConfidence.Value && listening)
                    {
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
                            DisableAllMines();
                        }
                    }
                });
            }

            // Spike traps
            if (Plugin.registerDisableSpikeTrap.Value)
            {
                string[] phrases = Plugin.disableSpikeTrapCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, disable the trap", "VEGA, disable trap", "VEGA, disable the spike trap", "VEGA, disable spike trap" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable the trap" && recognized.Message != "VEGA, disable trap" && recognized.Message != "VEGA, disable the spike trap" && recognized.Message != "VEGA, disable spike trap") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.trapsConfidence.Value && listening)
                    {
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
                            DisableSpikeTrap();
                        }
                    }
                });
            }
            if (Plugin.registerDisableAllSpikeTraps.Value)
            {
                string[] phrases = Plugin.disableAllSpikeTrapsCommands.Value.Split("/", StringSplitOptions.RemoveEmptyEntries);
                // Voice.RegisterPhrases(new string[] { "VEGA, disable all traps", "VEGA, disable all spike traps" });
                Voice.RegisterPhrases(phrases);
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    // if (recognized.Message != "VEGA, disable all traps" && recognized.Message != "VEGA, disable all spike traps") return;
                    if (!phrases.Contains(recognized.Message)) return;
                    if (recognized.Confidence > Plugin.trapsConfidence.Value && listening)
                    {
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
                            DisableAllSpikeTraps();
                        }
                    }
                });
            }
        }

        internal static void RegisterMoonsInfo()
        {
            if (Plugin.registerMoonsInfo.Value)
            {
                // Vanilla
                Voice.RegisterPhrases(new string[] { "VEGA, info about experimentation" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about experimentation") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("41-EXP");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about assurance" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about assurance") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("220-ASS");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about vow" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about vow") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("56-VOW");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about offense" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about offense") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("21-OFF");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about march" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about march") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("61-MAR");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about adamance" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about adamance") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("20-ADA");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about rend" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about rend") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("85-REN");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about dine" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about dine") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("7-DIN");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about titan" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about titan") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("8-TIT");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about artifice" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about artifice") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("68-ART");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about embrion" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about embrion") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("5-EMB");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about liquidation" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about liquidation") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("44-LIQ");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about mars" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about mars") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("4-MARS");
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about the Company", "VEGA, info about the Company building", "VEGA, info about Gordion" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about the Company" && recognized.Message != "VEGA, info about the Company building" && recognized.Message != "VEGA, info about Gordion") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        PlayLine("71-GOR");
                    }
                });

                // Modded
                /*
                 * Scrapped for now. Will only do vanilla moons in the forseeable future.
                Voice.RegisterPhrases(new string[] { "VEGA, info about Asteroid 13" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Asteroid 13") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("57 Asteroid-13"))
                        {
                            PlayAudio("57-AST-13");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Atlantica" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Atlantica") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("44 Atlantica"))
                        {
                            PlayAudio("44-ATL");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Cosmocos" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Cosmocos") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("42 Cosmocos"))
                        {
                            PlayAudio("42-COS");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Desolation" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Desolation") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("48 Desolation"))
                        {
                            PlayAudio("48-DES");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Etern" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Etern") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("154 Etern"))
                        {
                            PlayAudio("154-ETE");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Fission C" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Fission C") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("25 Fission-C"))
                        {
                            PlayAudio("25-FIS-C");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Gloom" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Gloom") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("36 Gloom"))
                        {
                            PlayAudio("36-GLO");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Gratar" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Gratar") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("147 Gratar"))
                        {
                            PlayAudio("147-GRA");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Infernis" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Infernis") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("46 Infernis"))
                        {
                            PlayAudio("46-INF");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Junic" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Junic") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("84 Junic"))
                        {
                            PlayAudio("84-JUN");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Oldred" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Oldred") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("134 Oldred"))
                        {
                            PlayAudio("134-OLD");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Polarus" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Polarus") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("94 Polarus"))
                        {
                            PlayAudio("94-POL");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Acidir" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Acidir") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("76 Acidir"))
                        {
                            PlayAudio("76-ACI");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Affliction" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Affliction") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("59 Affliction"))
                        {
                            PlayAudio("59-AFF");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Eve M" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Eve M") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("127 Eve-M"))
                        {
                            PlayAudio("127-EVE-M");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Sector 0" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Sector 0") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("71 Sector-0"))
                        {
                            PlayAudio("71-SEC-0");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Summit" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Summit") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("290 Summit"))
                        {
                            PlayAudio("290-SUM");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Penumbra" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Penumbra") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("813 Penumbra"))
                        {
                            PlayAudio("813-PEN");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Argent" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Argent") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("32 Argent"))
                        {
                            PlayAudio("32-ARG");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Azure" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Azure") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("39 Azure"))
                        {
                            PlayAudio("39-AZU");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Budapest" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Budapest") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("618 Budapest"))
                        {
                            PlayAudio("618-BUD");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Celestria" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Celestria") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("9 Celestria"))
                        {
                            PlayAudio("9-CEL");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Crystallum" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Crystallum") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("Crystallum"))
                        {
                            PlayAudio("???-CRY");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Echelon" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Echelon") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("30 Echelon"))
                        {
                            PlayAudio("30-ECH");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Harloth" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Harloth") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("93 Harloth"))
                        {
                            PlayAudio("93-HAR");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Maritopia" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Maritopia") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("153 Maritopia"))
                        {
                            PlayAudio("153-MAR");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Nimbus" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Nimbus") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("Nimbus"))
                        {
                            PlayAudio("???-NIM");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Nyx" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Nyx") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("34 Nyx"))
                        {
                            PlayAudio("34-NYX");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Psych Sanctum" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Psych Sanctum") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("111 PsychSanctum"))
                        {
                            PlayAudio("111-PSY");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Spectralis" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Spectralis") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("Spectralis"))
                        {
                            PlayAudio("???-SPE");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Zenit" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Zenit") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("37 Zenit"))
                        {
                            PlayAudio("37-ZEN");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Calt Prime" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Calt Prime") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("35 CaltPrime"))
                        {
                            PlayAudio("35-CAL");
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Sanguine" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Sanguine") return;
                    if (recognized.Confidence > Plugin.confidence.Value && listening)
                    {
                        if (ClientHasMoon("Sanguine"))
                        {
                            PlayAudio("???-SAN");
                        }
                    }
                });
                */
            }
        }

        internal static void RegisterBestiaryEntries()
        {
            if (Plugin.registerBestiaryEntries.Value)
            {
                // Vanilla
                Voice.RegisterPhrases(new string[] { "VEGA, read Hawk entry", "VEGA, read Baboon entry", "VEGA, read Baboon hawk entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Hawk entry" && recognized.Message != "VEGA, read Baboon entry" && recognized.Message != "VEGA, read Baboon hawk entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(16))
                        {
                            PlayLine("BaboonHawk");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Bunker spider entry", "VEGA, read Spider entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Bunker spider entry" && recognized.Message != "VEGA, read Spider entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(12))
                        {
                            PlayLine("BunkerSpider");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Hoarding bug entry", "VEGA, read Loot bug entry", "VEGA, read Yippee bug entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Hoarding bug entry" && recognized.Message != "VEGA, read Loot bug entry" && recognized.Message != "VEGA, read Yippee bug entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(4))
                        {
                            PlayLine("YippeeBug");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Bracken entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Bracken entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(1))
                        {
                            PlayLine("Bracken");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Butler entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Butler entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(19))
                        {
                            PlayLine("Butler");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Coil head entry", "VEGA, read Coil entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Coil head entry" && recognized.Message != "VEGA, read Coil entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(7))
                        {
                            PlayLine("Coil-Head");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Forest Keeper entry", "VEGA, read Giant entry", "VEGA, read Keeper entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Forest Keeper entry" && recognized.Message != "VEGA, read Giant entry" && recognized.Message != "VEGA, read Keeper entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(6))
                        {
                            PlayLine("ForestKeeper");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Eyeless dog entry", "VEGA, read dog entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Eyeless dog entry" && recognized.Message != "VEGA, read dog entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(3))
                        {
                            PlayLine("EyelessDog");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Earth Leviathan entry", "VEGA, read Leviathan entry", "VEGA, read Worm entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Earth Leviathan entry" && recognized.Message != "VEGA, read Leviathan entry" && recognized.Message != "VEGA, read Worm entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(9))
                        {
                            PlayLine("Sandworm");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Jester entry", "VEGA, read Jack in the box entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Jester entry" && recognized.Message != "VEGA, read Jack in the box entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(10))
                        {
                            PlayLine("Jester");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Roaming locusts entry", "VEGA, read Locusts entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Roaming locusts entry" && recognized.Message != "VEGA, read Locusts entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(15))
                        {
                            PlayLine("Locusts");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Manticoil entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Manticoil entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(13))
                        {
                            PlayLine("Manticoil");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Nutcracker entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Nutcracker entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(17))
                        {
                            PlayLine("Nutcracker");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Old bird entry", "VEGA, read Bird entry", "VEGA, read Mech entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Old bird entry" && recognized.Message != "VEGA, read Bird entry" && recognized.Message != "VEGA, read Mech entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(18))
                        {
                            PlayLine("OldBird");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Circuit bees entry", "VEGA, read Bees entry", "VEGA, read Red Bees entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Circuit bees entry" && recognized.Message != "VEGA, read Bees entry" && recognized.Message != "VEGA, read Red Bees entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(14))
                        {
                            PlayLine("RedBees");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Hygrodere entry", "VEGA, read Slime entry", "VEGA, read Blob entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Hygrodere entry" && recognized.Message != "VEGA, read Slime entry" && recognized.Message != "VEGA, read Blob entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(5))
                        {
                            PlayLine("Slime");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Tulip snake entry", "VEGA, read Tulip entry", "VEGA, read Snake entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Tulip snake entry" && recognized.Message != "VEGA, read Tulip entry" && recognized.Message != "VEGA, read Snake entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(21))
                        {
                            PlayLine("Snakes");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Snare flea entry", "VEGA, read Flea entry", "VEGA, read Centipede entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Snare flea entry" && recognized.Message != "VEGA, read Flea entry" && recognized.Message != "VEGA, read Centipede entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(0))
                        {
                            PlayLine("SnareFlea");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Spore lizard entry", "VEGA, read Lizard entry", "VEGA, read Spore doggy entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Spore lizard entry" && recognized.Message != "VEGA, read Lizard entry" && recognized.Message != "VEGA, read Spore doggy entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(11))
                        {
                            PlayLine("SporeLizard");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Thumper entry", "VEGA, read Crawler entry", "VEGA, read Halve entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Thumper entry" && recognized.Message != "VEGA, read Crawler entry" && recognized.Message != "VEGA, read Halve entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(2))
                        {
                            PlayLine("Thumper");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });

                // Modded
                Voice.RegisterPhrases(new string[] { "VEGA, read Redwood entry", "VEGA, read Redwood Giant entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Redwood entry" && recognized.Message != "VEGA, read Redwood Giant entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("RedWood Giant")).creatureFileID))
                            {
                                PlayLine("RedWood Giant");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Driftwood entry", "VEGA, read Driftwood Giant entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Driftwood entry" && recognized.Message != "VEGA, read Driftwood Giant entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("DriftWood Giant")).creatureFileID))
                            {
                                PlayLine("DriftWood Giant");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Slender entry", "VEGA, read Slenderman entry", "VEGA, read Faceless Stalker entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Slender entry" && recognized.Message != "VEGA, read Slenderman entry" && recognized.Message != "VEGA, read Faceless Stalker entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Stalker")).creatureFileID))
                            {
                                PlayLine("Stalker");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Football entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Football entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Football")).creatureFileID))
                            {
                                PlayLine("Football");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Shy guy entry", "VEGA, read SCP-096 entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Shy guy entry" && recognized.Message != "VEGA, read SCP-096 entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Shy guy")).creatureFileID))
                            {
                                PlayLine("Shy guy");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Locker entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Locker entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Locker")).creatureFileID))
                            {
                                PlayLine("Locker");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Siren Head entry", "VEGA, read Sirenhead entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Siren Head entry" && recognized.Message != "VEGA, read Sirenhead entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Siren Head")).creatureFileID))
                            {
                                PlayLine("Siren Head");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Rolling Giant entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Rolling Giant entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Rolling Giant")).creatureFileID))
                            {
                                PlayLine("Rolling Giant");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Peepers entry", "VEGA, read Peeper entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Peepers entry" && recognized.Message != "VEGA, read Peeper entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Peepers")).creatureFileID))
                            {
                                PlayLine("Peepers");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Shockwave drone entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Shockwave drone entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Shockwave Drone")).creatureFileID))
                            {
                                PlayLine("Shockwave Drone");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Cleaning drone entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Cleaning drone entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Peepers")).creatureFileID))
                            {
                                PlayLine("Cleaning Drone");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Moving turret entry", "VEGA, read Mobile turret entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Moving turret entry" && recognized.Message != "VEGA, read Mobile turret entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Moving Turret")).creatureFileID))
                            {
                                PlayLine("Moving Turret");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read The Lost entry", "VEGA, read Maggie entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read The Lost entry" && recognized.Message != "VEGA, read Maggie entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Maggie")).creatureFileID))
                            {
                                PlayLine("Maggie");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, read Shrimp entry" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, read Shrimp entry") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Shrimp")).creatureFileID))
                            {
                                PlayLine("Shrimp");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(1, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(1, 5));
                        }
                    }
                });
            }
        }

        internal static void RegisterEntityInfo()
        {
            if (Plugin.registerCreatureInfo.Value)
            {
                // Vanilla
                Voice.RegisterPhrases(new string[] { "VEGA, info about Hawks", "VEGA, info about Baboons", "VEGA, info about Baboon hawks" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Hawks" && recognized.Message != "VEGA, info about Baboons" && recognized.Message != "VEGA, info about Baboon hawks") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(16))
                        {
                            PlayLine("BaboonHawkShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Bunker spiders", "VEGA, info about Spiders" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Bunker spiders" && recognized.Message != "VEGA, info about Spiders") return;
                    if (recognized.Confidence >= Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(12))
                        {
                            PlayLine("BunkerSpiderShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Hoarding bugs", "VEGA, info about Loot bugs", "VEGA, info about Yippee bugs" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Hoarding bugs" && recognized.Message != "VEGA, info about Loot bugs" && recognized.Message != "VEGA, info about Yippee bugs") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(4))
                        {
                            PlayLine("YippeeBugShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Brackens", "VEGA, info about the Bracken" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Brackens" && recognized.Message != "VEGA, info about the Bracken") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(1))
                        {
                            PlayLine("BrackenShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Butlers" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Butlers") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(19))
                        {
                            PlayLine("ButlerShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Coil heads", "VEGA, info about Coils" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Coil heads" && recognized.Message != "VEGA, info about Coils") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(7))
                        {
                            PlayLine("Coil-HeadShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Forest Keepers", "VEGA, info about Giants", "VEGA, info about Keepers" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Forest Keepers" && recognized.Message != "VEGA, info about Giants" && recognized.Message != "VEGA, info about Keepers") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(6))
                        {
                            PlayLine("ForestKeeperShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Eyeless dogs", "VEGA, info about dogs" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Eyeless dogs" && recognized.Message != "VEGA, info about dogs") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(3))
                        {
                            PlayLine("EyelessDogShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Earth Leviathans", "VEGA, info about Leviathans", "VEGA, info about Worms" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Earth Leviathans" && recognized.Message != "VEGA, info about Leviathans" && recognized.Message != "VEGA, info about Worms") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(9))
                        {
                            PlayLine("SandwormShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Jesters", "VEGA, info about the Jack in the box" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Jesters" && recognized.Message != "VEGA, info about the Jack in the box") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(10))
                        {
                            PlayLine("JesterShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Roaming locusts", "VEGA, info about Locusts" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Roaming locusts" && recognized.Message != "VEGA, info about Locusts") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(15))
                        {
                            PlayLine("LocustsShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Manticoils" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Manticoils") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(13))
                        {
                            PlayLine("ManticoilShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Nutcrackers" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Nutcrackers") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(17))
                        {
                            PlayLine("NutcrackerShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Old birds", "VEGA, info about Birds", "VEGA, info about Mechs" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Old birds" && recognized.Message != "VEGA, info about Birds" && recognized.Message != "VEGA, info about Mechs") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(18))
                        {
                            PlayLine("OldBirdShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Circuit bees", "VEGA, info about Bees", "VEGA, info about Red Bees" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Circuit bees" && recognized.Message != "VEGA, info about Bees" && recognized.Message != "VEGA, info about Red Bees") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(14))
                        {
                            PlayLine("RedBeesShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Hygroderes", "VEGA, info about Slimes", "VEGA, info about Blobs" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Hygroderes" && recognized.Message != "VEGA, info about Slimes" && recognized.Message != "VEGA, info about Blobs") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(5))
                        {
                            PlayLine("SlimeShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Tulip snakes", "VEGA, info about Tulips", "VEGA, info about Snakes" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Tulip snakes" && recognized.Message != "VEGA, info about Tulips" && recognized.Message != "VEGA, info about Snakes") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(21))
                        {
                            PlayLine("SnakesShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Snare fleas", "VEGA, info about Fleas", "VEGA, info about Centipedes" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Snare fleas" && recognized.Message != "VEGA, info about Fleas" && recognized.Message != "VEGA, info about Centipedes") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(0))
                        {
                            PlayLine("SnareFleaShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Spore lizards", "VEGA, info about Lizards", "VEGA, info about Spore doggies" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Spore lizards" && recognized.Message != "VEGA, info about Lizards" && recognized.Message != "VEGA, info about Spore doggies") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(11))
                        {
                            PlayLine("SporeLizardShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Thumpers", "VEGA, info about Crawlers", "VEGA, info about Halves" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Thumpers" && recognized.Message != "VEGA, info about Crawlers" && recognized.Message != "VEGA, info about Halves") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyIDs.Contains(2))
                        {
                            PlayLine("ThumperShort");
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });

                // Modded
                Voice.RegisterPhrases(new string[] { "VEGA, info about Redwoods", "VEGA, info about Redwood Giants" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Redwoods" && recognized.Message != "VEGA, info about Redwood Giants") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("RedWood Giant")).creatureFileID))
                            {
                                PlayLine("RedWood GiantShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Driftwoods", "VEGA, info about Driftwood Giants" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Driftwoods" && recognized.Message != "VEGA, info about Driftwood Giants") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("DriftWood Giant")).creatureFileID))
                            {
                                PlayLine("DriftWood GiantShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Slender", "VEGA, info about Slenderman", "VEGA, info about the Faceless Stalker" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Slender" && recognized.Message != "VEGA, info about Slenderman" && recognized.Message != "VEGA, info about the Faceless Stalker") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Stalker")).creatureFileID))
                            {
                                PlayLine("StalkerShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Football" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Football") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Football")).creatureFileID))
                            {
                                PlayLine("FootballShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Shy guy", "VEGA, info about SCP-096" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Shy guy" && recognized.Message != "VEGA, info about SCP-096") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        if (TerminalPatch.scannedEnemyFiles.Any(file => file.creatureName.Equals("Shy guy")))
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Shy guy")).creatureFileID))
                            {
                                PlayLine("Shy guyShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        else
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about the Locker", "VEGA, info about Lockers" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about the Locker" && recognized.Message != "VEGA, info about Lockers") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Locker")).creatureFileID))
                            {
                                PlayLine("LockerShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Siren Head", "VEGA, info about Sirenhead" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Siren Head" && recognized.Message != "VEGA, info about Sirenhead") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Siren Head")).creatureFileID))
                            {
                                PlayLine("Siren HeadShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about the Rolling Giant", "VEGA, info about Rolling Giants" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about the Rolling Giant" && recognized.Message != "VEGA, info about Rolling Giants") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Rolling Giant")).creatureFileID))
                            {
                                PlayLine("Rolling GiantShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Peepers" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Peepers") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Peepers")).creatureFileID))
                            {
                                PlayLine("PeepersShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Shockwave drones" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Shockwave drones") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Shockwave Drone")).creatureFileID))
                            {
                                PlayLine("Shockwave DroneShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Cleaning drones" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Cleaning drones") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Peepers")).creatureFileID))
                            {
                                PlayLine("Cleaning DroneShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Moving turrets", "VEGA, info about Mobile turrets" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Moving turrets" && recognized.Message != "VEGA, info about Mobile turrets") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Moving Turret")).creatureFileID))
                            {
                                PlayLine("Moving TurretShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about The Lost", "VEGA, info about Maggie" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about The Lost" && recognized.Message != "VEGA, info about Maggie") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Maggie")).creatureFileID))
                            {
                                PlayLine("MaggieShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
                Voice.RegisterPhrases(new string[] { "VEGA, info about Shrimps" });
                Voice.RegisterCustomHandler((obj, recognized) =>
                {
                    if (recognized.Message != "VEGA, info about Shrimps") return;
                    if (recognized.Confidence > Plugin.infoConfidence.Value && listening)
                    {
                        try
                        {
                            if (TerminalPatch.scannedEnemyIDs.Contains(TerminalPatch.scannedEnemyFiles.Find(file => file.creatureName.Equals("Shrimp")).creatureFileID))
                            {
                                PlayLine("ShrimpShort");
                            }
                            else
                            {
                                PlayRandomLine("NoEntityData", Random.Range(2, 5));
                            }
                        }
                        catch (System.Exception)
                        {
                            PlayRandomLine("NoEntityData", Random.Range(2, 5));
                        }
                    }
                });
            }
        }
    }
}
