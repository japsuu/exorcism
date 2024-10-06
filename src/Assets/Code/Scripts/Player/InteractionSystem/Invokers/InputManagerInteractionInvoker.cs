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
        private float _grabDistanceSpeed = 0.2f;


        protected override bool IsInteractKeyPressed => Input.GetKeyDown(interactKey);
        protected override bool IsInteractKeyReleased => Input.GetKeyUp(interactKey);
        protected override bool IsInteractionEnabled => true;
        protected override int InteractionIndexDelta => Input.mouseScrollDelta.y < 0 ? 1 : Input.mouseScrollDelta.y > 0 ? -1 : 0;
        protected override float GrabDistanceDelta => Input.mouseScrollDelta.y * _grabDistanceSpeed;


        protected override void HandleInteractionStart(IInteraction interaction)
        {
            try
            {
                interaction.OnStartInteraction();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while starting interaction '{interaction.GetName()}': {e}");
            }
        }


        protected override void HandleInteractionStop(IInteraction interaction)
        {
            try
            {
                interaction.OnStopInteraction();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while stopping interaction '{interaction.GetName()}': {e}");
            }
        }
    }
}