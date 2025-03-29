using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Movement
    //Run
    private float runSpeed = 10f;
    private bool isFacingRight = true;
    //Jump
    private float jumpPower = 18f;
    //Climb
    private float climbSpeed = 5f;
    private bool isJumping;
    //Dash
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 5f;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    private float gravityScaleAtStart;
    [SerializeField] private TrailRenderer trailRenderer;
    //Attack
    [SerializeField] Vector2 attackOffset;
    [SerializeField] float attackRange = 0.5f;
    private bool canAttack = true;
    private bool isAttacking;
    private float attackTime = 0.2f;
    private float attackCooldown = 0.5f;
    //Live/Death
    public bool isAlive = true;
    private Vector2 deathkick = new Vector2(10f, 10f);
    #endregion

    //Music
    [SerializeField] AudioClip attackSFX, jumpSFX, deathSFX, backgroundSFX;
    private AudioSource audioSource;

    //Layer
    public LayerMask groundLayer, climbLayer, enemyLayer, deathZoneLayer;
    public Vector2 boxSize;   //Check ground
    public float castDistance;

    public Vector2 moveInput;
    public CapsuleCollider2D myCapsuleCollider;
    public Rigidbody2D myRigidbody;
    public Animator myAnimator;

    public float minJumpPower = 5f;
    public float maxJumpPower = 10f;
    public float maxChargeTime = 1f;
    private float chargeTime = 0f;
    private bool isChargingJump = false;

    public float wallClimbSpeed = 5f;
    private bool isWallClimbing = false;

    void Start()
    {

        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = backgroundSFX;
        audioSource.Play();
    }

    void Update()
    {
        if (!isAlive) return;
        if (isDashing) return;

        Run();
        Die();
        FlipSprite();

        myAnimator.SetFloat("yVelocity", myRigidbody.linearVelocity.y);
        if (isGrounded() && myRigidbody.linearVelocity.y <= 0.2)
        {
            myAnimator.SetBool("isJumping", false);
        }


        if (isDashing)
        {
            return;
        }

        myAnimator.SetFloat("yVelocity", myRigidbody.linearVelocity.y);

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
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        }
    }

    #region Run
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.linearVelocity.y);
        myRigidbody.linearVelocity = playerVelocity;
        myAnimator.SetFloat("xVelocity", Math.Abs(playerVelocity.x));

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
    #endregion

    #region Jump
    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded())
        {
            isChargingJump = true;
            chargeTime = 0f;
        }
        else if (!value.isPressed && isChargingJump)
        {
            isChargingJump = false;
            float jumpPower = Mathf.Lerp(minJumpPower, maxJumpPower, chargeTime / maxChargeTime);
            StartCoroutine(PerformJump(jumpPower));
        }
    }

    private IEnumerator PerformJump(float jumpPower)
    {
        isJumping = true;
        Vector2 playerVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpPower);
        myRigidbody.linearVelocity = playerVelocity;
        myAnimator.SetBool("isJumping", true);
        audioSource.PlayOneShot(jumpSFX);

        yield return new WaitForSeconds(0.5f); // Simulate jump duration

        isJumping = false;
    }
    #endregion
    public bool isGrounded()
    {
        bool groundCheck = Physics2D.BoxCast(transform.position, boxSize, 0f, -transform.up, castDistance, groundLayer);
        bool wallCheck = isWallClimbing; // Consider grounded if wall climbing

        return groundCheck || wallCheck;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }


    #region Die
    public void Die()
    {
        if (myCapsuleCollider.IsTouchingLayers(enemyLayer) || myCapsuleCollider.IsTouchingLayers(deathZoneLayer))
        {
            Debug.Log(myCapsuleCollider.IsTouchingLayers(deathZoneLayer));
            AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position);

            isAlive = false;
            myAnimator.SetTrigger("isDying");
            if (!isFacingRight)
                myRigidbody.linearVelocity = deathkick;
            else
            {
                deathkick.x = -deathkick.x;
                myRigidbody.linearVelocity = deathkick;
            }

            //FindAnyObjectByType<HealthBar>().TakeDamage(1);
            FindAnyObjectByType<GameSession>().ProcessPlayerDeath();


        }
    }
    #endregion

    #region Attack
    void OnAttack(InputValue value)
    {
        if (isJumping == true) return;
        if (value.isPressed && canAttack)
        {
            StartCoroutine(Attack());

        }
    }
    IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;
        myAnimator.SetTrigger("Attack");

        AudioSource.PlayClipAtPoint(attackSFX, Camera.main.transform.position);

        Vector2 attackPosition = (Vector2)transform.position + (isFacingRight ? attackOffset : new Vector2(-attackOffset.x, attackOffset.y));
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyMovement>().TakeDamage(100);
        }
        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

    }

    private void OnDrawGizmosSelected()
    {
        if (transform == null) return;

        Vector2 attackPosition = (Vector2)transform.position + (isFacingRight ? attackOffset : new Vector2(-attackOffset.x, attackOffset.y));
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
    #endregion

    #region Wall Climb
    void StartWallClimb()
    {
        isWallClimbing = true;
        myRigidbody.gravityScale = 0f; // Disable gravity for smooth climbing
        myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, wallClimbSpeed);
        myAnimator.SetBool("isClimbing", true);
    }

    void StopWallClimb()
    {
        if (isWallClimbing)
        {
            isWallClimbing = false;
            myRigidbody.gravityScale = 8f; // Reset gravity
            myAnimator.SetBool("isClimbing", false);
        }
    }

    public bool isTouchingWall()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0f, direction, castDistance, groundLayer);
        return hit.collider != null;
    }

    #endregion

    #region Dash
    void OnSprint(InputValue value)
    {
        if (value.isPressed && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = myRigidbody.gravityScale;
        myRigidbody.gravityScale = 0;
        myRigidbody.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);
        trailRenderer.emitting = false;
        myRigidbody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
    #endregion

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("Die");
            Die();
        }
    }

}