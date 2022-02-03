using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Virtupad
{
    public class UIFullBodyTrackerDisplay : MonoBehaviour
    {
        [SerializeField] private Color okayColor;
        [SerializeField] private Color errorColor;

        [SerializeField] private RectTransform okayPanel;
        [SerializeField] private RectTransform errorPanel;

        [SerializeField] private Image avatar;

        [SerializeField] private Image hip;
        [SerializeField] private Image leftFoot;
        [SerializeField] private Image rightFoot;

        public void Init(RigMaker.Config? config)
        {
            if (config == null)
            {
                ComponentObjectUtil.SetGameObjectActive(new Component[] { errorPanel, avatar }, true);
                ComponentObjectUtil.SetGameObjectActive(new Component[] { okayPanel, hip, leftFoot, rightFoot }, false);
                avatar.color = errorColor;
                return;
            }

            avatar.color = okayColor;
            List<Component> toEnable = new List<Component>()
            {
                okayPanel, avatar
            };

            switch (config.Value)
            {
                case RigMaker.Config.ThreePoints:
                    // should not be possible?
                    break;
                case RigMaker.Config.FourPoints:
                    toEnable.Add(hip);
                    break;
                case RigMaker.Config.FivePoints:
                    toEnable.Add(leftFoot);
                    toEnable.Add(rightFoot);
                    break;
                case RigMaker.Config.SixPoints:
                    toEnable.Add(hip);
                    toEnable.Add(leftFoot);
                    toEnable.Add(rightFoot);
                    break;
            }

            ComponentObjectUtil.SetGameObjectActive(toEnable.ToArray(), true);
            errorPanel.gameObject.SetActive(false);
        }
    }
}
