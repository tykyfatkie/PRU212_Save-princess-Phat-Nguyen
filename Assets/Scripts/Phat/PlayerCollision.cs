using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private bool isHurt = false;
    public float knockbackForce = 10f;
    public float hurtDuration = 0.5f;
    private GameManager gameManager;
    private bool haveKey = false;
    private AudioManagerAct6 audioManager;
    private HealthBarPlayer healthBar;
    private PlayerControllerAct6 playerController;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        healthBar = FindAnyObjectByType<HealthBarPlayer>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();      
        audioManager = FindAnyObjectByType<AudioManagerAct6>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthPotion"))
        {
            Destroy(collision.gameObject);
            if (gameManager != null) ;
            healthBar.IncreaseHealth();
        }
        else if (collision.CompareTag("Key"))
        {
            Debug.Log("YOU GOT THE KEY!");
            Destroy(collision.gameObject);
            haveKey = true;

        }
        else if (collision.CompareTag("Trap"))
        {
            Debug.Log("YOU ARE DEADDDD!!!!!");

            if (gameManager != null)
            {
                gameManager.GameOver();
            }

        }
        else if (collision.CompareTag("Enemy - Jumper"))
        {
            Debug.Log("YOU HAVE BEEN HIT BY " + collision.tag);
            audioManager.HitSound();
            healthBar.TakeDamage(1);
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
        playerController.TakeDamage();

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
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.ResetTrigger("isHurting");
        }
    }

}
