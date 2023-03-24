using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Maniac.UISystem
{
    public class UITransitionHandler:MonoBehaviour
    {
        private readonly float defaultRootCanvasDuration = 0.2f;
        
        public bool shouldUseTransitionPreset;
        [HideIf("shouldUseTransitionPreset")]
        [SerializeField] private UITransition uiTransition;

        [ShowIf("shouldUseTransitionPreset")]
        [ValueDropdown("FetchAllUITransitions")]
        [SerializeField] [OnValueChanged("OnPresetNameChanged")]
        private string uiTransitionPresetName = string.Empty;
        
        [ShowIf("shouldUseTransitionPreset")]
        [SerializeField] private UITransition uiTransitionPreset;

        [SerializeField] private Image blackBlackGround;
        [SerializeField] private CanvasGroup rootCanvasGroup;
        [SerializeField] private CanvasGroup mainCanvasGroup;
        
        [SerializeField] private RectTransform rootRectTransform;
        [SerializeField] private RectTransform mainRectTransform;

        public UITransition CurrentUITransition => shouldUseTransitionPreset ? uiTransitionPreset : uiTransition;
        
        public async UniTask DoTransition(bool isTransitionEnter = true)
        {
            blackBlackGround.enabled = CurrentUITransition.shouldEnableBlackBackGround;
            var duration = CurrentUITransition.shouldSeparateDuration
                ? isTransitionEnter ? CurrentUITransition.enterDuration : CurrentUITransition.exitDuration
                : CurrentUITransition.duration;

            var transition = isTransitionEnter
                ? CurrentUITransition.transitionEnter
                : CurrentUITransition.transitionExit;

            

            var ease = isTransitionEnter ? CurrentUITransition.easeEnter : CurrentUITransition.easeExit;

            await PlayTransitionSoundFX(isTransitionEnter, transition);

            switch (transition)
            {
                case UITransition.Transition.None:
                    SetBothCanvasGroup(1, true);
                    break;
                
                case UITransition.Transition.FadeIn:
                    SetCanvasGroup(rootCanvasGroup, 0, false);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    await rootCanvasGroup.DOFade(1, duration).SetEase(ease).AsyncWaitForCompletion();
                    SetBothCanvasGroup(1,true);
                    break;
                    
                case UITransition.Transition.FadeOut:
                    SetCanvasGroup(rootCanvasGroup, 1, true);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    await rootCanvasGroup.DOFade(0, duration).SetEase(ease).AsyncWaitForCompletion();
                    SetBothCanvasGroup(0,false);
                    break;
                
                case UITransition.Transition.ZoomIn:
                    SetCanvasGroup(rootCanvasGroup, 0, false);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    rootCanvasGroup.DOFade(1, 0.2f);
                    mainRectTransform.DOScale(Vector3.zero, 0);
                    SetBothCanvasGroup(1,true);
                    await mainRectTransform.DOScale(Vector3.one, duration).SetEase(ease).AsyncWaitForCompletion();
                    break;
                
                case UITransition.Transition.ZoomOut:
                    SetCanvasGroup(rootCanvasGroup, 1, true);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    mainRectTransform.DOScale(Vector3.one, 0);
                    await mainRectTransform.DOScale(Vector3.zero, duration).SetEase(ease).AsyncWaitForCompletion();
                    rootCanvasGroup.DOFade(0, defaultRootCanvasDuration);
                    SetBothCanvasGroup(0,false);
                    break;
                
                case UITransition.Transition.Top2Mid:
                    SetCanvasGroup(rootCanvasGroup, 0, false);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    rootCanvasGroup.DOFade(1, defaultRootCanvasDuration);
                    mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.up * mainRectTransform.rect.height, 0);
                    SetBothCanvasGroup(1,true);
                    await mainRectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(ease).AsyncWaitForCompletion();
                    break;
                
                case UITransition.Transition.Bot2Mid:
                    SetCanvasGroup(rootCanvasGroup, 0, false);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    rootCanvasGroup.DOFade(1, defaultRootCanvasDuration);
                    mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.down * mainRectTransform.rect.height, 0);
                    SetBothCanvasGroup(1,true);
                    await mainRectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(ease).AsyncWaitForCompletion();
                    break;
                
                case UITransition.Transition.Left2Mid:
                    SetCanvasGroup(rootCanvasGroup, 0, false);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    rootCanvasGroup.DOFade(1, defaultRootCanvasDuration);
                    mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.left * mainRectTransform.rect.width, 0);
                    SetBothCanvasGroup(1,true);
                    await mainRectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(ease).AsyncWaitForCompletion();
                    break;
                
                case UITransition.Transition.Right2Mid:
                    SetCanvasGroup(rootCanvasGroup, 0, false);
                    SetCanvasGroup(mainCanvasGroup, 1, true);
                    rootCanvasGroup.DOFade(1, defaultRootCanvasDuration);
                    mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.right * mainRectTransform.rect.width, 0);
                    SetBothCanvasGroup(1,true);
                    await mainRectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(ease).AsyncWaitForCompletion();
                    break;
                
                case UITransition.Transition.Mid2Top:
                    SetBothCanvasGroup( 1, true);
                    mainRectTransform.DOAnchorPos(Vector2.zero, 0);
                    rootCanvasGroup.DOFade(0, defaultRootCanvasDuration);
                    await mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.up * mainRectTransform.rect.height, duration).SetEase(ease).AsyncWaitForCompletion();
                    SetBothCanvasGroup(0,false);
                    break;
                
                case UITransition.Transition.Mid2Bot:
                    SetBothCanvasGroup( 1, true);
                    mainRectTransform.DOAnchorPos(Vector2.zero, 0);
                    rootCanvasGroup.DOFade(0, defaultRootCanvasDuration);
                    await mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.down * mainRectTransform.rect.height, duration).SetEase(ease).AsyncWaitForCompletion();
                    SetBothCanvasGroup(0,false);
                    break;
                
                case UITransition.Transition.Mid2Left:
                    SetBothCanvasGroup( 1, true);
                    mainRectTransform.DOAnchorPos(Vector2.zero, 0);
                    rootCanvasGroup.DOFade(0, defaultRootCanvasDuration);
                    await mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.left * mainRectTransform.rect.width, duration).SetEase(ease).AsyncWaitForCompletion();
                    SetBothCanvasGroup(0,false);
                    break;
                
                case UITransition.Transition.Mid2Right:
                    SetBothCanvasGroup( 1, true);
                    mainRectTransform.DOAnchorPos(Vector2.zero, 0);
                    rootCanvasGroup.DOFade(0, defaultRootCanvasDuration);
                    await mainRectTransform.DOAnchorPos(
                        mainRectTransform.anchoredPosition + Vector2.right * mainRectTransform.rect.width, duration).SetEase(ease).AsyncWaitForCompletion();
                    SetBothCanvasGroup(0,false);
                    break;
            }
            
           
            await UniTask.CompletedTask;
        }

        // This has to be implemented with SoundSystem
        private async Task PlayTransitionSoundFX(bool isTransitionEnter, UITransition.Transition transition)
        {
            switch (transition)
            {
                case UITransition.Transition.None:
                    break;

                case UITransition.Transition.FadeIn:
                case UITransition.Transition.FadeOut:
                case UITransition.Transition.ZoomIn:
                case UITransition.Transition.ZoomOut:
                    break;

                case UITransition.Transition.Bot2Mid:
                case UITransition.Transition.Top2Mid:
                case UITransition.Transition.Left2Mid:
                case UITransition.Transition.Right2Mid:
                case UITransition.Transition.Mid2Bot:
                case UITransition.Transition.Mid2Left:
                case UITransition.Transition.Mid2Right:
                case UITransition.Transition.Mid2Top:
                    break;
            }
        }

        private void SetBothCanvasGroup(float alpha, bool blockRaycast)
        {
            SetCanvasGroup(rootCanvasGroup, alpha, blockRaycast);
            SetCanvasGroup(mainCanvasGroup, alpha, blockRaycast);
        }
        
        public void SetCanvasGroup(CanvasGroup canvasGroup,float alpha, bool blockRaycast)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.blocksRaycasts = blockRaycast;
        }

#if UNITY_EDITOR
        [Button]
        public void SaveUITransitionPreset(string presetName)
        {
            if (shouldUseTransitionPreset) return;

            UIData.ActiveUIData.SaveTransitionPreset(presetName, uiTransition);
        }
        
        public IEnumerable<string> FetchAllUITransitions()
        {
            return UIData.ActiveUIData.GetAllTransitionPresets().Select(x=>x.name).ToList();
        }

        public void OnPresetNameChanged()
        {
            uiTransitionPreset = UIData.ActiveUIData.GetTransitionPreset(uiTransitionPresetName);
        }
#endif
    }
}