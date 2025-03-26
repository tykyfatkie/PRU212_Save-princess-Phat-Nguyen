using System.Collections;
using UnityEngine;
public class Laser : MonoBehaviour
{
    public float chargeTime = 5f; // Thời gian tụ lực 
    public float shootDuration = 1f; // Thời gian bắn ngang
    public float returnTime = 1f; // Thời gian dịch chuyển về vị trí ban đầu
    public float cooldownTime = 5f; // Thời gian cooldown
    public float speed = 60f;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private bool isCharging = false;
    private bool isShooting = false;
    private bool isReturning = false; // Thêm biến để kiểm tra trạng thái quay về
    private AudioManagerAct6 audio;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
    }

    private void Start()
    {
        StartCoroutine(LaserCycle());
        audio = FindAnyObjectByType<AudioManagerAct6>();
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            // Tụ lực
            yield return StartCoroutine(ChargeLaser());
            // Bắn ngang
            yield return StartCoroutine(ShootLaser());
            // Quay về vị trí ban đầu
            yield return StartCoroutine(ReturnToOrigin());

            // Cooldown
            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private IEnumerator ChargeLaser()
    {
        isCharging = true;
        float elapsedTime = 0f;
        while (elapsedTime < chargeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / chargeTime);
            Color currentColor = spriteRenderer.color;
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        isCharging = false;
    }

    private IEnumerator ShootLaser()
    {
        isShooting = true;
        audio.LaserSound();
        float elapsedTime = 0f;
        while (elapsedTime < shootDuration)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isShooting = false;
    }

    private IEnumerator ReturnToOrigin()
    {
        isReturning = true; // Đánh dấu đang quay về
        float elapsedTime = 0f;
        while (elapsedTime < returnTime)
        {
            SetAlphaToZero();
            transform.position = Vector3.Lerp(transform.position, originalPosition, elapsedTime / returnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
        isReturning = false; // Kết thúc quay về
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Chỉ gây dame khi đang ở trạng thái bắn
        if (collision.gameObject.CompareTag("Player") && isShooting && !isReturning)
        {
            HealthBarPlayer healthBar = FindObjectOfType<HealthBarPlayer>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(3);
            }
        }
    }

    public void SetAlphaToZero()
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
    }
}