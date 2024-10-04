using System.Collections.Generic;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Add to an object to make it interactable with physics.<br/><br/>
    ///
    /// The object can be grabbed and moved around in the world, and can be rotated while holding the right mouse button.<br/>
    /// Doesn't necessarily require any further interactions to be defined.
    /// </summary>
    [RequireComponent(typeof(PhysicsDraggingInteraction))]
    public class DynamicInteractableObject : InteractableObject
    {
        private PhysicsDraggingInteraction _draggingInteraction;


        protected override void RegisterInteractions(List<IInteraction> supportedInteractions)
        {
            _draggingInteraction = GetComponent<PhysicsDraggingInteraction>();
            if (_draggingInteraction == null)
            {
                Debug.LogWarning($"No {nameof(PhysicsDraggingInteraction)} assigned to {nameof(DynamicInteractableObject)}. This object will not be draggable.", gameObject);
                return;
            }
            
            supportedInteractions.Add(_draggingInteraction);
        }
    }
}