using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Destroys the GameObject on load.
    /// </summary>
    public class DestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}