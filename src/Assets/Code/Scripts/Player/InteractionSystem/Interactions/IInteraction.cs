namespace Player.InteractionSystem
{
    public interface IInteraction
    {
        public string GetName();
        public void Start();
        public void Stop();
    }
}