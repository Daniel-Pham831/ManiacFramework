using Maniac.Utils;
using UnityEngine;

namespace Maniac.TimeSystem
{
    public class TimeUpdator:MonoBehaviour
    {
        private TimeManager _timeManager => Locator<TimeManager>.Instance;

        private void Update()
        {
            _timeManager.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _timeManager.FixedUpdate(Time.fixedDeltaTime);
        }
    }
}