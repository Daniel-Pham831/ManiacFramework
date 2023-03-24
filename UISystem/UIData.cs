using System;
using System.Collections.Generic;
using System.Linq;
using Maniac.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Maniac.UISystem
{
    [CreateAssetMenu(fileName = "UIData", menuName = "UI/Data", order = 1)]
    public class UIData : ScriptableObject
    {
        [Serializable]
        public class UITransitionPreset
        {
            public string name;
            public UITransition preset;
        }
        
#if UNITY_EDITOR
        public static UIData ActiveUIData
        {
            get
            {
                if (Locator<UIData>.Instance == null)
                {
                    UIData uiData = AssetDatabase.FindAssets($"t: {typeof(UIData).Name}").ToList()
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<UIData>)
                    .FirstOrDefault();

                    Locator<UIData>.Set(uiData);
                }

                return Locator<UIData>.Instance;
            }
        }
#endif

        [SerializeField] private List<BaseUI> uis = new List<BaseUI>();
        [SerializeField] private List<string> uiLayers = new List<string>();
        [SerializeField] private List<UITransitionPreset> presets = new List<UITransitionPreset>();

        public BaseUI Get(string uiName)
        {
            return uis.FirstOrDefault(ui => ui.uiName == uiName);
        }

        public List<string> GetUILayers()
        {
            if(uiLayers.Count == 0)
                uiLayers.Add("Default");
            
            return uiLayers;
        }

        public void Add(BaseUI ui)
        {
            uis.RemoveAll(item => item == null);

            if (uis.Contains(ui))
            {
                Debug.LogError($"UI: {ui.uiName} already exists!");
                return;
            }
            else
            {
                uis.Add(ui);
            }
        }

        public UITransition GetTransitionPreset(string name)
        {
            return presets.FirstOrDefault(x => x.name == name)?.preset;
        }

        public IEnumerable<UITransitionPreset> GetAllTransitionPresets()
        {
            return presets;
        }

        public void SaveTransitionPreset(string name, UITransition transition)
        {
            if (presets.FirstOrDefault(x => x.name == name) != null)
            {
                Debug.Log($"Preset name: {name} already exist!");
                return;
            }
            
            presets.Add(new UITransitionPreset()
            {
                name = name,
                preset = transition
            });
        }

#if UNITY_EDITOR

        [Button]
        public void ClearNull()
        {
            uis.RemoveAll(item => item == null);
        }

        [Button]
        public void FetchAll()
        {
            ClearNull();
            string[] guids = AssetDatabase.FindAssets("t:prefab");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (go.TryGetComponent<BaseUI>(out BaseUI ui))
                {
                    if (!uis.Contains(ui))
                        Add(ui);
                }
            }
        }
        
        public static void CreateUIData()
        {
            UIData asset = ScriptableObject.CreateInstance<UIData>();
            AssetDatabase.CreateAsset(asset, $"Assets/Resources/UIData.asset");
            AssetDatabase.SaveAssets();
        }
#endif
    }
}