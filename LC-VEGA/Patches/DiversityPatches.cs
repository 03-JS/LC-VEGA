using Diversity;
using HarmonyLib;
using UnityEngine;
using static Diversity.DiversitySoundManager;

namespace LC_VEGA.Patches
{
    internal class DiversityPatches
    {
        [HarmonyPatch(typeof(DiversitySoundManager), "GetRandomAudioClipByTypeAndId")]
        [HarmonyPostfix]
        static void ReplyToSpeaker(SoundType soundType, int id)
        {
            DiversityAssets.RandomSoundAudioClip[] randomSoundAudioClipArray = DiversityAssets.Instance.randomSoundAudioClipArray;
            foreach (DiversityAssets.RandomSoundAudioClip randomSoundAudioClip in randomSoundAudioClipArray)
            {
                if (randomSoundAudioClip.soundType == soundType)
                {
                    int randomNumber = Random.Range(0, 10);
                    switch (randomSoundAudioClip.audioClips[id].name)
                    {
                        case "Welcome_1":
                        case "Welcome_2":
                            if (SaveManager.firstTimeDiversity)
                            {
                                SaveManager.firstTimeDiversity = false;
                                VEGA.PlayAudio("WelcomeReply", randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            break;
                        case "Reaching_Quota":
                        case "Reaching_Quota_2":
                        case "Reaching_Quota_Again":
                        case "Reaching_Quota_Again_2":
                            if (SaveManager.firstTimeDiversity)
                            {
                                SaveManager.firstTimeDiversity = false;
                                VEGA.PlayAudio("FirstTimeReply", randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            else if (randomNumber <= 2)
                            {
                                VEGA.PlayAudioWithVariant("Reply", Random.Range(1, 6), randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            break;
                        case "Terminal_Error_2":
                        case "Terminal_Error_No_Laugh":
                        case "Terminal_Error_With_Laugh":
                            if (SaveManager.firstTimeDiversity)
                            {
                                SaveManager.firstTimeDiversity = false;
                                VEGA.PlayAudio("FirstTimeReply", randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            else if (randomNumber <= 2)
                            {
                                VEGA.PlayAudioWithVariant("Reply", Random.Range(1, 6), randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            break;
                        case "Turning_Off_Speaker_2":
                        case "Attempt_To_Shut_Off_Speaker":
                            if (randomNumber <= 4)
                            {
                                VEGA.PlayAudioWithVariant("SilenceReply", Random.Range(1, 4), randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            break;
                        case "Dog_Spawn_Or_When_Close_to_Ship":
                            if (randomNumber <= 4)
                            {
                                VEGA.PlayAudio("DogsReply", randomSoundAudioClip.audioClips[id].length + 0.25f);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
