using Cysharp.Threading.Tasks;
using Maniac.Utils;

namespace Maniac.SpawnerSystem
{
    public class ResetSpawnerManagerCommand : Command.Command
    {
        public override async UniTask Execute()
        {
            var spawnerManager = Locator<SpawnerManager>.Instance;
            if(spawnerManager == null)
                new SpawnerManager();
            
            spawnerManager.ResetAllSpawner();
            await UniTask.CompletedTask;
        }
    }
}