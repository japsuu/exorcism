using DG.Tweening;
using Player.InteractionSystem;
using UnityEngine;

namespace Environment
{
    /// <summary>
    /// A button that can be interacted with to toggle another object on/off.<br/>
    /// Keeps its state between interactions.
    /// </summary>
    public class LatchingButton : StaticInteractableObject
    {
        [Tooltip("The object that will be toggled on/off.")]
        [SerializeField]
        private GameObject _toggleTarget;

        [Tooltip("The transform of the button that will be animated when pressed.")]
        [SerializeField]
        private Transform _buttonTransform;

        [SerializeField]
        private float _buttonPressDistance = 0.03f;

        [SerializeField]
        private float _buttonPressDuration = 0.1f;

        private bool _isObjectActive;
        private float _cooldown;


        protected override void Awake()
        {
            base.Awake();
            
            _toggleTarget.SetActive(false);
        }


        protected override void Update()
        {
            base.Update();
            
            if (_cooldown > 0)
                _cooldown -= Time.deltaTime;
        }


        public override string GetDescription()
        {
            return base.GetDescription()?.Replace("{status}", _isObjectActive ? "<color=red>off</color>" : "<color=green>on</color>");
        }


        protected override void UseStart()
        {
            if (_cooldown > 0)
                return;

            _isObjectActive = !_isObjectActive;
            _toggleTarget.SetActive(_isObjectActive);
            
            AnimateButton();
        }


        protected override void UseStop()
        {
            // Nothing to do here.
        }


        private void AnimateButton()
        {
            float targetY = _isObjectActive ? _buttonTransform.localPosition.y - _buttonPressDistance : _buttonTransform.localPosition.y + _buttonPressDistance;
            _buttonTransform.DOLocalMoveY(targetY, _buttonPressDuration);
            _cooldown = _buttonPressDuration;
        }
    }
}