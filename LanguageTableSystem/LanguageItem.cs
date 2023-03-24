using System;
using System.Collections.Generic;
using System.Linq;
using Maniac.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Maniac.LanguageTableSystem
{
    public class LanguageItem : ScriptableObject
    {
        [Serializable]
        public class LanguageInfo
        {
            [HideInInspector]
            public string languageName;
            [LabelText("$languageName")][TextArea(4,10)]
            public string text = string.Empty;
        }

        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;

        [ReadOnly]
        public string id;
        [SerializeField][ListDrawerSettings(IsReadOnly = true)]
        public List<LanguageInfo> languageInfos;

        public string GetTextOfLanguage(string languageName)
        {
            string textResult = languageInfos.FirstOrDefault(x => x.languageName == languageName)?.text;
            if (textResult == string.Empty)
            {
                textResult = languageInfos.FirstOrDefault(x => x.languageName == _languageTable.GetDefaultLanguage())
                    ?.text;
            }

            return textResult;
        }

        public string GetCurrentLanguageText()
        {
            return languageInfos.FirstOrDefault(x => x.languageName == _languageTable.CurrentLanguage)?.text;
        }
        
        #if UNITY_EDITOR
        [Button]
        private void DeleteThisLanguageItem()
        {
            if (EditorUtility.DisplayDialog($"Delete {id} LanguageItem?",
                    $"Are you sure you want to delete {id} LanguageItem?", "Yes", "Do"))
            {
                LanguageTable.ActiveLanguageTable.DeleteLanguageItem(this);
            }
        }
        #endif
    }
}