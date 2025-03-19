using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DynamicPortals
{
    public class Portal : PortalBase
    {
        [SerializeField] Portal _targetPortal;
        [SerializeField] int _maxRecursion = 6;

        [SerializeField, Range(0, 10)] float _playerVelocityMultiplier = 1;
        [SerializeField, Range(0, 10)] float _objectVelocityMultiplier = 1;
        [SerializeField, Range(0, 10)] float _portalVelocityMultiplier = 1;

        public enum RenderWhen { IsInView, TargetIsInView, Always }
        [SerializeField] RenderWhen _renderWhen = RenderWhen.IsInView;

        bool _playerPreviousSide;
        bool _isPlayerInBound;
        Vector3 _portalVelocity;
        Vector3 _lastPos;
        Collider _surfaceColl;

        Player _player;
        Rigidbody _playerRb;

        readonly List<TravellerObject> _travellerObjects = new();

        public override bool IsInEditorPreview
        {
            get => _isInEditorPreview;
            set
            {
                _isInEditorPreview = value;
                _targetPortal._isInEditorPreview = value;
                if (value)
                {
                    Initialize();
                    _targetPortal.Initialize();
                }
            }
        }

        protected override void Start()
        {
            if (!Application.isPlaying) return;
            base.Start();

            _player = PortalManager.Instance.Player;
            _playerRb = _player.GetComponent<Rigidbody>();
            _surfaceColl = SurfaceColl();
        }

        void Update()
        {
            if (!Application.isPlaying) return;
            RenderCamera();

            if (Dot(_observerCam.transform) < 0 != _playerPreviousSide && _isPlayerInBound) TeleportPlayer();
            _playerPreviousSide = Dot(_observerCam.transform) < 0;

            for (int i = 0; i < _travellerObjects.Count; i++)
            {
                TravellerObject traveller = _travellerObjects[i];
                ManageColl(traveller.transform, true, false);
                Matrix4x4 matrix = _targetPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;
                if (Dot(traveller.transform) < 0 != traveller.PreviousSide)
                {
                    Vector3 oldPos = traveller.transform.position;
                    Quaternion oldRot = traveller.transform.rotation;
                    TeleportObj(matrix, traveller.transform);
                    traveller.Clone.transform.SetPositionAndRotation(oldPos, oldRot);
                    _targetPortal.OnTravellerEnterTrigger(traveller);
                    _travellerObjects.RemoveAt(i);
                    i--;
                }
                else
                {
                    traveller.Clone.transform.SetPositionAndRotation(matrix.GetColumn(3), matrix.rotation);
                    traveller.PreviousSide = Dot(traveller.transform) < 0;
                }
            }
        }

        void FixedUpdate()
        {
            ManageColl(_player.transform, _isPlayerInBound, _targetPortal._isPlayerInBound);
            CalculatePortalVelocity();
        }

        protected override void InitializeCamera()
        {
            _cam.enabled = false;
            RenderTexture renderTexture = new(_renderTextureSize.x, _renderTextureSize.y, 0);
            _targetPortal.Cam.targetTexture = renderTexture;
            _renderer.material = new Material(Resources.Load<Material>("Materials/Portal"));
            _renderer.material.SetTexture("_MainTex", renderTexture);
            _renderer.material.SetFloat("_IsMirror", 0);
        }

        public Collider SurfaceColl()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitForward, 1)) return hitForward.collider;
            else if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hitBackward, 1)) return hitBackward.collider;
            return null;
        }

        protected override void RenderCamera()
        {
            if (_renderWhen == RenderWhen.IsInView)
            {
                if (!_targetPortal.IsInView()) return;
            }
            else if (_renderWhen == RenderWhen.TargetIsInView)
            {
                if (!IsInView() && !_targetPortal.IsInView()) return;
            }

            Matrix4x4 playerMatrix = _observerCam.transform.localToWorldMatrix;
            Matrix4x4[] matrices = new Matrix4x4[_maxRecursion];
            for (int i = 0; i < _maxRecursion; i++)
            {
                playerMatrix = transform.localToWorldMatrix * _targetPortal.transform.worldToLocalMatrix * playerMatrix;
                matrices[i] = playerMatrix;
            }

            for (int i = _maxRecursion - 1; i >= 0; i--)
            {
                _renderer.enabled = false;
                _cam.transform.SetPositionAndRotation(matrices[i].GetColumn(3), matrices[i].rotation);
                SetProjectionMatrix();
                _cam.Render();
                _renderer.enabled = true;
            }
        }

        void TeleportPlayer()
        {
            Matrix4x4 matrix = _targetPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * _observerCam.transform.localToWorldMatrix;
            _player.Teleport(matrix.GetColumn(3), matrix.rotation.eulerAngles);

            Vector3 combinedVelocity = _playerRb.velocity * _playerVelocityMultiplier - _portalVelocity * _portalVelocityMultiplier;
            Vector3 outVelocity = _targetPortal.transform.TransformDirection(transform.InverseTransformDirection(combinedVelocity));
            _player.Velocity = outVelocity;
        }

        void TeleportObj(Matrix4x4 matrix, Transform obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            obj.SetPositionAndRotation(matrix.GetColumn(3), matrix.rotation);

            Vector3 combinedVelocity = rb.velocity * _objectVelocityMultiplier - _portalVelocity * _portalVelocityMultiplier;
            Vector3 outVelocity = _targetPortal.transform.TransformDirection(transform.InverseTransformDirection(combinedVelocity));
            rb.velocity = outVelocity;
        }

        void CalculatePortalVelocity()
        {
            _portalVelocity = (transform.position - _lastPos) / Time.deltaTime;
            _lastPos = transform.position;
        }

        void ManageColl(Transform objectToIgnore, bool isInBound, bool isInTargetBound)
        {
            if (_surfaceColl)
            {
                Collider coll = objectToIgnore.GetComponent<Collider>();
                if (_surfaceColl == _targetPortal.SurfaceColl())
                {
                    if (isInBound) Physics.IgnoreCollision(_surfaceColl, coll, true);
                    else if (!isInBound && !isInTargetBound) Physics.IgnoreCollision(_surfaceColl, coll, false);
                }
                else Physics.IgnoreCollision(_surfaceColl, coll, isInBound);
            }
        }

        void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.GetComponent<Player>()) _isPlayerInBound = true;
            else if (coll.gameObject.GetComponent<TravellerObject>())
            {
                TravellerObject TravellerObject = coll.gameObject.GetComponent<TravellerObject>();
                OnTravellerEnterTrigger(TravellerObject);
            }
        }

        void OnTravellerEnterTrigger(TravellerObject traveller)
        {
            if (!_travellerObjects.Contains(traveller))
            {
                _travellerObjects.Add(traveller);
                traveller.EnableClone();
                traveller.PreviousSide = Dot(traveller.transform) < 0;
            }
        }

        void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.GetComponent<Player>()) _isPlayerInBound = false;
            else if (coll.gameObject.GetComponent<TravellerObject>())
            {
                ManageColl(coll.transform, false, false);
                TravellerObject TravellerObject = coll.gameObject.GetComponent<TravellerObject>();
                if (!_targetPortal._travellerObjects.Contains(TravellerObject)) TravellerObject.DisableClone();
                _travellerObjects.Remove(TravellerObject);
            }
        }

        void OnValidate()
        {
            if (_targetPortal != null)
            {
                if (_targetPortal._targetPortal != this) _targetPortal._targetPortal = this;
            }
        }
    }
}