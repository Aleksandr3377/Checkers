using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundControl : MonoBehaviour
{ 
    private AudioSource _audioSource;
    [SerializeField] private List<SoundEffectInfo> _soundEffects = new();
    
    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlaySound(SoundEffectType soundType)
    {
        var soundEffectInfo = _soundEffects.FirstOrDefault(soundInfo => soundInfo.SoundEffectType == soundType);
        if (soundEffectInfo != null)
        {
            _audioSource.clip = soundEffectInfo.Clip;
            _audioSource.Play();
        }
    }
}

[Serializable]
public class SoundEffectInfo
{
    public AudioClip Clip;
    public SoundEffectType SoundEffectType;
}