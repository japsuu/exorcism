using Player.InteractionSystem;
using UnityEngine;

namespace Environment
{
    /// <summary>
    /// Interactable lightswitch that can be turned on/off.
    /// </summary>
    public class BasicLightswitch : InteractableBehaviour
    {
        [SerializeField]
        private Light _light;

        private bool _isLightOn;


        private void Awake()
        {
            _isLightOn = _light.enabled;
        }


        public override string GetInteractDescription() =>
            _isLightOn
                ? "Turn <color=red>off</color> the lightswitch."
                : "Turn <color=green>on</color> the lightswitch.";


        public override void Interact()
        {
            _isLightOn = !_isLightOn;
            _light.enabled = _isLightOn;
        }
    }
}