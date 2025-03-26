using System.Collections;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public float initialDelay = 30f; // Thời gian chờ trước đợt đầu tiên
    public float fadeInTime = 2f; // Thời gian mờ 30%
    public float fullIntensityTime = 2f; // Thời gian đậm 100%
    public float cycleCooldown = 5f; // Thời gian giữa các chu kỳ (có thể điều chỉnh)

    private SpriteRenderer spriteRenderer;
    private HealthBarPlayer healthBar;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar = FindObjectOfType<HealthBarPlayer>();
    }

    private void Start()
    {
        StartCoroutine(ThunderCycle());
    }

    private IEnumerator ThunderCycle()
    {
        // Chờ 30s trước đợt đầu tiên
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Mờ 30%
            yield return StartCoroutine(FadeInThunder());

            // Đậm 100% và gây sát thương
            yield return StartCoroutine(FullIntensityThunder());

            // Cooldown giữa các chu kỳ
            yield return new WaitForSeconds(cycleCooldown);
        }
    }

    private IEnumerator FadeInThunder()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            // Mờ từ 0% đến 30%
            float alpha = Mathf.Lerp(0f, 0.3f, elapsedTime / fadeInTime);
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                alpha
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FullIntensityThunder()
    {
        // Đậm 100% ngay lập tức
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            1f
        );

        // Gây sát thương
        if (healthBar != null)
        {
            healthBar.TakeDamage(1);
        }

        // Giữ trạng thái đậm trong 2s
        yield return new WaitForSeconds(fullIntensityTime);

        // Mờ về 0%
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            0f
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Chỉ gây sát thương khi thunder ở trạng thái đậm 100%
        if (collision.gameObject.CompareTag("Player") &&
            spriteRenderer.color.a == 1f)
        {
            if (healthBar != null)
            {
                healthBar.TakeDamage(1);
            }
        }
    }
}