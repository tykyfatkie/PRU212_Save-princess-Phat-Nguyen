using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private bool isHurt = false;
    private bool isInvincible = false;
    public float knockbackForce = 10f;
    public float hurtDuration = 0.5f;
    public float invincibilityDuration = 2f; // Thời gian bất tử sau khi bị thương
    public float flashInterval = 0.1f; // Thời gian nháy khi bất tử
    private GameManager gameManager;
    private bool haveKey = false;
    private bool boosted = false;
    private AudioManagerAct6 audioManager;
    private HealthBarPlayer healthBar;
    private PlayerControllerAct6 playerController;
    private SpriteRenderer spriteRenderer; // Để làm hiệu ứng nháy khi bất tử

    // Tham số cho hiệu ứng rung camera
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.3f; // Tăng cường độ rung
    private Transform cameraTransform;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        healthBar = FindObjectOfType<HealthBarPlayer>();
        playerController = GetComponent<PlayerControllerAct6>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Tìm camera chính
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManagerAct6>();

        // Nếu chưa tìm được camera trong Awake, thử lại trong Start
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthPotion"))
        {
            Destroy(collision.gameObject);
            if (gameManager != null && healthBar != null)
            {
                healthBar.IncreaseHealth();
            }
        }
        else if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject);
            haveKey = true;
        }
        else if (collision.CompareTag("BoostPotion"))
        {
            Destroy(collision.gameObject);
            boosted = true;
        }
        else if (collision.CompareTag("Trap"))
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
        else if ((collision.CompareTag("Enemy") || collision.CompareTag("Enemy - Jumper")) && !isInvincible)
        {
            Debug.Log("YOU HAVE BEEN HIT BY " + collision.tag);
            if (audioManager != null)
            {
                audioManager.HitSound();
            }

            if (healthBar != null)
            {
                healthBar.TakeDamage(1);
            }

            TakenDamage(collision.transform);

            if (gameManager != null)
            {
                Debug.Log("Reducing health...");
            }
        }
    }

    void TakenDamage(Transform enemy)
    {
        if (isHurt) return;

        isHurt = true;

        // Kiểm tra và kích hoạt hiệu ứng rung camera
        if (cameraTransform != null && !isShaking)
        {
            originalCameraPosition = cameraTransform.localPosition;
            StartCoroutine(ShakeCamera());
            Debug.Log("Camera shake initiated");
        }

        // Kích hoạt chế độ bất tử và hiệu ứng nhấp nháy
        if (!isInvincible && spriteRenderer != null)
        {
            StartCoroutine(TemporaryInvincibility());
            Debug.Log("Invincibility started");
        }

        // Kiểm tra playerController trước khi sử dụng
        if (playerController != null)
        {
            playerController.TakeDamage();
        }
        else
        {
            Debug.LogError("PlayerController is null. Make sure this script is attached to the same GameObject as PlayerControllerAct6.");
        }

        if (anim != null)
        {
            anim.SetTrigger("isHurting");
        }

        // Tính toán hướng bật lùi dựa trên vị trí của enemy và player
        Vector2 knockbackDirection = (transform.position - enemy.position).normalized;

        // Áp dụng lực bật lùi theo phương ngang, giữ nguyên vận tốc theo phương dọc
        rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, rb.linearVelocity.y);

        Invoke(nameof(ResetHurt), hurtDuration);
    }

    void ResetHurt()
    {
        isHurt = false;

        // Chỉ đặt vận tốc ngang về 0, giữ vận tốc dọc
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.ResetTrigger("isHurting");
        }
    }

    // Coroutine tạo hiệu ứng bất tử tạm thời với nhấp nháy đều đặn
    private IEnumerator TemporaryInvincibility()
    {
        // Đảm bảo sprite renderer được bật lúc bắt đầu
        spriteRenderer.enabled = true;
        isInvincible = true;

        // Số lần nhấp nháy = thời gian bất tử / khoảng thời gian nhấp nháy
        int flashCount = Mathf.FloorToInt(invincibilityDuration / (flashInterval * 2));

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }

        // Đảm bảo sprite renderer được bật lại khi kết thúc
        spriteRenderer.enabled = true;
        isInvincible = false;
        Debug.Log("Invincibility ended");
    }

    // Coroutine tạo hiệu ứng rung camera được cải tiến
    private IEnumerator ShakeCamera()
    {
        isShaking = true;
        float elapsedTime = 0f;

        // Lưu vị trí ban đầu của camera
        Vector3 originalPos = cameraTransform.localPosition;

        while (elapsedTime < shakeDuration)
        {
            // Tạo vị trí ngẫu nhiên xung quanh vị trí ban đầu
            float xOffset = Random.Range(-1f, 1f) * shakeIntensity;
            float yOffset = Random.Range(-1f, 1f) * shakeIntensity;

            // Áp dụng vị trí mới cho camera
            cameraTransform.localPosition = new Vector3(
                originalPos.x + xOffset,
                originalPos.y + yOffset,
                originalPos.z
            );

            // Tăng thời gian đã trôi qua
            elapsedTime += Time.deltaTime;

            // Cho phép game chạy một frame trước khi tiếp tục
            yield return null;
        }

        // Đặt lại vị trí ban đầu của camera
        cameraTransform.localPosition = originalPos;
        isShaking = false;
        Debug.Log("Camera shake ended");
    }
}