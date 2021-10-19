using System.Collections;
using UnityEngine;
using VRM;

// Parts of this class were taken from:
// https://stackoverflow.com/questions/37443851/unity3d-check-microphone-input-volume/37446636

[RequireComponent(typeof(AudioSource))]
public class VRMMouthMover : MonoBehaviour
{
    public static float micLoudness;

    public float testSound;

    private string microphoneDeviceName;
    private AudioClip clipRecord;
    private int sampleWindow = 128;
    private float[] waveData;

    private VRMBlendShapeProxy blendShapes;

    void InitMic()
    {
        //if (device == null)
        //{
        microphoneDeviceName = Microphone.devices[0];
        clipRecord = Microphone.Start(microphoneDeviceName, true, 999, 44100);
        //}
    }

    void StopMicrophone()
    {
        Microphone.End(microphoneDeviceName);
    }

    private float GetMaxLevelOfCurrentSample()
    {
        float levelMax = 0;
        int micPosition = Microphone.GetPosition(null) - (sampleWindow + 1);
        if (micPosition < 0)
        {
            if (Microphone.IsRecording(microphoneDeviceName) == false)
            {
                StopMicrophone();
                InitMic();
                return testSound;
            }

            return 0;
        }
        clipRecord.GetData(waveData, micPosition);
        for (int i = 0; i < sampleWindow; ++i)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

    private IEnumerator UpdateMouth()
    {
        float prevSound = 0.0f;
        const float length = 0.1f;

        while (true)
        {
            micLoudness = Mathf.Min(1.0f, GetMaxLevelOfCurrentSample() * 2.0f);
            testSound = micLoudness;

            float timer = 0.0f;
            while (timer < length)
            {
                blendShapes.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), Mathf.Min(1.0f, Mathf.Lerp(prevSound, micLoudness, length / timer)));
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }

    private void OnEnable()
    {
        blendShapes = GetComponent<VRMBlendShapeProxy>();
        waveData = new float[sampleWindow];
        InitMic();
        Debug.Log("VRMMouthMover is using this microphone: " + microphoneDeviceName);
        StartCoroutine(UpdateMouth());
    }

    private void OnDisable()
    {
        StopMicrophone();
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopMicrophone();
    }
}
