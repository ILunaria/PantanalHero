using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGrounded = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // DO COYOTE TIME AND JUMP BUFFERING
        if(context.performed && isGrounded)
        {
            Debug.Log("Jump! " + context.phase);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        rb.gravityScale = 3f; // TO DO BETTER
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = true;
        rb.gravityScale = 1f; // TO DO BETTER
    }
}
