using System;

namespace Maniac.TimeSystem
{
    public class Timer
    {
        protected float currentTime = 0f;
        protected Action callback;

        public void SetCallback(Action callback)
        {
            this.callback = callback;
        }

        public void Start(float duration, Action callback = null)
        {
            this.currentTime = duration;
            this.callback = callback;

            if (duration == 0)
            {
                callback?.Invoke();
            }
        }

        public void Update(float dt)
        {
            if (IsTimerActive)
            {
                this.currentTime -= dt;
                if (currentTime <= 0)
                {
                    OnTimerComplete();
                }
            }
        }

        public void OnTimerComplete()
        {
            currentTime = 0;
            callback?.Invoke();
            DeActiveTimer();
        }

        public void DeActiveTimer()
        {
            callback = null;
            currentTime = 0;
        }

        public bool IsTimerActive => this.currentTime > 0;
    }
}