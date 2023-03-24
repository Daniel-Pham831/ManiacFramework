using UnityEngine;

namespace Maniac.Utils
{
    public static class Locator<T>
    {
        private static T instance;
        public static T Instance { get { return instance; } }
        public static void Set(T ins)
        {
            if (ins == null)
            {
                Debug.LogError($"{typeof(T).Name} is null!");
                return;
            }

            if (instance == null)
            {
                Locator<T>.instance = ins;
            }
            else
            {
                Debug.LogWarning("Instance already set. Please check and remove the SetInstance method.");
            }
        }

        public static void Remove(T ins)
        {
            if (ins == null) return;
            else
            {
                if (!instance.Equals(ins)) return;
                else instance = default(T);
            }
        }
    }
}