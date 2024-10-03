using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Player.InteractionSystem;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Shows the info of the interactable the player is currently hovering.
    /// TODO: Stop using Camera.main.
    /// </summary>
    public class InteractSelectionUI : MonoBehaviour
    {
        private const int MENU_ENTRY_POOL_SIZE = 8;
        
        [SerializeField]
        private Canvas _canvas;
        
        [SerializeField]
        private Transform _interactionUIParent;
        
        [SerializeField]
        private Transform _interactionMenuEntryParent;

        [SerializeField]
        private InteractMenuEntry _interactMenuEntryReference;

        [SerializeField]
        private RectTransform _selectionBounds;

        [SerializeField]
        private RectTransform _nameTextParent;

        [SerializeField]
        private TMP_Text _nameText;

        [SerializeField]
        private RectTransform _descriptionTextParent;

        [SerializeField]
        private TMP_Text _descriptionText;

        [CanBeNull]
        private IInteractable _targetedInteractable;
        private Camera _camera;
        private InteractMenuEntry[] _availableMenuEntries;
        private int _selectedInteractionIndex;


        private void Awake()
        {
            _camera = Camera.main;
            
            InitializeInteractionMenuEntries();
            
            SetVisible(false);

            InteractionInvoker.TargetChanged += OnInteractTargetChanged;
            InteractionInvoker.SelectedInteractionIndexChanged += OnSelectedInteractionIndexChanged;
        }


        private void OnDestroy()
        {
            InteractionInvoker.TargetChanged -= OnInteractTargetChanged;
            InteractionInvoker.SelectedInteractionIndexChanged -= OnSelectedInteractionIndexChanged;
        }


        private void Update()
        {
            UpdateTargetName();
            UpdateTargetDescription();
            
            UpdateSelectionBounds();
        }


        private void UpdateTargetName()
        {
            string nameStr = _targetedInteractable?.GetName();
            SetTextOrHide(_nameTextParent, _nameText, nameStr);
        }


        private void UpdateTargetDescription()
        {
            string descriptionStr = _targetedInteractable?.GetDescription();
            SetTextOrHide(_descriptionTextParent, _descriptionText, descriptionStr);
        }
        
        
        private static void SetTextOrHide(RectTransform textParent, TMP_Text text, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                textParent.gameObject.SetActive(false);
                return;
            }
            textParent.gameObject.SetActive(true);
            text.text = value;
        }


        private void InitializeInteractionMenuEntries()
        {
            _availableMenuEntries = new InteractMenuEntry[MENU_ENTRY_POOL_SIZE];
            for (int i = 0; i < MENU_ENTRY_POOL_SIZE; i++)
            {
                InteractMenuEntry entry = Instantiate(_interactMenuEntryReference, _interactionMenuEntryParent);
                entry.gameObject.SetActive(true);
                entry.Clear();
                _availableMenuEntries[i] = entry;
            }
            
            Destroy(_interactMenuEntryReference.gameObject);
        }


        private void UpdateInteractionMenuEntries([CanBeNull] string[] names)
        {
            if (names == null)
            {
                foreach (InteractMenuEntry entry in _availableMenuEntries)
                {
                    entry.gameObject.SetActive(false);
                }
                return;
            }
            
            if (names.Length > MENU_ENTRY_POOL_SIZE)
            {
                Debug.LogWarning($"Too many interaction options for object {_targetedInteractable?.GetName()}. Only {MENU_ENTRY_POOL_SIZE} will be shown.");
            }

            for (int i = 0; i < _availableMenuEntries.Length; i++)
            {
                InteractMenuEntry entry = _availableMenuEntries[i];
                entry.Clear();
                if (i < names.Length)
                {
                    entry.gameObject.SetActive(true);
                    entry.SetText(i, names[i]);
                }
                else
                {
                    entry.gameObject.SetActive(false);
                }
            }
            
            UpdateSelectedInteractionIndex();
        }
        
        
        private void UpdateSelectedInteractionIndex()
        {
            for (int i = 0; i < _availableMenuEntries.Length; i++)
            {
                _availableMenuEntries[i].SetSelected(i == _selectedInteractionIndex);
            }
        }
        
        private void OnSelectedInteractionIndexChanged(int index)
        {
            _selectedInteractionIndex = index;
            UpdateSelectedInteractionIndex();
        }


        private void OnInteractTargetChanged(InteractionInvoker.LookAtChangedEventArgs args)
        {
            bool hasTarget = args.NewLookAt != null;
            
            _targetedInteractable = args.NewLookAt;
            
            UpdateInteractionMenuEntries(_targetedInteractable?.GetSupportedInteractionNames());
            
            SetVisible(hasTarget);
            UpdateSelectionBounds();
        }


        private void SetVisible(bool visible)
        {
            _interactionUIParent.gameObject.SetActive(visible);
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

            IEnumerable<Vector3> screenCorners = worldCorners.Select(corner => _camera.WorldToScreenPoint(corner));
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
        }
    }
}