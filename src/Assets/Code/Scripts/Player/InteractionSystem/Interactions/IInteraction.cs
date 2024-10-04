namespace Player.InteractionSystem
{
    /// <summary>
    /// Describes a single interaction that can be performed on an object.
    /// </summary>
    public interface IInteraction
    {
        /// <summary>
        /// Gets the UI friendly name of the interaction.
        /// </summary>
        /// <returns></returns>
        public string GetName();
        
        /// <summary>
        /// Gets whether the interaction needs to stop right now.<br/>
        /// If true, the interaction will be stopped immediately, and <see cref="OnStop"/> will be called.
        /// <example>
        /// Set to true when the player stops looking at the object.
        /// </example>
        /// </summary>
        public bool ShouldStop(RaycastDataArgs args);
        
        /// <summary>
        /// Called when the interaction is started.<br/>
        /// Called when the player presses the interaction key.
        /// For every <see cref="OnStart"/> call there will be a matching <see cref="OnStop"/> call.
        /// </summary>
        public void OnStart();
        
        /// <summary>
        /// Called every frame while the interaction is active.
        /// </summary>
        public void OnUpdate();
        
        /// <summary>
        /// Called when the interaction is stopped.<br/>
        /// Called automatically when the player releases the interaction key.
        /// </summary>
        public void OnStop();
    }
}