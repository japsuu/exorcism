using System;
using JetBrains.Annotations;
using Tools;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

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

        /// <summary>
        /// Called when the player looks at a new object.
        /// </summary>
        public static event Action<LookAtChangedEventArgs> TargetChanged;

        /// <summary>
        /// Called when the currently selected interaction index changes.
        /// </summary>
        public static event Action<int> SelectedInteractionIndexChanged;

        protected abstract Vector3 RaycastPosition { get; }
        protected abstract Vector3 RaycastDirection { get; }
        protected abstract float InteractionDistance { get; }
        protected abstract bool IsInteractKeyPressed { get; }
        protected abstract bool IsInteractKeyReleased { get; }
        protected abstract bool IsInteractionEnabled { get; }
        protected abstract int InteractionIndexDelta { get; }
        
        
        [SerializeField]
        private bool _doDrawDebugLine;
        private int _selectedInteractionIndex;


        /// <summary>
        /// Called when the player starts interacting with an object.
        /// </summary>
        /// <param name="interactable">The interactable object.</param>
        /// <param name="index">The index of the interaction type.</param>
        protected abstract void HandleInteractionStart(IInteractable interactable, int index);


        /// <summary>
        /// Called when the player stops interacting with an object.
        /// </summary>
        /// <param name="interactable">The interactable object.</param>
        protected abstract void HandleInteractionStop(IInteractable interactable);

        
        [CanBeNull] private IInteractable _previousLookAtTarget;    // The object that the player was looking at last frame.
        [CanBeNull] private IInteractable _interactionTarget;       // The object that the player is currently interacting with.
        private readonly RaycastHit[] _results = new RaycastHit[16];


        protected virtual void Update()
        {
            // Check if interaction has been disabled.
            // This could happen at any time.
            if (!IsInteractionEnabled)
            {
                ResetTargets();
                return;
            }

            // Check if the player is currently interacting with an object.
            // If so, do not allow any other interactions.
            if (_interactionTarget != null)
            {
                if (IsInteractKeyReleased)
                    StopInteraction();
                else
                    return;
            }

            // The player is not interacting with an object.
            // Raycast if we are looking at one.
            int count = Physics.RaycastNonAlloc(RaycastPosition, RaycastDirection, _results, InteractionDistance);
            for (int i = 0; i < count; i++)
            {
                GameObject hit = _results[i].transform.gameObject;

                if (!hit.TryGetComponent(out IInteractable currentLookAtTarget))
                    continue;
                
                // Looking at an interactable object.
                // Check if it's different from the last frame.
                if (currentLookAtTarget != _previousLookAtTarget)
                    ChangeLookAtTarget(currentLookAtTarget);
            
                UpdateInteractionIndex(currentLookAtTarget);

                if (IsInteractKeyPressed)
                    StartInteraction(currentLookAtTarget, _selectedInteractionIndex);
                
                return;
            }

            // No hit, reset the targets.
            ResetTargets();
        }


        private void UpdateInteractionIndex(IInteractable currentLookAtTarget)
        {
            if (InteractionIndexDelta == 0)
                return;
            
            int optionCount = currentLookAtTarget.GetSupportedInteractionNames().Length;
            if (optionCount == 0)
                return;
            
            int newIndex = _selectedInteractionIndex + InteractionIndexDelta;
            if (newIndex < 0)
                newIndex = optionCount - 1;
            else if (newIndex >= optionCount)
                newIndex = 0;
            
            ChangeSelectedInteractionIndex(newIndex);
        }


        private void ChangeSelectedInteractionIndex(int newIndex)
        {
            _selectedInteractionIndex = newIndex;
            SelectedInteractionIndexChanged?.Invoke(newIndex);
        }


        private void StartInteraction(IInteractable interactable, int index)
        {
            Debug.Assert(_interactionTarget == null, nameof(_interactionTarget) + " == null");
            Debug.Assert(interactable != null, nameof(interactable) + " != null");
            _interactionTarget = interactable;
            HandleInteractionStart(_interactionTarget, index);
        }


        private void StopInteraction()
        {
            Debug.Assert(_interactionTarget != null, nameof(_interactionTarget) + " != null");
            HandleInteractionStop(_interactionTarget);
            _interactionTarget = null;
        }


        private void ResetTargets()
        {
            if (_previousLookAtTarget != null)
                ChangeLookAtTarget(null);
            
            if (_interactionTarget != null)
                StopInteraction();
        }


        private void ChangeLookAtTarget([CanBeNull] IInteractable currentlyTargetedInteractable)
        {
            TargetChanged?.Invoke(new LookAtChangedEventArgs(_previousLookAtTarget, currentlyTargetedInteractable));
            
            _previousLookAtTarget = currentlyTargetedInteractable;
            ChangeSelectedInteractionIndex(0);
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