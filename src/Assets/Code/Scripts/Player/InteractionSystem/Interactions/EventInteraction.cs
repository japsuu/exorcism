using System;
using JetBrains.Annotations;

namespace Player.InteractionSystem
{
    public sealed class EventInteraction : IInteraction
    {
        private readonly string _name;
        [CanBeNull] private readonly Action _onStart;
        [CanBeNull] private readonly Action _onStop;
        private readonly Func<RaycastDataArgs, bool> _shouldStop;
        
        public EventInteraction(string name, [CanBeNull] Action onStart, [CanBeNull] Action onStop, Func<RaycastDataArgs, bool> shouldStop = null)
        {
            _name = name;
            _onStart = onStart;
            _onStop = onStop;
            _shouldStop = shouldStop ?? (_ => false);
        }
        
        public string GetName() => _name;

        public bool ShouldStop(RaycastDataArgs args) => _shouldStop(args);

        public void OnStart() => _onStart?.Invoke();

        public void OnUpdate()
        {
            // Do nothing
        }
        
        public void OnStop() => _onStop?.Invoke();
    }
}