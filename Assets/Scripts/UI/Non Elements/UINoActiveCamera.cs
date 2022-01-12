using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UINoActiveCamera : MonoBehaviour
    {
        public static UINoActiveCamera Instance { get; private set; }
        public Texture2D CustomTexture { get; private set; }

        [SerializeField] private RawImage image;
        [SerializeField] private TMP_Text text;

        public bool OverwriteNoOutput
        {
            get => overwriteNoOutput;
            set
            {
                overwriteNoOutput = value;
                OnCameraChanged(StudioCameraManager.Instance.ActiveCamera);
            }
        }
        private bool overwriteNoOutput = false;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("UINoActiveCamera already in scene. Deleting myself!");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            StudioCameraManager.Instance.OnActiveStudioCameraChanged += OnCameraChanged;
            OnCameraChanged(StudioCameraManager.Instance.ActiveCamera);

            SetCustomImage(SaveFileManager.Instance.saveGame.customNoCameraActivePath);
        }

        public void SetAndTransferOwnership(Texture2D texture)
        {
            DisposeTexture();

            text.gameObject.SetActive(false);

            CustomTexture = texture;
            image.color = Color.white;
            image.texture = texture;
        }

        public bool SetCustomImage(string path)
        {
            if (File.Exists(path) == false)
                return false;

            Texture2D customTex = new Texture2D(2, 2);
            byte[] fileData;

            bool success = false;

            try
            {
                fileData = File.ReadAllBytes(path);
                success = customTex.LoadImage(fileData);
            }
            catch (System.Exception)
            {
            }

            if (success == false)
            {
                Destroy(customTex);
                return false;
            }

            SetAndTransferOwnership(customTex);
            return true;
        }

        public void ResetCustomTexture()
        {
            DisposeTexture();

            text.gameObject.SetActive(true);
            image.color = Color.black;
        }

        private void OnCameraChanged(StudioCamera newCamera)
        {
            gameObject.SetActive(newCamera == null || OverwriteNoOutput == true);
        }

        private void DisposeTexture()
        {
            if (CustomTexture != null)
                Destroy(CustomTexture);

            CustomTexture = null;
            image.texture = null;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (StudioCameraManager.Instance)
                StudioCameraManager.Instance.OnActiveStudioCameraChanged -= OnCameraChanged;
        }
    }
}
