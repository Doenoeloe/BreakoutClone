using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    private AudioClip micClip;
    private string micDevice;
    private const int SAMPLE_WINDOW = 128;

    void Start()
    {
        // Pak het eerste beschikbare microfoon apparaat
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            // Loop opname: 1 seconde buffer, 44100 Hz
            micClip = Microphone.Start(micDevice, true, 1, 44100);
        }
    }

    public float GetMicVolume()
    {
        if (!Microphone.IsRecording(micDevice)) return 0f;

        float[] samples = new float[SAMPLE_WINDOW];
        int micPos = Microphone.GetPosition(micDevice) - SAMPLE_WINDOW;
        if (micPos < 0) return 0f;

        micClip.GetData(samples, micPos);

        // Bereken RMS (Root Mean Square) = gemiddeld volume
        float sum = 0f;
        foreach (float s in samples)
            sum += s * s;

        return Mathf.Sqrt(sum / SAMPLE_WINDOW);
    }

    void OnDestroy()
    {
        Microphone.End(micDevice);
    }
}