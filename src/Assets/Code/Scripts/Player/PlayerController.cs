using System;
using Tools;
using UnityEngine;

namespace Player
{
    public class PlayerController : SingletonBehaviour<PlayerController>
    {
        public event Action<InputData> OnInputsUpdated;

        [SerializeField]
        [ReadOnly]
        private InputData _inputData;
        private PlayerInputController _inputController;


        private void Awake()
        {
            _inputController = gameObject.AddComponent<PlayerInputController>();
        }


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }


        private void Update()
        {
            // Update inputs for all subscribed scripts.
            _inputData = _inputController.GetInput();

            OnInputsUpdated?.Invoke(_inputData);
        }
    }
}