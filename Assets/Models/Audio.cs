using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SAWDAudio.Models
{
    [Serializable]
    public class Audio {
        public int audio_id;
        public int user_id;
        public int audio_package_id;
        public string file;
        public int character_id;
        public DateTime created_at;
        public DateTime updated_at;
        public int base_audio_id;
        public int tag_id;
        public int sort;
        public Tag tag;
        public Tag sub_tag;
        public Character character;
        public string deleted_at;
        [NonSerialized] public AudioClip clip;

        [NonSerialized]
        public byte[] downloadedData;

        public string isDownloaded {
            get
            {
                // Debug.Log("FILE local:"+this.localFilePath);
                // Debug.Log("FILE project:"+this.projectFilePath);
                return existsInProject() ? this.projectFilePath : existsInCache() ? this.localFilePath : "";
            }
        }

        public bool existsInProject()
        {
            return File.Exists(this.projectFilePath);
        }

        public bool existsInCache()
        {
            return File.Exists(this.localFilePath);
        }

        public string localFilePath {
            get {
                string saveDir = string.Format("{0}/SAWDSound/Sounds/{1}/", Application.persistentDataPath, this.audio_package_id.ToString());
                if (!Directory.Exists(saveDir)) {
                    Directory.CreateDirectory(saveDir);
                }
                return string.Format("{0}{1}{2}", saveDir, this.audio_id.ToString(), Path.GetExtension(this.file));
            }
        }

        public string projectFilePath
        {
            get {
                string saveDir = string.Format("Assets/SAWDSound_downloaded/Sounds/{0}/", this.audio_package_id.ToString());
                if (!Directory.Exists(saveDir)) {
                    Directory.CreateDirectory(saveDir);
                }
                return string.Format("{0}{1}{2}", saveDir, this.audio_id.ToString(), Path.GetExtension(this.file));
            }
        }

        public UnityWebRequest StartDownload() {
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(isDownloaded != "" ? ("file://" + isDownloaded) : (SAWDAudioManager.filesURL + file), AudioType.UNKNOWN);
            uwr.SendWebRequest();
            return uwr;
        }

        public async Task<AudioClip> GetClip() {
            if (clip != null) {
                return clip;
            }
            using (UnityWebRequest uwr = StartDownload()) {
                // wrap tasks in try/catch, otherwise it'll fail silently
                try {
                    while (!uwr.isDone) await Task.Delay(5);
                    switch (uwr.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                        case UnityWebRequest.Result.ProtocolError:
                            Debug.LogError("Connection error: "+uwr.error);
                            return null;
                    }
                    clip = DownloadHandlerAudioClip.GetContent(uwr);
                    downloadedData = uwr.downloadHandler.data;
                } catch (Exception err) {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }
            return clip;
        }


        public string saveToCache()
        {
            if (downloadedData != null && isDownloaded == "")
            {
                File.WriteAllBytes(localFilePath, downloadedData);
                return localFilePath;
            }

            return "";
        }

        public string saveToProject()
        {
            if (downloadedData != null && isDownloaded == "")
            {
                File.WriteAllBytes(projectFilePath, downloadedData);
                return projectFilePath;
            }

            return "";
        }

        public void deleteFile()
        {
            if (existsInCache())
            {
                File.Delete(localFilePath);
            }

            if (existsInProject())
            {
                File.Delete(projectFilePath);
            }
            
        }
        
    }
}