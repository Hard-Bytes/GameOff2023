using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            public string Id
            {
                get => gameObject.name;
                set => gameObject.name = value;
            }
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

            public bool isPaused;
            public void Play() => source.Play();
            public void Stop() => source.Stop();
            public void Pause()
            {
                 source.Pause();
                 isPaused = true;
            }
            public void Resume()
            {
                Play();
                isPaused = false;
            }
        }
        
        private static uint staticIds = 0;
        [SerializeField] private int _maxSoundsPool = 10;
        // private Dictionary<string, Tuple<string, AudioSource>> soundsPool = new();
        private Dictionary<string,Sound> _soundsPool = new();
        private GameObject _mainMusic;
    
        // TESTING
        public AudioClip test;
        public string idTest;
        
        public AudioMixerGroup musicAudioMixerGroup;
        public AudioMixerGroup sfxAudioMixerGroup;
        public AudioMixerGroup masterAudioMixerGroup;

        [Button("Test sound")]
        public void TestSound()
        {
            PlayOneShot(gameObject, test,SoundType.Sfx,true,0f);
        }
        
        [Button("Clear sound objects")]
        public void ClearSoundsPool()
        {
            foreach (Transform child in transform)
            {   
                Destroy(child.gameObject);
            }
            _soundsPool.Clear();
        }

        #region testingButtons
        [Button("Populate sound objects",enabledMode:EButtonEnableMode.Playmode)]
        public void TestPopulateSoundsPool()
        {
            PopulateSoundsPool();
        }

        [Button("Find sound test")]
        public void FindTest()
        {
            Debug.Log(FindById(idTest)?.Id);
        }
        [Button("Find owned sounds test")]
        public void FindOwnerTest()
        {
            Debug.Log(GetOwnedSounds(GetHashCode().ToString()));
        }

        [Button("Pause a sound")]
        public void PauseSoundTest()
        {
            PauseSound(idTest);
        }
        [Button("Resume a sound")]
        public void ResumeSoundTest()
        {
            ResumeSound(idTest);
        }
        [Button("Stop a sound")]
        public void StopSoundTest()
        {
            StopSound(idTest);
        }
        #endregion
        
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

                _soundsPool.Add(newGameObject.name,newSoundObj);
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
            Sound idleSound = FindIdleAudioSource();

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
                    Id = newGameObject.name,
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

        private Sound FindIdleAudioSource()
        {
            return _soundsPool.FirstOrDefault(x => !x.Value.isPlaying && !x.Value.isPaused).Value;
        }
        
        private Sound FindIdleAudioSource(SoundType soundType)
        {
            return _soundsPool.FirstOrDefault(x => x.Value.isPlaying && x.Value.soundType == soundType && !x.Value.isPaused).Value;
        }

        private Sound FindById(String id)
        {
            return _soundsPool.FirstOrDefault(x => x.Key == id).Value;
        }

        /// <summary>
        /// Plays a sound on an idle soundObject or allocates a new one to play the sound. Can play the
        /// sound instantly or after a delay
        /// </summary>
        /// <param name="caller">Who is the audioSource owner</param> 
        /// <param name="audioClip">Sound to be played</param>
        /// <param name="soundType">Sfx or Music (Music is played on loop)</param>
        /// <param name="scheduleSound">Should play at a later time</param>
        /// <param name="scheduleTime">Delay in seconds to play the sound (only used if scheduleSound is True)</param>
        /// <returns>The id of the soundObject used to host the sound</returns>
        public string PlayOneShot(GameObject caller, AudioClip audioClip, SoundType soundType = SoundType.Sfx,
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

            return newSound.Id;
        }


        /// <summary>
        /// Allocates an AudioClip on an idle or new soundObject but does not play it
        /// </summary>
        /// <param name="caller">Who is the audioSource owner</param>
        /// <param name="audioClip">Sound to be played</param>
        /// <param name="soundType">Sfx or Music (Music is played on loop)</param>
        /// <returns>The id of the soundObject used to host the sound</returns>
        public string PreCacheSound(GameObject caller, AudioClip audioClip, SoundType soundType = SoundType.Sfx)
        {
            var callerHash = caller.GetHashCode().ToString();
            Sound newSound = CreateAudioSource(callerHash, audioClip, soundType);
            return newSound.Id;
        }
        
        /// <summary>
        /// Stops a sound given it's object ID
        /// </summary>
        /// <param name="id">audioSource's id</param>
        public void StopSound(string id)
        {
            FindById(id)?.Stop();
        }

        /// <summary>
        /// Pauses a sound given it's object ID. Paused sounds WILL NOT be recycled
        /// </summary>
        /// <param name="id">audioSource's id</param>
        public void PauseSound(string id)
        {
            FindById(id)?.Pause();
        }

        /// <summary>
        /// Resumes playing a Paused sound given it's ID
        /// </summary>
        /// <param name="id">audioSource's id</param>
        public void ResumeSound(string id)
        {
            FindById(id)?.Resume();
        }

        /// <summary>
        /// Returns an array with the id's of the soundObjects owned by ownerHash
        /// </summary>
        /// <param name="ownerHash">owner's hash</param>
        /// <returns>Array with the soundObjects' ids owned by ownerHash</returns>
        public string[] GetOwnedSounds(string ownerHash)
        {
            return _soundsPool.Keys.Where(x => _soundsPool[x].ownerHash == ownerHash).ToArray();
        }
        
        private IEnumerator ScheduledSound(Sound sound, float scheduleTime)
        {
            yield return new WaitForSeconds(scheduleTime);
            sound.Play();
        }


        //TODO: setters for the audioMixer's exposed volume variable names
        
        /// <summary>
        /// Sets the value for the Music audioMixer
        /// </summary>
        /// <param name="value"></param>
        public void AdjustMusicVolume(float value)
        {
            musicAudioMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(value)*20);
        }
        
        /// <summary>
        /// Sets the value for the SFX audioMixer
        /// </summary>
        /// <param name="value"></param>
        public void AdjustSfxVolume(float value)
        {
            musicAudioMixerGroup.audioMixer.SetFloat("SFX Volume", Mathf.Log10(value)*20);
        }
        
        /// <summary>
        /// Sets the value for the Master audioMixer
        /// </summary>
        /// <param name="value"></param>
        public void AdjustMasterVolume(float value)
        {
            masterAudioMixerGroup.audioMixer.SetFloat("Master Volume", Mathf.Log10(value)*20);
        }
        
    }
}