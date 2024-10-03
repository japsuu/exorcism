using JetBrains.Annotations;

namespace Player.InteractionSystem
{
    public readonly struct LookAtChangedEventArgs
    {
        [CanBeNull]
        public readonly IInteractable OldLookAt;

        [CanBeNull]
        public readonly IInteractable NewLookAt;


        public LookAtChangedEventArgs(IInteractable oldLookAt, IInteractable newLookAt)
        {
            OldLookAt = oldLookAt;
            NewLookAt = newLookAt;
        }
    }
}