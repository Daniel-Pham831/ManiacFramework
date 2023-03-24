using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Maniac.AudioSystem
{
    public class InitAudioManagerService : Service
    {
        private readonly AudioData _audioData;
        private readonly AudioObjectInstance _audioObjectInstance;

        public InitAudioManagerService(AudioData audioData, AudioObjectInstance audioObjectInstance)
        {
            _audioData = audioData;
            _audioObjectInstance = audioObjectInstance;
            Locator<AudioData>.Set(_audioData);
        }

        public override async UniTask<IService.Result> Execute()
        {
            var audioManager = new AudioManager();
            audioManager.Init(_audioObjectInstance);
            
            Locator<AudioManager>.Set(audioManager);
            return IService.Result.Success;
        }
    }
}