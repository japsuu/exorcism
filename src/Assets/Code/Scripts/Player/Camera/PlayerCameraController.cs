using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraYaw;

        [SerializeField]
        private Transform cameraPitch;

        [Header("Input settings")]
        [Range(0.1f, 9f)]
        [SerializeField]
        private float rotationSensitivity = 2f;

        [Range(0f, 90f)]
        [SerializeField]
        private float cameraVerticalLimit = 88f;

        //WARN: Increments over 360 degrees. Might cause floating point issues at some point?
        private float _targetHorizontalAxis;
        private float _targetVerticalAxis;
        private float _additiveVerticalAxis;
        private PlayerController _playerController;


        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }


        public void ResetCameraRotation()
        {
            _targetHorizontalAxis = 0f;
            _targetVerticalAxis = 0f;
            _additiveVerticalAxis = 0f;
        }


        private void OnEnable()
        {
            _playerController.OnInputsUpdated += SetInputs;
        }


        private void OnDisable()
        {
            _playerController.OnInputsUpdated -= SetInputs;
        }


        private void SetInputs(Inputs input)
        {
            _targetHorizontalAxis += input.MouseInput.x * rotationSensitivity;
            _targetVerticalAxis += -input.MouseInput.y * rotationSensitivity;

            _targetVerticalAxis = Mathf.Clamp(_targetVerticalAxis, -cameraVerticalLimit, cameraVerticalLimit);
        }


        /// <summary>
        /// Adds the defined value to vertical axis.
        /// </summary>
        public void InterceptVerticalRotation(float additiveRot)
        {
            _targetVerticalAxis += additiveRot;
        }


        private void LateUpdate()
        {
            float finalVerticalAxis = _targetVerticalAxis + _additiveVerticalAxis;

            float clampedVertical = Mathf.Clamp(finalVerticalAxis, -cameraVerticalLimit, cameraVerticalLimit);

            cameraYaw.localRotation = Quaternion.AngleAxis(_targetHorizontalAxis, Vector3.up);
            cameraPitch.localRotation = Quaternion.AngleAxis(clampedVertical, Vector3.right);
        }
    }
}