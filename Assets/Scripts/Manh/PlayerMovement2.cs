using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement2 : MonoBehaviour
{
    bool isFacingRight = true;
    public Vector2 boxSize;
    public float castDistance;
    public LayerMask groundLayer;

    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;

    public float runSpeed = 10f;
    public float minJumpPower = 5f;
    public float maxJumpPower = 10f;
    public float maxChargeTime = 1f;
    private float chargeTime = 0f;
    private bool isChargingJump = false;
    public float wallJumpForce = 10f;


    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 5f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    public float wallClimbSpeed = 5f;
    private bool isWallClimbing = false;

    [SerializeField] private TrailRenderer tr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Run();
        FlipSprite();
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (isGrounded() && rb.linearVelocity.y == 0)
        {
            animator.SetBool("isJumping", false);
        }

        if (isTouchingWall() && moveInput.y > 0)
        {
            StartWallClimb();
        }
        else
        {
            StopWallClimb();
        }

        if (isChargingJump)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime); // Clamp charge time
        }
    }

    void StartWallClimb()
    {
        isWallClimbing = true;
        rb.gravityScale = 0f; // Disable gravity for smooth climbing
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallClimbSpeed);
        animator.SetBool("isWallClimbing", true);
    }

    void StopWallClimb()
    {
        if (isWallClimbing)
        {
            isWallClimbing = false;
            rb.gravityScale = 8f; // Reset gravity
            animator.SetBool("isWallClimbing", false);
        }
    }

    public bool isTouchingWall()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, direction, castDistance, groundLayer);
        return hit.collider != null;
    }

    void OnSprint(InputValue value)
    {
        if (value.isPressed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        //Debug.Log(moveInput);
    }

    //bool OnWallTouch()

    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded())
        {
            if (isGrounded()) {
                // Start charging the jump
                isChargingJump = true;
                chargeTime = 0f; // Reset charge time
            }
        }

        else if (!value.isPressed && isChargingJump)
        {
            // Release the jump button and execute the jump
            isChargingJump = false;
            float jumpPower = Mathf.Lerp(minJumpPower, maxJumpPower, chargeTime / maxChargeTime); // Calculate jump power
            Vector2 playerVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            rb.linearVelocity = playerVelocity;
            animator.SetBool("isJumping", true);
        }
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.linearVelocity.y);
        rb.linearVelocity = playerVelocity;
        animator.SetFloat("xVelocity", Math.Abs(playerVelocity.x));
    }

    void FlipSprite()
    {
        bool rightToLeft = isFacingRight && moveInput.x < 0f;
        bool leftToRight = !isFacingRight && moveInput.x > 0f;

        if (rightToLeft || leftToRight)
        {
            isFacingRight = !isFacingRight;
            Vector2 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, castDistance, groundLayer);
        return hit.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * castDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * castDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * castDistance);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}