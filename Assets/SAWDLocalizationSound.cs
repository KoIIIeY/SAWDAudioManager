using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

namespace SAWDAudio
{
    /**
     * https://forum.unity.com/threads/release-announcements-and-notes.597262/ - how to install unity localization package
     */
    public class SAWDLocalizationSound : MonoBehaviour
    {

        public string soundAssetTable = "";
        public LocalizedAssetTable myAssetTable;
        public LocalizeAudioClipEvent[] LocalizeAudioClipEvents;
        
        private void Start()
        {
            if (myAssetTable == null)
            {
                myAssetTable = new LocalizedAssetTable(soundAssetTable);
            }
            
            
            myAssetTable.TableChanged += UpdateTable;
            FindLocalizeAudioClipEvents();
        }

        async void UpdateTable(AssetTable value)
        {
            foreach (var tagsSounds in SAWDAudioManager.GetTaggedTracksList())
            {
                if (tagsSounds.Value[0].sub_tag?.title == null)
                {
                    continue;
                }
                var entry = value.GetEntry(tagsSounds.Value[0].sub_tag.title) ?? value.AddEntry(tagsSounds.Value[0].sub_tag.title, string.Empty);
                Debug.Log("Tries "+tagsSounds.Value[0].sub_tag.title);
                if (entry == null)
                {
                    continue;
                }

                Debug.Log("Installed "+tagsSounds.Value[0].sub_tag.title);
                entry.SetAssetOverride(await tagsSounds.Value[0].GetClip());
                foreach (var lace in LocalizeAudioClipEvents)
                {
                    if (lace.enabled)
                    {
                        lace.enabled = false;
                        lace.enabled = true;
                    }
                    
                }
            }
        }


        public void FindLocalizeAudioClipEvents()
        {
            LocalizeAudioClipEvents = FindObjectsOfType<LocalizeAudioClipEvent>();
        }
        
        
    }

}