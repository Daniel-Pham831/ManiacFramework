using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.UISystem
{
    public class UIManager : MonoBehaviour
    {
        private UIData _uiData => Locator<UIData>.Instance;
        
        [SerializeField] private Transform layerRoot;
        [SerializeField] private GameObject layerPrefab;
        [SerializeField] private Canvas canvas;
        
        private Dictionary<string,Transform> _uiLayerTransformRoots;
        private Dictionary<string, BaseUI> _showedUI;

        private HashSet<string> _closeAllExcepts = new HashSet<string>();

        private void Awake()
        {
            _uiLayerTransformRoots = new Dictionary<string, Transform>();
            _showedUI = new Dictionary<string, BaseUI>();
        }

        public void InitializeUILayer()
        {
            _uiLayerTransformRoots ??= new Dictionary<string, Transform>();
            var uiLayers = _uiData.GetUILayers();
            foreach (var uiLayer in uiLayers)
            {
                if (_uiLayerTransformRoots.ContainsKey(uiLayer)) continue;
                
                var layerTransform = Instantiate(layerPrefab, layerRoot);
                layerTransform.name = uiLayer;
                
                _uiLayerTransformRoots.Add(uiLayer,layerTransform.transform);
            }
        }

        public void CloseAllCurrentShowed()
        {
            IEnumerable<string> keys = _showedUI.Keys.ToList();
            if (_closeAllExcepts != null)
                keys = keys.Where(x => !_closeAllExcepts.Contains(x));
            
            foreach (var key in keys)
            {
#pragma warning disable CS4014
                Close(key);
#pragma warning restore CS4014
            }
        }

        public List<BaseUI> GetAllShowedUIs()
        {
            return _showedUI.Values.ToList();
        }

        public void AddToCloseAllExceptList(string ui)
        {
            _closeAllExcepts ??= new HashSet<string>();
            
            _closeAllExcepts.Add(ui);
        }

        public async UniTask<BaseUI> Show<T>(object parameter = null) where T : BaseUI
        {
            string uiName = typeof(T).ToString();
            return await Show(uiName, parameter);
        }

        public T GetShowedUI<T>() where T : BaseUI
        {
            string uiName = typeof(T).ToString();
            return GetShowedUI(uiName) as T;
        }

        public BaseUI GetShowedUI(Type uiType)
        {
            string uiName = uiType.ToString();
            return GetShowedUI(uiName);
        }

        public BaseUI GetShowedUI(string uiName)
        {
            if (_showedUI.TryGetValue(uiName, out BaseUI ui))
            {
                return ui;
            }

            return null;
        }

        private async UniTask<BaseUI> Show(string uiName, object parameter = null)
        {
            if (_showedUI.ContainsKey(uiName))
            {
                Debug.LogError($"UI: {uiName} already showed.");
                return null;
            }

            BaseUI ui = CreateUI(uiName);
            if (ui != null)
            {
                _showedUI.Add(ui.uiName, ui);

                ui.OnSetup(@parameter);
                await ui.OnTransitionEnter(@parameter);
                ui.OnShow(@parameter);
            }

            return ui;
        }

        public async UniTask<BaseUI> Show(Type type, object parameter = null)
        {
            string uiName = type.ToString();
            return await Show(uiName, parameter);
        }

        public async UniTask Close<T>() where T : BaseUI
        {
            string uiName = typeof(T).ToString();
            await Close(uiName);
        }

        public async UniTask Close(string uiName)
        {
            if (_showedUI.TryGetValue(uiName, out BaseUI ui))
            {
                _showedUI.Remove(uiName);

                await ui.OnTransitionExit();
                await UniTask.CompletedTask;
                DestroyUI(ui);
            }
            else
            {
                Debug.LogError($"UI: {uiName} already closed");
            }
        }

        // Close without transitionExit
        public async UniTask ForceClose<T>() where T : BaseUI
        {
            string uiName = typeof(T).ToString();
            await ForceClose(uiName);
        }
        
        public async UniTask ForceClose(string uiName)
        {
            if (_showedUI.TryGetValue(uiName, out BaseUI ui))
            {
                _showedUI.Remove(uiName);

                await UniTask.CompletedTask;
                DestroyUI(ui);
            }
            else
            {
                Debug.LogError($"UI: {uiName} already closed");
            }
        }

        public BaseUI CreateUI(string uiName)
        {
            BaseUI uiPrefab = _uiData.Get(uiName);
            if (uiPrefab == null)
            {
                Debug.LogError($"UI: {uiName} doesn't exists in UIData. Please investigate!");
                return null;
            }
            
            var layerRoot = GetCorrectLayer(uiPrefab.Layer);
            if (layerRoot == null)
            {
                Debug.LogError($"uiPrefab.Layer: {uiPrefab.Layer} doesn't exists in UIData. Please investigate!");
                return null;
            }
            
            return Instantiate(uiPrefab, layerRoot);
        }

        public void DestroyUI(BaseUI ui)
        {
            if (ui == null)
                Debug.LogError($"UI: {nameof(ui)} is null");
            else
                Destroy(ui.gameObject);
        }

        public void UpdateMainCamera()
        {
            canvas.worldCamera = Helper.Camera;
        }

        public Transform GetCorrectLayer(string layerName)
        {
            if (!_uiLayerTransformRoots.TryGetValue(layerName, out var layerTransform))
            {
                InitializeUILayer();
            }

            return _uiLayerTransformRoots[layerName];
        }
    }
}
