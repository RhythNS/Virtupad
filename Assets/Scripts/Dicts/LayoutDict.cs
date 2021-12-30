using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRKeys;

namespace Virtupad
{
    public class LayoutDict : MonoBehaviour
    {
        [System.Serializable]
        public struct LayoutElement
        {
            public Layout layout;
            public string locale;
        }

        public static LayoutDict Instance { get; private set; }

        [SerializeField] List<LayoutElement> layouts = new List<LayoutElement>();

        public Layout GetLayout(string locale) => layouts.Find(x => x.locale == locale).layout;

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("LayoutDict already in scene. Deleting myself!");
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
