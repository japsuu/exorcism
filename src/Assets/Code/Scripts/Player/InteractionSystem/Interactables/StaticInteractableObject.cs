using System.Collections.Generic;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Inherit to create a static non-physics interactable object.<br/><br/>
    ///
    /// Requires at least one interaction to be defined.
    /// </summary>
    public abstract class StaticInteractableObject : InteractableObject
    {
#region Protected API
        
        /// <summary>
        /// Override to add custom interactions to the object.
        /// </summary>
        /// <param name="supportedInteractions">The list to add interactions to.</param>
        protected override void RegisterInteractions(List<IInteraction> supportedInteractions)
        {
            supportedInteractions.Add(new EventInteraction("Use", UseStart, UseStop, ShouldStop));
        }


        /// <summary>
        /// Override to implement the "Use" interaction type.
        /// Called when the player presses the "interact" key.
        /// </summary>
        protected abstract void UseStart();
        
        
        /// <summary>
        /// Override to implement the "Use" interaction type.
        /// Called when the player releases the "interact" key.
        /// </summary>
        protected abstract void UseStop();
        
        
        protected virtual bool ShouldStop(InteractionDataArgs args)
        {
            if (!args.IsLookingAtObject)
                return true;
            
            return false;
        }

#endregion
    }
}