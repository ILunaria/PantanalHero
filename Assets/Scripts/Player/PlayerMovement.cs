using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Gameobject")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize;

    [Header("Movement Presets")]
    public float moveSpeed;
    public float moveInput;
    public float acceleration;
    public float desacceleration;
    private bool isFacingRight = true;

    [Header ("Jump Presets")]
    public float jumpForce;
    public float onGroundGravity;
    public float fallGravityMultiplier;
    public bool isGrounded = true;

    [Header("Coyote Time")]
    public float coyoteTime;
    public float coyoteTimeCounter;

    [Header("Jump Buffering")]
    public float jumpBufferTime;
    public float jumpBufferCounter;

    private bool isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = onGroundGravity;

    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else if (!IsGrounded())
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        Move();
        Jump();
        Flip();
    }

    private void Move()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;

        float accelrate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : desacceleration;
        float movement = speedDif * accelrate;

        rb.AddForce(movement * Vector2.right);
    }
    private void Jump()
    {
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            StartCoroutine(JumpCooldown());
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool IsGrounded()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
        {
            rb.gravityScale = onGroundGravity;
            isGrounded = true;
            return true;
        }
        else
        {
            rb.gravityScale = onGroundGravity * fallGravityMultiplier;
            isGrounded = false;
            return false;
        }
           
    }
}
