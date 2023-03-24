using UnityEngine;
using UnityEngine.Pool;

namespace Maniac.SpawnerSystem
{
    public class Spawner<T> where T :MonoBehaviour
    {
        private readonly MonoBehaviour _prefab;
        private readonly ObjectPool<T> _pool;
        public ObjectPool<T> Pool => _pool;
        private int counter = 0;
        
        public Spawner(T prefab) 
        {
            _prefab = prefab;
            _pool = new ObjectPool<T>(CreateFunction, GetFunction, ReleaseFunction, DestroyFunction, false);
        }

        private T CreateFunction()
        {
            var mono = UnityEngine.Object.Instantiate(_prefab);
            mono.name = _prefab.name + SpawnerManager.Indicator + counter++;
            
            return mono as T;
        }

        private void GetFunction(T mono)
        {
            mono.gameObject.SetActive(true);
        }

        private void ReleaseFunction(T mono)
        {
            mono.gameObject.SetActive(false);
        }

        private void DestroyFunction(T mono)
        {
            if(mono != null && mono.gameObject != null)
                UnityEngine.Object.Destroy(mono.gameObject);
        }
    }
}