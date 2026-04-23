using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 playerInput;
    Rigidbody2D rb;

    float speed = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = playerInput * speed;

        float border = Camera.main.orthographicSize * Camera.main.aspect;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -border +1 , border -1);
        transform.position = pos;
    }

    public void GetMovementInput(InputAction.CallbackContext context)
    {
        playerInput = context.ReadValue<Vector2>();
    }
}
