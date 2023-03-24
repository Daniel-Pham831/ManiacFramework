using System;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Maniac.UISystem
{
    [Serializable]
    public class UITransition
    {
        public enum Transition
        {
            None,
            FadeIn,
            FadeOut,
            ZoomIn,
            ZoomOut,
            Top2Mid,
            Mid2Top,
            Left2Mid,
            Mid2Left,
            Right2Mid,
            Mid2Right,
            Bot2Mid,
            Mid2Bot,
        }

        public Transition transitionEnter = Transition.FadeIn;
        public Transition transitionExit = Transition.FadeOut;
        public Ease easeEnter = Ease.OutCubic;
        public Ease easeExit = Ease.OutCubic;
        public bool shouldEnableBlackBackGround;
        public bool shouldSeparateDuration;
        [HideIf("shouldSeparateDuration")]
        public float duration = 0.5f;
        [ShowIf("shouldSeparateDuration")]
        public float enterDuration;
        [ShowIf("shouldSeparateDuration")]
        public float exitDuration;
    }
}