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
        protected override bool WasInteractionKeyPressedDownThisFrame => Input.GetKeyDown(interactKey);
        protected override bool WasInteractionKeyLiftedUpThisFrame => Input.GetKeyUp(interactKey);
        protected override bool IsInteractionKeyHeldCurrently => Input.GetKey(interactKey);
        protected override bool ShouldAllowInteraction => true;

        protected override void HandleInteraction(IInteractable interactable)
        {
            interactable.Interact();
        }
    }
}