namespace Player.InteractionSystem
{
    public readonly struct InteractionDataArgs
    {
        /// <summary>
        /// True, if the player is looking at the object and is close enough to initiate an interaction.<br/>
        /// False, if the player is looking at the object but is too far away to initiate an interaction.
        /// </summary>
        public readonly bool IsLookingAtObject;
        
        /// <summary>
        /// The current distance between the player and the interactable object.
        /// </summary>
        public readonly float DistanceToInteractable;


        public InteractionDataArgs(bool isLookingAtObject, float distanceToInteractable)
        {
            IsLookingAtObject = isLookingAtObject;
            DistanceToInteractable = distanceToInteractable;
        }
    }
}