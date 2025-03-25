using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int health = 1000;  // Lượng máu của boss
    public Transform weakPoint; // Điểm yếu của boss
    public GameObject thunderPrefab; // Prefab tia sét
    public GameObject laserPrefab; // Prefab tia laser
    public float thunderInterval = 5f; // Thời gian triệu hồi tia sét
    public float laserInterval = 3f; // Thời gian bắn tia laser
    private bool isDead = false;

    private Animator animator;  // Tham chiếu đến Animator

    void Start()
    {
        animator = GetComponent<Animator>();  // Lấy Animator từ GameObject
        StartCoroutine(ThunderRoutine());
        StartCoroutine(LaserRoutine());
    }

    void Update()
    {
        // Kiểm tra va chạm khi người chơi đánh vào điểm yếu
        if (isDead) return;
    }

    // Khi boss nhận sát thương từ người chơi
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    // Triệu hồi tia sét từ trên trời
    private IEnumerator ThunderRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(thunderInterval);

            // Kích hoạt animation BossRage khi triệu hồi tia sét
            animator.SetTrigger("Rage");

            Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 10f, 0f);
            Instantiate(thunderPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Bắn tia laser
    private IEnumerator LaserRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(laserInterval);
            Vector3 laserPosition = new Vector3(transform.position.x, transform.position.y, 0f);
            Instantiate(laserPrefab, laserPosition, Quaternion.identity);
        }
    }

    // Khi boss chết
    private void Die()
    {
        if (!isDead)
        {
            isDead = true;

            // Kích hoạt animation BossDie khi boss chết
            animator.SetTrigger("Die");

            // Thêm hiệu ứng hoặc âm thanh khi boss chết
            Debug.Log("Boss Defeated!");
        }
    }
}
