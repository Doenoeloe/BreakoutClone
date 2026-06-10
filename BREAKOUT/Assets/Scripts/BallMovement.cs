using System.Collections;
using TMPro;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;
    private bool launched = false;
    [SerializeField] private TextMeshProUGUI startText;
    private Coroutine _blinkCoroutine;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _blinkCoroutine = StartCoroutine(BlinkLoop());

    }

    void Update()
    {
        if (!launched && Input.GetKeyDown(KeyCode.Space))

        {
            StopCoroutine(_blinkCoroutine);
            startText.alpha = 0f;
            Debug.Log("spatie");
            Launch();
        }
    }

    void Launch()
    {
        launched = true;
        rb.linearVelocity = new Vector2(speed, speed);
    }

    // Houd de snelheid constant na botsingen
    void FixedUpdate()
    {
        if (launched && rb.linearVelocity.magnitude != speed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(0f, 1f, 0.6f));
            yield return StartCoroutine(Fade(1f, 0f, 0.6f));
        }
    }
    IEnumerator Fade(float from, float to, float duration)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            startText.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        startText.alpha = to;
    }
}