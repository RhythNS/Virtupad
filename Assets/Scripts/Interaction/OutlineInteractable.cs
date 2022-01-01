using cakeslice;
using System.Collections.Generic;
using UnityEngine;

namespace Virtupad
{
    public abstract class OutlineInteractable : Interactable
    {
        [SerializeField] private int color = 0;
        public int Color
        {
            get => color;
            set
            {
                if (color == value)
                    return;

                color = value;

                if (outlines == null)
                    return;

                for (int i = 0; i < outlines.Length; i++)
                    outlines[i].color = value;
            }
        }
        protected Outline[] outlines;

        protected virtual bool EnabledAtStart => false;

        protected virtual void Awake()
        {
            ScanForOutlines();
        }

        protected virtual void Start()
        {
            for (int i = 0; i < outlines.Length; i++)
            {
                outlines[i].enabled = EnabledAtStart;
            }
        }

        protected virtual void ScanForOutlines()
        {
            List<Outline> outlines = new List<Outline>();
            List<Renderer> renderers = new List<Renderer>();

            transform.GetComponentsInChildren(false, renderers);
            //renderers.AddRange(transform.GetComponents<Renderer>());

            for (int i = 0; i < renderers.Count; i++)
            {
                Outline outline = renderers[i].gameObject.AddComponent<Outline>();
                outline.color = color;
                outlines.Add(outline);
            }

            this.outlines = outlines.ToArray();
        }

        public virtual void ActivateOutline()
        {
            for (int i = 0; i < outlines.Length; i++)
                outlines[i].enabled = true;
        }

        public virtual void DeActivateOutline()
        {
            for (int i = 0; i < outlines.Length; i++)
                outlines[i].enabled = false;
        }

        protected override void OnBeginHover(Vector3 impactPoint)
        {
            ActivateOutline();
        }

        public override void OnBeginSelecting(Vector3 impactPoint)
        {
            ActivateOutline();
        }

        protected override void OnLeaveHover()
        {
            DeActivateOutline();
        }

        public override void OnEndSelecting()
        {
            DeActivateOutline();
        }
    }
}
