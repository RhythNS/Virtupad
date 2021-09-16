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

    private void Start()
    {
        blendShapes = GetComponent<VRMBlendShapeProxy>();
        waveData = new float[sampleWindow];
    }

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

    private void Update()
    {
        micLoudness = GetMaxLevelOfCurrentSample();
        testSound = micLoudness;
        blendShapes.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), testSound);
    }

    private void OnEnable()
    {
        InitMic();
        Debug.Log("VRMMouthMover is using this microphone: " + microphoneDeviceName);
    }

    private void OnDisable()
    {
        StopMicrophone();
    }

    private void OnDestroy()
    {
        StopMicrophone();
    }
}
