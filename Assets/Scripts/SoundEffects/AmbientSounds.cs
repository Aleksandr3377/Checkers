using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoundEffects
{
    public class AmbientSounds : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _audioClips;
        private AudioSource _audioSource;
        private static AmbientSounds _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this; // без цього не працювало
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            _audioSource = gameObject.AddComponent<AudioSource>();
            PlayRandomSound();
        }

        private void Update()
        {
            if (!IsSoundEnded()) return;

            PlayRandomSound();
        }

        private void PlayRandomSound()
        {
            var randomSoundIndex = Random.Range(0, _audioClips.Count);
            _audioSource.volume = 0.2f;
            _audioSource.clip = _audioClips[randomSoundIndex];
            _audioSource.Play();
        }

        private bool IsSoundEnded()
        {
            return !_audioSource.isPlaying;
        }
    }
}