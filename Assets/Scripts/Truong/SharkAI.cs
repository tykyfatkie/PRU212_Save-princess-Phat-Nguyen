using UnityEngine;

public class SharkAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float huntSpeed = 4f;
    public float detectionRange = 5f;    // Khoảng cách phát hiện người chơi

    private Rigidbody2D rb;
    private bool isHunting = false;
    private Vector2 moveDirection;
    private float moveTimer;
    private float moveInterval = 2f;     // Thời gian thay đổi hướng ngẫu nhiên

    [Header("Water Boundaries")]
    public BoxCollider2D bottomBoundary;
    public float maxWaterLevel = 3f;

    private float minX, maxX, minY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        moveTimer = moveInterval;

        // Kiểm tra và khởi tạo ranh giới nước
        if (bottomBoundary == null)
        {
            Debug.LogError("BottomBoundary chưa được gán trong Inspector! Sử dụng vị trí mặc định.");
            minX = -10f; // Giá trị mặc định nếu không có boundary
            maxX = 10f;
            minY = -10f;
        }
        else
        {
            Bounds bounds = bottomBoundary.bounds;
            minX = bounds.min.x;
            maxX = bounds.max.x;
            minY = bounds.max.y;
        }

        // Giữ vị trí ban đầu từ Scene/Inspector thay vì đặt ngẫu nhiên
        Vector2 initialPosition = transform.position;
        // Đảm bảo vị trí ban đầu nằm trong giới hạn
        initialPosition.x = Mathf.Clamp(initialPosition.x, minX, maxX);
        initialPosition.y = Mathf.Clamp(initialPosition.y, minY, maxWaterLevel);
        transform.position = initialPosition;

        ChooseNewDirection();
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player chưa được gán!");
            return;
        }

        // Kiểm tra khoảng cách đến người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isHunting = distanceToPlayer <= detectionRange;

        if (!isHunting)
        {
            MoveRandomly();
        }
        else
        {
            HuntPlayer();
        }

        ClampPosition();
        UpdateFacingDirection();
    }

    void ChooseNewDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        moveDirection = new Vector2(randomX, randomY).normalized;
    }

    void MoveRandomly()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0)
        {
            ChooseNewDirection();
            moveTimer = moveInterval;
        }

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void HuntPlayer()
    {
        if (player != null)
        {
            moveDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = moveDirection * huntSpeed;
        }
    }

    void UpdateFacingDirection()
    {
        Vector2 currentVelocity = rb.linearVelocity;
        if (Mathf.Abs(currentVelocity.x) > 0.1f)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(currentVelocity.x),
                1f,
                1f
            );
        }
    }

    void ClampPosition()
    {
        Vector2 currentPos = transform.position;
        float clampedX = Mathf.Clamp(currentPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(currentPos.y, minY, maxWaterLevel);
        
        transform.position = new Vector2(clampedX, clampedY);

        if (Mathf.Abs(clampedX - currentPos.x) > 0.01f)
        {
            moveDirection.x *= -1;
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
        }
        if (Mathf.Abs(clampedY - currentPos.y) > 0.01f)
        {
            moveDirection.y *= -1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Người chơi bị tấn công!");
            SpriteRenderer playerRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (playerRenderer != null)
            {
                playerRenderer.color = Color.red;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}