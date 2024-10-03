using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// More fleshed out version of <see cref="IInteractable"/>.<br/><br/>
    ///
    /// Implement this class if you need to define custom interactions for an object.
    /// If no custom interactions are necessary, use <see cref="WorldObject"/>.
    /// </summary>
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        [Tooltip("The collider that will be used to detect interactions.")]
        [SerializeField]
        private Collider _interactTriggerCollider;

        public Bounds GetWorldBounds() => _interactTriggerCollider.bounds;
        
        public abstract string GetName();
        public abstract string GetDescription();
        public abstract string[] GetSupportedInteractionNames();

        public abstract void InteractionStart(int index);
        public abstract void InteractionStop();
    }
}