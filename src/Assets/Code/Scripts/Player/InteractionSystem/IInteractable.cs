namespace Player.InteractionSystem
{
    /// <summary>
    /// Inherit to make the object interactable.
    /// Requires some collider to be detected by <see cref="InteractionInvoker"/>
    /// </summary>
    public interface IInteractable
    {
        public float HoldInteractionLengthSeconds { get; }


        /// <summary>
        /// Can this object be interacted with right now by the defined interactor?
        /// </summary>
        /// <returns></returns>
        public bool CanBeInteractedWith();


        /// <summary>
        /// Returns the text that will be shown on the UI when this object can be interacted with.
        /// </summary>
        /// <returns></returns>
        public string GetInteractDescription();
    
        
        /// <summary>
        /// Called when an interaction is requested.
        /// </summary>
        public void Interact();
    }
}