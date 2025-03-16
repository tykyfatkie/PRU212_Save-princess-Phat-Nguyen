using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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
    private bool isAlive = true;
    private Vector2 deathkick = new Vector2(10f, 10f);
    #endregion

    //Music
    [SerializeField] AudioClip attackSFX, deathSFX;

    //Layer
    public LayerMask groundLayer, climbLayer, enemyLayer;
    public Vector2 boxSize;   //Check ground
    public float castDistance;

    public Vector2 moveInput;
    public CapsuleCollider2D myCapsuleCollider;
    public Rigidbody2D myRigidbody;
    public Animator myAnimator;

    void Start()
    {

        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) return;
        if (isDashing) return;
        if (isJumping) return;

        Run();
        ClimbLadder();
        Die();
        FlipSprite();

        myAnimator.SetFloat("yVelocity", myRigidbody.linearVelocity.y);
        if (isGrounded() && myRigidbody.linearVelocity.y <= 0.2)
        {
            myAnimator.SetBool("isJumping", false);
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


    #region Climb
    void ClimbLadder()
    {
        if (!myCapsuleCollider.IsTouchingLayers(climbLayer))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
        else
        {
            Vector2 climbVelocity = new Vector2(myRigidbody.linearVelocity.x, moveInput.y * climbSpeed);
            myRigidbody.linearVelocity = climbVelocity;
            myRigidbody.gravityScale = 0f;

            bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.linearVelocity.y) > Mathf.Epsilon;
            myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
        }
    }
    #endregion

   

    #region Jump
    void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded())
        {
            isJumping = true;
            Vector2 playerVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpPower);
            myRigidbody.linearVelocity = playerVelocity;
            myAnimator.SetBool("isJumping", true);
            isJumping = false;
        }
    }
    #endregion
    public bool isGrounded() => Physics2D.BoxCast(transform.position, boxSize, 0f, -transform.up, castDistance, groundLayer);


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
    

    #region Die
    void Die()
    {
        if (myCapsuleCollider.IsTouchingLayers(enemyLayer))
        {
            AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position);

            isAlive = false;
            myAnimator.SetTrigger("isDying");
            myRigidbody.linearVelocity = deathkick;

            FindAnyObjectByType<GameSession>().ProcessPlayerDeath();
            FindAnyObjectByType<HealthBar>().TakeDamage(1);

        }
    }
    #endregion

    #region Attack
    void OnAttack(InputValue value)
    {
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

  
}
