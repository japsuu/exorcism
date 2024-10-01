using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public event Action<Inputs> OnInputsUpdated; 

        private CharacterInputController _inputController;

        private void Awake()
        {
            _inputController = gameObject.AddComponent<CharacterInputController>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Update inputs for all subscribed scripts.
            Inputs inputs = _inputController.GetInput();
            
            OnInputsUpdated?.Invoke(inputs);
        }
    }
}