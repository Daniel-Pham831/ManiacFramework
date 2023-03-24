using Cysharp.Threading.Tasks;
using DG.Tweening;
using Maniac.Services;

namespace Maniac.Bootstrap.Scripts
{
    public class InitDotweenService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            DOTween.Init();
            return IService.Result.Success;
        }
    }
}