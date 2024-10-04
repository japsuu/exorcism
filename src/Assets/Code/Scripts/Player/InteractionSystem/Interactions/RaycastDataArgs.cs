using JetBrains.Annotations;
using UnityEngine;

namespace Player.InteractionSystem
{
    public readonly struct RaycastDataArgs
    {
        [CanBeNull]
        private readonly IInteractable _targetedInteractable;
        private readonly Vector3 _raycastSourcePos;
        private readonly RaycastHit _hit;
        
        /// <summary>
        /// The current distance between the raycast source and the interactable object's center.
        /// </summary>
        public float DistanceToCenter(IInteractable target) => Vector3.Distance(_raycastSourcePos, target.GameObject.transform.position);
        
        /// <summary>
        /// True, if the player is looking at the specified object and is close enough to initiate an interaction.<br/>
        /// False otherwise.
        /// </summary>
        public bool IsLookingAtInteractable(IInteractable target) => target == _targetedInteractable;


        public RaycastDataArgs([CanBeNull] IInteractable targetedInteractable, Vector3 raycastSourcePos, RaycastHit hit)
        {
            _targetedInteractable = targetedInteractable;
            _raycastSourcePos = raycastSourcePos;
            _hit = hit;
        }
    }
}