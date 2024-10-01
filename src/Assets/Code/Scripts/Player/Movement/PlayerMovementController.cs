using KinematicCharacterController;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerMovementController : MonoBehaviour, ICharacterController
    {
        [SerializeField]
        private KinematicCharacterMotor Motor;

        [Space(30f)]
        [Header("Stable Movement")]
        [SerializeField]
        private float WalkSpeedMax = 5f;

        [SerializeField]
        private float SprintSpeedMax = 10f;

        [Tooltip("How fast the character can accelerate while on ground")]
        [SerializeField]
        private float MovementSharpness = 8f;

        [Space(30f)]
        [Header("Air Movement")]
        [SerializeField]
        private float AirSpeedMax = 7f;

        [Tooltip("How fast the character can accelerate while in air")]
        [SerializeField]
        private float AirMovementSharpness = 10f;

        [SerializeField]
        private float AirDrag = 0.3f;

        [Space(30f)]
        [Header("Jumping")]
        [Tooltip("Allow jumping while sliding along terrain.")]
        [SerializeField]
        private bool AllowJumpingWhenSliding;

        [SerializeField]
        private float JumpForce = 10f;

        [Tooltip("Additional movement force applied to the character while jumping.")]
        [SerializeField]
        private float JumpScalableForwardSpeed;

        [Tooltip("CoyoteTime before entering ground (Jump Queuing).")]
        [SerializeField]
        private float JumpPreGroundingGraceTime = 0.1f;

        [Tooltip("CoyoteTime after leaving ground (Normal Coyote).")]
        [SerializeField]
        private float JumpPostGroundingGraceTime = 0.1f;

        [Space(30f)]
        [Header("Misc")]
        [SerializeField]
        private Vector3 Gravity = new(0, -30f, 0);

        [Tooltip("Optional transform to calculate the local movement direction from.")]
        [SerializeField]
        private Transform movementDirectionTransform;

        private Vector3 _movementInput;
        private float _targetXAxis;
        private bool _sprinting;
        private bool _jumpRequested;
        private bool _jumpConsumed;
        private bool _jumpedThisFrame;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump;
        private PlayerController _playerController;


        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            Motor.CharacterController = this;
        }


        private void OnEnable()
        {
            _playerController.OnInputsUpdated += SetInputs;
        }


        private void OnDisable()
        {
            _playerController.OnInputsUpdated -= SetInputs;
        }


        private void SetInputs(InputData inputData)
        {
            if (movementDirectionTransform == null)
                _movementInput = inputData.NormalizedMovementInput;
            else
                _movementInput = movementDirectionTransform.TransformDirection(inputData.NormalizedMovementInput);

            // Jumping input
            if (inputData.IsJumping)
            {
                _timeSinceJumpRequested = 0f;
                _jumpRequested = true;
            }

            _sprinting = inputData.IsSprinting;
        }


        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }


        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            // Ground movement
            if (Motor.GroundingStatus.IsStableOnGround)
            {
                float currentVelocityMagnitude = currentVelocity.magnitude;

                Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                // Reorient velocity on slope
                currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                float moveSpeed = _sprinting ? SprintSpeedMax : WalkSpeedMax;

                // Calculate target velocity
                Vector3 inputRight = Vector3.Cross(_movementInput, Motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _movementInput.magnitude;
                Vector3 targetMovementVelocity = reorientedInput * moveSpeed;


                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-MovementSharpness * deltaTime));
            }

            // Air movement
            else
            {
                // Add move input
                if (_movementInput.sqrMagnitude > 0f)
                {
                    Vector3 addedVelocity = _movementInput * (AirMovementSharpness * deltaTime);

                    Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                    // Limit air velocity from inputs
                    if (currentVelocityOnInputsPlane.magnitude < AirSpeedMax)
                    {
                        // clamp addedVel to make total vel not exceed max vel on inputs plane
                        Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, AirSpeedMax);
                        addedVelocity = newTotal - currentVelocityOnInputsPlane;
                    }
                    else
                    {
                        // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                        if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                    }

                    // Prevent air-climbing sloped walls
                    if (Motor.GroundingStatus.FoundAnyGround)
                    {
                        if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                        {
                            Vector3 perpendicularObstructionNormal = Vector3.Cross(
                                Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpendicularObstructionNormal);
                        }
                    }

                    // Apply added velocity
                    currentVelocity += addedVelocity;
                }

                // Gravity
                currentVelocity += Gravity * deltaTime;

                // Drag
                currentVelocity *= 1f / (1f + AirDrag * deltaTime);
            }

            // Handle jumping
            _jumpedThisFrame = false;
            _timeSinceJumpRequested += deltaTime;
            if (_jumpRequested)
            {
                // See if we actually are allowed to jump

                if (!_jumpConsumed)
                {
                    bool grounded = AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround;
                    bool canCoyote = _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime;

                    if (grounded || canCoyote)
                    {
                        // Calculate jump direction before ungrounding
                        Vector3 jumpDirection = Motor.CharacterUp;
                        if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                            jumpDirection = Motor.GroundingStatus.GroundNormal;

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                        Motor.ForceUnground();

                        // Add to the return velocity and reset jump state
                        currentVelocity += jumpDirection * JumpForce -
                                           Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity += _movementInput * JumpScalableForwardSpeed;
                        _jumpRequested = false;
                        _jumpConsumed = true;
                        _jumpedThisFrame = true;
                    }
                }
            }
        }


        public void BeforeCharacterUpdate(float deltaTime)
        {
        }


        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
                OnLanded();
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
                OnLeaveStableGround();
        }


        public void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                    _jumpRequested = false;

                if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!_jumpedThisFrame)
                        _jumpConsumed = false;
                    _timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    _timeSinceLastAbleToJump += deltaTime;
                }
            }
        }


        public bool IsColliderValidForCollisions(Collider coll) => true;


        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }


        public void OnMovementHit(
            Collider hitCollider,
            Vector3 hitNormal,
            Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }


        public void ProcessHitStabilityReport(
            Collider hitCollider,
            Vector3 hitNormal,
            Vector3 hitPoint,
            Vector3 atCharacterPosition,
            Quaternion atCharacterRotation,
            ref HitStabilityReport hitStabilityReport)
        {
        }


        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }


        private void OnLanded()
        {
        }


        private void OnLeaveStableGround()
        {
        }
    }
}