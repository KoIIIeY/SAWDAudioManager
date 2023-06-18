using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;
using SAWDAudio.Models;
// using UnityEditor.PackageManager;

namespace SAWDAudio
{
    public class SAWDAudioManager : MonoBehaviour
    {
        public static SAWDAudioManager s_instance;

        public delegate void audioLoaded(List<Audio> audios);

        [Header("References")]
        private static readonly Dictionary<string, audioLoaded>
            _taggedDelegates = new Dictionary<string, audioLoaded>();

        private static readonly Dictionary<string, List<Audio>> _taggedTracks = new Dictionary<string, List<Audio>>();

        [Header("Settings")]
        public static string api_token = "";

        public static int _game_id = -1; // fill with your game id!
        public static int game_id {
            get
            {
                return PlayerPrefs.HasKey("SAWD_audio_game_id") ? PlayerPrefs.GetInt("SAWD_audio_game_id") : _game_id;
            }
        }
        public const string baseURL = "https://audio.sa-wd.ru/api/v1";
        public const string filesURL = "https://audio.sa-wd.ru/";
        public const string audioPackagesUrl = "/call/Game/getPackages?game_id=";
        public const string audiosURL = "/call/AudioPackage/package?audio_package_ids=";
        

        [Header("Debugging")] [SerializeField] private bool isInitialized;

        private static SoundsList soundList;
        private static AudioPackagesList audioPackagesList = new AudioPackagesList();
        private string _audio_package_ids = "";
        private string audio_package_ids
        {
            get => _audio_package_ids;
            set
            {
                _audio_package_ids = value;
                soundList.audio_package_ids = value;
            }
        }

        [Tooltip("If you don't need our canvas with a list of sound packs, leave it blank.")]
        public SAWDAudioUICanvas sawdAudioUiCanvas;

        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
                soundList = new SoundsList(audio_package_ids,string.Format("{0}/SAWDSound/Configs/", Application.persistentDataPath));
                if (PlayerPrefs.HasKey("SAWD_audio_token"))
                {
                    api_token = PlayerPrefs.GetString("SAWD_audio_token");
                }
                else
                {
                    Debug.Log("Not authorized (click UnityLocalizationPlugin, fill email and password and click 'Login')");
                }
            }
            else
            {
                Debug.LogError("Only one SAWDAudioManager allowed!");
                Destroy(gameObject);
            }
        }

        public async Task UpdateAudioPackageIds(string audio_package_ids)
        {
            this.audio_package_ids = audio_package_ids;
            await ReloadPackage();
        }

        private async void Start()
        {
            await GetAudioPackagesList();
            await ReloadPackage();
        }

        public static Dictionary<string, List<Audio>> GetTaggedTracksList()
        {
            return _taggedTracks;
        }

        private async Task<bool> GetAudioPackagesList()
        {
            await audioPackagesList.DownloadFromServer();
            if (sawdAudioUiCanvas != null && audioPackagesList.data != null)
            {
                sawdAudioUiCanvas.UpdateUI(audioPackagesList);
            }

            return audioPackagesList.data != null;
        }

        private async Task ReloadPackage()
        {
            // block input from the outside until this controller is finished with the downloads
            isInitialized = false;
            // Here either use the approach with first receiving a list from the server
            Debug.Log("Packages list START");
            GetAudioClipsInfo();
            Debug.Log("Clips list START");
            await GetAudioClipsParallel();
            Debug.Log("Both DONE");

            // allow to do things from this point on
            isInitialized = true;
        }

        // This is the routine that downloads the URL list from the server
        // then it starts the individual downloads
        private async void GetAudioClipsInfo()
        {
            if (audio_package_ids == "") return;
            Debug.Log("CLIPS 1");
            soundList.LoadFromCache();
            Debug.Log("CLIPS 1.5");
            await soundList.DownloadFromServer();
            Debug.Log("CLIPS 2");
        }

        // This version starts all downloads at once and waits until they are all done
        // probably faster than the sequencial version
        private async Task GetAudioClipsParallel()
        {
            try
            {
                if (soundList.data == null) return;
            Debug.Log("soundsList.data is not null");

            // musicClips.Clear();
            var requests = new Dictionary<Audio, UnityWebRequest>();
            _taggedTracks.Clear();

            soundList.downloadPercent = 0f;
            foreach (Audio audio in soundList.data)
            {
                if (audio.isDownloaded != "")
                {
                    addTaggedTrack(audio);
                    continue;
                }

                requests.Add(audio, audio.StartDownload());
            }

            // Wait for all requests to finish
            if (requests.Count > 0)
            {
                Debug.Log("Clips delayed!");
                while (requests.Any(r => !r.Value.isDone))
                {
                    soundList.downloadPercent = requests.Values.Sum(a => a.downloadProgress / requests.Count);
                    Debug.Log("Percent:"+soundList.downloadPercent);
                    await Task.Delay(5);
                }
                Debug.Log("Requests done");
            }
            

            // Now examine and use all results
            foreach (var www in requests)
            {
                switch (www.Value.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"Could not get clip from \"{www.Value.url}\"! Error: {www.Value.error}", this);
                        www.Value.Dispose();
                        continue;
                }

                www.Key.saveToCache();
                // if (!www.Key.isDownloaded)
                // {
                //     File.WriteAllBytes(www.Key.localFilePath, www.Value.downloadHandler.data);
                // }

                Debug.Log("Audio loaded:"+www.Key.localFilePath);
                //www.Key.clip = DownloadHandlerAudioClip.GetContent(www.Value);
                addTaggedTrack(www.Key);
                www.Value.Dispose();
            }

            foreach (var soundTag in _taggedTracks.Keys)
            {

                if (_taggedDelegates.ContainsKey(soundTag))
                {

                    _taggedDelegates[soundTag]?.Invoke(_taggedTracks[soundTag]);
                }
            }
            
            soundList.downloadPercent = 1f;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

            
        }

        public void addTaggedTrack(Audio audio)
        {
            if (!_taggedTracks.ContainsKey(audio.tag.title))
            {
                _taggedTracks[audio.tag.title] = new List<Audio>();
            }

            _taggedTracks[audio.tag.title].Add(audio);
        }

        public static void SubscribeOnAudioLoad(string tag, audioLoaded al)
        {

            if (_taggedTracks.ContainsKey(tag))
            {
                al.Invoke(_taggedTracks[tag]);
            }

            if (!_taggedDelegates.ContainsKey(tag))
            {
                _taggedDelegates[tag] = al;
            }
            else
            {
                _taggedDelegates[tag] += al;
            }
        }

        public static void UnsubscribeAudioLoad(string tag, audioLoaded al)
        {
            _taggedDelegates[tag] -= al;
        }
    }

}
