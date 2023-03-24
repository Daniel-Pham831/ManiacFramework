using System;
using Maniac.Utils;
using TMPro;
using UnityEngine;

namespace Maniac.LanguageTableSystem
{
    [RequireComponent(typeof(TMP_Text))]
    public class LanguageText:MonoBehaviour
    {
        private TMP_Text _text;
        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;
        
        [SerializeField]
        private LanguageItem _languageItem;
        
        public event Action<string, string> OnLanguageTextChanged;
        
        private void Awake()
        {
            if (_languageItem != null)
            {
                _text = GetComponent<TMP_Text>();
                SetLanguageItem(_languageItem);
            }
        }

        private void OnDestroy()
        {
            _languageTable.OnGlobalLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(string oldlanguage, string newlanguage)
        {
            var oldText = _text.text;
            _text.text = _languageItem.GetTextOfLanguage(newlanguage);
            OnLanguageTextChanged?.Invoke(oldText,_text.text);
        }

        public void SetLanguageItem(LanguageItem languageItem)
        {
            if (languageItem == null) return;
            
            _languageTable.OnGlobalLanguageChanged -= OnLanguageChanged;
            _languageTable.OnGlobalLanguageChanged += OnLanguageChanged;
            _languageItem = languageItem;
            _text.text = _languageItem.GetCurrentLanguageText();
        }

        public string GetCurrentLanguageText()
        {
            return _languageItem.GetCurrentLanguageText();
        }
    }
}