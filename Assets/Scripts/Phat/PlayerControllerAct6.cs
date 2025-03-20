using System.Collections;
using UnityEngine;

public class PlayerControllerAct6 : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float runSpeed = 15f;
    [SerializeField] public float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float moveInput;
    private bool isGrounded;
    private Animator animator;
    private GameManager gameManager;
    private Rigidbody2D rb;
    private AudioManagerAct6 audioManager;
    private float timer = 0;
    private bool canMove = false;
    private bool isHurt = false; // Thêm biến kiểm tra trạng thái bị thương

    //==================attack==================
    private bool isAttacking = false;
    public Transform attackPoint;
    public float attackRange = 2.5f;
    public LayerMask enemyLayer;
    public int attackDamage = 20;
    public float attackRate = 1f;
    private float nextAttackTime = 1f;
    //==========================================

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManagerAct6>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(StartDelay());
    }

    void Update()
    {
        if (!canMove || isHurt) return; // Không di chuyển nếu đang bị thương

        if (gameManager.IsGameOver())
            return;

        HandleJump();
        HandleMovement();
        HandleAttack();
        UpdateAnimation();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            moveSpeed = runSpeed;
            audioManager.PlayDashSound();
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            moveSpeed = 5f;
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(4, 4, 4);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-4, 4, 4);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            audioManager.PlayJumpSound();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleAttack()
    {
        isAttacking = true;
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= nextAttackTime)
        {
            animator.SetTrigger("isAttacking");
            audioManager.PlayAttackSound();
            nextAttackTime = Time.time + 1f / attackRate;
            StartCoroutine(ResetAttackAnimation());

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyJumper enemyComponent = enemy.GetComponent<EnemyJumper>();
                if (enemyComponent != null)
                {
                    enemyComponent.JumperTakeDamage(attackDamage);
                }
            }
        }
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(4f);
        canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        animator.ResetTrigger("isAttacking");
    }

    // Hàm xử lý khi nhân vật bị thương
    public void TakeDamage()
    {
        if (isHurt) return; // Nếu đang bị thương thì không xử lý tiếp

        isHurt = true; // Đặt trạng thái bị thương
        StartCoroutine(DisableMovementForSeconds(1f)); // Không cho di chuyển trong 1 giây
    }

    // Coroutine để vô hiệu hóa di chuyển trong một khoảng thời gian
    private IEnumerator DisableMovementForSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        isHurt = false; // Đặt lại trạng thái bị thương
    }
}