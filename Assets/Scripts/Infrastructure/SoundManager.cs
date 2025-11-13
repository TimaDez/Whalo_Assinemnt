using DataTypes;
using Models;
using UnityEngine;

namespace Whalo.Infrastructure
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        #region Editor

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        public AudioClip[] soundEffects;
        public AudioClip[] musicTracks;

        [SerializeField] private SfxModel _sfxModel;

        #endregion

        #region Methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            if (sfxSource == null)
                sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Play one-shot sound effect
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip);
        }

        // Play one-shot by name
        public void PlaySFX(string clipName)
        {
            var clip = FindClip(soundEffects, clipName);
            PlaySFX(clip);
        }

        public void PlaySFX(SfxType sfxType)
        {
            var clip = _sfxModel.GetClip(sfxType);
            PlaySFX(clip);
        }
        
        // Play music (loops)
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void PlayMusic(string clipName)
        {
            var clip = FindClip(musicTracks, clipName);
            PlayMusic(clip);
        }

        private AudioClip FindClip(AudioClip[] list, string name)
        {
            foreach (var clip in list)
                if (clip != null && clip.name == name)
                    return clip;

            Debug.LogWarning($"SoundManager: Clip '{name}' not found!");
            return null;
        }

        #endregion
    }
}