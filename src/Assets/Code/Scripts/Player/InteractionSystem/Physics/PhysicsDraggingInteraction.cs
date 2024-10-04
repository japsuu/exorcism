using UnityEngine;

namespace Player.InteractionSystem
{
    /// <summary>
    /// Drag a rigidbody with the mouse using a spring joint.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PhysicsDraggingInteraction : MonoBehaviour, IInteraction
    {
        /// <summary>
        /// How far the object can be from the target position before the dragging is canceled.
        /// </summary>
        [SerializeField] private float _maxDistanceToTarget = 3.1f;

        private Transform _positionTarget;
        private Rigidbody _rb;


        public string GetName() => "Drag";
        public bool ShouldStop(RaycastDataArgs args) => args.DistanceTo(gameObject) > _maxDistanceToTarget;


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
                InteractionInvoker.Instance.GrabForce,
                InteractionInvoker.Instance.GrabDamping);
        }


        public void OnUpdate()
        {
            if (_positionTarget == null)
                return;
            
            _positionTarget.position = InteractionInvoker.Instance.GrabTargetPosition;
            
            if (!Input.GetMouseButton(1))
                return;
            
            // TODO. Move to GrabTargetRotation in InteractionInvoker?
            Quaternion rotation = _positionTarget.rotation;
            rotation *= Quaternion.Euler(Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0);
            _positionTarget.rotation = rotation;
        }


        public void OnStop()
        {
            if (_positionTarget == null)
                Debug.LogWarning("Position target does not exist. This should not happen.", this);
            
            Destroy(_positionTarget.gameObject);
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
        /// <param name="force"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        private static Transform CreatePositionTarget(Rigidbody rb, Vector3 attachmentPosition, Quaternion attachmentRotation, float force, float damping)
        {
            GameObject target = new("Attachment Point");
            //target.hideFlags = HideFlags.HideInHierarchy;
            target.transform.position = attachmentPosition;
            target.transform.rotation = attachmentRotation;

            Rigidbody targetRb = target.AddComponent<Rigidbody>();
            targetRb.isKinematic = true;

            ConfigurableJoint joint = target.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = rb;
            joint.configuredInWorldSpace = true;
            joint.xDrive = CreateJointDrive(force, damping);
            joint.yDrive = CreateJointDrive(force, damping);
            joint.zDrive = CreateJointDrive(force, damping);
            joint.slerpDrive = CreateJointDrive(force, damping);
            joint.rotationDriveMode = RotationDriveMode.Slerp;

            return target.transform;
        }


        private static JointDrive CreateJointDrive(float force, float damping)
        {
            JointDrive drive = new()
            {
                positionSpring = force,
                positionDamper = damping,
                maximumForce = Mathf.Infinity
            };
            return drive;
        }
    }
}