using System.Collections.Generic;
using JetBrains.Annotations;
using Tools;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// </summary>
    public abstract class WorldObject : InteractableBehaviour
    {
        [Tooltip("Whether you can position the object in the world.\n" +
                 "Hold the right mouse button while grabbing to rotate the object around the X and Y axis.")]
        [Header("Interaction Settings")]
        [SerializeField] private bool _canBeGrabbed;

        [Tooltip("Whether the item can be placed in your hand.")]
        [SerializeField] private bool _canBeHeld;

        [Tooltip("Whether the item can be placed in your inventory.")]
        [SerializeField] private bool _canBeCollected;
        
        
        [Header("Object Info")]
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        
        [CanBeNull]
        private IInteraction _currentInteraction;
        private IInteraction[] _supportedInteractions;
        [SerializeField, ReadOnly]
        private string[] _supportedInteractionNamesCache;
        
        /// <summary>
        /// Whether the object supports the "Use" interaction type.<br/>
        /// If true, the <see cref="UseStart"/> and <see cref="UseStop"/> methods should be overridden.
        /// </summary>
        protected abstract bool SupportsUseInteraction { get; }


#region Public API

        /// <summary>
        /// Override to implement a dynamic name.
        /// </summary>
        /// <returns>The name of the object, shown in the interaction menu.</returns>
        public override string GetName() => _name;
        
        
        /// <summary>
        /// Override to implement a dynamic description.
        /// </summary>
        /// <returns>The description of the object, shown in the interaction menu.</returns>
        public override string GetDescription() => _description;


        public void GrabStart() => Debug.LogError("Grabbing not implemented.");
        public void GrabStop() => Debug.LogError("Grabbing not implemented.");
        public void Hold() => Debug.LogError("Holding not implemented.");
        public void Collect() => Debug.LogError("Collecting/Inventory not implemented.");


        public sealed override void InteractionStart(int index)
        {
            if (_supportedInteractions == null)
            {
                Debug.LogWarning("FindSupportedInteractions has not been called. Did you accidentally override Awake() without calling base.Awake()?");
                return;
            }
            if (_supportedInteractions.Length == 0)
                return;
            
            if (index < 0 || index >= _supportedInteractions.Length)
            {
                Debug.LogWarning($"Invalid interaction index: {index}. Expected 0 to {_supportedInteractions.Length - 1}.");
                return;
            }
            
            if (_currentInteraction != null)
            {
                Debug.LogWarning("Interaction already in progress. Stopping current interaction.");
                _currentInteraction.Stop();
            }
            
            _currentInteraction = _supportedInteractions[index];
            _currentInteraction!.Start();
        }


        public sealed override void InteractionStop()
        {
            if (_supportedInteractions == null)
            {
                Debug.LogWarning("FindSupportedInteractions has not been called. Did you accidentally override Awake() without calling base.Awake()?");
                return;
            }
            if (_supportedInteractions.Length == 0)
                return;

            if (_currentInteraction == null)
            {
                Debug.LogWarning("No interaction to stop.");
                return;
            }
            
            _currentInteraction.Stop();
            _currentInteraction = null;
        }
        
        
        public sealed override string[] GetSupportedInteractionNames() => _supportedInteractionNamesCache;

#endregion


#region Protected API

        /// <summary>
        /// Override to add custom interactions to the object.
        /// </summary>
        /// <param name="supportedInteractions">The list to add interactions to.</param>
        protected virtual void AddCustomInteractions(List<IInteraction> supportedInteractions) { }
        
        
        /// <summary>
        /// Override to implement the "Use" interaction type.
        /// Called when the player presses the "interact" key.
        /// 
        /// <remarks>
        /// Only called if <see cref="SupportsUseInteraction"/> is true.
        /// </remarks>
        /// </summary>
        protected virtual void UseStart() { }
        
        
        /// <summary>
        /// Override to implement the "Use" interaction type.
        /// Called when the player releases the "interact" key.
        /// 
        /// <remarks>
        /// Only called if <see cref="SupportsUseInteraction"/> is true.
        /// </remarks>
        /// </summary>
        protected virtual void UseStop() { }


        protected virtual void Awake()
        {
            FindSupportedInteractions();
        }

#endregion


#region Private API

        private void FindSupportedInteractions()
        {
            List<IInteraction> supportedInteractions = new();
            
            // Add default interactions
            AddDefaultInteractions(supportedInteractions);

            // Add custom interactions
            AddCustomInteractions(supportedInteractions);

            _supportedInteractions = supportedInteractions.ToArray();
            
            // Cache names
            _supportedInteractionNamesCache = new string[_supportedInteractions.Length];
            for (int i = 0; i < _supportedInteractions.Length; i++)
                _supportedInteractionNamesCache[i] = _supportedInteractions[i].GetName();
            
            if (_supportedInteractions.Length == 0)
                Debug.LogWarning($"No interactions found for object {gameObject}.");
        }


        private void AddDefaultInteractions(List<IInteraction> supportedInteractions)
        {
            if (_canBeGrabbed)          supportedInteractions.Add(new ActionInteraction("Grab", GrabStart, GrabStop));
            if (_canBeHeld)             supportedInteractions.Add(new ActionInteraction("Hold", Hold, null));
            if (_canBeCollected)        supportedInteractions.Add(new ActionInteraction("Collect", Collect, null));
            if (SupportsUseInteraction) supportedInteractions.Add(new ActionInteraction("Use", UseStart, UseStop));
        }

#endregion
    }
}