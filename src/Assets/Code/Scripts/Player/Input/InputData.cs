using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public struct InputData
    {
        public Vector3 NormalizedMovementInput;
        public Vector2 MouseInput;
        public bool IsJumping;
        public bool IsSprinting;
        public bool IsHoldingPrimaryMouseButton;
        public bool IsHoldingSecondaryMouseButton;
    }
}