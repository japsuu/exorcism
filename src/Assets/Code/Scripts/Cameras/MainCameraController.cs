using Tools;
using UnityEngine;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class MainCameraController : SingletonBehaviour<MainCameraController>
    {
        public Camera Camera { get; private set; }


        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }
    }
}