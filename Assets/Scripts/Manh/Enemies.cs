using UnityEngine;

public class Enemies : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float moveSpeed = 2f;
    //[SerializeField] int maxHealth = 100;

    int currentHealth = 100;
    bool isFacingRight = false;
    Rigidbody2D myRigidbody;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

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
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
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
