using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SAWDAudio.Models
{
    [Serializable]
    public class SoundsList
    {
        public Audio[] data;
        public string audio_package_ids = "";

        public string saveDir = "";

        private string _savePath = "";
        public string savePath { 
            get => _savePath != "" ? _savePath : string.Format("{0}{1}{2}.json", saveDir, "last_sounds_", audio_package_ids);
            set => _savePath = value;
        }
        
        public float downloadPercent = 0f;

        public SoundsList(string audio_package_ids = "", string saveDir = "")
        {
            this.audio_package_ids = audio_package_ids;
            this.saveDir = saveDir;
        }

        public async Task<bool> DownloadFromServer(bool saveToCache = true)
        {
            try
            {
                using (var request = UnityWebRequest.Get(SAWDAudioManager.baseURL + SAWDAudioManager.audiosURL + audio_package_ids))
                {
                    request.SendWebRequest();
                    Debug.Log("Request sended");
                    while (!request.isDone) await Task.Delay(5);
                    Debug.Log("Request received");
                    switch (request.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                        case UnityWebRequest.Result.ProtocolError:
                            Debug.LogError($"SAWD:Could not get list of packages! Error: {request.error}");
                            return false;
                    }

                    if (saveToCache)
                    {
                        SaveToCache(request.downloadHandler.text);
                    }
                    data = JsonUtility.FromJson<SoundsList>(request.downloadHandler.text).data;
                    Debug.Log("SoundList.DownloadFromServer done");
                    return true;
                }
            }
            catch (Exception e)
            {
                // Debug.LogError(e.Message);
                // Debug.LogError(e.StackTrace);
                Debug.LogError(e.ToString());
            }

            return false;


        }

        public bool LoadFromCache()
        {
            try
            {
                if (!File.Exists(savePath)) return false;
                data = JsonUtility.FromJson<SoundsList>(File.ReadAllText(savePath)).data;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            
        }

        public void SaveToCache(string data)
        {
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            System.IO.File.WriteAllText(savePath, data);
        }
        
    }
}