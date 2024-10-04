using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Drag a rigidbody with the mouse using a spring joint.
    ///
    /// Can be rotated while holding the right mouse button, and thrown by clicking the left mouse button.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PhysicsDraggingInteraction : MonoBehaviour, IInteraction
    {
        /// <summary>
        /// How far the object can be from the target position before the dragging is canceled.
        /// </summary>
        [SerializeField] private float _maxDistanceToTarget = 3.1f;

        /// <summary>
        /// How large of an angle difference is allowed between the view direction and the direction to the object.
        /// This is used to cancel interaction objects that are too heavy to follow the view direction
        /// if the player is trying to rotate too quickly.
        /// </summary>
        [SerializeField] private float _maxAngleDifference = 55f;

        private Transform _positionTarget;
        private Rigidbody _rb;
        private bool _requestThrow;


        public string GetName() => "Drag";
        
        
        public bool ShouldStop(RaycastDataArgs args)
        {
            if (args.DistanceTo(gameObject) > _maxDistanceToTarget)
                return true;
            
            if (_requestThrow)
                return true;
            
            // Calculate the direction from raycast position to the object.
            Vector3 directionToObject = transform.position - args.RaycastSourcePos;
            // Calculate the angle between the view direction and the direction to the object.
            float angleDifference = Vector3.Angle(args.RaycastSourceDirection, directionToObject);
            if (angleDifference > _maxAngleDifference)
                return true;
            
            return false;
        }


        public void OnStart()
        {
            if (_positionTarget != null)
            {
                Debug.LogWarning("Position target already exists. This should not happen.", this);
                OnStop();
            }
            
            _positionTarget = CreatePositionTarget(
                _rb,
                InteractionInvoker.Instance.GrabTargetPosition,
                transform.rotation,
                InteractionInvoker.Instance.GrabStrength,
                InteractionInvoker.Instance.GrabMaxForce);
        }


        public void OnUpdate()
        {
            if (_positionTarget == null)
                return;
            
            _positionTarget.position = InteractionInvoker.Instance.GrabTargetPosition;

            if (Input.GetMouseButton(1))
            {
                // TODO. Move to GrabTargetRotation in InteractionInvoker?
                Quaternion rotation = _positionTarget.rotation;
                rotation *= Quaternion.Euler(Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0);
                _positionTarget.rotation = rotation;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _requestThrow = true;
            }
        }


        public void OnStop()
        {
            if (_positionTarget == null)
                Debug.LogWarning("Position target does not exist. This should not happen.", this);
            
            Destroy(_positionTarget.gameObject);
            
            if (_requestThrow)
                ThrowSelf();
        }


        private void ThrowSelf()
        {
            Vector3 forceDirection = transform.position - InteractionInvoker.Instance.RaycastPosition;
            _rb.AddForce(forceDirection * InteractionInvoker.Instance.ThrowForce, ForceMode.Impulse);
            _requestThrow = false;
        }


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }


        /// <summary>
        /// Creates a position target for the defined rigidbody,
        /// and attaches the rigidbody to it using a configurable joint.
        /// </summary>
        /// <param name="rb">The rigidbody to attach to the target.</param>
        /// <param name="attachmentPosition"></param>
        /// <param name="attachmentRotation"></param>
        /// <param name="springStrength"></param>
        /// <param name="maxForce"></param>
        /// <returns></returns>
        private static Transform CreatePositionTarget(Rigidbody rb, Vector3 attachmentPosition, Quaternion attachmentRotation, float springStrength, float maxForce)
        {
            GameObject target = new("Attachment Point");
            //target.hideFlags = HideFlags.HideInHierarchy;
            target.transform.position = attachmentPosition;
            target.transform.rotation = attachmentRotation;

            Rigidbody targetRb = target.AddComponent<Rigidbody>();
            targetRb.isKinematic = true;
            
            // The damping needs to be high enough to prevent the object from bouncing around,
            // but low enough to not make the object feel sluggish.
            // Damping of 50 is good for mass 5.
            // Damping of 200 is good for mass 50.
            // Damping of 400 is good for mass 100.
            // Damping of 800 is good for mass 200.
            // We can calculate the damping based on the mass, by lerping between 50 and 800.
            float springDamping = Mathf.Lerp(50, 800, rb.mass / 200f);

            ConfigurableJoint joint = target.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = rb;
            joint.configuredInWorldSpace = true;
            joint.xDrive = CreateJointDrive(springStrength, springDamping, maxForce);
            joint.yDrive = CreateJointDrive(springStrength, springDamping, maxForce);
            joint.zDrive = CreateJointDrive(springStrength, springDamping, maxForce);
            joint.slerpDrive = CreateJointDrive(springStrength, springDamping, maxForce);
            joint.rotationDriveMode = RotationDriveMode.Slerp;

            return target.transform;
        }


        private static JointDrive CreateJointDrive(float springStrength, float springDamping, float maxForce)
        {
            JointDrive drive = new()
            {
                maximumForce = maxForce,
                positionDamper = springDamping,
                positionSpring = springStrength,
                useAcceleration = false
            };
            return drive;
        }


        private void OnDrawGizmos()
        {
            if (_positionTarget == null)
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _positionTarget.position);
        }
    }
}