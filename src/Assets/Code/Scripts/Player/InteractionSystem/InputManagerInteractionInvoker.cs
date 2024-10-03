using System;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Uses input manager to trigger interactions.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class InputManagerInteractionInvoker : InteractionInvoker
    {
        [Tooltip("Used to raycast forward from.")]
        [SerializeField]
        private Transform _head;
        
        [SerializeField]
        private KeyCode interactKey = KeyCode.F;
        
        [SerializeField]
        private float _interactionDistance = 2f;


        protected override Vector3 RaycastPosition => _head.position;
        protected override Vector3 RaycastDirection => _head.forward;
        protected override float InteractionDistance => _interactionDistance;
        protected override bool IsInteractKeyPressed => Input.GetKeyDown(interactKey);
        protected override bool IsInteractKeyReleased => Input.GetKeyUp(interactKey);
        protected override bool IsInteractionEnabled => true;
        protected override int InteractionIndexDelta => Input.mouseScrollDelta.y < 0 ? 1 : Input.mouseScrollDelta.y > 0 ? -1 : 0;

        protected override void HandleInteractionStart(IInteractable interactable, int index)
        {
            try
            {
                interactable.InteractionStart(index);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while starting interaction with {interactable}: {e}");
            }
        }


        protected override void HandleInteractionStop(IInteractable interactable)
        {
            try
            {
                interactable.InteractionStop();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while stopping interaction with {interactable}: {e}");
            }
        }
    }
}