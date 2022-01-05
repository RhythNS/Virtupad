using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace Virtupad
{
    public class VRMEmoteListener : MonoBehaviour, IUIQuickSelectListener
    {
        private VRMEmotionManager emotionManager;

        private List<BlendShapePreset> presets = new List<BlendShapePreset>();

        private bool TryGetEmotionManager()
        {
            if (VRMController.Instance == null)
            {
                emotionManager = null;
                return false;
            }

            emotionManager = VRMController.Instance.VRMEmotionManager;
            return true;
        }

        public int GetCurrentSelection()
        {
            if (emotionManager == null)
                return 0;

            BlendShapePreset preset = emotionManager.GetLastKey().Preset;
            int indexOf = presets.IndexOf(preset);

            return indexOf == -1 ? 0 : indexOf;
        }

        public List<string> GetSelections()
        {
            List<string> selections = new List<string>();

            if (presets.Count > 0)
                presets.ForEach(x => selections.Add(x.ToString()));
            else
                ; // TODO:

            return selections;
        }

        public void OnPreviewChanged(int newIndex)
        {
            if (emotionManager == null)
                return;


        }

        public void OnSelectionChanged(int newIndex)
        {
            if (emotionManager == null)
                return;

            emotionManager.SetEmote(presets[newIndex]);
        }

        public void OnStart()
        {
            if (TryGetEmotionManager() == false)
                return;

            presets = emotionManager.Presets;
        }

        public void OnStop() { }
    }
}
