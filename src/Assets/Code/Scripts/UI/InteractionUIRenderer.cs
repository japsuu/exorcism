using JetBrains.Annotations;
using Player.InteractionSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Shows the info of the interactable the player is currently hovering.
    /// </summary>
    public class InteractionUIRenderer : MonoBehaviour
    {
        [SerializeField]
        private Transform _interactionUiParentTransform;

        [SerializeField]
        private TMP_Text _interactText;

        [SerializeField]
        private Image _interactHoldProgressImage;

        [CanBeNull]
        private IInteractable _targetedInteractable;


        private void Awake()
        {
            _interactHoldProgressImage.fillAmount = 0;
            _interactionUiParentTransform.gameObject.SetActive(false);

            InteractionInvoker.TargetChanged += OnInteractTargetChanged;
        }


        private void Update()
        {
            _interactHoldProgressImage.fillAmount = InteractionInvoker.InteractionProgress;
            _interactText.text = _targetedInteractable?.GetInteractDescription();
        }


        private void OnDestroy()
        {
            InteractionInvoker.TargetChanged -= OnInteractTargetChanged;
        }


        private void OnInteractTargetChanged(InteractionInvoker.LookAtChangedEventArgs args)
        {
            _targetedInteractable = args.NewLookAt;
            _interactHoldProgressImage.fillAmount = InteractionInvoker.InteractionProgress;
            _interactionUiParentTransform.gameObject.SetActive(args.NewLookAt != null);
            _interactText.text = "";
        }
    }
}