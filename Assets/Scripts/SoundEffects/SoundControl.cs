using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundEffects
{
    public class SoundControl : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] protected List<SoundEffectInfo> _soundEffects = new();
    
        protected virtual void Start()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    
        public void PlaySound(SoundEffectType soundType)
        {
            var soundEffectInfo = _soundEffects.FirstOrDefault(soundInfo => soundInfo.SoundEffectType == soundType);
            if (soundEffectInfo == null)
            {
                Debug.Log($"Sound is not defined {soundType} ");
                return;
            }
        
            _audioSource.clip = soundEffectInfo.Clip;
            _audioSource.Play();
        }
    }

    [Serializable]
    public class SoundEffectInfo
    {
        public AudioClip Clip;
        public SoundEffectType SoundEffectType;
    }
}