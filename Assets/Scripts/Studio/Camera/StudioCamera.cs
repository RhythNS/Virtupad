using System;
using UnityEngine;

namespace Virtupad
{
    public class StudioCamera : OutlineInteractable
    {
        [System.Serializable]
        public enum ToTrack
        {
            Head, UpperBody, Waist, RightHand, LeftHand, RightFoot, LeftFoot
        }

        public bool IsTracking => Tracking != null;
        public Transform Tracking { get; set; }
        public ToTrack TrackingBodyPart
        {
            get => trackingBodyPart;
            set
            {
                trackingBodyPart = value;
                UpdateTrackingBone();
            }
        }
        private ToTrack trackingBodyPart;
        public HumanBodyBones TrackingBone { get; private set; }
        public Transform Anchor { get; set; }
        public bool Grabbable { get; set; }
        public bool SmoothPickUp { get; set; }
        public bool Playing { get; private set; } = false;
        public CameraMover Mover { get; private set; }
        public int Id { get; set; }

        public Camera OutputCamera { get => outputCamera; private set => outputCamera = value; }
        [SerializeField] private Camera outputCamera;

        public bool IsPreviewOutputting { get; private set; }
        public Camera PreviewCamera { get => previewCamera; set => previewCamera = value; }
        [SerializeField] private Camera previewCamera;

        [SerializeField] private Transform previewOutput;

        [SerializeField] private Vector2 maxDesiredDimensions = new Vector2(1.0f, 0.8f);

        public RenderTexture PreviewTexture { get; private set; }

        protected override void Start()
        {
            base.Start();

            previewCamera.enabled = false;
            OnDeActive();

            StudioCameraManager.Instance.Register(this, out Vector2 resolution);
            ChangePreviewResolution(resolution);
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
            ReleasePreviewTexture();

            if (StudioCameraManager.Instance != null)
                StudioCameraManager.Instance.DeRegister(this);
        }

        private void UpdateTrackingBone()
        {
            TrackingBone = TrackingBodyPart switch
            {
                ToTrack.Head => HumanBodyBones.Head,
                ToTrack.UpperBody => HumanBodyBones.Chest,
                ToTrack.Waist => HumanBodyBones.Hips,
                ToTrack.RightHand => HumanBodyBones.RightHand,
                ToTrack.LeftHand => HumanBodyBones.LeftHand,
                ToTrack.RightFoot => HumanBodyBones.RightFoot,
                ToTrack.LeftFoot => HumanBodyBones.LeftFoot,
                _ => HumanBodyBones.Head,
            };
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
            IsPreviewOutputting = true;
            previewOutput.gameObject.SetActive(true);
        }

        public void DeactivatePreview()
        {
            IsPreviewOutputting = false;
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
