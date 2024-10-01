using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Destroys the GameObject after a certain amount of time.
    /// </summary>
    public class DestroyAfterSeconds : MonoBehaviour
    {
        [SerializeField] private float _seconds = 1f;


        protected virtual void Awake()
        {
            Destroy(gameObject, _seconds);
        }
    }
}