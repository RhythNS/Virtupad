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

        [SerializeField] private GameObject listenerObject;
        private IUIQuickSelectListener listener;

        [SerializeField] SteamVR_Action_Boolean quickSelectButton;
        [SerializeField] SteamVR_Input_Sources listenForSource;
        [SerializeField] private Transform hand;

        [SerializeField] private TMP_Text textDisplay;
        [SerializeField] private int index;
        private int defaultIndex;

        [SerializeField] private Transform quickSelectPanel;
        [SerializeField] private bool fixedElements;
        [SerializeField] private UIQuickSelectElement elementPrefab;

        [SerializeField] private Vector3 closingScale;
        private Vector3 normalScale;
        [SerializeField] private float animationDuration;

        [SerializeField] private float minimalDistanceForSelectionSizePercentage = 0.25f;
        private float minimalDistanceForSelectionSquared = 0.0625f;

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
        private Vector3 planeUp;
        private Vector3 planePosition;

        private ExtendedCoroutine hideOrShowingCoroutine;
        private ExtendedCoroutine selectionCoroutine;

        private void Start()
        {
            if (listenerObject.TryGetComponent(out listener) == false && TryGetComponent(out listener) == false)
                Debug.LogError(gameObject.name + " does not have a listener!");

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
            if (selections.Count == 0)
            {
                textDisplay.text = "";
                return;
            }

            listener.OnPreviewChanged(index);
            textDisplay.text = selections[index];
        }

        public void Init()
        {
            listener.OnStart();
            index = listener.GetCurrentSelection();
            defaultIndex = index;

            planePosition = hand.position;

            Transform headTrans = Player.instance.hmdTransform;
            planeNormal = (headTrans.position - hand.position).normalized;
            planeUp = Quaternion.LookRotation(planeNormal) * Quaternion.Euler(-90.0f, 0.0f, 0.0f) * new Vector3(0.0f, 0.0f, 1.0f);

            if (fixedElements == true)
                InitFixedElements();
            else
            {
                selections = listener.GetSelections();
                InitElements();
            }
            OnUpdateValue();

            selectionCoroutine = new ExtendedCoroutine(this, DoSelection());

            if (hideOrShowingCoroutine != null && hideOrShowingCoroutine.IsFinshed == false)
                hideOrShowingCoroutine.Stop(false);

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, normalScale, CurveDict.Instance.UIInAnimation, animationDuration),
                    OnActive
                );
        }

        private void OnActive()
        {
            minimalDistanceForSelectionSquared = Mathf.Pow(
                (transform as RectTransform).sizeDelta.x * 0.5f * minimalDistanceForSelectionSizePercentage * transform.lossyScale.x, 2.0f);
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

                transform.position = planePosition;
                transform.rotation = Quaternion.LookRotation(-planeNormal);

                Plane plane = new Plane(planeNormal, planePosition);

                Vector3 currentPos = plane.ClosestPointOnPlane(hand.position);

                int newIndex;
                if ((currentPos - planePosition).sqrMagnitude < minimalDistanceForSelectionSquared)
                    newIndex = defaultIndex;
                else
                {
                    Vector3 toCurrentPosDir = (currentPos - planePosition).normalized;

                    float rot = Vector3.SignedAngle(planeUp, toCurrentPosDir, planeNormal);
                    if (rot < 0f)
                        rot = 360.0f + rot;

                    newIndex = Mathf.FloorToInt((rot / 360.0f) * selections.Count);
                }

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
                hideOrShowingCoroutine.Stop(false);
            if (selectionCoroutine != null && selectionCoroutine.IsFinshed == false)
                selectionCoroutine.Stop(false);

            if (invokeChange == true)
                listener.OnSelectionChanged(index);

            listener.OnStop();

            hideOrShowingCoroutine = new ExtendedCoroutine(this,
                EnumeratorUtil.ScaleInSecondsCurve
                    (transform, closingScale, CurveDict.Instance.UIInAnimation, animationDuration),
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
