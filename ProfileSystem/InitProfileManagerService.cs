using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Maniac.ProfileSystem
{
    public class InitProfileManagerService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var profileManager = new ProfileManager();
            Locator<ProfileManager>.Set(profileManager);
            
            profileManager.Init();
            return IService.Result.Success;
        }
    }
}