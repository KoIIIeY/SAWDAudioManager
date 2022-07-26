using System;
using System.Collections;
using System.Collections.Generic;
using SAWDAudio;
using SAWDAudio.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefToggle : MonoBehaviour {
    private Toggle toggle;
    public AudioPackage AudioPackage;

    private void Start() {
        // Debug.Log("PlayerPrefToggle::Start(); --");
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
        StartCoroutine(StartCort());
    }

    public void SetAudioPackage(AudioPackage audioPackage) {
        // Debug.Log("PlayerPrefToggle::SetAudioPackage(); -- audioPackage:" + audioPackage);
        AudioPackage = audioPackage;
    }

    private IEnumerator StartCort() {
        // Debug.Log("PlayerPrefToggle::StartCort(); -1- toggle:" + toggle + ",AudioPackage:" + AudioPackage);
        while (true) {
            if (toggle != null && AudioPackage != null) {
                toggle.isOn = PlayerPrefs.HasKey("audio_package_id:" + AudioPackage.audio_package_id);
                // Debug.Log("PlayerPrefToggle::StartCort(); -2- toggle:" + toggle + ",toggle.isOn:" + toggle.isOn + ",AudioPackage:" + AudioPackage);
                yield break;
                // break;
            }
            // yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(1f);
            // Debug.Log("PlayerPrefToggle::StartCort(); -3- toggle:" + toggle + ",AudioPackage:" + AudioPackage);
        }
        // Debug.Log("PlayerPrefToggle::StartCort(); -4- toggle:" + toggle + ",AudioPackage:" + AudioPackage);
    }

    private void OnValueChanged(bool value) {
        // Debug.Log("PlayerPrefToggle::OnValueChanged(); -- value:" + value);
        if (value) {
            PlayerPrefs.SetString("audio_package_id:" + AudioPackage.audio_package_id, "_");
        } else {
            PlayerPrefs.DeleteKey("audio_package_id:" + AudioPackage.audio_package_id);
        }
    }
}
