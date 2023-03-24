using DG.Tweening;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.ProfileSystem;
using Maniac.Services;
using Maniac.SpawnerSystem;
using Maniac.UISystem;
using UnityEngine;

namespace Maniac.Bootstrap.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private DataBase dataBase;
        [SerializeField] private LanguageTable languageTable;
        
        [SerializeField] private UIData uiData;
        [SerializeField] private UIManager uiManagerPrefab;
        
        private void Awake()
        {
            DOTween.Init();
        }

        private async void Start()
        {
            var mainService = CreateGameService();

            await mainService.Run();
        }

        private Service CreateGameService()
        {
            var startServiceGroup = new SequenceServiceGroup("Start Service");
            
            startServiceGroup.Add(new InitDotweenService());
            startServiceGroup.Add(new InitUIManagerService(uiData,uiManagerPrefab));
            startServiceGroup.Add(new InitDataBaseService(dataBase));

            var essentialServiceGroup = new SequenceServiceGroup("Essential Service");
            
            essentialServiceGroup.Add(new InitSpawnerManagerService());
            essentialServiceGroup.Add(new InitProfileManagerService());
            essentialServiceGroup.Add(new InitLanguageTableService(languageTable)); //this should be behind InitProfileManagerService


            var gameService = new SequenceServiceGroup("Game Service");
            gameService.Add(startServiceGroup);
            gameService.Add(essentialServiceGroup);

            return gameService;
        }
    }
}