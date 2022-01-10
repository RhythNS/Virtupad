using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace Virtupad
{
    public class VRMEmotionManager : MonoBehaviour
    {
        private Dictionary<BlendShapePreset, BlendShapeKey> keyForBlendShape = new Dictionary<BlendShapePreset, BlendShapeKey>();
        public List<BlendShapePreset> Presets { get; private set; }
        
        public VRMBlendShapeProxy VRMBlendShapeProxy { get; private set; }

        private float lastValue = 0.0f;
        private BlendShapeKey lastKey;

        private ExtendedCoroutine settingCoroutine;

        private float changeEmoteInSeconds;

        private void Awake()
        {
            VRMBlendShapeProxy = GetComponent<VRMBlendShapeProxy>();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            changeEmoteInSeconds = VRMDict.Instance.ChangeEmoteInSeconds;

            Presets = new List<BlendShapePreset>(VRMDict.Instance.DefaultsPresets);

            foreach (KeyValuePair<BlendShapeKey, float> keys in VRMBlendShapeProxy.GetValues())
            {
                int index = Presets.FindIndex(x => x == keys.Key.Preset);
                if (index == -1)
                    continue;

                keyForBlendShape.Add(Presets[index], keys.Key);
            }
        }

        public BlendShapeKey GetLastKey()
        {
            return lastKey;
        }

        public BlendShapeKey? AddBlendShapePreset(BlendShapePreset preset)
        {
            foreach (KeyValuePair<BlendShapeKey, float> keys in VRMBlendShapeProxy.GetValues())
            {
                if (keys.Key.Preset != preset)
                    continue;

                Presets.Add(preset);
                keyForBlendShape.Add(preset, keys.Key);
                return keys.Key;
            }

            return null;
        }

        public void SetEmote(BlendShapePreset preset)
        {
            if (settingCoroutine != null)
            {
                settingCoroutine.Stop(false);
                float toSeconds = settingCoroutine.IsFinshed != true ? changeEmoteInSeconds * (1.0f - lastValue) : changeEmoteInSeconds;
                StartCoroutine(SetValue(false, lastKey, lastValue, 0.0f, toSeconds));
            }

            if (keyForBlendShape.TryGetValue(preset, out BlendShapeKey toFind) == false)
            {
                BlendShapeKey? found = AddBlendShapePreset(preset);
                if (found == null)
                    return;

                toFind = found.Value;
            }

            settingCoroutine = new ExtendedCoroutine(this, SetValue(true, toFind, 0.0f, 1.0f, changeEmoteInSeconds));
        }

        private IEnumerator SetValue(bool setField, BlendShapeKey key, float from, float to, float seconds)
        {
            if (setField)
                lastKey = key;

            float timer = 0.0f;

            bool shouldContinue = true;
            while (shouldContinue == true)
            {
                yield return null;

                timer += Time.deltaTime;
                float progress = timer / seconds;
                shouldContinue = progress < 1.0f;
                if (shouldContinue == false)
                    progress = 1.0f;

                float value = Mathf.Lerp(from, to, progress);

                if (setField == true)
                    lastValue = value;

                VRMBlendShapeProxy.ImmediatelySetValue(key, value);
            }
        }
    }
}
