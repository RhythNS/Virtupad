using TMPro;
using UnityEngine;

namespace Virtupad
{
    public class FlotAsText : FloatAs
    {
        [SerializeField] private TMP_Text onText;

        protected override void OnUpdateValue(float value)
        {
            onText.text = value.ToString();
        }
    }
}
