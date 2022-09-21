using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;

    private float moveInput;
    private bool isFacingRight = true;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = Data.onGroundGravity;     
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (IsGrounded())
        {
            coyoteTimeCounter = Data.coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = Data.jumpBufferTime;
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
        float targetSpeed = moveInput * Data.moveSpeed;
        float speedDif = targetSpeed - rb.velocity.x;

        float accelrate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.acceleration : Data.desacceleration;
        float movement = speedDif * accelrate;

        rb.AddForce(movement * Vector2.right);
    }
    private void Jump()
    {
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, Data.jumpForce);
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
        if (Physics2D.OverlapBox(groundCheck.position, Data.groundCheckSize, 0, Data.groundLayer))
        {
            rb.gravityScale = Data.onGroundGravity;
            return true;
        }
        else
        {
            rb.gravityScale = Data.onGroundGravity * Data.fallGravityMultiplier;
            return false;
        }
    }
}
