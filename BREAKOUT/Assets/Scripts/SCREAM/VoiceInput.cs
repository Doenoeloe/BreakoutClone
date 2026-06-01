using UnityEngine;

namespace VoiceBreakout
{
    /// <summary>
    /// Captures microphone input and exposes a normalized volume level (0–1).
    /// Attach this to any persistent GameObject (e.g. GameManager).
    /// </summary>
    public class VoiceInput : MonoBehaviour
    {
        [Header("Microphone Settings")]
        [Tooltip("Leave empty to use the default system microphone.")]
        public string microphoneDevice = "";

        [Tooltip("Sample window in milliseconds used to calculate volume.")]
        [Range(10, 200)]
        public int sampleWindowMs = 64;

        [Header("Volume Calibration")]
        [Tooltip("Raw volume below this threshold is treated as silence.")]
        [Range(0f, 0.1f)]
        public float noiseFloor = 0.005f;

        [Tooltip("Raw volume that counts as 'maximum' (maps to 1.0). Adjust if your mic is quiet or loud.")]
        [Range(0.01f, 1f)]
        public float maxExpectedVolume = 0.3f;

        [Header("Smoothing")]
        [Tooltip("How quickly the volume reading reacts. Lower = smoother but more lag.")]
        [Range(0f, 1f)]
        public float smoothing = 0.15f;

        // ── Public read-only state ────────────────────────────────────────────
        /// <summary>Raw RMS volume this frame (0–1 ish, unsmoothed).</summary>
        public float RawVolume { get; private set; }

        /// <summary>Normalized, smoothed volume (0–1) ready to drive gameplay.</summary>
        public float NormalizedVolume { get; private set; }

        /// <summary>True while the microphone clip is recording.</summary>
        public bool IsListening { get; private set; }

        // ── Internals ─────────────────────────────────────────────────────────
        private AudioClip _micClip;
        private float[]   _samples;
        private float     _smoothedVolume;
        private const int SampleRate = 44100;

        // ─────────────────────────────────────────────────────────────────────

        private void Start()
        {
            StartListening();
        }

        private void OnDestroy()
        {
            StopListening();
        }

        private void Update()
        {
            if (!IsListening) return;

            RawVolume       = GetCurrentVolume();
            float target    = Remap(RawVolume);
            _smoothedVolume = Mathf.Lerp(_smoothedVolume, target, smoothing + (1f - smoothing) * Time.deltaTime * 60f);
            NormalizedVolume = Mathf.Clamp01(_smoothedVolume);
        }

        // ── Public API ────────────────────────────────────────────────────────

        public void StartListening()
        {
            if (IsListening) return;

            string device = string.IsNullOrEmpty(microphoneDevice) ? null : microphoneDevice;

            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("[VoiceInput] No microphone detected.");
                return;
            }

            int windowSamples = Mathf.CeilToInt(SampleRate * (sampleWindowMs / 1000f));
            _samples  = new float[windowSamples];
            _micClip  = Microphone.Start(device, true, 1, SampleRate);
            IsListening = true;
            Debug.Log($"[VoiceInput] Listening on: {Microphone.devices[0]}");
        }

        public void StopListening()
        {
            if (!IsListening) return;
            Microphone.End(microphoneDevice == "" ? null : microphoneDevice);
            IsListening = false;
        }

        // ── Internals ─────────────────────────────────────────────────────────

        private float GetCurrentVolume()
        {
            if (_micClip == null) return 0f;

            int micPos = Microphone.GetPosition(microphoneDevice == "" ? null : microphoneDevice);
            if (micPos < _samples.Length) return 0f;

            int startSample = micPos - _samples.Length;
            _micClip.GetData(_samples, startSample);

            // RMS
            float sum = 0f;
            for (int i = 0; i < _samples.Length; i++)
                sum += _samples[i] * _samples[i];

            return Mathf.Sqrt(sum / _samples.Length);
        }

        private float Remap(float raw)
        {
            if (raw < noiseFloor) return 0f;
            return Mathf.Clamp01((raw - noiseFloor) / (maxExpectedVolume - noiseFloor));
        }
    }
}
