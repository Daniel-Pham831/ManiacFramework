using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Maniac.AudioSystem
{
    public class AudioData : ScriptableObject
    {
        [SerializeField]
        private List<AudioInfo> audioInfos;

        private AudioObjectInstance _audioObjectPrefab;
        
        public void Add(AudioInfo audioInfo)
        {
            if (!audioInfos.Contains(audioInfo))
            {
                audioInfos.Add(audioInfo);
            }
        }
        
        public AudioInfo Get(string audioName)
        {
            return audioInfos.FirstOrDefault(x => x.name == audioName);
        }

        public IEnumerable<string> GetAllAudioNames()
        {
            return audioInfos.Select(x => x.name);
        }

        public void GetAudioObjectPrefab()
        {
            if (_audioObjectPrefab == null)
            {
                
            }
        }

#if UNITY_EDITOR
        private const string defaultConfigPath = "Assets/Resources/Audio";
        private const string keyScriptPath = "Assets/Resources/Audio/Audio.cs";
        private const string keyScriptTemplatePath = "Maniac/AudioSystem/AudioTemplate.txt";
        private const string keyFormat = "\n\t\tpublic static readonly string {0} = \"{0}\";";

        [Button]
        public void ClearAllNullKey()
        {
            audioInfos = audioInfos.Where(x => x != null).ToList();
        }
        
        [Button]
        private void UpdateAllAudios()
        {
            string keyTemplate = Path.Combine(Application.dataPath, keyScriptTemplatePath);
            StringBuilder keyScriptContent = new StringBuilder(); 
            keyScriptContent.Append(File.ReadAllText(keyTemplate));

            foreach (var soundInfo in audioInfos)
            {
                string newKey = string.Format(keyFormat, soundInfo.name);
                keyScriptContent = keyScriptContent.Insert(keyScriptContent.ToString().LastIndexOf("]")+1, newKey);
            }
            
            using (StreamWriter outfile = File.CreateText(keyScriptPath))
            {
                outfile.WriteLine(keyScriptContent);
            }
            CompileScript();
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
        
        [Button]
        public void CreateNewAudioInfo(string name)
        {
            if (!Helper.IsValidVariableName(name))
            {
                Debug.LogError($"{name} is not a valid {"VARIABLE".AddColor(Color.red)} name");
                return;
            }

            if (audioInfos.FirstOrDefault(x => x.name == name) != null)
            {
                Debug.LogError($"id:{name} is already existed. Please change it!");
                return;
            }
            
            AddNewAudioInfo(name);
        }

        private void AddNewAudioInfo(string name)
        {
            var newAudioInfo = new AudioInfo()
            {
                name = name,
                variations = new List<AudioClip>()
            };

            Add(newAudioInfo);

            UpdateAllAudios();
        }
        
        public static void CreateAudioData()
        {
            AudioData asset = ScriptableObject.CreateInstance<AudioData>();
            if (!Directory.Exists(defaultConfigPath))
            {
                Directory.CreateDirectory(defaultConfigPath);
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/AudioData.asset");
            AssetDatabase.SaveAssets();
        }
#endif
    }
}