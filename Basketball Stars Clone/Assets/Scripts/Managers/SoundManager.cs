using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using AudioType = Interfaces.AudioType;

namespace Managers
{
    /// <summary>
    /// Manages all audio functionalities in the game, including music, sound effects and ambient sounds.
    /// Handles playback of random and predefined sounds based on the current scene and game events.
    /// Integrates with a service locator to allow global access to audio playback functions.
    /// </summary>

    [ExecuteInEditMode]
    public class SoundManager : MonoBehaviour, ISoundService
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;
    
        [SerializeField] private SoundList[] soundList;
        [SerializeField] private AudioClip[] musicList;
        [SerializeField] private AudioClip[] ambientList;
        [SerializeField] private float ambientDelay = 10f;
        
        private static SoundManager Instance { get; set; }

        private float MusicVolume { get; set; } = 0.5f;
        private float SoundVolume { get; set; } = 1;
        
        private Coroutine   _ambientLoop;
        
        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            ServiceLocator.SoundService = this;
            
            #if UNITY_EDITOR
            Assert.IsNotNull(musicSource, $"{nameof(AudioSource)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(sfxSource, $"{nameof(AudioSource)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(ambientSource, $"{nameof(AudioSource)} reference is missing on '{gameObject.name}'");
            #endif
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            string[] names = Enum.GetNames(typeof(SoundType));
            Array.Resize(ref soundList, names.Length);
            for (int i = 0; i < soundList.Length; i++)
            {
                soundList[i].name = names[i];
            }
            #endif
            
            SceneManager.sceneLoaded += ChangeMusic;
        }
        
        public void PlayRandomSound(SoundType sound)
        {
            AudioClip[] clips = soundList[(int) sound].Sounds;
            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
            sfxSource.PlayOneShot(randomClip, SoundVolume);
        }
    
        private void PlaySound(SoundType sound)
        {
            AudioClip[] clips = soundList[(int) sound].Sounds;
            sfxSource.PlayOneShot(clips[0], SoundVolume);
        }
    
        private void PlayMusic(MusicType music)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        
            musicSource.clip = music switch
            {
                MusicType.MENU => musicList[0],
                MusicType.GAME => musicList[1],
                _ => musicList[0]
            };
        
            musicSource.Play();
            musicSource.volume = MusicVolume;
        }

        private void ChangeMusic(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MainMenuScene")
            {
                PlayMusic(MusicType.MENU);
                
                if (_ambientLoop != null)
                {
                    StopCoroutine(_ambientLoop);
                    _ambientLoop = null;
                }
                ambientSource.Stop();
            }
            else
            {
                PlayMusic(MusicType.GAME);

                if (_ambientLoop == null && ambientList.Length > 0)
                {
                    _ambientLoop = StartCoroutine(AmbientLoop());
                }
            }
        }
        
        private IEnumerator AmbientLoop()
        {
            while (true)
            {
                var clip = ambientList[UnityEngine.Random.Range(0, ambientList.Length)];
                ambientSource.clip = clip;
                ambientSource.Play();
                
                yield return new WaitForSeconds(clip.length);
                
                yield return new WaitForSeconds(ambientDelay);
            }
        }

        public float GetVolume(AudioType type)
        {
            return type == AudioType.Music ? MusicVolume : SoundVolume;
        }

        public void SetVolume(AudioType type, float volume)
        {
            if (type == AudioType.Music)
            {
                MusicVolume = volume;
                musicSource.volume = volume;
            }
            else
            {
                SoundVolume = volume;
                sfxSource.volume = volume;
            }
        }

        void ISoundService.PlaySound(SoundType type)
        {
            PlaySound(type);
        }
    }

    [Serializable]
    public struct SoundList
    {
        public AudioClip[] Sounds => sounds;
        [HideInInspector] public string name;
        [SerializeField] private AudioClip[] sounds;
    }
}