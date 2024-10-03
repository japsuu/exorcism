using System;
using JetBrains.Annotations;

namespace Player.InteractionSystem
{
    public sealed class ActionInteraction : IInteraction
    {
        private readonly string _name;
        [CanBeNull] private readonly Action _onStart;
        [CanBeNull] private readonly Action _onStop;
        
        public ActionInteraction(string name, [CanBeNull] Action onStart, [CanBeNull] Action onStop)
        {
            _name = name;
            _onStart = onStart;
            _onStop = onStop;
        }
        
        public string GetName() => _name;
        public void Start() => _onStart?.Invoke();
        public void Stop() => _onStop?.Invoke();
    }
}