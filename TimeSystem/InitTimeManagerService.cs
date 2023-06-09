using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;
using Unity.VisualScripting;

namespace Maniac.TimeSystem
{
    public class InitTimeManagerService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var timeManager = new TimeManager();
            Locator<TimeManager>.Set(timeManager);

            timeManager.Init();
            return IService.Result.Success;
        }
    }
}