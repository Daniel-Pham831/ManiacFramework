using UnityEngine;
using UnityEngine.Pool;

namespace Maniac.SpawnerSystem
{
    public class Spawner<T> where T :Object
    {
        private readonly Object _prefab;
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

        private void GetFunction(T obj)
        {
            switch (obj)
            {
                case GameObject gobj:
                    gobj.SetActive(true);
                    break;
                
                case MonoBehaviour mono:
                    mono.gameObject.SetActive(true);
                    break;
            }
        }

        private void ReleaseFunction(T obj)
        {
            switch (obj)
            {
                case GameObject gobj:
                    gobj.SetActive(false);
                    break;
                
                case MonoBehaviour mono:
                    mono.gameObject.SetActive(false);
                    break;
            }
        }

        private void DestroyFunction(T obj)
        {
            if (obj == null) return;
            
            Object.Destroy(obj);
        }

        public void Reset()
        {
            counter = 0;
            _pool.Clear();
        }
    }
}