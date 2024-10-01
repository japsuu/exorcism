using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Player.InteractionSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Shows the info of the interactable the player is currently hovering.
    /// TODO: Stop using Camera.main.
    /// </summary>
    public class InteractionUIRenderer : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;
        
        [SerializeField]
        private Transform _interactionUiParentTransform;

        [SerializeField]
        private TMP_Text _interactText;

        [SerializeField]
        private Image _interactHoldProgressImage;

        [SerializeField]
        private RectTransform _selectionBounds;

        [CanBeNull]
        private IInteractable _targetedInteractable;


        private void Awake()
        {
            _interactHoldProgressImage.fillAmount = 0;
            _interactionUiParentTransform.gameObject.SetActive(false);
            _selectionBounds.gameObject.SetActive(false);

            InteractionInvoker.TargetChanged += OnInteractTargetChanged;
        }


        private void Update()
        {
            _interactHoldProgressImage.fillAmount = InteractionInvoker.InteractionProgress;
            _interactText.text = _targetedInteractable?.GetInteractDescription();
            
            UpdateSelectionBounds();
        }


        private void OnDestroy()
        {
            InteractionInvoker.TargetChanged -= OnInteractTargetChanged;
        }


        private void OnInteractTargetChanged(InteractionInvoker.LookAtChangedEventArgs args)
        {
            bool hasTarget = args.NewLookAt != null;
            
            _targetedInteractable = args.NewLookAt;
            _interactHoldProgressImage.fillAmount = InteractionInvoker.InteractionProgress;
            _interactText.text = "";
            _interactionUiParentTransform.gameObject.SetActive(hasTarget);
            _selectionBounds.gameObject.SetActive(hasTarget);
            UpdateSelectionBounds();
        }


        private void UpdateSelectionBounds()
        {
            if (_targetedInteractable == null)
                return;

            Bounds bounds = _targetedInteractable.GetWorldBounds();
            
            
            Vector3 c = bounds.center;
            Vector3 e = bounds.extents;

            Vector3[] worldCorners = {
                new( c.x + e.x, c.y + e.y, c.z + e.z ),
                new( c.x + e.x, c.y + e.y, c.z - e.z ),
                new( c.x + e.x, c.y - e.y, c.z + e.z ),
                new( c.x + e.x, c.y - e.y, c.z - e.z ),
                new( c.x - e.x, c.y + e.y, c.z + e.z ),
                new( c.x - e.x, c.y + e.y, c.z - e.z ),
                new( c.x - e.x, c.y - e.y, c.z + e.z ),
                new( c.x - e.x, c.y - e.y, c.z - e.z ),
            };

            IEnumerable<Vector3> screenCorners = worldCorners.Select(corner => Camera.main.WorldToScreenPoint(corner));
            float maxX = float.MinValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float minY = float.MaxValue;
            foreach (Vector3 corner in screenCorners)
            {
                if (corner.x > maxX)
                {
                    maxX = corner.x;
                }
                if (corner.x < minX)
                {
                    minX = corner.x;
                }
                if (corner.y > maxY)
                {
                    maxY = corner.y;
                }
                if (corner.y < minY)
                {
                    minY = corner.y;
                }
            }

            Vector3 topLeft = new(minX, maxY, 0);
            Vector3 bottomRight = new(maxX, minY, 0);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, topLeft, _canvas.worldCamera, out Vector2 localTopLeft);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, bottomRight, _canvas.worldCamera, out Vector2 localBottomRight);

            _selectionBounds.anchoredPosition = localTopLeft;
            _selectionBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localBottomRight.x - localTopLeft.x);
            _selectionBounds.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localTopLeft.y - localBottomRight.y);
            
            /*Vector3 topRight = new(maxX, maxY, 0);
            Vector3 bottomLeft = new(minX, minY, 0);
            _left.position = topLeft;
            _top.position = topRight;
            _right.position = bottomRight;
            _bottom.position = bottomLeft;*/
        }
    }
}