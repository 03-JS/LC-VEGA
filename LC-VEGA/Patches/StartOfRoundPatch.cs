using HarmonyLib;
using TMPro;
using UnityEngine;

namespace LC_VEGA.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start(StartOfRound __instance)
        {
            Plugin.LogToConsole("Creating the VEGA audio sources");

            GameObject audioGameObject = new GameObject("VEGA");
            VEGA.audioSource = audioGameObject.AddComponent<AudioSource>();
            if (VEGA.audioSource != null)
            {
                Plugin.LogToConsole("VEGA audio source created successfully");
                VEGA.audioSource.volume = Plugin.volume.Value;
                VEGA.audioSource.ignoreListenerVolume = Plugin.ignoreMasterVolume.Value;
                VEGA.audioSource.playOnAwake = false;
                VEGA.audioSource.bypassEffects = true;
                VEGA.audioSource.bypassListenerEffects = true;
                VEGA.audioSource.bypassReverbZones = true;
            }
            else
            {
                Plugin.LogToConsole("Unable to create VEGA audio source", "error");
            }

            GameObject sfxGameObject = new GameObject("VEGA-SFXs");
            VEGA.sfxAudioSource = sfxGameObject.AddComponent<AudioSource>();
            if (VEGA.sfxAudioSource != null)
            {
                Plugin.LogToConsole("VEGA-SFXs audio source created successfully");
                VEGA.sfxAudioSource.volume = Plugin.volume.Value;
                VEGA.sfxAudioSource.ignoreListenerVolume = Plugin.ignoreMasterVolume.Value;
                VEGA.sfxAudioSource.playOnAwake = false;
                VEGA.sfxAudioSource.bypassEffects = true;
                VEGA.sfxAudioSource.bypassListenerEffects = true;
                VEGA.sfxAudioSource.bypassReverbZones = true;
                VEGA.sfxAudioSource.clip = VEGA.audioClips.Find(clip => clip.name == "Deactivate");

            }
            else
            {
                Plugin.LogToConsole("Unable to create VEGA SFXs audio source", "error");
            }

            if (Plugin.useManualListening.Value && VEGA.listening)
            {
                VEGA.PlaySFX("Activate", 3.5f, false);
            }
            if (Plugin.playIntro.Value && !SaveManager.playedIntro)
            {
                if (Plugin.vocalLevel.Value >= VocalLevels.Low)
                {
                    VEGA.PlayLine("Intro", 4.5f, false);
                    SaveManager.playedIntro = true;
                }
            }
            VEGA.creditsChar = HUDManager.Instance.totalValueText.text.ToCharArray()[0];

            InstantiateAdvancedScannerItems();
        }

        internal static GameObject? ConfigureScannerObjs(HUDManager hudManager, GameObject parent, Vector2 pos, TextAlignmentOptions alignment, float yPos = -25f, string defaultText = "")
        {
            if (parent == null)
            {
                Plugin.LogToConsole("Parent object not found", "error");
                return null;
            }

            GameObject obj = new GameObject("VEGAScannerTextObject");
            obj.transform.SetParent(parent.transform, false); // Set the parent
            TextMeshProUGUI textComponent = obj.AddComponent<TextMeshProUGUI>();

            // Get the weightCounter to clone properties
            TextMeshProUGUI weightText = hudManager.weightCounter;
            textComponent.font = weightText.font;
            textComponent.fontSize = weightText.fontSize;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Left;
            textComponent.enableAutoSizing = weightText.enableAutoSizing;
            textComponent.fontSizeMin = weightText.fontSizeMin;
            textComponent.fontSizeMax = weightText.fontSizeMax;

            // Clone material properties
            if (weightText.fontMaterial != null)
            {
                textComponent.fontSharedMaterial = new Material(weightText.fontMaterial);
            }

            // Apply rotation from weightCounter's parent to textComponent
            if (weightText.transform.parent != null)
            {
                RectTransform weightCounterParentRect = weightText.transform.parent.GetComponent<RectTransform>();
                if (weightCounterParentRect != null)
                {
                    RectTransform textComponentRect = textComponent.GetComponent<RectTransform>();
                    textComponentRect.localRotation = Quaternion.Euler(weightCounterParentRect.localRotation.x, yPos, weightCounterParentRect.localRotation.z);
                }
            }

            // Configure RectTransform for positioning
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);

            // Move the text to the position we want
            rectTransform.anchoredPosition = pos;

            // Set the default text & its alingment
            textComponent.SetText(defaultText);
            textComponent.alignment = alignment;
            return obj;
        }

        internal static void InstantiateAdvancedScannerItems()
        {
            HUDManager hudManager = HUDManager.Instance;
            GameObject parent = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner");
            float configScale = Plugin.scale.Value;
            Vector3 customScale = parent.transform.localScale * configScale;
            float xPos = Plugin.horizontalPosition.Value;
            float yEnemies = Plugin.verticalPosition.Value;
            float entitiesTilt = Plugin.entitiesTilt.Value;
            float itemsTilt = Plugin.itemsTilt.Value;
            if (ModChecker.hasEladsHUD)
            {
                customScale = GameObject.Find("PlayerInfo(Clone)").transform.localScale * configScale;
                parent = GameObject.Find("CinematicGraphics").transform.parent.gameObject;
                xPos = Plugin.horizontalPosition.Value - 12f; // 33f default
                entitiesTilt = Plugin.entitiesTilt.Value - 17f; // 5f default
                itemsTilt = Plugin.itemsTilt.Value - 17f; // 5f default
                yEnemies = Plugin.verticalPosition.Value - 40f; // 140 base default
            }
            float yItems = yEnemies + Plugin.verticalGap.Value * customScale.y; // 50f default gap

            HUDManagerPatch.entities = ConfigureScannerObjs(hudManager, parent, new Vector2(xPos, -yEnemies), Plugin.entitiesAlignment.Value, -entitiesTilt);
            HUDManagerPatch.entities.transform.localScale = ModChecker.hasEladsHUD ? customScale * 1.15f : customScale;
            HUDManagerPatch.items = ConfigureScannerObjs(hudManager, parent, new Vector2(xPos + Plugin.horizontalGap.Value, -yItems), Plugin.itemsAlignment.Value, -itemsTilt);
            HUDManagerPatch.items.transform.localScale = ModChecker.hasEladsHUD ? customScale * 1.15f : customScale;

            float entitiesLength = ModChecker.hasEladsHUD ? Plugin.entitiesTextLength.Value + 100f : Plugin.entitiesTextLength.Value;
            HUDManagerPatch.entities.GetComponent<RectTransform>().sizeDelta = new Vector2(entitiesLength, HUDManagerPatch.entities.GetComponent<RectTransform>().sizeDelta.y);
            float itemsLength = ModChecker.hasEladsHUD ? Plugin.itemsTextLength.Value + 100f : Plugin.itemsTextLength.Value;
            HUDManagerPatch.items.GetComponent<RectTransform>().sizeDelta = new Vector2(itemsLength, HUDManagerPatch.items.GetComponent<RectTransform>().sizeDelta.y);

            VEGA.entitiesTextComponent = HUDManagerPatch.entities.GetComponent<TextMeshProUGUI>();
            VEGA.itemsTextComponent = HUDManagerPatch.items.GetComponent<TextMeshProUGUI>();
        }

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPrefix]
        static void ResetMalfunctionWarnings()
        {
            if (ModChecker.hasMalfunctions)
            {
                VEGA.ResetMalfunctionValues();
            }
        }

        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static void StopMeltdownCountdown()
        {
            if (ModChecker.hasFacilityMeltdown)
            {
                FacilityMeltdownPatches.index = 0;
                FacilityMeltdownPatches.condition = 60;
                FacilityMeltdownPatches.playedCountdown = true;
                if (VEGA.audioSource.clip == null) return;
                if (VEGA.audioSource.clip.name == "Countdown" && VEGA.audioSource.isPlaying)
                {
                    VEGA.audioSource.Stop();
                }
            }
        }
    }
}
