using System;
using UnityEngine;

namespace Virtupad
{
    public class StudioCamera : Interactable
    {
        [System.Serializable]
        public enum ToTrack
        {
            Head, UpperBody, Waist, RightHand, LeftHand, RightFoot, LeftFoot
        }

        public enum CameraType
        {
            StudioCamera, Phone
        }

        public bool Tracking { get; private set; }
        public bool SmoothPostition { get; private set; }
        public ToTrack TrackingBodyPart { get; private set; }
        public HumanBodyBones TrackingBone { get; private set; }

        public bool SmoothRotation { get; private set; }
        public float StayInAngle { get; private set; }
        public bool AutoFollow { get; private set; }
        public CameraMover.TrackingSpace TrackingSpace { get; private set; }

        public bool Playing { get; private set; } = false;
        public CameraMover Mover { get; private set; }
        public int Id { get; set; }
        public AttachGrabbable Grabbable { get; private set; }

        public int CurrentCameraIndex
        {
            get => currentCameraIndex;
            set
            {
                value = Mathf.Clamp(value, 0, outputCameras.Length - 1);

                if (currentCameraIndex == value)
                    return;

                if (outputCameras[currentCameraIndex].enabled == true)
                {
                    outputCameras[currentCameraIndex].enabled = false;
                    outputCameras[value].enabled = true;
                }
                if (IsPreviewOutputting == true)
                {
                    previewCameras[currentCameraIndex].enabled = false;
                    previewCameras[value].enabled = true;
                }

                currentCameraIndex = value;
            }
        }
        [SerializeField] private int currentCameraIndex;
        public Camera OutputCamera => outputCameras[currentCameraIndex];
        [SerializeField] private Camera[] outputCameras;

        public bool IsPreviewOutputting { get; private set; }
        public Camera PreviewCamera => previewCameras[currentCameraIndex];
        [SerializeField] private Camera[] previewCameras;

        [SerializeField] private Transform previewOutput;
        [SerializeField] private Material previewMaterialPrefab;
        private Material[] previewMaterials;

        [SerializeField] private bool previewCanResize = true;
        [SerializeField] private Vector2 maxDesiredDimensions = new Vector2(1.0f, 0.8f);

        public CameraType PrefabType => prefabType;
        [SerializeField] private CameraType prefabType;

        public Rigidbody Body { get; private set; }

        public RenderTexture PreviewTexture => PreviewTextures[currentCameraIndex];
        public RenderTexture[] PreviewTextures { get; private set; }

        public event OnRenderTextureChanged OnRenderTextureChanged;

        protected virtual void Awake()
        {
            SnapToObject = true;

            Grabbable = GetComponent<AttachGrabbable>();
            Body = GetComponent<Rigidbody>();

            Array.ForEach(previewCameras, x => x.enabled = false);
            Array.ForEach(outputCameras, x => x.enabled = false);
            OnDeActive();

            PreviewTextures = new RenderTexture[previewCameras.Length];
            previewMaterials = new Material[outputCameras.Length];
            for (int i = 0; i < previewMaterials.Length; i++)
                previewMaterials[i] = new Material(previewMaterialPrefab);

            previewOutput.GetComponent<MeshRenderer>().material = previewMaterials[currentCameraIndex];
            previewOutput.gameObject.SetActive(false);

            ChangePreviewResolution(StudioCameraManager.Instance.DesiredResolution);

            Mover = GetComponent<CameraMover>();
            if (Mover == null)
                SetCameraMover(StudioCameraManager.Instance.DefaultMover);

            StudioCameraManager.Instance.Register(this);
        }

        private void ReleasePreviewTextures()
        {
            for (int i = 0; i < previewMaterials.Length; i++)
            {
                if (PreviewTextures[i] == null)
                    continue;

                previewMaterials[i].mainTexture = null;
                previewCameras[i].targetTexture = null;
                PreviewTextures[i].Release();
                PreviewTextures[i] = null;
            }
        }

        protected virtual void OnDestroy()
        {
            for (int i = 0; i < previewMaterials.Length; i++)
            {
                Destroy(previewMaterials[i]);
            }

            ReleasePreviewTextures();

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
            ReleasePreviewTextures();

            Vector2 outputSize = baseRes * 0.01f;

            NormalizeScale(ref outputSize);

            Vector2Int outputSizeInt = Vector2Int.RoundToInt(outputSize);
            Vector2Int baseResInt = Vector2Int.RoundToInt(baseRes);

            if (previewCanResize == true)
                previewOutput.localScale = new Vector3(outputSizeInt.x, 0.00001f, outputSizeInt.y);

            for (int i = 0; i < outputCameras.Length; i++)
            {
                PreviewTextures[i] = new RenderTexture(baseResInt.x, baseResInt.y, 16, RenderTextureFormat.ARGB32);
                previewCameras[i].targetTexture = PreviewTextures[i];

                previewMaterials[i].mainTexture = PreviewTextures[i];

            }
            OnRenderTextureChanged?.Invoke(PreviewTextures[currentCameraIndex]);
        }

        public void ActivatePreview()
        {
            IsPreviewOutputting = true;
            PreviewCamera.enabled = true;
            previewOutput.gameObject.SetActive(true);
        }

        public void DeactivatePreview()
        {
            IsPreviewOutputting = false;
            PreviewCamera.enabled = false;
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

            if (Mover)
            {
                Mover.OnRemove();
                Mover.enabled = false;
            }

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

        public void ChangeType(CameraType cameraType)
        {
            // TODO:
        }

        public void SetToTrack(bool shouldTrack)
        {
            Tracking = shouldTrack;
            // TODO:
        }

        public void SetTrackingBodyPart(ToTrack toTrack)
        {
            TrackingBodyPart = toTrack;
            UpdateTrackingBone();
        }

        public void SetAutoFollow(bool newValue)
        {
            AutoFollow = newValue;

            if (newValue && Mover != null)
                Mover.OnFollowTypeChanged();
        }

        public void SetFollowType(CameraMover.TrackingSpace trackingSpace)
        {
            TrackingSpace = trackingSpace;

            if (Mover != null)
                Mover.OnFollowTypeChanged();
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
            UISingleCameraSettingsPanel.Instance.Open(this);
        }
    }
}
