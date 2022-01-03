using UnityEngine;

namespace Virtupad
{
    public abstract class FloatAs : MonoBehaviour
    {
        [System.Serializable]
        public enum ProcessValue
        {
            None, Round, Floor
        }

        [SerializeField] protected ProcessValue processValue;

        public void UpdateValue(float value)
        {
            switch (processValue)
            {
                case ProcessValue.Round:
                    value = Mathf.Round(value);
                    break;
                case ProcessValue.Floor:
                    value = Mathf.Floor(value);
                    break;
            }

            OnUpdateValue(value);
        }

        protected abstract void OnUpdateValue(float value);
    }
}
