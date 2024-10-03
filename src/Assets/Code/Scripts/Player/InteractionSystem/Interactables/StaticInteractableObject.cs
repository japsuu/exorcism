using System.Collections.Generic;
using JetBrains.Annotations;
using Tools;
using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// A simple wrapper around <see cref="InteractableBehaviour"/> for objects that can be interacted with.
    /// </summary>
    public abstract class InteractableObject : InteractableBehaviour
    {
        [Header("Object Info")]
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        
        [CanBeNull]
        private IInteraction _currentInteraction;
        private IInteraction[] _supportedInteractions;
        [SerializeField, ReadOnly]
        private string[] _supportedInteractionNamesCache;

        
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


#region Public API


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

        protected virtual void Awake()
        {
            FindSupportedInteractions();
        }


        protected virtual void Update()
        {
            
        }
        
        protected abstract void RegisterInteractions(List<IInteraction> supportedInteractions);

#endregion


#region Private API

        private void FindSupportedInteractions()
        {
            List<IInteraction> supportedInteractions = new();
            
            // Register all available interactions
            RegisterInteractions(supportedInteractions);

            _supportedInteractions = supportedInteractions.ToArray();
            
            // Cache names
            _supportedInteractionNamesCache = new string[_supportedInteractions.Length];
            for (int i = 0; i < _supportedInteractions.Length; i++)
                _supportedInteractionNamesCache[i] = _supportedInteractions[i].GetName();
            
            if (_supportedInteractions.Length == 0)
                Debug.LogWarning($"No interactions found for object {gameObject}.");
        }

#endregion
    }
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
            supportedInteractions.Add(new ActionInteraction("Use", UseStart, UseStop));
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

#endregion
    }
}