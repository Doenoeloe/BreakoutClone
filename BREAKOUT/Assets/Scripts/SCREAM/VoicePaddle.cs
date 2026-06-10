using UnityEngine;

namespace VoiceBreakout
{
    /// <summary>
    /// Moves a Breakout paddle horizontally.
    /// Voice volume (from VoiceInput) multiplies the paddle's speed.
    /// Also supports optional keyboard/touch input as a fallback.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class VoicePaddle : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The VoiceInput component anywhere in the scene.")]
        public VoiceInput voiceInput;

        [Header("Movement")]
        [Tooltip("Base speed when voice is at maximum (NormalizedVolume = 1).")]
        public float maxSpeed = 18f;

        [Tooltip("Minimum speed so the paddle isn't completely stuck at low volume.")]
        public float minSpeed = 1f;

        [Tooltip("Optional keyboard fallback. The voice volume still applies as a multiplier.")]
        public bool allowKeyboard = true;

        [Tooltip("Optional touch/mouse fallback — moves paddle toward pointer X.")]
        public bool allowPointer = false;

        [Header("Bounds")]
        [Tooltip("Leftmost X position the paddle centre can reach.")]
        public float leftBound = -8f;

        [Tooltip("Rightmost X position the paddle centre can reach.")]
        public float rightBound = 8f;

        [Header("Curve (optional)")]
        [Tooltip("Remap how NormalizedVolume maps to speed. Leave as a straight line for linear response, or curve it so you have to REALLY scream.")]
        public AnimationCurve volumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        // ── Internals ─────────────────────────────────────────────────────────
        private Rigidbody2D _rb;
        private float _direction;   // -1, 0, +1 from keyboard
        private Camera _cam;

        // ── Public state ──────────────────────────────────────────────────────
        /// <summary>Current effective speed this frame.</summary>
        public float CurrentSpeed { get; private set; }

        // ─────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _cam = Camera.main;

            // Lock rotation & gravity — paddle moves horizontally only
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

            if (voiceInput == null)
                voiceInput = FindObjectOfType<VoiceInput>();

            if (voiceInput == null)
                Debug.LogWarning("[VoicePaddle] No VoiceInput found in scene. Speed will default to minSpeed.");
        }

        private void Update()
        {
            _direction = 0f;

            if (allowKeyboard)
            {
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) _direction = -1f;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) _direction = 1f;
            }
        }

        private void FixedUpdate()
        {
            float vol = voiceInput != null ? voiceInput.NormalizedVolume : 0f;
            float curved = volumeCurve.Evaluate(vol);
            CurrentSpeed = Mathf.Lerp(minSpeed, maxSpeed, curved);

            float velocity = 0f;

            if (allowPointer && (Input.touchCount > 0 || Input.GetMouseButton(0)))
            {
                velocity = SolvePointerVelocity(CurrentSpeed);
            }
            else
            {
                velocity = _direction * CurrentSpeed;
            }

            // Apply and clamp position
            _rb.linearVelocity = new Vector2(velocity, 0f);

            float clampedX = Mathf.Clamp(transform.position.x, leftBound, rightBound);
            if (!Mathf.Approximately(clampedX, transform.position.x))
            {
                _rb.MovePosition(new Vector2(clampedX, transform.position.y));
                _rb.linearVelocity = Vector2.zero;
            }
        }

        private float SolvePointerVelocity(float speed)
        {
            Vector3 worldPos;

            if (Input.touchCount > 0)
                worldPos = _cam.ScreenToWorldPoint(Input.GetTouch(0).position);
            else
                worldPos = _cam.ScreenToWorldPoint(Input.mousePosition);

            float diff = worldPos.x - transform.position.x;
            // Move toward pointer at CurrentSpeed, stop when close
            if (Mathf.Abs(diff) < 0.05f) return 0f;
            return Mathf.Sign(diff) * speed;
        }
    }
}
