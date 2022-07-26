using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SAWDAudio.Models
{
    [Serializable]
    public class AudioPackagesList
    {
        public AudioPackage[] data;

        public async Task<bool> DownloadFromServer()
        {
            using (var request = UnityWebRequest.Get(SAWDAudioManager.baseURL + SAWDAudioManager.audioPackagesUrl + SAWDAudioManager.game_id))
            {
               request.SendWebRequest();
               
               while (!request.isDone) await Task.Delay(5);

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"SAWD:Could not get list of audio packages! Error: {request.error}");
                        return false;
                }

                Debug.Log("AudioPackagesList.DownloadFromServer");
                data = JsonUtility.FromJson<AudioPackagesList>(request.downloadHandler.text).data;
                return true;
            }
        }
        
        
        public async Task<bool> DownloadTranslatedFromServer()
        {
            using (var request = UnityWebRequest.Get(SAWDAudioManager.baseURL+"/call/audioPackage/getPackagesForGame?game_id="+SAWDAudioManager.game_id+"&api_token="+(!String.IsNullOrEmpty(SAWDAudioManager.api_token) ? SAWDAudioManager.api_token : PlayerPrefs.HasKey("SAWD_audio_token") ? PlayerPrefs.GetString("SAWD_audio_token") : "")))
            {
                request.SendWebRequest();
               
                while (!request.isDone) await Task.Delay(5);

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"SAWD:Could not get list of audio packages! Error: {request.error}");
                        return false;
                }

                Debug.Log("AudioPackagesList.DownloadTranslatedFromServer");
                data = JsonUtility.FromJson<AudioPackagesList>(request.downloadHandler.text).data;
                return true;
            }
        }
        
    }
}