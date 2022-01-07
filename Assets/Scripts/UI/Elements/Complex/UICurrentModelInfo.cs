using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRM;

namespace Virtupad
{
    public class UICurrentModelInfo : UIPrimitiveElement
    {
        [SerializeField] private UIElementSwitcher switcher;

        [SerializeField] private RawImage modelPreview;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text authorName;

        [SerializeField] private Transform fullBodyTransPanel;

        public override void OnInit()
        {
            base.OnInit();
            VRMController controller = VRMController.Instance;

            if (controller == null)
            {
                switcher.SwitchChild(0);
                return;
            }

            switcher.SwitchChild(1);

            VRMMeta meta = controller.GetComponent<VRMMeta>();
            if (meta == null)
            {
                modelPreview.texture = null;
                nameText.text = "";
                authorName.text = "";
            }
            else
            {
                modelPreview.texture = meta.Meta.Thumbnail;
                nameText.text = meta.Meta.Title;
                authorName.text = meta.Meta.Author;
            }

            fullBodyTransPanel.gameObject.SetActive(VRSetTracker.HasTrackers() == true);
        }

        public void UnloadModel()
        {

        }

        public void ResetModel()
        {

        }

        public void ResetFullyBodyTracking()
        {

        }
    }
}
