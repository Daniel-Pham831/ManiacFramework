using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Maniac.DataBaseSystem
{
    public class DataBaseConfig : ScriptableObject
    {
        [JsonIgnore]
        public string ID => GetType().Name;

#if UNITY_EDITOR
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
        
        [Sirenix.OdinInspector.Button("Export JSON", ButtonSizes.Medium)]
        [PropertyOrder(-1)]
        [HorizontalGroup("Buttons-JSON")]
        public void ExportToJSON()
        {
            string path = EditorUtility.SaveFilePanel("Choose JSON File", Application.dataPath, $"{ID}", "json");
            if (!string.IsNullOrEmpty(path))
            {
                var json = JsonConvert.SerializeObject(this, _jsonSerializerSettings);
                File.WriteAllText(path, json);
                Debug.Log("Exported!");
            }
        }
#endif
    }
}