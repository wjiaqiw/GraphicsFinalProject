using UnityEngine;
using UnityEditor;

namespace DynamicPortals
{
    public class Mirror : PortalBase
    {
        protected override void Start()
        {
            if (!Application.isPlaying) return;
            base.Start();
        }

        void Update()
        {
            if (!Application.isPlaying) return;
            RenderCamera();
        }

        protected override void InitializeCamera()
        {
            _cam.enabled = false;
            RenderTexture renderTexture = new(_renderTextureSize.x, _renderTextureSize.y, 0);
            _cam.targetTexture = renderTexture;
            _renderer.sharedMaterial.SetTexture("_MainTex", renderTexture);
            _renderer.sharedMaterial.SetFloat("_IsMirror", 1);
        }

        protected override void RenderCamera()
        {
            if (IsInView())
            {
                _renderer.gameObject.SetActive(false);

                _cam.transform.SetPositionAndRotation(_observerCam.transform.position, _observerCam.transform.rotation);
                _cam.transform.RotateAround(transform.position, transform.up, 180);
                _cam.transform.SetLocalPositionAndRotation(new(-_cam.transform.localPosition.x, _cam.transform.localPosition.y, _cam.transform.localPosition.z),
                    Quaternion.Euler(_cam.transform.localEulerAngles.x, -_cam.transform.localEulerAngles.y, -_cam.transform.localEulerAngles.z));
                SetProjectionMatrix();
                _cam.Render();

                _renderer.gameObject.SetActive(true);
            }
        }
    }
}