using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int health = 1000;
    public int maxHealth = 2500;
    private int currentHealth;
    private bool isDead = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        StartCoroutine(BossRoutine());  // Bắt đầu Coroutine điều khiển boss
    }

    void Update()
    {
        if (isDead) return;
    }

    public void BossTakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Coroutine điều khiển hành động của boss
    private IEnumerator BossRoutine()
    {
        // Boss sẽ ở trạng thái Idle trong 30 giây đầu tiên
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(30f);  // Boss sẽ idle trong 30 giây

        // Sau 30 giây, Boss sẽ thực hiện animation Rage trong 10 giây
        animator.SetTrigger("Rage");
        yield return new WaitForSeconds(10f);  // Thực hiện Rage trong 10 giây

        // Quay lại trạng thái Idle sau khi kết thúc Rage
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(30f);  // Chờ thêm 30 giây nữa trước khi tiếp tục lặp lại
    }

    // Xử lý khi boss chết
    private void Die()
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetTrigger("Die"); // Kích hoạt animation Die khi boss chết
            Debug.Log("Boss Defeated!");
        }
    }
}
