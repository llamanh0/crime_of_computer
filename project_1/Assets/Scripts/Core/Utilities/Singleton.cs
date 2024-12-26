// Assets/Scripts/Core/Utilities/Singleton.cs
using UnityEngine;

namespace MyGame.Core.Utilities
{
    /// <summary>
    /// Generic Singleton sınıfı, sadece bir örneğin var olmasını sağlar.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(_lock)
                    {
                        _instance = FindFirstObjectByType<T>();
                        if (FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
                        {
                            Debug.LogError($"Singleton: {typeof(T)} sınıfından birden fazla örnek bulundu!");
                            return _instance;
                        }
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject(typeof(T).Name);
                            _instance = singleton.AddComponent<T>();
                            DontDestroyOnLoad(singleton);
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
