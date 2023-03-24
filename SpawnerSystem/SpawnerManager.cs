using System.Collections.Generic;
using UnityEngine;

namespace Maniac.SpawnerSystem
{
    public class SpawnerManager
    {
        private Dictionary<string, Spawner<MonoBehaviour>> _spawners;

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
        
        public MonoBehaviour Get(MonoBehaviour prefab)
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
    }
}