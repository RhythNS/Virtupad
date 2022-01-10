using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UICameraSelection : UIPrimitiveElement
    {
        public StudioCamera StudioCamera => studioCamera;
        [SerializeField] private StudioCamera studioCamera;

        [SerializeField] private TMP_Text cameraNameDisplay;

        [SerializeField] private RawImage previewImage;

        public void Init(StudioCamera onCamera)
        {
            if (onCamera != null)
                Desubscribe();

            studioCamera = onCamera;
            cameraNameDisplay.text = (onCamera.Id + 1).ToString();

            previewImage.texture = onCamera.PreviewTexture;

            onCamera.OnRenderTextureChanged += OnRenderTextureChanged;
        }

        private void OnRenderTextureChanged(RenderTexture newTexture)
        {
            previewImage.texture = newTexture;
        }

        private void OnDisable()
        {
            Desubscribe();
        }

        private void Desubscribe()
        {
            if (studioCamera == null)
                return;

            studioCamera.OnRenderTextureChanged -= OnRenderTextureChanged;
            studioCamera = null;
        }

        public void OnClick()
        {
            UICameraPanel.Instance.OnChangeCamera(studioCamera);
        }
    }
}
