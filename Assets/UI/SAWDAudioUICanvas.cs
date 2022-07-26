using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAWDAudio;
using SAWDAudio.Models;
using UnityEngine;
using UnityEngine.UI;

public class SAWDAudioUICanvas : MonoBehaviour {
    private readonly List<(Toggle, AudioPackage)> _toggles = new List<(Toggle, AudioPackage)>();
    private readonly Dictionary<int, AudioPackage> _audioPackages = new Dictionary<int, AudioPackage>();
    
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject treeViewElementPrefab;
    [SerializeField] private Transform container;

    public void UpdateUI(AudioPackagesList audioPackagesList) {
        Debug.Log("SAWDAudioUICanvas::UpdateUI(); -- audioPackagesList:" + audioPackagesList);
        foreach (var package in audioPackagesList.data) {
            if (package.base_audio_package_id == 0) {
                _audioPackages.Add(package.audio_package_id, package);
            } else {
                if (_audioPackages.ContainsKey(package.base_audio_package_id)) {
                    _audioPackages[package.base_audio_package_id].childsList.Add(package);
                } else {
                    
                }
            }
        }

        foreach (AudioPackage baseAudioPackage in _audioPackages.Values) {
            var go = Instantiate(treeViewElementPrefab, container, false);
            var treeViewElement = go.GetComponent<TreeViewElement>();
            treeViewElement.buttonsPanel.SetActive(false);
            _toggles.Add((treeViewElement.rootToggle, baseAudioPackage));
            treeViewElement.textLabel.text = baseAudioPackage.name;
            PlayerPrefToggle playerPrefToggle = treeViewElement.GetComponentInChildren<Toggle>().gameObject.AddComponent<PlayerPrefToggle>();
            playerPrefToggle.SetAudioPackage(baseAudioPackage);

            if (baseAudioPackage.childsList.Count != 0) {
                foreach (AudioPackage childAudioPackage in baseAudioPackage.childsList) {
                    go = Instantiate(treeViewElementPrefab, treeViewElement.childContentPanel.transform, false);
                    treeViewElement = go.GetComponent<TreeViewElement>();
                    _toggles.Add((treeViewElement.rootToggle, childAudioPackage));
                    treeViewElement.textLabel.text = childAudioPackage.name;
                    RectTransform rectTransform = treeViewElement.textLabel.gameObject.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 0.8f, rectTransform.sizeDelta.y);
                    playerPrefToggle = treeViewElement.GetComponentInChildren<Toggle>().gameObject.AddComponent<PlayerPrefToggle>();
                    playerPrefToggle.SetAudioPackage(childAudioPackage);
                }
            } else {
                treeViewElement.childContentPanel.SetActive(false);
            }
        }
        container.GetComponent<VerticalLayoutGroup>().enabled = true;
        RectTransform uitransform = container.GetComponent<RectTransform>();
        uitransform.anchorMin = new Vector2(0.5f, 1);
        uitransform.anchorMax = new Vector2(0.5f, 1);
        uitransform.pivot = new Vector2(0.5f, 1);
    }

    public void Apply() {
        List<int> audios = new List<int>();
        foreach (var item in _toggles) {
            var toggle = item.Item1;
            var audioPackage = item.Item2;

            if (toggle.isOn) {
                audios.Add(audioPackage.audio_package_id);
            }
        }

        string audio_package_ids = string.Join(",", audios);
        print(audio_package_ids);
        SAWDAudioManager.s_instance.UpdateAudioPackageIds(audio_package_ids);
        
        // canvas.SetActive(false); // uncomment to close canvas
    }

    public void OpenSoundsCanvas() {
        canvas.SetActive(true);
    }
}
