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
    public class DynamicInteractableObject : InteractableObject
    {
        private bool _isDragging;


        public void GrabStart()
        {
            _isDragging = true;
        }


        public void GrabStop()
        {
            _isDragging = false;
        }


        protected override void RegisterInteractions(List<IInteraction> supportedInteractions)
        {
            supportedInteractions.Add(new ActionInteraction("Grab", GrabStart, GrabStop));
        }


        protected override void Update()
        {
            base.Update();
            
            if (_isDragging)
            {
                transform.position = InteractionInvoker.Instance.GrabTargetPosition;
                
                if (Input.GetMouseButton(1))
                {
                    Vector3 eulerAngles = transform.eulerAngles;
                    eulerAngles.x += Input.GetAxis("Mouse Y") * 2;
                    eulerAngles.y += Input.GetAxis("Mouse X") * 2;
                    transform.eulerAngles = eulerAngles;
                }
            }
        }
    }
}