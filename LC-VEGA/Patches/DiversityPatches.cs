using HarmonyLib;
using UnityEngine;
using Diversity;
using static Diversity.DiversitySoundManager;
using System.Collections;

namespace LC_VEGA.Patches
{
    internal class DiversityPatches
    {
        public static bool firstTimeWelcome = true;
        public static bool firstTimeReply = true;

        internal static IEnumerator PlayWelcomeReply(AudioClip shipIntroSpeechSFX)
        {
            yield return new WaitForSeconds(5.5f);

            QuickMenuManager quickMenu = Object.FindObjectOfType<QuickMenuManager>();

            yield return new WaitUntil(() => !quickMenu.isMenuOpen);
            yield return new WaitForSeconds(0.2f);

            VEGA.PlayLine("WelcomeReply", shipIntroSpeechSFX.length + 0.25f, false);

            yield return new WaitForSeconds(shipIntroSpeechSFX.length + 0.25f);

            firstTimeWelcome = false;
        }

        [HarmonyPatch(typeof(StartOfRound), "PlayFirstDayShipAnimation")]
        [HarmonyPostfix]
        static void ReplyToIntro(StartOfRound __instance)
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.diversitySpeaker.Value && __instance.shipIntroSpeechSFX.name.Contains("Welcome_"))
            {
                if (SaveManager.firstTimeDiversity && firstTimeWelcome)
                {
                    CoroutineManager.StartCoroutine(PlayWelcomeReply(__instance.shipIntroSpeechSFX));
                }
            }
        }

        [HarmonyPatch(typeof(DiversitySoundManager), "GetRandomAudioClipByType")]
        [HarmonyPostfix]
        static void ReplyToSpeakerByTypeRandom(SoundType soundType, AudioClip __result)
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.diversitySpeaker.Value)
            {
                int randomNumber = Random.Range(0, 10);
                switch (soundType)
                {
                    case SoundType.SpeakerDogs:
                        VEGA.audioSource.Stop();
                        if (randomNumber <= 6)
                        {
                            VEGA.PlayLine("DogsReply", __result.length + 0.25f);
                        }
                        break;
                    case SoundType.SpeakerShutOff:
                        VEGA.audioSource.Stop();
                        if (firstTimeWelcome && SaveManager.firstTimeDiversity)
                        {
                            VEGA.PlayLine("WelcomeReply", __result.length + 0.25f);
                        }
                        if (randomNumber <= 6)
                        {
                            if (__result.name.Equals("Turning_Off_Speaker_2") && randomNumber <= 4)
                            {
                                VEGA.PlayLine("Reply-4", __result.length + 0.25f);
                            }
                            VEGA.PlayRandomLine("SilenceReply", Random.Range(1, 4), __result.length + 0.25f);
                        }
                        break;
                    case SoundType.SpeakerQuota:
                    case SoundType.SpeakerQuotaAgain:
                        VEGA.audioSource.Stop();
                        if (SaveManager.firstTimeDiversity && firstTimeReply)
                        {
                            firstTimeReply = false;
                            VEGA.PlayLine("FirstTimeReply", __result.length + 0.25f);
                        }
                        else if (randomNumber <= 4)
                        {
                            VEGA.PlayRandomLine("Reply", Random.Range(1, 6), __result.length + 0.25f);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(DiversitySoundManager), "GetRandomAudioClipByTypeAndId")]
        [HarmonyPostfix]
        static void ReplyToSpeakerByTypeAndId(SoundType soundType, int id, AudioClip __result)
        {
            if (Plugin.vocalLevel.Value >= VocalLevels.Low && Plugin.diversitySpeaker.Value)
            {
                int randomNumber = Random.Range(0, 10);
                switch (soundType)
                {
                    case SoundType.SpeakerTerminal:
                        VEGA.audioSource.Stop();
                        if (SaveManager.firstTimeDiversity && firstTimeReply)
                        {
                            firstTimeReply = false;
                            VEGA.PlayLine("FirstTimeReply", __result.length + 0.25f);
                        }
                        else if (randomNumber <= 4)
                        {
                            VEGA.PlayRandomLine("Reply", Random.Range(1, 6), __result.length + 0.25f);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDay), "SyncNewProfitQuotaClientRpc")]
        [HarmonyPostfix]
        static void SetDiversitySaveValue()
        {
            if (!firstTimeWelcome || !firstTimeReply)
            {
                SaveManager.firstTimeDiversity = false;
            }
        }
    }
}
