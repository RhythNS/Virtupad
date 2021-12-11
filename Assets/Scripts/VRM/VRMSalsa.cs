using CrazyMinnow.SALSA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRM;
using static VRM.BlendShapeBindingMerger;

public class VRMSalsa : MonoBehaviour
{
    private struct BlendInfo
    {
        public SkinnedMeshRenderer renderer;
        public int index;

        public BlendInfo(SkinnedMeshRenderer renderer, int index)
        {
            this.renderer = renderer;
            this.index = index;
        }
    }

    private IEnumerator Start()
    {
        VRMBlendShapeProxy blendShapeProxy = GetComponent<VRMBlendShapeProxy>();

        while (blendShapeProxy.BlendShapeAvatar == null)
            yield return new WaitForSeconds(0.1f);

        Dictionary<BlendShapePreset, BlendInfo> infos = new Dictionary<BlendShapePreset, BlendInfo>();

        BlendShapePreset[] requiredBlendShapes = new BlendShapePreset[]
        {
            BlendShapePreset.E,
            BlendShapePreset.U,
            BlendShapePreset.A,
            BlendShapePreset.I,
            BlendShapePreset.O,
        };

        // ----------- Modififed Version of blendshapeproxy to get the proper blendshapes
        BlendShapeAvatar blendShapeAvatar = blendShapeProxy.BlendShapeAvatar;
        Dictionary<BlendShapeKey, BlendShapeClip> clipMap = ((IEnumerable<BlendShapeClip>)blendShapeAvatar.Clips).ToDictionary(x => BlendShapeKey.CreateFromClip(x), x => x);
        DictionaryKeyBlendShapeBindingComparer comparer = new DictionaryKeyBlendShapeBindingComparer();
        Dictionary<BlendShapeBinding, Action<float>> m_blendShapeSetterMap = new Dictionary<BlendShapeBinding, Action<float>>(comparer);

        foreach (KeyValuePair<BlendShapeKey, BlendShapeClip> kv in clipMap)
        {
            if (requiredBlendShapes.Contains(kv.Value.Preset) == false)
                continue;

            foreach (BlendShapeBinding binding in kv.Value.Values)
            {
                if (!m_blendShapeSetterMap.ContainsKey(binding))
                {
                    var _target = transform.Find(binding.RelativePath);
                    SkinnedMeshRenderer target = null;
                    if (_target != null)
                    {
                        target = _target.GetComponent<SkinnedMeshRenderer>();
                    }
                    if (target != null)
                    {
                        if (binding.Index >= 0 && binding.Index < target.sharedMesh.blendShapeCount)
                        {
                            infos.Add(kv.Value.Preset, new BlendInfo(target, binding.Index));
                        }
                        else
                        {
                            Debug.LogWarningFormat("Invalid blendshape binding: {0}: {1}", target.name, binding);
                        }
                        continue;
                    }

                    Debug.LogWarningFormat("SkinnedMeshRenderer: {0} not found", binding.RelativePath);
                }
            }
        }
        // ----------- Modififed Version of blendshapeproxy to get the proper blendshapes

        AudioSource audSrc = gameObject.GetComponent<AudioSource>();
        if (audSrc == null)
            audSrc = gameObject.AddComponent<AudioSource>();
        audSrc.playOnAwake = false;
        audSrc.loop = true;
        audSrc.clip = SalsaDict.Instance.EmptyClip;

        audSrc.Play();

        QueueProcessor qp = gameObject.GetComponent<QueueProcessor>();
        if (qp == null)
            qp = gameObject.AddComponent<QueueProcessor>();

        Salsa salsa = gameObject.GetComponent<Salsa>();
        if (salsa == null)
            salsa = gameObject.AddComponent<Salsa>();

        salsa.audioSrc = audSrc;
        salsa.queueProcessor = qp;

        // adjust salsa settings
        //  - data analysis settings
        salsa.autoAdjustAnalysis = true;
        salsa.autoAdjustMicrophone = true;
        salsa.audioUpdateDelay = 0.0875f;

        //  - advanced dynamics
        salsa.loCutoff = 0.03f;
        salsa.hiCutoff = 1.00f;
        salsa.useAdvDyn = true;
        salsa.advDynPrimaryBias = 0.50f;
        salsa.useAdvDynJitter = true;
        salsa.advDynJitterAmount = 0.10f;
        salsa.advDynJitterProb = 0.25f;
        salsa.advDynSecondaryMix = 0.0f;

        salsa.useTimingsOverride = true;
        salsa.globalDurOffBalance = -0.55f;
        salsa.globalNuanceBalance = -0.22f;

        salsa.useEasingOverride = true;
        salsa.globalEasing = LerpEasings.EasingType.CubicOut;

        // setup visemes
        salsa.visemes.Clear(); // start fresh

        for (int i = 0; i < requiredBlendShapes.Length; i++)
        {
            BlendInfo blendInfo = infos[requiredBlendShapes[i]];

            salsa.visemes.Add(new LipsyncExpression(i.ToString(), new InspectorControllerHelperData(), 0.11f * i));
            Expression visme = salsa.visemes[i].expData;

            visme.components[0].name = "Component";
            visme.controllerVars[0].smr = blendInfo.renderer;
            visme.controllerVars[0].blendIndex = blendInfo.index;
            visme.controllerVars[0].minShape = 0.0f;
            visme.controllerVars[0].maxShape = 1.0f;
        }

        SalsaMicInput input = GetComponent<SalsaMicInput>();
        if (input == null)
            input = gameObject.AddComponent<SalsaMicInput>();

        input.sampleRate = 4800;
        input.overrideSampleRate = true;
        input.linkWithSalsa = true;
        input.isAutoStart = true;

        SalsaMicPointerSync micSync = GetComponent<SalsaMicPointerSync>();
        if (micSync == null)
            micSync = gameObject.AddComponent<SalsaMicPointerSync>();

        // apply api trigger distribution...
        salsa.DistributeTriggers(LerpEasings.EasingType.SquaredIn);
        // at runtime: apply controller baking...
        salsa.UpdateExpressionControllers();
    }
}
