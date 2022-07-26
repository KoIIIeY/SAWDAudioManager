using System.Collections;
using System.Collections.Generic;
using SAWDAudio.Models;
using UnityEngine;


namespace SAWDAudio
{
    [RequireComponent(typeof(AudioSource))]

    public class SAWDAudioSourceHelper : MonoBehaviour
    {

        public string sound_tag;
        public string sound_subTag;

        public AudioSource source;

        // Start is called before the first frame update
        void Start()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }
        }
        
        void OnEnable()
        {
            SAWDAudioManager.SubscribeOnAudioLoad(sound_tag, ApplyNewAudios);
        }

        void OnDisable()
        {
            SAWDAudioManager.UnsubscribeAudioLoad(sound_tag, ApplyNewAudios);
        }

        private async void ApplyNewAudios(List<Audio> audios)
        {
            if (sound_subTag != "")
            {
                foreach (var audio in audios)
                {
                    if (audio.sub_tag.title == sound_subTag)
                    {
                        source.clip = await audio.GetClip();
                        
                        Debug.Log("Applied helper sub:"+audio.clip);
                        break;
                    }
                }
            }
            else
            {
                source.clip = await audios[0].GetClip();
                Debug.Log("Applied helper no sub:"+audios[0].localFilePath);
            }
            source.Play();
        }

    }
}
