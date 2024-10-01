using System.Linq;
using UnityEngine;

namespace Player
{
    public class CharacterInputController : MonoBehaviour
    {
        [HideInInspector] public bool MouseInputEnabled = true;
        [HideInInspector] public bool KeyboardInputEnabled = true;
        
        private const string HORIZONTAL_INPUT_AXIS_NAME = "Horizontal";
        private const string VERTICAL_INPUT_AXIS_AXIS_NAME = "Vertical";
        private const string MOUSE_HORIZONTAL_AXIS_NAME = "Mouse X";
        private const string MOUSE_VERTICAL_AXIS_NAME = "Mouse Y";
        private const KeyCode JUMP_KEY = KeyCode.Space;
        private const KeyCode SPRINT_KEY = KeyCode.LeftShift;
        private const int PRIMARY_MOUSE_BUTTON = 0;
        private const int SECONDARY_MOUSE_BUTTON = 1;

        private const int MAX_ROT_CACHE_SIZE = 3;

        private float[] _rotCacheHorizontal;
        private float[] _rotCacheVertical;
        private int _rotCacheIndex;

        private Vector3 _normalizedMovementInput;
        private Vector2 _mouseInput;
        private bool _isJumping;
        private bool _isSprinting;
        private bool _isHoldingPrimaryMouseButton;
        private bool _isHoldingSecondaryMouseButton;

        public void Awake()
        {
            _rotCacheHorizontal = new float[MAX_ROT_CACHE_SIZE];
            _rotCacheVertical = new float[MAX_ROT_CACHE_SIZE];
        }

        public void Update()
        {
            if (MouseInputEnabled)
            {
                _mouseInput = new Vector2(
                    GetSmoothedHorizontalAxis(),
                    GetSmoothedVerticalAxis());
            
                IncreaseRotCacheIndex();
            }

            if (KeyboardInputEnabled)
            {
                Vector3 movementInput = new(
                    UnityEngine.Input.GetAxisRaw(HORIZONTAL_INPUT_AXIS_NAME),
                    0,
                    UnityEngine.Input.GetAxisRaw(VERTICAL_INPUT_AXIS_AXIS_NAME));
            
                _normalizedMovementInput = Vector3.ClampMagnitude(movementInput, 1f);
                
                _isJumping = UnityEngine.Input.GetKeyDown(JUMP_KEY);
                _isSprinting = UnityEngine.Input.GetKey(SPRINT_KEY);
            }
            
            _isHoldingPrimaryMouseButton = UnityEngine.Input.GetMouseButton(PRIMARY_MOUSE_BUTTON);
            _isHoldingSecondaryMouseButton = UnityEngine.Input.GetMouseButton(SECONDARY_MOUSE_BUTTON);
        }

        public Inputs GetInput()
        {
            return new Inputs
            {
                // Build the CharacterInputs struct
                NormalizedMovementInput = _normalizedMovementInput,
                MouseInput = _mouseInput,
                IsJumping = _isJumping,
                IsSprinting = _isSprinting,
                IsHoldingPrimaryMouseButton = _isHoldingPrimaryMouseButton,
                IsHoldingSecondaryMouseButton = _isHoldingSecondaryMouseButton
            };
        }

        private float GetSmoothedHorizontalAxis()
        {
            _rotCacheHorizontal[_rotCacheIndex] = UnityEngine.Input.GetAxis(MOUSE_HORIZONTAL_AXIS_NAME);
            
            float result = _rotCacheHorizontal.Sum();
            return result / _rotCacheHorizontal.Length;
        }

        private float GetSmoothedVerticalAxis()
        {
            _rotCacheVertical[_rotCacheIndex] = UnityEngine.Input.GetAxis(MOUSE_VERTICAL_AXIS_NAME);
            
            float result = _rotCacheVertical.Sum();
            return result / _rotCacheVertical.Length;
        }

        private void IncreaseRotCacheIndex()
        {
            _rotCacheIndex++;
            _rotCacheIndex %= MAX_ROT_CACHE_SIZE;
        }
    }
}