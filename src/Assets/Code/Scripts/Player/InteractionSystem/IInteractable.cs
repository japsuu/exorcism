using JetBrains.Annotations;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Inherit to make the object interactable.<br/><br/>
    /// 
    /// When an interactable object is looked at, a menu is shown to select the interaction type.<br/>
    /// Whatever option is selected is how you interact with this item when you press the "interact" key.
    ///
    /// <remarks>
    /// Requires a collider to be detected by <see cref="InteractionInvoker"/>.
    /// </remarks>
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Returns the name of the object.
        /// Displayed in the interaction menu.
        ///
        /// <example>
        /// "Briefcase"
        /// </example>
        /// </summary>
        [CanBeNull]
        public string GetName();


        /// <summary>
        /// Returns an optional description for the object.
        /// Displayed in the interaction menu.
        ///
        /// <example>
        /// "Use to view contents."
        /// </example>
        /// </summary>
        [CanBeNull]
        public string GetDescription();


        /// <summary>
        /// Returns the bounds of the object in world space.
        /// </summary>
        public Bounds GetWorldBounds();

        
        /// <summary>
        /// Returns the human-readable names for the supported interactions for this object.
        /// Each entry will be shown as a selectable entry in the interaction menu.
        /// </summary>
        public string[] GetSupportedInteractionNames();
        
        
        /// <summary>
        /// Called when the interaction key is pressed.
        /// For every <see cref="InteractionStart"/> call there will be a matching <see cref="InteractionStop"/> call.
        /// </summary>
        /// <param name="index">The index of interaction requested (see <see cref="GetSupportedInteractionNames"/>).</param>
        public void InteractionStart(int index);
        
        
        /// <summary>
        /// Called when the interaction key is released.
        /// </summary>
        public void InteractionStop();
    }
}