using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

namespace LC_VEGA.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {
        [HarmonyPatch("MoveTimeOfDay")]
        [HarmonyPostfix]
        static void Give8pmWarning(ref int ___hour)
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                if (___hour == 14 || ___hour == 15)
                {
                    if (!VEGA.warningGiven)
                    {
                        if (GameNetworkManager.Instance.localPlayerController.isInsideFactory)
                        {
                            if (!VEGA.audioSource.isPlaying)
                            {
                                VEGA.warningGiven = true;
                            }
                            VEGA.PlayAudioWithVariant("GettingLate", Random.Range(1, 4));
                        }
                        else
                        {
                            VEGA.warningGiven = true;
                        }
                    }
                }
                else
                {
                    VEGA.warningGiven = false;
                }
            }
        }

        [HarmonyPatch("SetShipLeaveEarlyClientRpc")]
        [HarmonyPostfix]
        static void GiveVoteWarning()
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low)
            {
                VEGA.PlayAudio("ShipLeavingEarly");
            }
        }
    }
}
