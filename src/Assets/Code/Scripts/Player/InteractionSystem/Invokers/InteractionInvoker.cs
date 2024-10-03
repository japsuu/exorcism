using System;
using JetBrains.Annotations;
using Tools;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Interaction "Manager".
    /// Triggers interactions and raises events.
    /// </summary>
    public abstract class InteractionInvoker : SingletonBehaviour<InteractionInvoker>   // Singleton to ensure only one instance is active.
    {
        /// <summary>
        /// Called when the player looks at a new object.
        /// </summary>
        public static event Action<LookAtChangedEventArgs> TargetChanged;

        /// <summary>
        /// Called when the currently selected interaction index changes.
        /// </summary>
        public static event Action<int> SelectedInteractionIndexChanged;
        
        [Header("Raycasting")]
        [Tooltip("Used to raycast forward from.")]
        [SerializeField] private Transform _head;
        [SerializeField] private bool _doDrawDebugLine;
        
        [Header("Interaction Settings")]
        [SerializeField] private float _interactionDistance = 2f;
        
        [Header("Grabbing Settings")]
        [SerializeField] private float _grabDistanceMin = 1f;
        [SerializeField] private float _grabDistanceMax = 3f;
        [Range(0f, 1f)]
        [SerializeField] private float _grabDistanceLerpSpeed = 0.5f;
        
        private int _selectedInteractionIndex;
        // Grab distance without lerping.
        private float _targetGrabDistance = 1f;
        private float _grabDistance = 1f;

        private Vector3 RaycastPosition => _head.position;
        private Vector3 RaycastDirection => _head.forward;
        
        protected abstract bool IsInteractKeyPressed { get; }
        protected abstract bool IsInteractKeyReleased { get; }
        protected abstract bool IsInteractionEnabled { get; }
        protected abstract int InteractionIndexDelta { get; }
        protected abstract float GrabDistanceDelta { get; }
        
        
        /// <summary>
        /// The position the player is currently looking at, with the current grab distance applied.
        /// </summary>
        public Vector3 GrabTargetPosition => RaycastPosition + RaycastDirection * _targetGrabDistance;


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
                {
                    UpdateGrabDistance();
                    return;
                }
            }

            // The player is not interacting with an object.
            // Raycast if we are looking at one.
            int count = Physics.RaycastNonAlloc(RaycastPosition, RaycastDirection, _results, _interactionDistance);
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
                {
                    StartInteraction(currentLookAtTarget, _selectedInteractionIndex);
                    float distanceToInteractable = Vector3.Distance(RaycastPosition, hit.transform.position);
                    _targetGrabDistance = Mathf.Clamp(distanceToInteractable, _grabDistanceMin, _grabDistanceMax);
                }
                
                return;
            }

            // No hit, reset the targets.
            ResetTargets();
        }


        private void UpdateGrabDistance()
        {
            _targetGrabDistance += GrabDistanceDelta;
            _targetGrabDistance = Mathf.Clamp(_targetGrabDistance, _grabDistanceMin, _grabDistanceMax);
            
            // Hacky, but it works.
            _grabDistance = Mathf.Lerp(_grabDistance, _targetGrabDistance, _grabDistanceLerpSpeed);
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

            GizmosExtensions.DrawArrow(RaycastPosition, RaycastPosition + RaycastDirection * _interactionDistance);
        }
    }
}