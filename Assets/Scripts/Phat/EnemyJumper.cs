using System.Collections;
using UnityEngine;

public class EnemyJumper : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float distance = 8f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private Transform player;
    [SerializeField] private float freezeTime = 0.5f;
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float playerContactFreezeTime = 3f; // Thời gian đứng yên khi chạm vào Player
    private Rigidbody2D rb;
    private Vector3 startPos;
    private bool movingRight = true;
    public int maxHealth = 40;
    private int currentHealth;
    private bool isChasing = false;
    private bool isFrozen = false;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

    }

    void Update()
    {
        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;

        if (isFrozen) return;

        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                movingRight = true;
                Flip();
            }
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaseRange)
            {
                isChasing = true;
            }
            else if (distanceToPlayer > chaseRange * 1.2f) // Tránh bị "dính" chase khi Player rời xa
            {
                isChasing = false;
            }
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Flip()
    {
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void JumperTakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            JumperDie();
        }
        else
        {
            StartCoroutine(FreezeAndFlash());
        }
    }


    void JumperDie()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if ((direction.x > 0 && !movingRight) || (direction.x < 0 && movingRight))
        {
            Flip();
        }
    }

    void Patrol()
    {
        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;

        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    private IEnumerator FreezeAndFlash()
    {
        isFrozen = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(freezeTime);

        isFrozen = false;
    }

    // Xử lý va chạm khi tiếp xúc với Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Khi va chạm với Player, đứng yên trong 3 giây
            StartCoroutine(FreezeOnPlayerContact());
        }
    }

    // Xử lý va chạm khi trigger với Player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Khi tiếp xúc với Player thông qua trigger, đứng yên trong 3 giây
            StartCoroutine(FreezeOnPlayerContact());
        }
    }

    // Coroutine xử lý trạng thái đứng yên khi chạm vào Player
    private IEnumerator FreezeOnPlayerContact()
    {
        isFrozen = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Đổi màu để thể hiện trạng thái bị đóng băng
        spriteRenderer.color = Color.cyan;

        Debug.Log("Enemy frozen after player contact!");

        // Đứng yên trong 3 giây
        yield return new WaitForSeconds(playerContactFreezeTime);

        // Trở lại màu ban đầu
        spriteRenderer.color = originalColor;

        isFrozen = false;
        Debug.Log("Enemy unfrozen after player contact!");
    }
}