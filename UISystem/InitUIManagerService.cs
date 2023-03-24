using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.UISystem
{
    public class InitUIManagerService : Service
    {
        private readonly UIData _uiData;
        private readonly UIManager _uiManagerPrefab;

        public InitUIManagerService(UIData uiData,UIManager uiManagerPrefab)
        {
            _uiData = uiData;
            _uiManagerPrefab = uiManagerPrefab;
        }
        
        public override async UniTask<IService.Result> Execute()
        {
            var uiManager = Object.Instantiate(_uiManagerPrefab);
            uiManager.name = "UI Manager";
            
            Locator<UIData>.Set(_uiData);
            Locator<UIManager>.Set(uiManager);
            
            uiManager.InitializeUILayer();
            
            Object.DontDestroyOnLoad(uiManager);
            return IService.Result.Success;
        }
    }
}