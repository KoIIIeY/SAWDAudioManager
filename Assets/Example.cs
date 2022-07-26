using System.Collections;
using System.Collections.Generic;
using SAWDAudio.Models;
using UnityEngine;

namespace SAWDAudio {

    
    public class Example : MonoBehaviour
    {
        // any sounds you already have in game
        //public AudioClip[] sounds;
        //or
        //public Dictionary<string, AudioClip> soundsDictionary = new Dictionary<string, AudioClip>();
        //or 
        public Queue<AudioClip> queue = new Queue<AudioClip>();
        
        public AudioSource audioSource;
        
        

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }
    
        // Update is called once per frame
        void Update()
        {
            if (!audioSource.isPlaying && queue.Count > 0) {
                audioSource.PlayOneShot(queue.Dequeue());
            }
        }
        
        // You can subscribe to the base tag, subtags are needed for internal separation
        void OnEnable()
        {
            // it's a "Base tag"!
            SAWDAudioManager.SubscribeOnAudioLoad("Many sounds", ApplyNewAudios);
        }
    
        void OnDisable()
        {
            // it's a "Base tag"!
            SAWDAudioManager.UnsubscribeAudioLoad("Many sounds", ApplyNewAudios);
        }
    
        // sounds will be replaced with new downloaded from server
        private async void ApplyNewAudios(List<Audio> audios)
        {
            queue.Clear();
            // subtags not provided or ignored!
            for (int i = 0; i < audios.Count; i++ )
            {
                queue.Enqueue(await audios[i].GetClip());
            }
            
            // or in case you have subtag
            // If there are no subtags, it is considered that files are uploaded by this tag in any amount, otherwise one file is one subtag
            // Only the developer determines how the files will be uploaded, so if you need a lot of sounds like "sword strike" - do not put subtags on them.
            // If you need a lot of sounds like "training" - put some tag "training" and inside each phrase - your own subtag like "welcome", "jump" and so on and so forth

            // todo: uncomment to check how it's work
            // for (int i = 0; i<audios.Count;i++)
            // {
                // soundsDictionary[audios[i].sub_tag.title] = audios[i].clip;
            // }
            
        }
        
    }


}