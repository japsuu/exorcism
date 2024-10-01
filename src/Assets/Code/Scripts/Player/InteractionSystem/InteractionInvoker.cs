using System;
using JetBrains.Annotations;
using Tools;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Interaction "Manager".
    /// Triggers interactions and tells the UI to update.
    /// </summary>
    public abstract class InteractionInvoker : MonoBehaviour
    {
        public readonly struct LookAtChangedEventArgs
        {
            [CanBeNull]
            public readonly IInteractable OldLookAt;

            [CanBeNull]
            public readonly IInteractable NewLookAt;


            public LookAtChangedEventArgs(IInteractable oldLookAt, IInteractable newLookAt)
            {
                OldLookAt = oldLookAt;
                NewLookAt = newLookAt;
            }
        }

        public readonly struct InteractingEventArgs
        {
            public readonly IInteractable Interactable;


            public InteractingEventArgs(IInteractable interactable)
            {
                Interactable = interactable;
            }
        }

        /// <summary>
        /// Called when the player looks at a new object.
        /// </summary>
        public static event Action<LookAtChangedEventArgs> TargetChanged;
        
        /// <summary>
        /// Called after the player has interacted with an object.
        /// </summary>
        public static event Action<InteractingEventArgs> Interacted;
        
        /// <summary>
        /// The progress of the interaction.
        /// Range: 0-1.
        /// </summary>
        public static float InteractionProgress;

        [SerializeField]
        private bool _doDrawDebugLine;

        protected abstract Vector3 RaycastPosition { get; }
        protected abstract Vector3 RaycastDirection { get; }
        protected abstract float InteractionDistance { get; }
        protected abstract bool WasInteractionKeyPressedDownThisFrame { get; }
        protected abstract bool WasInteractionKeyLiftedUpThisFrame { get; }
        protected abstract bool IsInteractionKeyHeldCurrently { get; }
        protected abstract bool ShouldAllowInteraction { get; }


        /// <summary>
        /// Called when the player interacts with an object.
        /// </summary>
        /// <param name="interactable">The interactable object.</param>
        protected abstract void HandleInteraction(IInteractable interactable);

        
        private IInteractable _lastFrameTargetedInteractable;
        private float _interactionKeyHeldForSeconds;
        private bool _waitForInteractKeyRelease;
        private readonly RaycastHit[] _results = new RaycastHit[16];


        protected virtual void Update()
        {
            if (!ShouldAllowInteraction)
            {
                ResetInteractionTarget();
                return;
            }

            CheckInteractionKeyRelease();

            // Raycast if hit an object.
            int count = Physics.RaycastNonAlloc(RaycastPosition, RaycastDirection, _results, InteractionDistance);

            for (int i = 0; i < count; i++)
            {
                GameObject hit = _results[i].transform.gameObject;

                if (!hit.TryGetComponent(out IInteractable currentlyTargetedInteractable))
                    continue;
                
                HandleTargetedInteractable(currentlyTargetedInteractable);
                return;
            }

            // No hit, reset the targeted object.
            ResetInteractionTarget();
        }


        private void HandleTargetedInteractable(IInteractable currentlyTargetedInteractable)
        {
            if (IsInteractionKeyHeldCurrently)
                _interactionKeyHeldForSeconds += Time.deltaTime;
            InteractionProgress = 0;
            
            // Handle changed target.
            if (currentlyTargetedInteractable != _lastFrameTargetedInteractable)
                HandleChangedTarget(currentlyTargetedInteractable);

            // If we need to wait for the interaction key to be lifted, return.
            // This can happen if we just interacted with some other object, and a new one is detected before the key is lifted.
            // Without this, we would instantly interact with the new object.
            if (_waitForInteractKeyRelease)
                return;

            // Return if the object doesn't currently accept interactions.
            if (!currentlyTargetedInteractable.CanBeInteractedWith())
                return;

            // Check for instant interaction.
            if (TryInteractInstant(currentlyTargetedInteractable))
                return;

            // Interact with interaction time.
            TryInteractDelayed(currentlyTargetedInteractable);
        }


        private void HandleChangedTarget(IInteractable currentlyTargetedInteractable)
        {
            TargetChanged?.Invoke(new LookAtChangedEventArgs(_lastFrameTargetedInteractable, currentlyTargetedInteractable));
                
            // Disallow all interactions until the interaction key is lifted, to avoid instant interaction. See below.
            if (IsInteractionKeyHeldCurrently)
                _waitForInteractKeyRelease = true;

            _interactionKeyHeldForSeconds = 0;
            _lastFrameTargetedInteractable = currentlyTargetedInteractable;
        }


        private bool TryInteractInstant(IInteractable currentlyTargetedInteractable)
        {
            if (currentlyTargetedInteractable.HoldInteractionLengthSeconds != 0 || !WasInteractionKeyPressedDownThisFrame)
                return false;
            
            InvokeInteraction(currentlyTargetedInteractable);
            
            return true;
        }


        private void TryInteractDelayed(IInteractable currentlyTargetedInteractable)
        {
            if (!IsInteractionKeyHeldCurrently)
            {
                _interactionKeyHeldForSeconds = 0;
                return;
            }
            
            if (_interactionKeyHeldForSeconds >= currentlyTargetedInteractable.HoldInteractionLengthSeconds)
                InvokeInteraction(currentlyTargetedInteractable);

            float interactionProgress = _interactionKeyHeldForSeconds / currentlyTargetedInteractable.HoldInteractionLengthSeconds;
            InteractionProgress = Mathf.Min(1f, interactionProgress);
        }


        private void InvokeInteraction(IInteractable interactable)
        {
            HandleInteraction(interactable);
            Interacted?.Invoke(new InteractingEventArgs(interactable));

            // Disallow further interactions until the interaction key is lifted.
            _waitForInteractKeyRelease = true;
            
            // Reset interaction time.
            _interactionKeyHeldForSeconds = 0;
        }


        protected virtual void ResetInteractionTarget()
        {
            _interactionKeyHeldForSeconds = 0;

            CheckInteractionKeyRelease();

            if (_lastFrameTargetedInteractable == null)
                return;

            // Notify UI that we stopped looking at an object.
            TargetChanged?.Invoke(new LookAtChangedEventArgs(_lastFrameTargetedInteractable, null));

            _lastFrameTargetedInteractable = null;
        }


        private void CheckInteractionKeyRelease()
        {
            // If we do not allow interacting multiple times with a single "hold" of key,
            // reset interaction allowance when the interaction key is lifted.
            if (!_waitForInteractKeyRelease)
                return;

            if (WasInteractionKeyLiftedUpThisFrame)
                _waitForInteractKeyRelease = false;
        }


        private void OnDrawGizmosSelected()
        {
            if (!_doDrawDebugLine)
                return;

            Gizmos.color = Color.green;

            GizmosExtensions.DrawArrow(RaycastPosition, RaycastPosition + RaycastDirection * InteractionDistance);
        }
    }
}