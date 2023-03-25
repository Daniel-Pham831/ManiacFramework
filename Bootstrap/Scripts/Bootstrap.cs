using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Game;
using Maniac.AudioSystem;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.ProfileSystem;
using Maniac.Services;
using Maniac.SpawnerSystem;
using Maniac.TimeSystem;
using Maniac.UISystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Maniac.Bootstrap.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private DataBase dataBase;
        [SerializeField] private LanguageTable languageTable;
        
        [Header("UI")]
        [SerializeField] private UIData uiData;
        [SerializeField] private UIManager uiManagerPrefab;
        
        [Header("Audio")]
        [SerializeField] private AudioData audioData;
        [SerializeField] private AudioObjectInstance audioObjectPrefab;

        private async void Start()
        {
            var bootStrapService = CreateBootStrapServiceGroup();

            await bootStrapService.Run();
        }

        private Service CreateBootStrapServiceGroup()
        {
            var essentialServiceGroup = CreateEssentialServiceGroup();
            var gameServiceGroup = CreateGameServiceGroup();

            var bootStrap = new SequenceServiceGroup("BootStrap Service");
            bootStrap.Add(essentialServiceGroup);
            bootStrap.Add(gameServiceGroup);

            return bootStrap;
        }

        private Service CreateEssentialServiceGroup()
        {
            var essentialServiceGroup = new SequenceServiceGroup("Maniac Essential Services");
            
            essentialServiceGroup.Add(new InitDotweenService());
            essentialServiceGroup.Add(new InitTimeManagerService());
            essentialServiceGroup.Add(new InitDataBaseService(dataBase));
            essentialServiceGroup.Add(new InitUIManagerService(uiData,uiManagerPrefab));
            essentialServiceGroup.Add(new InitSpawnerManagerService());
            essentialServiceGroup.Add(new InitProfileManagerService());
            essentialServiceGroup.Add(new InitLanguageTableService(languageTable)); //this should be behind InitProfileManagerService
            essentialServiceGroup.Add(new InitAudioManagerService(audioData,audioObjectPrefab)); //this should be behind InitProfileManagerService

            return essentialServiceGroup;
        }

        private Service CreateGameServiceGroup()
        {
            var subServiceGroup = new SequenceServiceGroup("Game Systems Services");

            // Add your game initialization services here
            // You should create your own Service to change to your scene
            return subServiceGroup;
        }
    }
}