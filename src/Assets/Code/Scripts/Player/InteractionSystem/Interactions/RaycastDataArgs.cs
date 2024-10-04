using JetBrains.Annotations;
using UnityEngine;

namespace Player.InteractionSystem
{
    public readonly struct RaycastDataArgs
    {
        [CanBeNull]
        public readonly IInteractable TargetedInteractable;
        public readonly Vector3 RaycastSourcePos;
        public readonly Vector3 RaycastSourceDirection;
        public readonly RaycastHit RaycastHit;
        
        /// <summary>
        /// The current distance between the raycast source and the object's center.
        /// </summary>
        public float DistanceTo(GameObject target) => Vector3.Distance(RaycastSourcePos, target.transform.position);
        
        /// <summary>
        /// True, if the player is looking at the specified object and is close enough to initiate an interaction.<br/>
        /// False otherwise.
        /// </summary>
        public bool IsLookingAtInteractable(IInteractable target) => target == TargetedInteractable;


        public RaycastDataArgs([CanBeNull] IInteractable targetedInteractable, Vector3 raycastSourcePos, Vector3 raycastSourceDir, RaycastHit raycastHit)
        {
            TargetedInteractable = targetedInteractable;
            RaycastSourcePos = raycastSourcePos;
            RaycastSourceDirection = raycastSourceDir;
            RaycastHit = raycastHit;
        }
    }
}