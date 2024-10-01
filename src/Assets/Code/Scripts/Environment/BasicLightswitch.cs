using DG.Tweening;
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

        [SerializeField]
        private Transform _buttonTransform;

        [SerializeField]
        private float _buttonPressDistance = 0.03f;

        [SerializeField]
        private float _buttonPressDuration = 0.1f;

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
            
            float targetY = _isLightOn ? _buttonTransform.localPosition.y - _buttonPressDistance : _buttonTransform.localPosition.y + _buttonPressDistance;
            _buttonTransform.DOLocalMoveY(targetY, _buttonPressDuration);
        }
    }
}