using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicPortals
{
    public class Player : MonoBehaviour
    {
        [Space, Header("-Camera Settings"), Space]

        [Tooltip("How fast the camera rotates")]
        [SerializeField] float _camSpeed = 0.25f;

        [Tooltip("How fast the camera recenters on the z axis")]
        [SerializeField] float _camRecenterSpeed = 10f;


        [Space, Header("-Move Settings"), Space]

        [Tooltip("How fast the player moves")]
        [SerializeField] float _speed = 5f;

        [Tooltip("How high the player jumps")]
        [SerializeField] float _jumpForce = 5f;

        [Tooltip("Sets a limit to the y velocity of the player")]
        [SerializeField] float _groundRaycastLenght = 1.1f;


        [Space, Header("-Grab Settings"), Space]

        [Tooltip("The speed at which the object follows")]
        [SerializeField] float _followingSpeed = 3f;

        [Tooltip("How near an object as to be to get grabbed")]
        [SerializeField] float _grabDistance = 3f;

        [Tooltip("How far an object as to be the get ungrabbed")]
        [SerializeField] float _ungrabDistance = 5f;

        float _camRotX, _camRotY, _camRotZ;
        Transform _cam;
        Rigidbody _rb;
        Rigidbody _grabbedObjectRb;
        Vector2 _smoothMoveInput;
        Vector2 _currentInputVelocity;
        Inputs _input;

        public Camera Camera => GetComponentInChildren<Camera>();

        public Vector3 Velocity
        {
            get => _rb.velocity;
            set { _rb.velocity = value; }
        }

        private Vector3 _knockbackVelocity;

        public void ApplyKnockback(Vector3 direction, float force)
        {
            // Set or add to our knockback velocity
            _knockbackVelocity = direction.normalized * force;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _camRotY = transform.eulerAngles.y;
            _rb = GetComponent<Rigidbody>();
            _cam = GetComponentInChildren<Camera>().transform;
        }

        void Update()
        {
            Jump();
            RotateCam();
            GrabObject();
        }

        void FixedUpdate()
        {
            Move();
            MoveGrabbedObject();
        }

        #region Move
        void Jump()
        {
            if (_input.Player.Jump.WasPressedThisFrame())
            {
                if (Physics.Raycast(transform.position, Vector3.down, _groundRaycastLenght)) _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
            }
        }


        void Move()
        {
            // Existing input-based movement
            Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();
            Vector3 inputDirection = _cam.right * moveInput.x 
                                + Quaternion.Euler(0, -90, 0) * _cam.right * moveInput.y;
            inputDirection.y = 0f;
            Vector3 moveOffset = inputDirection * _speed * Time.fixedDeltaTime;

            // Combine knockback offset (knockbackVelocity * deltaTime)
            Vector3 totalOffset = moveOffset + _knockbackVelocity * Time.fixedDeltaTime;

            // Move the player
            _rb.MovePosition(transform.position + totalOffset);

            // Dampen the knockback velocity over time
            _knockbackVelocity = Vector3.MoveTowards(
                _knockbackVelocity, 
                Vector3.zero, 
                Time.fixedDeltaTime * 10f // "10" is just a damping rate
            );
        }

        public void Teleport(Vector3 pos, Vector3 rot)
        {
            transform.position = pos - _cam.localPosition;
            _cam.rotation = Quaternion.Euler(rot);
            _camRotX = rot.x;
            _camRotY = rot.y;
            _camRotZ = rot.z;
        }

        void RotateCam()
        {
            Vector2 mouseDelta = _input.Player.MouseDelta.ReadValue<Vector2>() * _camSpeed;

            _camRotX -= mouseDelta.y;
            if (_camRotX < 0) _camRotX = 360f;
            else if (_camRotX > 360) _camRotX = 0f;
            if (_camRotX >= 90 && _camRotX < 180) _camRotX = 90;
            else if (_camRotX <= 270 && _camRotX > 180) _camRotX = 270;

            _camRotY += mouseDelta.x;
            _camRotZ = Mathf.LerpAngle(_camRotZ, 0, Time.deltaTime * _camRecenterSpeed);

            _cam.rotation = Quaternion.Euler(_camRotX, _camRotY, _camRotZ);
        }
        #endregion

        #region Grab
        void GrabObject()
        {
            if (_input.Player.Grab.WasPressedThisFrame())
            {
                if (_grabbedObjectRb != null)
                {
                    _grabbedObjectRb = null;
                }
                else
                {
                    if (Physics.Raycast(_cam.position, _cam.forward, out RaycastHit hit, _grabDistance))
                    {
                        if (hit.transform.gameObject.TryGetComponent(out Rigidbody hitObjectRb)) _grabbedObjectRb = hitObjectRb;
                    }
                }
            }
            if (_grabbedObjectRb != null)
            {
                if (Vector3.Distance(_grabbedObjectRb.transform.position, transform.position) > _ungrabDistance)
                {
                    _grabbedObjectRb = null;
                }
            }
        }

        void MoveGrabbedObject()
        {
            if (_grabbedObjectRb == null) return;
            _grabbedObjectRb.velocity = (_cam.position + _cam.forward * 2 - _grabbedObjectRb.transform.position) * _followingSpeed;
        }
        #endregion

        #region Debug
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * _groundRaycastLenght);
        }
        #endregion

        #region Inputs
        void Awake() => _input = new Inputs();
        void OnEnable() => _input.Enable();
        void OnDisable() => _input.Disable();
        #endregion
    }
}
