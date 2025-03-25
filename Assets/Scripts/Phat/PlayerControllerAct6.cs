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
    private bool isHurt = false;
    private bool boosted = false; // Biến lưu trạng thái BoostPotion


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
        if (!canMove || isHurt) return; 

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
        // Kiểm tra nếu boosted thì tăng tốc độ di chuyển
        float currentMoveSpeed = boosted ? moveSpeed * 1.5f : moveSpeed; // Tăng 1.5x nếu boosted

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * currentMoveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(4, 4, 4);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-4, 4, 4);
    }


    private void HandleJump()
    {
        // Kiểm tra nếu boosted thì tăng độ cao nhảy
        float currentJumpForce = boosted ? jumpForce + 5f : jumpForce; // Tăng thêm 5 nếu boosted

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            audioManager.PlayJumpSound();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }


    private void HandleAttack()
    {
        // Kiểm tra nếu boosted thì giảm thời gian hồi chiêu (tăng tốc độ tấn công)
        float currentAttackRate = boosted ? attackRate * 0.5f : attackRate; // Giảm 50% thời gian hồi chiêu nếu boosted

        if (Input.GetKeyDown(KeyCode.X) && Time.time >= nextAttackTime && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("isAttacking");
            audioManager.PlayAttackSound();
            nextAttackTime = Time.time + 1f / currentAttackRate; // Sử dụng attackRate đã được điều chỉnh

            // Gọi coroutine để xử lý sát thương sau 1 giây
            StartCoroutine(DealDamageAfterDelay(0.25f));
        }
    }


    // Coroutine mới để xử lý sát thương sau 1 giây
    private IEnumerator DealDamageAfterDelay(float delay)
    {
        // Đợi theo thời gian delay trước khi gây sát thương
        yield return new WaitForSeconds(delay);

        // Gây sát thương cho kẻ địch trong phạm vi
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyJumper enemyComponent = enemy.GetComponent<EnemyJumper>();
            if (enemyComponent != null)
            {
                enemyComponent.JumperTakeDamage(attackDamage);
            }
        }

        // Reset trạng thái tấn công
        isAttacking = false;
        animator.ResetTrigger("isAttacking");
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

    // Hàm xử lý khi nhân vật bị thương
    public void TakeDamage()
    {
        if (isHurt) return; // Nếu đang bị thương thì không xử lý tiếp

        isHurt = true; // Đặt trạng thái bị thương
        StartCoroutine(DisableMovementForSeconds(1.5f)); // Không cho di chuyển trong 1 giây
    }

    // Coroutine để vô hiệu hóa di chuyển trong một khoảng thời gian
    private IEnumerator DisableMovementForSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        isHurt = false; // Đặt lại trạng thái bị thương
    }
}