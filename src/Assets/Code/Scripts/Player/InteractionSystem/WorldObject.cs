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
        /// <summary>
        /// Allows you to position the object in the world.
        /// Hold the right mouse button while grabbing to rotate the object around the X and Y axis.
        /// </summary>
        [Header("Interaction Settings")]
        [SerializeField] private bool _canBeGrabbed;
        
        /// <summary>
        /// Places the item in your hand.
        /// </summary>
        [SerializeField] private bool _canBeHeld;
        
        /// <summary>
        /// Places the item in your inventory.
        /// </summary>
        [SerializeField] private bool _canBeCollected;
        
        
        [Header("Object Info")]
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        
        [CanBeNull]
        private IInteraction _currentInteraction;
        private IInteraction[] _supportedInteractions;
        [SerializeField, ReadOnly]
        private string[] _supportedInteractionNamesCache;


        protected virtual void Awake()
        {
            FindSupportedInteractions();
        }


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


        protected virtual void AddCustomInteractions(List<IInteraction> supportedInteractions)
        {
            // Override this method to add custom interactions.
        }


        public void GrabStart() => throw new System.NotImplementedException();
        public void GrabStop() => throw new System.NotImplementedException();
        public void Hold() => throw new System.NotImplementedException();
        public void Collect() => throw new System.NotImplementedException();
        protected virtual void UseStart() { }
        protected virtual void UseStop() { }
        protected abstract bool SupportsUseInteraction { get; }


        public override string GetName() => _name;
        public override string GetDescription() => _description;
        public sealed override string[] GetSupportedInteractionNames() => _supportedInteractionNamesCache;


        public override void InteractionStart(int index)
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


        public override void InteractionStop()
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
    }
}