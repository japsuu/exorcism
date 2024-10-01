using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// More fleshed out version of <see cref="IInteractable"/>.
    /// </summary>
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        [Tooltip("The collider that will be used to detect interactions.")]
        [SerializeField]
        private Collider _interactTriggerCollider;
        
        [Tooltip("For how long has the interaction key be pressed, to trigger interaction?")]
        [SerializeField] protected float _holdInteractionLengthSeconds = 1f;
        
        public float HoldInteractionLengthSeconds => _holdInteractionLengthSeconds;


        protected virtual void Update() { }


        public virtual bool CanBeInteractedWith() => true;


        public abstract string GetInteractDescription();
        public abstract void Interact();

        public Bounds GetWorldBounds() => _interactTriggerCollider.bounds;
    }
}