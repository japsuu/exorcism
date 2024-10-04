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
        public GameObject GameObject { get; }
        
        
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
        /// Returns all the available interactions for this object.
        /// Each entry will be shown as a selectable entry in the interaction menu.
        /// </summary>
        public IInteraction[] GetSupportedInteractions();
    }
}