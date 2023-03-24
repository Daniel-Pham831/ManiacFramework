using System.Collections.Generic;

namespace Maniac.TimeSystem
{
    public class TimerList
    {
        private List<Timer> timers;

        public TimerList()
        {
            timers = new List<Timer>();
        }

        public void Add(Timer timer)
        {
            timers.Add(timer);
        }

        // Update timers and remove completed timers
        public void Update(float deltaTime)
        {
            if (timers.Count == 0)
                return;

            for (int i = timers.Count - 1; i >= 0; i--)
            {
                Timer currentTimer = timers[i];
                currentTimer.Update(deltaTime);

                if (!currentTimer.IsTimerActive)
                {
                    timers.Remove(currentTimer);
                }
            }
        }
    }
}