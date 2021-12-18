using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using static Virtupad.LastLoadedVRMs;

namespace Virtupad
{
    public class UILoadVRMElement : UIPrimitiveElement
    {
        [SerializeField] private Image previewImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private VRMHumanoidDescription vrmPrefabOverride;

        private ExtendedCoroutine waitForImageCoroutine;

        public LoadVRM LoadVRM
        {
            get => loadVRM;
            set
            {
                loadVRM = value;
                if (loadVRM == null)
                    return;

                nameText.text = loadVRM.Name;
                if (loadVRM.previewTexture != null)
                    SetPreviewImage();
                else
                    StartWaitUntilImagePreview();
            }
        }
        private LoadVRM loadVRM;

        public void OnClicked()
        {
            if (vrmPrefabOverride != null)
            {
                UIVRMSelector.Instance.LoadVRMFromPrefab(vrmPrefabOverride);
                return;
            }

            UIVRMSelector.Instance.LoadVRMFromFilePath(loadVRM);
        }

        private void StartWaitUntilImagePreview()
        {
            if (waitForImageCoroutine != null && waitForImageCoroutine.IsFinshed == false)
                return;

            waitForImageCoroutine = new ExtendedCoroutine(this, WaitUntilImagePreview(), startNow: true);
        }

        private IEnumerator WaitUntilImagePreview()
        {
            Coroutine loadingAnimation = StartCoroutine(DoLoadingAnimation());
            while (true)
            {
                yield return null;

                if (loadVRM.previewTexture == null)
                    continue;

                StopCoroutine(loadingAnimation);
                SetPreviewImage();
                yield break;
            }
        }

        private IEnumerator DoLoadingAnimation()
        {
            Sprite[] loadingSpriteAnimation = UIVRMSelector.Instance.LoadingSpriteAnimation;
            int at = 0;
            float waitTime = UIVRMSelector.Instance.LoadingAnimationSecondsPerFrame;

            while (true)
            {
                if (++at >= loadingSpriteAnimation.Length)
                    at = 0;

                previewImage.sprite = loadingSpriteAnimation[at];
                yield return new WaitForSeconds(waitTime);
            }
        }

        private void SetPreviewImage()
        {
            previewImage.sprite = Sprite.Create(loadVRM.previewTexture,
                new Rect(0.0f, 0.0f, loadVRM.previewWidth, loadVRM.previewHeight), Vector2.zero);
        }
    }
}
