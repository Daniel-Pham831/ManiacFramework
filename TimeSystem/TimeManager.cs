using System;
using System.Collections.Generic;

namespace Maniac.TimeSystem
{
    public class TimeManager
    {
        public static float DeltaTime { get; private set; }
        public static float FixedDeltaTime { get; private set; }
        public static float Time { get; private set; }
        private static float _speedUpMultiplier = 1f;
        private Queue<Timer> _freeTimers = new Queue<Timer>();
        private List<Timer> _activeTimers = new List<Timer>();

        private void Init()
        {
            Time = 0;
        }

        public void OnTimeOut(Action callback, float duration)
        {
            if (duration == 0)
            {
                callback?.Invoke();

                return;
            }

            Timer timer = GetFreeTimer();
            timer.Start(duration, callback);
            _activeTimers.Add(timer);
        }

        public void Update(float dependentGamePlayDeltaTime = 0)
        {
            dependentGamePlayDeltaTime *= _speedUpMultiplier;
            UpdateActiveTimers(dependentGamePlayDeltaTime);
            DeltaTime = dependentGamePlayDeltaTime;
            Time += dependentGamePlayDeltaTime;
        }
        
        public void FixedUpdate(float fixedDeltaTime = 0)
        {
            FixedDeltaTime = fixedDeltaTime;
        }

        private void UpdateActiveTimers(float dependentGamePlayDeltaTime)
        {
            if (_activeTimers.Count == 0)
                return;

            for (int i = _activeTimers.Count - 1; i >= 0; i--)
            {
                Timer currentTimer = _activeTimers[i];
                currentTimer.Update(dependentGamePlayDeltaTime);

                if (!currentTimer.IsTimerActive)
                {
                    _activeTimers.Remove(currentTimer);
                    ReturnFreeTimer(currentTimer);
                }
            }
        }

        public void SpeedUp(float multiplier)
        {
            _speedUpMultiplier = multiplier;
        }

        public Timer GetFreeTimer()
        {
            return _freeTimers.Count == 0 ? new Timer() : _freeTimers.Dequeue();
        }

        public void ReturnFreeTimer(Timer freeTimer)
        {
            freeTimer.DeActiveTimer();
            _freeTimers.Enqueue(freeTimer);
        }
    }
}