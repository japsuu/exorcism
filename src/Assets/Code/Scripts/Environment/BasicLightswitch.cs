using DG.Tweening;
using Player.InteractionSystem;
using UnityEngine;

namespace Environment
{
    /// <summary>
    /// Interactable lightswitch that can be turned on/off.
    /// </summary>
    public class BasicLightswitch : StaticInteractableObject
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


        protected override void Awake()
        {
            base.Awake();
            
            _isLightOn = _light.enabled;
        }


        public override string GetDescription()
        {
            return base.GetDescription()?.Replace("{status}", _isLightOn ? "<color=red>off</color>" : "<color=green>on</color>");
        }


        protected override void UseStart()
        {
            _isLightOn = !_isLightOn;
            _light.enabled = _isLightOn;
            
            AnimateButton();
        }


        protected override void UseStop()
        {
            // Nothing to do here.
        }


        private void AnimateButton()
        {
            float targetY = _isLightOn ? _buttonTransform.localPosition.y - _buttonPressDistance : _buttonTransform.localPosition.y + _buttonPressDistance;
            _buttonTransform.DOLocalMoveY(targetY, _buttonPressDuration);
        }
    }
}