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
        [Header("Input Settings")]
        [SerializeField]
        private KeyCode interactKey = KeyCode.F;
        
        [SerializeField]
        private float _grabDistanceSpeed = 0.5f;


        protected override bool IsInteractKeyPressed => Input.GetKeyDown(interactKey);
        protected override bool IsInteractKeyReleased => Input.GetKeyUp(interactKey);
        protected override bool IsInteractionEnabled => true;
        protected override int InteractionIndexDelta => Input.mouseScrollDelta.y < 0 ? 1 : Input.mouseScrollDelta.y > 0 ? -1 : 0;
        protected override float GrabDistanceDelta => Input.mouseScrollDelta.y * _grabDistanceSpeed;


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