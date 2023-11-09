using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Code.Domain.Services
{
    public class AudioManager : MonoBehaviour

    {
        public enum SoundType
        {
            Music,
            Sfx
        }
        [System.Serializable]
        internal sealed class Sound
        {
            [HideInInspector] public AudioSource source;
            [HideInInspector] public GameObject gameObject;
            public SoundType soundType;
            public string clipName;
            public string ownerHash;

            public Transform parent
            {
                get => gameObject.transform;
                set => gameObject.transform.SetParent(value,false);
            }
            public AudioClip audioClip
            {
                get => source.clip;
                set => source.clip = value;
            }

            public bool isPlaying
            {
                get => source.isPlaying;
            }

            public bool playOnAwake
            {
                set => source.playOnAwake = value;
            }
            public float volume
            {
                get => source.volume;
                set => source.volume = value;
            }

            public bool loop
            {
                get => source.loop;
                set => source.loop = value;
            }
            public AudioMixerGroup audioMixerGroup
            {
                get => source.outputAudioMixerGroup;
                set => source.outputAudioMixerGroup = value;
            }

            public void Play() => source.Play();
            public void Stop() => source.Stop();
            
        }
        
        private static uint staticIds = 0;
        [SerializeField] private int _maxSoundsPool = 10;
        // private Dictionary<string, Tuple<string, AudioSource>> soundsPool = new();
        private Dictionary<string,Sound> _soundsPool = new();
        private GameObject _mainMusic;

        public AudioClip test;
        public AudioMixerGroup musicAudioMixerGroup;
        public AudioMixerGroup sfxAudioMixerGroup;
        public AudioMixerGroup masterAudioMixerGroup;

        [Button("clear sound objects")]
        public void ClearSoundsPool()
        {
            foreach (var sound in _soundsPool.Values)
            {   
                Destroy(sound.gameObject);
            }
            _soundsPool.Clear();            
        }

        [Button("Thats a lotta words...")]
        public void testSound()
        {
            PlayOneShot(gameObject, test,SoundType.Sfx,true,5f);
        }
        
        /// <summary>
        /// Pre-cache sound objects 
        /// </summary>
        public void PopulateSoundsPool()
        {
            ClearSoundsPool();
            for (int i = 0; i < _maxSoundsPool; ++i)
            {
                GameObject newGameObject = new GameObject("SoundObject_" + staticIds++);
                Sound newSoundObj = new Sound
                {
                    gameObject =  newGameObject,
                    parent = transform,
                    source = newGameObject.AddComponent<AudioSource>(),
                    audioMixerGroup = sfxAudioMixerGroup,
                    ownerHash = GetHashCode().ToString(),
                    playOnAwake = false,
                };

                _soundsPool.Add("SoundObject_" + staticIds,newSoundObj);
            }
        }
        private void Awake()
        {
        }

        private void Start()
        {
            //TODO: should NOT be here
            PopulateSoundsPool();
        }

        private Sound CreateAudioSource(string owner, [CanBeNull] AudioClip audio, SoundType soundType = SoundType.Sfx)
        {
            Sound idleSound = findIdleAudioSource();

            if (idleSound == null)
            {
                GameObject newGameObject = new GameObject("SoundObject_" + staticIds++);
                idleSound = new Sound
                {
                    gameObject =  newGameObject,
                    parent = transform,
                    source = newGameObject.AddComponent<AudioSource>(),
                    ownerHash = GetHashCode().ToString(),   
                    playOnAwake = false,
                };

                _soundsPool.Add("SoundObject_" + staticIds,idleSound);
            }

            idleSound.soundType = soundType;
            idleSound.audioMixerGroup = soundType == SoundType.Sfx ? sfxAudioMixerGroup : musicAudioMixerGroup;
            idleSound.ownerHash = owner;
            idleSound.audioClip = audio;
            idleSound.loop = soundType == SoundType.Music;

            return idleSound;
        }

        private Sound findIdleAudioSource()
        {
            foreach (var sound in _soundsPool.Values)
            {
                if (!sound.isPlaying)
                {
                    return sound;
                }
            }
            
            return null;
        }
        
        private Sound findIdleAudioSource(SoundType soundType)
        {
            foreach (var sound in _soundsPool.Values)
            {
                if (!sound.isPlaying && sound.soundType == soundType)
                {
                    return sound;
                }
            }
            
            return null;
        }

        public void PlayOneShot(GameObject caller, AudioClip audioClip, SoundType soundType = SoundType.Sfx,
            bool scheduleSound = false, float scheduleTime = 0f)
        {
            var callerHash = caller.GetHashCode().ToString();
            Sound newSound = CreateAudioSource(callerHash, audioClip, soundType);
            if (scheduleSound)
            {
                StartCoroutine(ScheduledSound(newSound, scheduleTime));
            }
            else
            {
                newSound.Play();
            }
        }

        private IEnumerator ScheduledSound(Sound sound, float scheduleTime)
        {
            yield return new WaitForSeconds(scheduleTime);
            sound.Play();
        }
        
        public void AdjustMusicVolume(float value)
        {
            musicAudioMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(value)*20);
        }
        public void AdjustSfxVolume(float value)
        {
            musicAudioMixerGroup.audioMixer.SetFloat("SFX Volume", Mathf.Log10(value)*20);
        }
        public void AdjustMasterVolume(float value)
        {
            masterAudioMixerGroup.audioMixer.SetFloat("Master Volume", Mathf.Log10(value)*20);
        }
        
    }
}