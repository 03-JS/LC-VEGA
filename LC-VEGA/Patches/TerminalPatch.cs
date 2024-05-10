using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LC_VEGA.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        public static List<int> scannedEnemyIDs;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void GetLists(ref List<int> ___scannedEnemyIDs)
        {
            scannedEnemyIDs = ___scannedEnemyIDs;
        }

        [HarmonyPatch("QuitTerminal")]
        [HarmonyPostfix]
        static void StopAudio(ref TerminalNode ___currentNode)
        {
            if (___currentNode.creatureFileID != -1)
            {
                VEGA.audioSource.Stop();
                VEGA.shouldBeInterrupted = false;
            }
        }

        [HarmonyPatch("LoadNewNode")]
        [HarmonyPostfix]
        static void LoadNodePatch(ref TerminalNode ___currentNode)
        {
            if (VEGA.shouldBeInterrupted)
            {
                VEGA.audioSource.Stop();
                VEGA.shouldBeInterrupted = false;
            }
            if (___currentNode.creatureFileID != -1 && Plugin.readBestiaryEntries.Value)
            {
                Plugin.LogToConsole("CREATURE FILE ID -> " + ___currentNode.creatureFileID, "warn");
                VEGA.shouldBeInterrupted = true;
                VEGA.audioSource.Stop();
                Plugin.LogToConsole("Attempting to play an audio from the bestiary -> " + ___currentNode.creatureName);
                float delay = 0.85f;
                VEGA.PlayAudio(VEGA.enemyList[___currentNode.creatureFileID], delay);
            }
        }
    }
}
