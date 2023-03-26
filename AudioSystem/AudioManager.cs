using System;
using System.IO;
using Maniac.SpawnerSystem;
using Maniac.Utils;
using UniRx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Maniac.AudioSystem
{
    public class AudioManager
    {
        private AudioData _audioData => Locator<AudioData>.Instance;
        private SpawnerManager _spawnerManager => Locator<SpawnerManager>.Instance;

        public event Action OnAllSoundPause;
        public event Action OnAllSoundUnPause;
        public event Action<string> OnStopSound;

        public FloatReactiveProperty MusicVolume = new FloatReactiveProperty();
        public FloatReactiveProperty SoundVolume = new FloatReactiveProperty();
        private AudioObjectInstance _audioObjectInstance;

        public void Init(AudioObjectInstance audioObjectInstance)
        {
            _audioObjectInstance = audioObjectInstance;
            
            // You should use a SettingProfile to store music and sound volume that user set, and then apply here.
            MusicVolume.Value = 1f;
            SoundVolume.Value = 1f;
        }

        public void Play(string audioName, bool isLoop = false)
        {
            var audioInfo = _audioData.Get(audioName);
            if (audioInfo == null)
            {
                Debug.LogError($"{audioName} doesn't exist!");
                return;
            }

            var newAudioObject = _spawnerManager.Get(_audioObjectInstance);
            newAudioObject.SetupAudio(audioInfo, audioName, isLoop);
        }

        public void SetMusicVolume(float value)
        {
            value = Mathf.Clamp01(value);

            MusicVolume.Value = value;
        }

        public void SetSoundVolume(float value)
        {
            value = Mathf.Clamp01(value);

            SoundVolume.Value = value;
        }
    }
}