using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;

namespace Maniac.SpawnerSystem
{
    public class InitSpawnerManagerService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var spawnerManager = new SpawnerManager();
            spawnerManager.Initialize();
            Locator<SpawnerManager>.Set(spawnerManager);

            return IService.Result.Success;
        }
    }
}