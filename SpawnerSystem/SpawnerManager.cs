using System;
using System.Collections.Generic;
using Maniac.TimeSystem;
using Maniac.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Maniac.SpawnerSystem
{
    public class SpawnerManager
    {
        public static readonly string Indicator = "~:||";

        private TimeManager _timeManager => Locator<TimeManager>.Instance;

        private Dictionary<string, Spawner<MonoBehaviour>> _spawners;
        private Dictionary<string, Timer> _timerForReleaseAfterMonos = new Dictionary<string, Timer>();

        public void Initialize()
        {
            _spawners = new Dictionary<string, Spawner<MonoBehaviour>>();
        }

        public void ResetAllSpawner()
        {
            foreach (var spawner in _spawners.Values)
            {
                spawner.Pool.Clear();
            }
        }

        public T Get<T>(T prefab) where T : MonoBehaviour
        {
            if (prefab == null)
                return null;

            return GetHelper(prefab) as T;
        }

        private MonoBehaviour GetHelper(MonoBehaviour prefab)
        {
            if (prefab == null)
                return null;
            
            var key = prefab.gameObject.name;
            if (!_spawners.ContainsKey(key))
            {
                var newSpawner = new Spawner<MonoBehaviour>(prefab);
                _spawners.Add(key,newSpawner);
            }

            return _spawners[key].Pool.Get();
        }

        public void Release(MonoBehaviour monoToRelease)
        {
            if (monoToRelease == null) return;
            CheckReleaseAfter(monoToRelease.gameObject.name); // Make sure to clear release after on this mono
            
            var key = monoToRelease.gameObject.name.Substring(0,monoToRelease.gameObject.name.IndexOf(SpawnerManager.Indicator, StringComparison.Ordinal));

            if (!_spawners.ContainsKey(key))
            {
                Debug.LogError($"There is no {key} spawner. Please investigate!");
                Object.Destroy(monoToRelease);
                return;
            }

            _spawners[key].Pool.Release(monoToRelease);
        }

        public void ReleaseAfter(MonoBehaviour monoToRelease, float duration)
        {
            var key = monoToRelease.gameObject.name;
            var timer = _timeManager.GetFreeTimer();
            _timerForReleaseAfterMonos.Add(key,timer);
            
            timer.Start(duration, () =>
            {
                CheckReleaseAfter(key);
                
                Release(monoToRelease);
            });
        }
        
        private void CheckReleaseAfter(string key)
        {
            if (!_timerForReleaseAfterMonos.ContainsKey(key)) return;
            
            _timerForReleaseAfterMonos.Remove(key,out var freeTimer);
            _timeManager.ReturnFreeTimer(freeTimer);
        }
    }
}