using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;
    private bool launched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Lanceer de bal met spatiebalk
        Debug.Log("hallo" + launched);
        if (!launched && Input.GetKeyDown(KeyCode.Space))

        {
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
}