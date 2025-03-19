using UnityEngine;
using UnityEditor;

namespace DynamicPortals
{
    [ExecuteInEditMode]
    public class PortalBase : MonoBehaviour
    {
        [SerializeField] protected Vector2Int _renderTextureSize = new(1920, 1080);
        [SerializeField] protected float _clipPlaneOffset = 0.25f;
        [SerializeField] protected bool _changeObserverCam;
        [SerializeField] protected Camera _alternativeCam;
        [SerializeField] protected bool _showAdvancedSettings;
        protected Camera _observerCam;
        protected Renderer _renderer;

        [SerializeField] protected bool _isInEditorPreview;
        public virtual bool IsInEditorPreview
        {
            get => _isInEditorPreview;
            set
            {
                _isInEditorPreview = value;
                if (value) Initialize();
            }
        }

        protected Camera _cam;
        public Camera Cam
        {
            get
            {
                if (_cam == null) _cam = GetComponentInChildren<Camera>();
                return _cam;
            }
        }

        protected virtual void RenderCamera() { }
        protected virtual void InitializeCamera() { }
        protected virtual void Start() => Initialize();

        protected void Initialize()
        {
            _cam = GetComponentInChildren<Camera>();
            _renderer = transform.Find("Renderer").GetComponent<Renderer>();
            GetObserverCam();
            InitializeCamera();
        }

        protected virtual void GetObserverCam()
        {
            if (_changeObserverCam) _observerCam = _alternativeCam;
            else if (_isInEditorPreview) _observerCam = SceneView.lastActiveSceneView.camera;
            else _observerCam = PortalManager.Instance.Player.Camera;
        }

        void OnRenderObject()
        {
            if (!Application.isPlaying && _isInEditorPreview) RenderCamera();
        }

        protected float Dot(Transform obj)
        {
            return Vector3.Dot(transform.forward, obj.position - transform.position);
        }

        protected void SetProjectionMatrix()
        {
            Vector3 camSpaceNormal = _cam.worldToCameraMatrix.MultiplyVector(transform.forward) * -System.Math.Sign(Dot(_cam.transform));
            float camDistance = Mathf.Abs(Dot(_cam.transform));
            float w = camDistance <= _clipPlaneOffset ? -_cam.nearClipPlane : -camDistance + _clipPlaneOffset;
            Vector4 clipPlaneMatrix = new(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, w);
            _cam.projectionMatrix = _observerCam.CalculateObliqueMatrix(clipPlaneMatrix);
        }

        protected bool IsInView()
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_observerCam);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, _renderer.bounds);
        }
    }
}
