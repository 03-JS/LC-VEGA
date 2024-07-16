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
            float xPos = Plugin.xPosition.Value;
            float yEnemies = Plugin.yPosition.Value;
            float yItems = yEnemies + 50 * customScale.y;
            float yRot = Plugin.tilt.Value;
            if (ModChecker.hasEladsHUD)
            {
                customScale = GameObject.Find("PlayerInfo(Clone)").transform.localScale * configScale;
                parent = GameObject.Find("CinematicGraphics").transform.parent.gameObject;
                xPos = Plugin.xPosition.Value - 12f; // 33f default
                yRot = Plugin.tilt.Value - 17f; // 5f default
                yEnemies = Plugin.yPosition.Value - 40f; // 140 base default
                yItems = yEnemies + 50 * customScale.y; // 190 base default
            }
            
            HUDManagerPatch.enemies = ConfigureScannerObjs(hudManager, parent, new Vector2(xPos, -yEnemies), Plugin.alignment.Value, -yRot);
            HUDManagerPatch.enemies.transform.localScale = customScale;
            HUDManagerPatch.items = ConfigureScannerObjs(hudManager, parent, new Vector2(xPos, -yItems), Plugin.alignment.Value, -yRot);
            HUDManagerPatch.items.transform.localScale = customScale;

            if (ModChecker.hasEladsHUD)
            {
                HUDManagerPatch.enemies.transform.localScale = customScale * 1.15f;
                HUDManagerPatch.enemies.GetComponent<RectTransform>().sizeDelta = new Vector2(400f, 45f);
                HUDManagerPatch.items.transform.localScale = customScale * 1.15f;
                HUDManagerPatch.items.GetComponent<RectTransform>().sizeDelta = new Vector2(400f, 35f);
            }

            VEGA.enemiesText = HUDManagerPatch.enemies.GetComponent<TextMeshProUGUI>();
            VEGA.itemsText = HUDManagerPatch.items.GetComponent<TextMeshProUGUI>();
        }

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPrefix]
        static void ResetMalfunctionWarnings()
        {
            if (ModChecker.hasMalfunctions)
            {
                Plugin.LogToConsole("Resetting malfunction values", "debug");

                VEGA.malfunctionPowerTriggered = false;
                VEGA.malfunctionTeleporterTriggered = false;
                VEGA.malfunctionDistortionTriggered = false;
                VEGA.malfunctionDoorTriggered = false;

                MalfunctionsPatches.playPowerWarning = true;
                MalfunctionsPatches.playTpWarning = true;
                MalfunctionsPatches.playCommsWarning = true;
                MalfunctionsPatches.playDoorWarning = true;
                MalfunctionsPatches.playLeverWarning = true;
            }
        }
    }
}
