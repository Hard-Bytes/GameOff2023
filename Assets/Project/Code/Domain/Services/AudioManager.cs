using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace Project.Code.Domain.Services
{
    using Sound = Tuple<string,AudioSource>;
    public class AudioManager : MonoBehaviour

    {
        private static uint staticIds = 0;
        [SerializeField] private int _maxSoundsPool = 10;
        private Dictionary<string, Tuple<string, AudioSource>> soundsPool = new Dictionary<string, Sound>();
        private GameObject _mainMusic;

        public AudioClip test;

        [Button("clear sound objects")]
        public void ClearSoundsPool()
        {
            foreach (var sound in soundsPool)
            {   
                Destroy(sound.Value.Item2);
            }
            soundsPool.Clear();
            Debug.Log(soundsPool.Count);
        }

        [Button("Thats a lotta words...")]
        public void testSound()
        {
            CreateAudioSource("testlmao"+staticIds, test).Item2.Play();
        }
        
        /// <summary>
        /// Pre-cache sound objects 
        /// </summary>
        public void PopulateSoundsPool()
        {
            ClearSoundsPool();
            for (int i = 0; i < _maxSoundsPool; ++i)
            {
                GameObject temp = new GameObject("SoundObject_" + staticIds++);
                temp.transform.SetParent(transform);
                soundsPool.Add("SoundObject_" + staticIds,new ("SoundObject_" + staticIds,temp.AddComponent<AudioSource>()));
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

        private Sound CreateAudioSource(string id, [CanBeNull] AudioClip audio)
        {
            GameObject newGameObject = findIdleAudioSource();
            AudioSource newAudioSource;
            if (newGameObject == null)
            {
                if (gameObject.transform.childCount < _maxSoundsPool)
                {
                    newGameObject = new GameObject(id);
                    soundsPool.Add("SoundObject_" + staticIds,new ("SoundObject_" + staticIds, newGameObject.AddComponent<AudioSource>()));
                }
                else
                {
                    throw new Exception("Run out of sound pool objects!");
                }
            }
            
            newGameObject.name = id;
            newAudioSource = newGameObject.GetComponent<AudioSource>();
            newGameObject.transform.SetParent(gameObject.transform);
            newAudioSource.playOnAwake = false;
            newAudioSource.clip = audio;
            var ret = new Sound(id, newAudioSource);
            ++staticIds;
            return ret;
        }

        private GameObject findIdleAudioSource()
        {
            foreach (var sound in soundsPool.Values)
            {
                if (!sound.Item2.isPlaying)
                {
                    return sound.Item2.gameObject;
                }
            }
            
            return null;
        }
        
    }
}