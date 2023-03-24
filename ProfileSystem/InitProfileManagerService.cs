using Cysharp.Threading.Tasks;
using Maniac.Services;

namespace Maniac.ProfileSystem
{
    public class InitProfileManagerService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            ProfileManager.LoadAllProfileRecordsIntoCache();
            return IService.Result.Success;
        }
    }
}