using System;
using UnityEngine;

namespace Virtupad
{
    public class StudioCamera : OutlineInteractable
    {
        public Transform Tracking { get; set; }
        public HumanBodyBones TrackingBone { get; set; }
        public Transform Anchor { get; set; }
        public bool Grabbable { get; set; }
        public bool NonXRotatable { get; set; }
        public bool Playing { get; private set; } = false;
        public CameraMover Mover { get; private set; }

        public Camera OutputCamera { get => outputCamera; private set => outputCamera = value; }
        [SerializeField] private Camera outputCamera;

        public bool IsPreviewOutputting => previewCamera.isActiveAndEnabled;
        public Camera PreviewCamera { get => previewCamera; set => previewCamera = value; }
        [SerializeField] private Camera previewCamera;

        [SerializeField] private Transform previewOutput;

        [SerializeField] private Vector2 maxDesiredDimensions = new Vector2(1.0f, 0.8f);

        public RenderTexture PreviewTexture { get; private set; }

        protected override void Start()
        {
            base.Start();

            OnDeActive();

            ChangePreviewResolution(StudioCameraManager.Instance.RegisterAndGetPreviewResolution(this));
            StudioCameraManager.Instance.ActiveCamera = this;
        }

        private void ReleasePreviewTexture()
        {
            if (PreviewTexture == null)
                return;

            previewCamera.targetTexture = null;
            PreviewTexture.Release();
            PreviewTexture = null;
        }

        protected virtual void OnDestroy()
        {
            if (StudioCameraManager.Instance != null)
                StudioCameraManager.Instance.DeRegister(this);
        }

        public void ChangePreviewResolution(Vector2 baseRes)
        {
            ReleasePreviewTexture();

            baseRes *= 0.01f;

            NormalizeScale(ref baseRes);

            Vector2Int baseResInt = Vector2Int.RoundToInt(baseRes);

            previewOutput.localScale = new Vector3(baseResInt.x, 0.00001f, baseResInt.y);

            PreviewTexture = new RenderTexture(baseResInt.x, baseResInt.y, 16, RenderTextureFormat.ARGB32);
            previewCamera.targetTexture = PreviewTexture;
        }

        public void ActivatePreview()
        {
            previewCamera.enabled = true;
            previewOutput.gameObject.SetActive(true);
        }

        public void DeactivatePreview()
        {
            previewCamera.enabled = false;
            previewOutput.gameObject.SetActive(false);
        }

        private void NormalizeScale(ref Vector2 baseScale)
        {
            float xScale = maxDesiredDimensions.x / baseScale.x;
            if (baseScale.y * xScale < maxDesiredDimensions.y)
            {
                baseScale *= xScale;
                return;
            }

            float yScale = maxDesiredDimensions.y / baseScale.y;
            baseScale *= yScale;
        }

        public void SetCameraMover(CameraMover.Type type, bool autoPlay = true)
        {
            Type moverSystemType = CameraMover.GetSystemTypeForType(type);
            if (moverSystemType == null)
            {
                Debug.LogWarning("Could not get mover type for " + type + "!");
                return;
            }

            Component moverComp = GetComponent(moverSystemType);

            CameraMover newMover = moverComp == null ?
                gameObject.AddComponent(moverSystemType) as CameraMover :
                moverComp as CameraMover;

            if (newMover == Mover)
                return;

            Mover.enabled = false;

            newMover.Init(this);
            newMover.enabled = true;

            if (autoPlay == false)
            {
                Playing = false;
            }
            else
            {
                Playing = true;
                newMover.Play();
            }

            Mover = newMover;
        }

        public void OnActive()
        {
            OutputCamera.enabled = true;
        }

        public void OnDeActive()
        {
            OutputCamera.enabled = false;
        }

        public void SetDefaultValues()
        {
            TrackingBone = HumanBodyBones.Head;
            Anchor = null;
            Grabbable = false;
        }

        public void Play()
        {
            if (Mover == null || Playing == true)
                return;

            Playing = true;
            Mover.Play();
        }

        public void Stop()
        {
            if (Mover == null || Playing == false)
                return;

            Playing = false;
            Mover.Stop();
        }

        public void Restart()
        {
            if (Mover == null)
                return;

            Playing = true;
            Mover.Restart();
            Mover.Play();
        }

        public override void Select()
        {

        }
    }
}
