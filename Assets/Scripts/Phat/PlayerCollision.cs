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
    public float invincibilityDuration = 2f; 
    public float flashInterval = 0.1f; 
    private GameManager gameManager;
    private bool haveKey = false;
    private AudioManagerAct6 audioManager;
    private HealthBarPlayer healthBar;
    private PlayerControllerAct6 playerController;
    private SpriteRenderer spriteRenderer; 


    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.3f; 
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

        Vector2 knockbackDirection = (transform.position - enemy.position).normalized;

        rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, rb.linearVelocity.y);

        Invoke(nameof(ResetHurt), hurtDuration);
    }

    void ResetHurt()
    {
        isHurt = false;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.ResetTrigger("isHurting");
        }
    }

    private IEnumerator TemporaryInvincibility()
    {

        spriteRenderer.enabled = true;
        isInvincible = true;

        int flashCount = Mathf.FloorToInt(invincibilityDuration / (flashInterval * 2));

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
        Debug.Log("Invincibility ended");
    }

    private IEnumerator ShakeCamera()
    {
        isShaking = true;
        float elapsedTime = 0f;

        Vector3 originalPos = cameraTransform.localPosition;

        while (elapsedTime < shakeDuration)
        {

            float xOffset = Random.Range(-1f, 1f) * shakeIntensity;
            float yOffset = Random.Range(-1f, 1f) * shakeIntensity;


            cameraTransform.localPosition = new Vector3(
                originalPos.x + xOffset,
                originalPos.y + yOffset,
                originalPos.z
            );


            elapsedTime += Time.deltaTime;


            yield return null;
        }

        cameraTransform.localPosition = originalPos;
        isShaking = false;
        Debug.Log("Camera shake ended");
    }
}