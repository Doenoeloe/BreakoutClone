using UnityEngine;
using UnityEngine.UI;

namespace VoiceBreakout
{
    /// <summary>
    /// Optional debug HUD. Shows a volume bar and speed readout.
    /// Works with or without a Canvas — if no UI references are set it
    /// falls back to an OnGUI overlay so you can drop it in instantly.
    /// </summary>
    public class VoiceDebugUI : MonoBehaviour
    {
        [Header("References (optional — leave empty for OnGUI fallback)")]
        public VoiceInput  voiceInput;
        public VoicePaddle voicePaddle;

        [Header("UI (optional)")]
        [Tooltip("A UI Image with Image Type = Filled used as the volume bar.")]
        public Image volumeBar;

        [Tooltip("A UI Text/TMP to display current speed.")]
        public Text  speedLabel;

        [Header("OnGUI Fallback Style")]
        public Color barColor    = new Color(0.2f, 1f, 0.4f, 0.85f);
        public Color bgColor     = new Color(0f, 0f, 0f, 0.55f);

        // ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (voiceInput  == null) voiceInput  = FindObjectOfType<VoiceInput>();
            if (voicePaddle == null) voicePaddle = FindObjectOfType<VoicePaddle>();
        }

        private void Update()
        {
            if (voiceInput == null) return;

            if (volumeBar != null)
                volumeBar.fillAmount = voiceInput.NormalizedVolume;

            if (speedLabel != null && voicePaddle != null)
                speedLabel.text = $"Speed: {voicePaddle.CurrentSpeed:F1}";
        }

        private void OnGUI()
        {
            if (voiceInput == null || volumeBar != null) return; // skip if using proper UI

            float vol   = voiceInput.NormalizedVolume;
            float speed = voicePaddle != null ? voicePaddle.CurrentSpeed : 0f;

            int w = 500, h = 28, pad = 24;
            Rect bgRect  = new Rect(pad, pad, w, h * 3 + pad);
            Rect barBg   = new Rect(pad + 4, pad + 4 + h, w - 8, h - 8);
            Rect barFill = new Rect(barBg.x, barBg.y, barBg.width * vol, barBg.height);

            // Background
            GUI.color = bgColor;
            GUI.DrawTexture(bgRect, Texture2D.whiteTexture);

            GUI.color = Color.white;
            GUI.Label(new Rect(pad + 6, pad + 2, w, h), $"🎤  Volume: {vol * 100f:F0}%");

            // Bar background
            GUI.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            GUI.DrawTexture(barBg, Texture2D.whiteTexture);

            // Bar fill — turns red when screaming hard
            GUI.color = Color.Lerp(barColor, new Color(1f, 0.2f, 0.2f, 0.9f), vol);
            GUI.DrawTexture(barFill, Texture2D.whiteTexture);

            GUI.color = Color.white;
            GUI.Label(new Rect(pad + 6, pad + h * 2 + 2, w, h), $"🏓  Speed: {speed:F1} u/s");
        }
    }
}
