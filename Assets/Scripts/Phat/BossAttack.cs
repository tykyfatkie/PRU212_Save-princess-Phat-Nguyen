using UnityEngine;

public class BossAttack : MonoBehaviour

{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackInterval = 2f;

    private float attackTimer;

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    void Attack()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}
