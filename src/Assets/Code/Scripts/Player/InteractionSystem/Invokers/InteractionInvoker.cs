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
        [SerializeField] private bool _doDebugDraw = true;

        [Tooltip("The layers that interactable objects should be on to be interactable.")]
        [SerializeField]
        private LayerMask _interactableLayers;
        
        [Header("Interaction Settings")]
        [SerializeField] private float _interactionDistance = 2f;
        
        [Header("Grabbing Settings")]
        [SerializeField] private float _grabDistanceMin = 1f;
        [SerializeField] private float _grabDistanceMax = 2.5f;
        [Range(0f, 1f)]
        [SerializeField] private float _grabDistanceLerpSpeed = 0.2f;
        [SerializeField] private float _grabStrength = 1200;
        [SerializeField] private float _grabMaxForce = 500;
        [SerializeField] private float _throwForce = 50;
        
        private const int MAX_RAYCAST_HITS = 16;
        private readonly RaycastHit[] _results = new RaycastHit[MAX_RAYCAST_HITS];
        private int _selectedInteractionIndex;
        private float _targetGrabDistance = 1f; // Grab distance without lerping.
        private float _grabDistance = 1f;       // Current grab distance.
        [CanBeNull] private IInteractable _previousLookAtTarget;    // The object that the player was looking at last frame.
        [CanBeNull] private IInteraction _currentInteraction;       // The currently performed interaction.
        
        protected abstract bool IsInteractKeyPressed { get; }
        protected abstract bool IsInteractKeyReleased { get; }
        protected abstract bool IsInteractionEnabled { get; }
        protected abstract int InteractionIndexDelta { get; }
        protected abstract float GrabDistanceDelta { get; }
        
        public Vector3 RaycastPosition => _head.position;
        public Vector3 RaycastDirection => _head.forward;
        /// <summary>
        /// The position the player is currently looking at, with the current grab distance applied.
        /// </summary>
        public Vector3 GrabTargetPosition => RaycastPosition + RaycastDirection * _grabDistance;
        public LayerMask InteractableLayers => _interactableLayers;
        public float GrabStrength => _grabStrength;
        public float GrabMaxForce => _grabMaxForce;
        public float ThrowForce => _throwForce;


        /// <summary>
        /// Called when the player starts interacting with an object.
        /// </summary>
        /// <param name="interaction">The interaction to start.</param>
        protected abstract void HandleInteractionStart(IInteraction interaction);


        /// <summary>
        /// Called when the player stops interacting with an object.
        /// </summary>
        /// <param name="interaction">The interaction to stop.</param>
        protected abstract void HandleInteractionStop(IInteraction interaction);


        protected virtual void Update()
        {
            // Check if interaction has been disabled.
            // This could happen at any time.
            if (!IsInteractionEnabled)
            {
                ResetTargets();
                return;
            }

            // Raycast if we are looking at an interactable object.
            IInteractable targetedInteractable = TryGetTargetedInteractable(out RaycastHit hit);

            // Check if the player is currently interacting with an object.
            // If so, do not allow any other interactions.
            if (_currentInteraction != null)
            {
                _currentInteraction.OnUpdateInteraction();
                UpdateGrabDistance();
                
                RaycastDataArgs args = new(targetedInteractable, RaycastPosition, RaycastDirection, hit);
                if (IsInteractKeyReleased || _currentInteraction.ShouldStop(args))
                    StopInteraction();
                else
                    return;
            }
            
            // The player is not interacting with an object.
            if (targetedInteractable != null)
            {
                // Looking at an interactable object.
                // Check if it's different from the last frame.
                if (targetedInteractable != _previousLookAtTarget)
                    ChangeLookAtTarget(targetedInteractable);
            
                UpdateInteractionIndex(targetedInteractable);

                if (IsInteractKeyPressed)
                {
                    StartInteraction(targetedInteractable, _selectedInteractionIndex);
                    float distanceToInteractable = Vector3.Distance(RaycastPosition, hit.transform.position);
                    _targetGrabDistance = Mathf.Clamp(distanceToInteractable, _grabDistanceMin, _grabDistanceMax);
                    _grabDistance = _targetGrabDistance;
                }
                
                return;
            }

            // No hit, reset the targets.
            ResetTargets();
        }


        private IInteractable TryGetTargetedInteractable(out RaycastHit hit)
        {
            int count = Physics.RaycastNonAlloc(RaycastPosition, RaycastDirection, _results, _interactionDistance, _interactableLayers);
            
            if (count == MAX_RAYCAST_HITS)
                Debug.LogWarning("Max raycast hits reached. Some objects may not be interactable.");
            
            // Small alloc, because the _results array is not filled up to MAX_RAYCAST_HITS.
            // We cannot sort it directly since it might contain old data. 
            RaycastHit[] sortedResults = new RaycastHit[count];
            Array.Copy(_results, sortedResults, count);
            
            // Sort the results by distance.
            Array.Sort(sortedResults, (a, b) => a.distance.CompareTo(b.distance));
            
            for (int i = 0; i < count; i++)
            {
                hit = sortedResults[i];
                GameObject go = hit.transform.gameObject;
                if (!go.TryGetComponent(out IInteractable currentLookAtTarget))
                    continue;
                
                return currentLookAtTarget;
            }

            hit = default;
            return null;
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
            
            int optionCount = currentLookAtTarget.GetSupportedInteractions().Length;
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
            Debug.Assert(_currentInteraction == null, nameof(_currentInteraction) + " == null");
            Debug.Assert(interactable != null, nameof(interactable) + " != null");
            _currentInteraction = interactable.GetSupportedInteractions()[index];
            HandleInteractionStart(_currentInteraction);
        }


        private void StopInteraction()
        {
            Debug.Assert(_currentInteraction != null, nameof(_currentInteraction) + " != null");
            HandleInteractionStop(_currentInteraction);
            _currentInteraction = null;
        }


        private void ResetTargets()
        {
            if (_previousLookAtTarget != null)
                ChangeLookAtTarget(null);
            
            if (_currentInteraction != null)
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
            if (!_doDebugDraw)
                return;

            Gizmos.color = Color.green;
            GizmosExtensions.DrawArrow(RaycastPosition, RaycastPosition + RaycastDirection * _interactionDistance);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GrabTargetPosition, 0.1f);
        }
    }
}