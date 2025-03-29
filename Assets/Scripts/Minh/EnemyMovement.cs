using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float moveSpeed = 2f;
    //[SerializeField] int maxHealth = 100;

    private Vector2 deathkick = new Vector2(10f, 10f);
    private int currentHealth = 200;
    private bool isFacingRight = false;
    Rigidbody2D myRigidbody;
    Animator myAnimation;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimation = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        myRigidbody.linearVelocity = new Vector2(-moveSpeed, 0f);
        FlipSprite();

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy Health: " + currentHealth);

        if (currentHealth > 0)
        {
            myAnimation.SetTrigger("Hurt");
            StartCoroutine(StunEffect(0.75f));
        }
        else
        {
            StartCoroutine(DieEffect()); 
        }
    }

    IEnumerator StunEffect(float stunDuration)
    {
        moveSpeed = 0; // Ngừng di chuyển
        yield return new WaitForSeconds(stunDuration);
        moveSpeed = 2f; // Quay lại tốc độ ban đầu
    }

    IEnumerator DieEffect()
    {
        myAnimation.SetTrigger("Die");
        yield return new WaitForSeconds(0.5f); 
        Destroy(gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        moveSpeed = -moveSpeed;

    }

    void FlipSprite()
    {
        bool rightToLeft = isFacingRight && myRigidbody.linearVelocity.x < 0f;
        bool leftToRight = !isFacingRight && myRigidbody.linearVelocity.x > 0f;

        if (rightToLeft || leftToRight)
        {
            isFacingRight = !isFacingRight;
            Vector2 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}
