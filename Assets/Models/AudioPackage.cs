using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Tables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Localization;
#endif


namespace SAWDAudio.Models
{
    [Serializable]
    public class AudioPackage {
        public int audio_package_id;
        public int user_id;
        public int game_id;
        public string name;
        public string language;
        public DateTime created_at;
        public DateTime updated_at;
        public int base_audio_package_id;
        public Audio[] audios;
        [NonSerialized] public List<AudioPackage> childsList = new List<AudioPackage>();

#if UNITY_EDITOR
        public async Task<bool> ImportToAssetTable()
        {

            Locale locale = LocalizationEditorSettings.GetLocale(language);
            if (locale == null)
            {
                locale = Locale.CreateLocale(language);
                if (!Directory.Exists("Assets/Locales"))
                {
                    Directory.CreateDirectory("Assets/Locales");
                }
                AssetDatabase.CreateAsset(locale, "Assets/Locales/"+language+".asset");
                LocalizationEditorSettings.AddLocale(locale);
            }

// Get the collection
            var collection = LocalizationEditorSettings.GetAssetTableCollection(name);
            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateAssetTableCollection(name, "Assets/Locales");
            }
            var newTable = collection.GetTable(locale.Identifier) as AssetTable;
            if (newTable == null)
            {
                newTable = collection.AddNewTable(locale.Identifier) as AssetTable;
            }

            foreach (Audio audio in audios)
            {
                
                if(audio.existsInProject() && !string.IsNullOrEmpty(audio.deleted_at))
                {
                    audio.deleteFile();
                } 
                else if (!audio.existsInProject() && string.IsNullOrEmpty(audio.deleted_at))
                {
                    await audio.GetClip();
                    audio.saveToProject();
                    AssetDatabase.Refresh();
                
                    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                    string assetGUID = AssetDatabase.AssetPathToGUID(audio.projectFilePath);
                    settings.CreateAssetReference(assetGUID);
                
                    var entry = newTable.AddEntry(audio.sub_tag.title, assetGUID);
                    // Add some metadata
                    // entry.AddMetadata(new Comment { CommentText = "This is a comment"});
                }
            }

// We need to mark the table and shared table data entry as we have made changes
            EditorUtility.SetDirty(newTable);
            EditorUtility.SetDirty(newTable.SharedData);
            return false;
        }
#endif
        
        
        
    }
}