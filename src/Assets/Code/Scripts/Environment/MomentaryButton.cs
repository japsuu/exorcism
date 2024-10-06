using DG.Tweening;
using Player.InteractionSystem;
using UnityEngine;

namespace Environment
{
    /// <summary>
    /// A button that can be interacted with to toggle another object on/off.<br/>
    /// Requires constant interaction to keep the object active.
    /// </summary>
    public class MomentaryButton : StaticInteractableObject
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


        protected override void UseStart()
        {
            if (_cooldown > 0)
                return;
            
            _toggleTarget.SetActive(true);
            AnimateButtonDown();
        }


        protected override void UseStop()
        {
            if (!_toggleTarget.activeSelf)
                return;
            
            _toggleTarget.SetActive(false);
            AnimateButtonUp();
        }


        private void AnimateButtonDown()
        {
            float targetY = _buttonTransform.localPosition.y - _buttonPressDistance;
            _buttonTransform.DOLocalMoveY(targetY, _buttonPressDuration);
            _cooldown = _buttonPressDuration;
        }


        private void AnimateButtonUp()
        {
            float targetY = _buttonTransform.localPosition.y + _buttonPressDistance;
            _buttonTransform.DOLocalMoveY(targetY, _buttonPressDuration);
            _cooldown = _buttonPressDuration;
        }
    }
}