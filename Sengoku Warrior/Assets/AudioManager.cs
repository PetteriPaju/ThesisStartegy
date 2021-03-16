using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SengokuWarrior
{
    public class AudioManager : MonoBehaviour
    {

        public static AudioManager _instance;

        public List<AudioSource> sources = new List<AudioSource>();
        public AudioSource MusicPlayer;
        public List<AudioClip> clips = new List<AudioClip>();


        public void PlayMusic(string ClipName)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].name.ToLower() == ClipName.ToLower()) {
                    MusicPlayer.clip = clips[i];
                    MusicPlayer.Play();
                    return;
                }
            }
        }


        public void PlayRandomOneShot(AudioClip [] clips)
        {

           int selected = Random.Range((int)0, (int)clips.Length);

            OneShotPlay(clips[selected]);

        }

        public void PlayClip(string ClipName)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].name.ToLower() == ClipName.ToLower()) { Play(clips[i]); return; }
            }
            Debug.LogWarning("No audioClip with name: " + ClipName + " found!");
        }

        void Awake()
        {
            _instance = this;
        }

        public void Play(AudioClip clip)
        {
            foreach (AudioSource source in sources)
            {
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.Play();
                    return;
                }
            }

            AudioSource na = GenerateNewAudioSourse(clip.length);
            na.clip = clip;
            na.Play();


        }

        public void OneShotPlay(AudioClip clip)
        {
            sources[0].PlayOneShot(clip);
        }


        private AudioSource GenerateNewAudioSourse(float lifetime)
        {
            AudioSource s = gameObject.AddComponent<AudioSource>();
            Component.Destroy(s, lifetime);
            return s;


        }

    }
}
