using UnityEngine;

namespace Player
{
    public struct Inputs
    {
        public Vector3 NormalizedMovementInput;
        public Vector2 MouseInput;
        public bool IsJumping;
        public bool IsSprinting;
        public bool IsHoldingPrimaryMouseButton;
        public bool IsHoldingSecondaryMouseButton;
    }
}