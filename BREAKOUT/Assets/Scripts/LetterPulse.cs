using UnityEngine;

public class LetterPulse : MonoBehaviour
{
    public float speed = 3f;
    public float scaleAmount = 0.2f;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float pulse = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        float scale = 1f + pulse * scaleAmount;

        transform.localScale = startScale * scale;
    }
}
