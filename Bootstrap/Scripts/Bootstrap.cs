using DG.Tweening;
using Maniac.AudioSystem;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.ProfileSystem;
using Maniac.Services;
using Maniac.SpawnerSystem;
using Maniac.TimeSystem;
using Maniac.UISystem;
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
            var subServiceGroup = CreateSubServiceGroup();

            var bootStrap = new SequenceServiceGroup("BootStrap Service");
            bootStrap.Add(essentialServiceGroup);
            bootStrap.Add(subServiceGroup);

            return bootStrap;
        }

        private Service CreateEssentialServiceGroup()
        {
            var essentialServiceGroup = new SequenceServiceGroup("Essential Services");
            
            essentialServiceGroup.Add(new InitDotweenService());
            essentialServiceGroup.Add(new InitTimeManagerService());
            essentialServiceGroup.Add(new InitUIManagerService(uiData,uiManagerPrefab));
            essentialServiceGroup.Add(new InitDataBaseService(dataBase));
            essentialServiceGroup.Add(new InitSpawnerManagerService());
            essentialServiceGroup.Add(new InitProfileManagerService());
            essentialServiceGroup.Add(new InitLanguageTableService(languageTable)); //this should be behind InitProfileManagerService
            essentialServiceGroup.Add(new InitAudioManagerService(audioData,audioObjectPrefab)); //this should be behind InitProfileManagerService

            return essentialServiceGroup;
        }

        private Service CreateSubServiceGroup()
        {
            var subServiceGroup = new SequenceServiceGroup("Init SubSystems Services");

            // Add your game initialization here


            return subServiceGroup;
        }
    }
}