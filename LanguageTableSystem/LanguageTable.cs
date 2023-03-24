using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Maniac.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Maniac.LanguageTableSystem
{
    public partial class LanguageTable:ScriptableObject
    {
        [Serializable]
        public class LanguageFlag
        {
            [ValueDropdown("FetchAllLanguages")]
            public string languageId;
            public Sprite flagIcon;
            #if UNITY_EDITOR
            public IEnumerable<string> FetchAllLanguages()
            {
                return LanguageTable.ActiveLanguageTable.GetAllLanguages();
            }
            #endif
        }
        
#if UNITY_EDITOR
        public static LanguageTable ActiveLanguageTable
        {
            get
            {
                if (Locator<LanguageTable>.Instance == null)
                {
                    LanguageTable languageTable = AssetDatabase.FindAssets($"t: {typeof(LanguageTable).Name}").ToList()
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<LanguageTable>)
                        .FirstOrDefault();

                    Locator<LanguageTable>.Set(languageTable);
                }

                return Locator<LanguageTable>.Instance;
            }
        }
#endif
        private const string defaultConfigPath = "Assets/Resources/LanguageItems";
        private const string keyScriptPath = "Assets/Resources/LanguageItems/LanguageTableExt.cs";
        private const string keyScriptTemplatePath = "Maniac/LanguageTableSystem/LanguageTableExtTemplate.txt";
        private const string keyFormat = "\n\t\tpublic static readonly string {0} = \"{0}\";";
        private const string placeFormat = "//[HERE]";
        
        [SerializeField][ReadOnly][ShowInInspector][ListDrawerSettings(IsReadOnly = true)]
        private List<string> _languages = new List<string>();
        [SerializeField] private List<LanguageFlag> _languageFlags;
        [SerializeField][ValueDropdown("_languages")]
        private string _defaultLanguage;

        [SerializeField]
        private List<LanguageItem> items;
        private Dictionary<string, LanguageItem> cacheItems = new Dictionary<string, LanguageItem>();

        public string CurrentLanguage { get; private set; } = string.Empty;
        
        //--Event--
        public delegate void OnLanguageChanged(string oldLanguage,string newLanguage);
        public OnLanguageChanged OnGlobalLanguageChanged;

        private Queue<string> _languageQueue = new Queue<string>();

        public void Init()
        {
            foreach (var language in _languages)
            {
                _languageQueue.Enqueue(language);
            }

            while (_languageQueue.Peek() == CurrentLanguage)
            {
                var languageId = _languageQueue.Dequeue();
                _languageQueue.Enqueue(languageId);
            }
        }
        
        public void ClearNull()
        {
            items.RemoveAll(item => item == null);
        }

        public Sprite GetFlagIcon(string languageId)
        {
            return _languageFlags.FirstOrDefault(x => x.languageId == languageId).flagIcon;
        }

        public Sprite GetCurrentFlagIcon()
        {
            return GetFlagIcon(CurrentLanguage);
        }
        
        public LanguageItem Get(string id)
        {
            if (!cacheItems.ContainsKey(id))
            {
                LanguageItem languageItem = items.FirstOrDefault(x => x.id == id);
                cacheItems.Add(id,languageItem);
            }
            
            return cacheItems[id];
        }

        public List<LanguageItem> GetAllItems()
        {
            ClearNull();
            return items;
        }

        public IEnumerable<string> GetAllLanguageItemIds()
        {
            List<string> result = new List<string>(){string.Empty};
            foreach (var item in items)
            {
                result.Add(item.id);
            }

            return result;
        }

        private void Add(LanguageItem item)
        {
            items.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageName">LanguageTable.[Name]</param>
        public void ChangeLanguage(string languageName)
        {
            if (!_languages.Contains(languageName))
            {
                Debug.LogError($"{languageName} doesn't exist.");
                return;
            }
            
            var oldLanguage = CurrentLanguage;
            CurrentLanguage = languageName;
            OnGlobalLanguageChanged?.Invoke(oldLanguage,CurrentLanguage);
        }

        public void ChangeNextLanguage()
        {
            var nextLanguageId = _languageQueue.Dequeue();
            _languageQueue.Enqueue(nextLanguageId);
            
            ChangeLanguage(nextLanguageId);
        }

        public string GetDefaultLanguage()
        {
            return _defaultLanguage;
        }

        public List<string> GetAllLanguages()
        {
            return _languages;
        }
        
#if UNITY_EDITOR
        [SerializeField]
        [ValueDropdown("ShowAllLanguage")] 
        private string languageToDelete;

        private List<string> ShowAllLanguage()
        {
            var result = _languages.ToList();
            result.Add("");
            return result;
        }
        
        [Button]
        private void DeleteLanguage()
        {
            if (languageToDelete == string.Empty && !_languages.Contains(languageToDelete)) return;

            if (EditorUtility.DisplayDialog($"Delete {languageToDelete} Language?",
                    $"Are you sure you want to delete {languageToDelete} Language?", "Yes", "Do"))
            {
                _languages.Remove(languageToDelete);
                UpdateLanguageForAllItems();
                UpdateAndClearAllNullKey();
            }
        }
        
        [Button]
        private void CreateNewLanguage(string newLanguageName)
        {
            if (newLanguageName == string.Empty)
            {
                Debug.LogError($"newLanguageName is empty!");
                return;
            }

            if (newLanguageName.FirstOrDefault(x => !char.IsLetter(x)) != default)
            {
                Debug.LogError($"newLanguageName is not valid!");
                return;
            }
            
            if (_languages.FirstOrDefault(x => x == newLanguageName) != null)
            {
                Debug.LogError($"id:{newLanguageName} is already existed. Please change it!");
                return;
            }

            AddNewLanguage(newLanguageName);
            UpdateAndClearAllNullKey();
        }
        
        private void AddNewLanguage(string languageName)
        {
            if (languageName == string.Empty && _languages.Contains(languageName)) return;

            if (_languages.Count == 0)
                _defaultLanguage = languageName;
            
            _languages.Add(languageName);
            UpdateLanguageForAllItems();
        }

        private void UpdateLanguageForAllItems()
        {
            foreach (var language in _languages)
            {
                foreach (var item in items)
                {
                    item.languageInfos = item.languageInfos
                        .Where(x => x.languageName != string.Empty && _languages.Contains(x.languageName)).ToList();

                    if (item.languageInfos.FirstOrDefault(x => x.languageName == language) != null) continue;

                    var newLanguageInfo = new LanguageItem.LanguageInfo();
                    newLanguageInfo.languageName = language;
                    item.languageInfos.Add(newLanguageInfo);
                }
            }
        }

        [Button]
        private void CreateNewLanguageItem(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogError($"id is empty!");
                return;
            }
            
            id = id.Replace(".", "_");
            if (items.FirstOrDefault(x => x.id == id) != null)
            {
                Debug.LogError($"id:{id} is already existed. Please change it!");
                return;
            }
            
            var asset = ScriptableObject.CreateInstance<LanguageItem>();
            AssetDatabase.CreateAsset(asset, $"{defaultConfigPath}/{id}.asset");
            Add(asset);
            AssetDatabase.SaveAssets();

            asset.id = id;
            asset.languageInfos = new List<LanguageItem.LanguageInfo>();
            foreach (var language in _languages)
            {
                var newLanguageInfo = new LanguageItem.LanguageInfo();
                newLanguageInfo.languageName = language;
                asset.languageInfos.Add(newLanguageInfo);
            }

            UpdateAndClearAllNullKey();
        }

        [Button]
        private void UpdateAndClearAllNullKey()
        {
            items = items.Where(item => item != null).ToList();

            string keyTemplate = Path.Combine(Application.dataPath, keyScriptTemplatePath);
            StringBuilder keyScriptContent = new StringBuilder(); 
            keyScriptContent.Append(File.ReadAllText(keyTemplate));

            foreach (var language in _languages)
            {
                string newKey = string.Format(keyFormat, language);
                keyScriptContent = keyScriptContent.Insert(keyScriptContent.ToString().LastIndexOf("]")+1, newKey);
            }
            
            foreach (var item in items)
            {
                string newKey = string.Format(keyFormat, item.id);
                keyScriptContent = keyScriptContent.Insert(keyScriptContent.ToString().LastIndexOf("]")+1, newKey);
            }
            
            using (StreamWriter outfile = File.CreateText(keyScriptPath))
            {
                outfile.WriteLine(keyScriptContent);
            }
            CompileScript();
        }
        
        

        [Button]
        private void FetchAllLanguageItems()
        {
            ClearNull();
            string[] guids = AssetDatabase.FindAssets("t:LanguageItem",new string[]{"Assets/Resources/LanguageItems"});

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                LanguageItem languageItem = AssetDatabase.LoadAssetAtPath<LanguageItem>(path);

                if (!items.Contains(languageItem))
                {
                    items.Add(languageItem);
                }
            }
        }

        private void CompileScript()
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            CompilationPipeline.RequestScriptCompilation();
            CompilationPipeline.assemblyCompilationFinished += OnCompileFinish;
        }

        private void OnCompileFinish(string arg1, CompilerMessage[] arg2)
        {
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
            CompilationPipeline.assemblyCompilationFinished -= OnCompileFinish;
        }
        
        [MenuItem("Assets/Create/LanguageTable/Create LanguageTable", priority = 81)]
        public static void CreateLanguageTable()
        {
            LanguageTable asset = ScriptableObject.CreateInstance<LanguageTable>();
            if (!Directory.Exists(defaultConfigPath))
            {
                Directory.CreateDirectory(defaultConfigPath);
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/LanguageTable.asset");
            AssetDatabase.SaveAssets();
        }
        
        public void DeleteLanguageItem(LanguageItem languageItem)
        {
            items.Remove(languageItem);
            AssetDatabase.DeleteAsset($"Assets/Resources/LanguageItems/{languageItem.id}.asset");
            AssetDatabase.SaveAssets();
            UpdateAndClearAllNullKey();
        }
#endif
    }
}