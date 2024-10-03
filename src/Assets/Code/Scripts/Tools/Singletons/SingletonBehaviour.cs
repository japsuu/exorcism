using UnityEngine;

// ReSharper disable StaticMemberInGenericType
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Tools
{
    /// <summary>
    /// Singleton object, that can be used from anywhere by calling ClassNameHere.Singleton.
    /// Only one singleton of some type can exist at once.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Check to see if we're about to be destroyed.
        private static bool shuttingDown;
        private static object lockObj = new();
        private static T instance;

        public static T Instance
        {
            get
            {
                if (!Application.isPlaying) return null;
            
                if (shuttingDown)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType(typeof(T));
                    }

                    return instance;
                }
            }
        }
        
        protected virtual void OnEnable()
        {
            shuttingDown = false;
        }


        protected virtual void OnApplicationQuit()
        {
            shuttingDown = true;
        }


        protected virtual void OnDestroy()
        {
            shuttingDown = true;
        }
    }
}