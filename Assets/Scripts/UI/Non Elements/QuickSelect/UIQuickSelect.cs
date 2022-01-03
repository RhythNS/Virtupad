using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Virtupad
{
    public class UIQuickSelect : MonoBehaviour
    {
        public List<string> selections = new List<string>();

        public UnityEvent<int> previewChanged;
        public UnityEvent<int> selectionChanged;
        public event GetInt getCurrentSelection;

        [SerializeField] SteamVR_Action_Boolean quickSelectButton;
        [SerializeField] SteamVR_Input_Sources listenForSource;
        [SerializeField] private Transform hand;

        [SerializeField] private TMP_Text textDisplay;
        [SerializeField] private int index;

        [SerializeField] private Transform quickSelectPanel;
        [SerializeField] private bool fixedElements;
        [SerializeField] private UIQuickSelectElement elementPrefab;

        [SerializeField] private Vector3 closingScale;
        private Vector3 normalScale;
        [SerializeField] private float animationDuration;

        [SerializeField] private float minimalDistanceForSelectionSquared = 0.0625f;

        public Color SelectColor => selectColor;
        [SerializeField] private Color selectColor;
        public Color DeselectColor => deselectColor;
        [SerializeField] private Color deselectColor;

        public Color DefaultSelectColor => defaultSelectColor;
        [SerializeField] private Color defaultSelectColor;
        public Color DefaultDeselectColor => defaultDeselectColor;
        [SerializeField] private Color defaultDeselectColor;

        private List<UIQuickSelectElement> elements = new List<UIQuickSelectElement>();

        private Vector3 planeNormal;
        private Vector3 planePosition;

        private ExtendedCoroutine hideOrShowingCoroutine;
        private ExtendedCoroutine selectionCoroutine;

        private void Start()
        {
            normalScale = transform.localScale;
            gameObject.SetActive(false);

            quickSelectButton.AddOnChangeListener(ActiveChanged, listenForSource);
        }

        private void ActiveChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool active)
        {
            if (active)
                Init();
            else
                Stop();
        }

        private void OnUpdateValue()
        {
            previewChanged?.Invoke(index);
            textDisplay.text = selections[index];
        }

        public void Init()
        {
            if (getCurrentSelection != null)
                this.index = getCurrentSelection.Invoke();
            planePosition = hand.position;

            if (fixedElements == true)
                InitFixedElements();
            else
                InitElements();

            selectionCoroutine = new ExtendedCoroutine(this, DoSelection());

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, normalScale, CurveDict.Instance.UIInAnimation, animationDuration)
                );
        }

        private void InitElements()
        {
            gameObject.SetActive(true);

            int childCount = quickSelectPanel.childCount;
            if (childCount < selections.Count)
            {
                for (int i = childCount; i < selections.Count; i++)
                {
                    Instantiate(elementPrefab, quickSelectPanel);
                }
            }
            else if (childCount > selections.Count)
            {
                for (int i = childCount - 1; i >= selections.Count; i--)
                {
                    Transform child = quickSelectPanel.GetChild(i);
                    child.parent = null;
                    Destroy(child.gameObject);
                }
            }

            elements.Clear();

            float singlePercentile = 1.0f / selections.Count;
            for (int i = 0; i < quickSelectPanel.childCount; i++)
            {
                UIQuickSelectElement element = quickSelectPanel.GetChild(i).GetComponent<UIQuickSelectElement>();
                element.Init(this, i == index, i, singlePercentile);
                elements.Add(element);
            }
        }

        private void InitFixedElements()
        {
            elements.Clear();

            for (int i = 0; i < quickSelectPanel.childCount; i++)
            {
                UIQuickSelectElement element = quickSelectPanel.GetChild(i).GetComponent<UIQuickSelectElement>();
                element.FixedInit(this, i == index);
                elements.Add(element);
            }
        }

        private IEnumerator DoSelection()
        {
            while (true)
            {
                yield return null;

                Transform headTrans = Player.instance.hmdTransform;
                planeNormal = (headTrans.position - hand.position).normalized;
                transform.rotation = Quaternion.LookRotation(planeNormal);

                Vector3 currentPos = Vector3.ProjectOnPlane(hand.position, planeNormal);
                Debug.DrawLine(currentPos + planeNormal * 0.5f, currentPos - planeNormal * 0.5f);

                if ((currentPos - planePosition).sqrMagnitude < minimalDistanceForSelectionSquared)
                    continue;

                float rot = Vector3.Angle(planePosition, currentPos);
                int newIndex = Mathf.FloorToInt((rot / 360.0f) * selections.Count);

                if (index == newIndex)
                    continue;

                elements[index].Deselect();
                elements[newIndex].Select();

                index = newIndex;
                OnUpdateValue();
            }
        }

        public void Stop(bool invokeChange = true)
        {
            if (hideOrShowingCoroutine.IsFinshed == false)
                hideOrShowingCoroutine.Stop();
            if (selectionCoroutine.IsFinshed == false)
                selectionCoroutine.Stop();

            if (invokeChange == true)
                selectionChanged?.Invoke(index);

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, closingScale, CurveDict.Instance.UIOutAnimation, animationDuration),
                OnHidden,
                true
                );
        }

        private void OnHidden()
        {
            gameObject.SetActive(false);
        }
    }
}
