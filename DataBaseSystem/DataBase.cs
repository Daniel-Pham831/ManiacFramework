using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Maniac.DataBaseSystem
{
    // Use DataBase.ActiveDatabase if you are in inspector mode
    // Use Locator<Database>.Instance if you are in play mode
    public class DataBase : ScriptableObject
    {
#if UNITY_EDITOR
        public static DataBase ActiveDatabase
        {
            get
            {
                if (Locator<DataBase>.Instance == null)
                {
                    DataBase db = AssetDatabase.FindAssets($"t: {typeof(DataBase).Name}").ToList()
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<DataBase>)
                    .FirstOrDefault();

                    Locator<DataBase>.Set(db);
                }

                return Locator<DataBase>.Instance;
            }
        }
#endif

        private const string defaultConfigPath = "Assets/Resources/DatabaseConfigs";
        [SerializeField] private List<DataBaseConfig> configs;

        private Dictionary<string, DataBaseConfig> configCache = new Dictionary<string, DataBaseConfig>();

        public void InitializeDataBase()
        {
            Locator<DataBase>.Set(this);
        }
        
        public DataBaseConfig Get(string id)
        {
            if (!configCache.ContainsKey(id))
            {
                DataBaseConfig config = configs.FirstOrDefault(config => config.ID == id);
                configCache.Add(id, config);
            }

            return configCache[id];
        }

        public bool OverrideConfigWithRemoteConfig(string key, string json)
        {
            bool isSuccess = true;
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                };
                var gameConfig = Get(key);
                JsonConvert.PopulateObject(json, gameConfig,serializerSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                isSuccess = false;
            }

            return isSuccess;
        }

        public T Get<T>() where T : DataBaseConfig
        {
            string type = typeof(T).Name;
            return Get(type) as T;
        }
        
        public void Add(DataBaseConfig config)
        {
            ClearNull();

            if (configs.Contains(config))
            {
                Debug.LogError($"DBConfig: {config.ID} already exists!");
                return;
            }
            else
            {
                configs.Add(config);
            }
        }

        public void ClearNull()
        {
            configs.RemoveAll(item => item == null);
        }

#if UNITY_EDITOR

        [Button]
        public void FetchAll()
        {
            ClearNull();
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DataBaseConfig config = AssetDatabase.LoadAssetAtPath<DataBaseConfig>(path);
                if (config != null)
                {
                    if (!configs.Contains(config))
                        Add(config);
                }
            }
        }

        [Button]
        private void CreateAllConfig()
        {
            ClearNull();
            IEnumerable<Type> allTypes = typeof(DataBaseConfig).GetAllSubclasses(GetExceptList());
            Debug.Log($"CreateAllConfig: {allTypes.Count()}");

            if (allTypes.Count() != 0)
            {
                foreach (Type type in allTypes)
                {
                    ScriptableObject asset = ScriptableObject.CreateInstance(type);
                    AssetDatabase.CreateAsset(asset, $"{defaultConfigPath}/{type}.asset");
                    AssetDatabase.SaveAssets();
                }
            }

            FetchAll();
        }

        private List<Type> GetExceptList()
        {
            List<Type> exceptList = new List<Type>();
            foreach (DataBaseConfig config in configs)
            {
                exceptList.Add(config.GetType());
            }

            return exceptList;
        }

        [Button]
        private void CreateNewDataBaseConfig(string scriptName)
        {
            if (scriptName.Length == 0) return;
            if (scriptName.Where(c => Char.IsLetter(c)).Count() != scriptName.Length) return;

            string copyPath = $"{defaultConfigPath}/{scriptName}.cs";
            if (!Directory.Exists(defaultConfigPath))
            {
                Directory.CreateDirectory(defaultConfigPath);
            }

            if (File.Exists(copyPath) == false)
            {
                string templatePath = Path.Combine(Application.dataPath, "Maniac/DataBaseSystem/DataBaseConfigGenerator/DataBaseConfigTemplate.txt");
                string dataTemplateContent = File.ReadAllText(templatePath);
                dataTemplateContent = dataTemplateContent.Replace("@ScriptName", scriptName);
                // do not overwrite
                using (StreamWriter outfile = File.CreateText(copyPath))
                {
                    outfile.WriteLine(dataTemplateContent);
                } //File written
                AssetDatabase.Refresh();
                PlayerPrefs.SetString("NewConfig", scriptName);
                CompileScript();
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

        [DidReloadScripts]
        private static void OnReloadScriptFinish()
        {
            string scriptName = PlayerPrefs.GetString("NewConfig", "");
            PlayerPrefs.DeleteKey("NewConfig");
            //continue to state 2
            if (scriptName.Length == 0)
            {
                return;
            }

            CreateScriptableObject(scriptName);
        }

        private static void CreateScriptableObject(string scriptName)
        {
            Type type = System.Type.GetType($"Game.{scriptName}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (type == null)
            {
                return;
            }

            DataBaseConfig asset = (DataBaseConfig)ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(asset, $"{defaultConfigPath}/{type}.asset");
            DataBase.ActiveDatabase.Add(asset);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/DataBase/Create DataBase", priority = 81)]
        public static void CreateDataBase()
        {
            DataBase asset = ScriptableObject.CreateInstance<DataBase>();
            if (!Directory.Exists(defaultConfigPath))
            {
                Directory.CreateDirectory(defaultConfigPath);
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/DataBase.asset");
            AssetDatabase.SaveAssets();
        }
#endif
    }
}