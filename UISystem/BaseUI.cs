using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Maniac.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Maniac.UISystem
{
    public class BaseUI : MonoBehaviour
    {
        [ValueDropdown("FetchAllUILayers")][SerializeField]
        private string layer;
        public string Layer => layer;

        [SerializeField] private bool shouldExceptThisUIOnCloseAllUI = false;
        
        public string uiName => GetType().ToString();
        protected CanvasGroup canvasGroup;
        protected RectTransform rectTransform;
        protected UIManager _uiManager => Locator<UIManager>.Instance;
        protected UITransitionHandler uiTransitionHandler;
        
        public Action<object> OnClose;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            uiTransitionHandler = GetComponent<UITransitionHandler>();

            if (canvasGroup == null)
                gameObject.AddComponent<CanvasGroup>();

            SetCanvasGroup(0, true);
            
            if(shouldExceptThisUIOnCloseAllUI)
                AddToCloseAllExcept();
        }

        public virtual void SetInteraction(bool interactable)
        {
            canvasGroup.interactable = interactable;
            SetCanvasGroup(interactable ? 1 : 0, interactable);
        }

        /// <summary>
        /// When uiManager.Show<T>. This will be called first.
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void OnSetup(object parameter = null) { }

        /// <summary>
        /// When uiManager.Show<T>. This will be called second.
        /// </summary>
        /// <param name="parameter"></param>
        public virtual async UniTask OnTransitionEnter(object parameter = null)
        {
            await uiTransitionHandler.DoTransition(true);
        }

        /// <summary>
        /// When uiManager.Show<T>. This will be called last.
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void OnShow(object parameter = null)
        {
        }

        public virtual async UniTask OnTransitionExit()
        {
            await uiTransitionHandler.DoTransition(false);
        }

        public virtual async UniTask Close(object param = null)
        {
            await _uiManager.Close(uiName);
            OnClose?.Invoke(param);
        }

        public virtual async void Back()
        {
            await Close();
        }
        
        //Force close without animation
        public virtual async UniTask ForceClose(object param = null)
        {
            await _uiManager.ForceClose(uiName);
            OnClose?.Invoke(param);
        }
        
        protected void SetCanvasGroup(int alpha, bool blockRaycast)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.blocksRaycasts = blockRaycast;
        }

        public async UniTask DOFade(float alpha, float duration)
        {
            await canvasGroup.DOFade(alpha, duration).AsyncWaitForCompletion();
            await UniTask.CompletedTask;
        }

        public async UniTask WaitCompletion()
        {
            bool hidden = false;
            OnClose += o => hidden = true;

            await UniTask.WaitUntil(() => hidden);
        }

        private void AddToCloseAllExcept()
        {
            _uiManager.AddToCloseAllExceptList(uiName);
        }

#if UNITY_EDITOR

        [Button]
        public void UpdateToUIData()
        {
            var uiData = UIData.ActiveUIData;
            if (uiData != null)
            {
                uiData.Add(this);
                return;
            }
            
            string[] results = AssetDatabase.FindAssets("UIData");
            foreach (string guid in results)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UIData uidata = AssetDatabase.LoadAssetAtPath(path, typeof(UIData)) as UIData;

                if (uidata != null)
                {
                    uidata.Add(this);
                }
            }
        }
        
        private List<string> FetchAllUILayers()
        {
            return UIData.ActiveUIData.GetUILayers();
        }
#endif
    }
}