using System;
using Maniac.SpawnerSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using UniRx;
using UnityEngine;

namespace Maniac.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioObjectInstance : MonoBehaviour
    {
        private AudioManager _audioManager => Locator<AudioManager>.Instance;
        private SpawnerManager _spawnerManager => Locator<SpawnerManager>.Instance;

        private AudioSource _audioSource;
        private AudioInfo _audioInfo;
        private string _audioName;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _audioManager.MusicVolume.Subscribe(UpdateMusicVolume).AddTo(this);
            _audioManager.SoundVolume.Subscribe(UpdateSoundVolume).AddTo(this);
            _audioManager.OnAllSoundPause -= OnAllSoundPause;
            _audioManager.OnAllSoundPause += OnAllSoundPause;
            _audioManager.OnAllSoundUnPause -= OnAllSoundUnPause;
            _audioManager.OnAllSoundUnPause += OnAllSoundUnPause;
            _audioManager.OnStopSound -= OnStopSound;
            _audioManager.OnStopSound += OnStopSound;
        }

        private void OnStopSound(string soundName)
        {
            if(_audioName == soundName)
                _spawnerManager.Release(this);
        }

        public void OnDestroy()
        {
            _audioManager.OnAllSoundPause -= OnAllSoundPause;
            _audioManager.OnAllSoundUnPause -= OnAllSoundUnPause;
            _audioManager.OnStopSound -= OnStopSound;
        }

        private void OnAllSoundUnPause()
        {
            _audioSource.UnPause();
        }

        private void OnAllSoundPause()
        {
            _audioSource.Pause();
        }

        private void UpdateMusicVolume(float value)
        {
            if (_audioInfo == null) return;
            if (!_audioInfo.isMusic) return;

            _audioSource.volume = value;
        }
        
        private void UpdateSoundVolume(float value)
        {
            if (_audioInfo == null) return;
            if (_audioInfo.isMusic) return;

            _audioSource.volume = value;
        }

        private void OnEnable()
        {
            Reset();
        }

        private void OnDisable()
        {
            Reset();
        }

        private void Reset()
        {
            _audioSource.Stop();
            _audioSource.clip = null;
        }

        public void SetupAudio(AudioInfo audioInfo,string audioName, bool isLoop = false)
        {
            _audioName = audioName;
            _audioInfo = audioInfo;
            _audioSource.clip = audioInfo.TakeRandom();
            _audioSource.loop = isLoop;
            
            UpdateMusicVolume(_audioManager.MusicVolume.Value);
            UpdateSoundVolume(_audioManager.SoundVolume.Value);
            
            if(!isLoop)
                _spawnerManager.ReleaseAfter(this,_audioSource.clip.length);
            
            _audioSource.Play();
        } 
    }
}