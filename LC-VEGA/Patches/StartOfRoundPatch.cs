﻿using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                Plugin.LogToConsole("VEGA SFXs audio source created successfully");
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
                }
            }
            VEGA.creditsChar = HUDManager.Instance.totalValueText.text.ToCharArray()[0];

            InstantiateAdvancedScannerItems();
        }

        internal static GameObject? ConfigureScannerObjs(HUDManager hudManager, GameObject topLeftCorner, Vector2 pos, float yPos = -25f, string defaultText = "")
        {
            if (topLeftCorner == null)
            {
                Plugin.LogToConsole("'TopLeftCorner' not found", "error");
                return null;
            }

            GameObject obj = new GameObject("VEGAScannerTextObject");
            obj.transform.SetParent(topLeftCorner.transform, false); // Parent to 'TopLeftCorner'
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

            // Set the default text
            textComponent.SetText(defaultText);
            return obj;
        }

        internal static void InstantiateAdvancedScannerItems()
        {
            HUDManager hudManager = HUDManager.Instance;
            GameObject topLeftCorner = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner");

            HUDManagerPatch.enemies = ConfigureScannerObjs(hudManager, topLeftCorner, new Vector2(45, -180), -22f);
            HUDManagerPatch.items = ConfigureScannerObjs(hudManager, topLeftCorner, new Vector2(45, -230), -22f);

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
