using UnityEngine;

public class SlimerAI : MonoBehaviour
{
	public Transform player;  //Gán Player vào đây trong Unity
	public Animator animator;
	public float detectRange = 2f; //Khoảng cách phát hiện người chơi
	public float attackRange = 1.5f; //Khoảng cách tấn công
	public float attackCooldown = 2f; //Thời gian chờ giữa các đợt tấn công
	public int attackDamage = 1;	//Sát thương gây ra
	private float lastAttackTime = 0f;

	void Update()
	{
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);

		//Quay mặt về hướng nhân vật
		Flip();

		if (distanceToPlayer <= detectRange)
		{
			//Bị phát hiện → Scaring
			animator.SetBool("isScaring", true);
		}
		else
		{
			animator.SetBool("isScaring", false);
		}

		//Kiểm tra nếu có thể tấn công (chờ cooldown xong)
		if (distanceToPlayer <= attackRange && animator.GetBool("isScaring") && Time.time >= lastAttackTime + attackCooldown)
		{
			Attack();
		}
	}

	void Attack()
	{
		//Kích hoạt animation Attack
		animator.SetTrigger("Attack");

		//Gây sát thương sau 0.5 giây để khớp với animation
		Invoke("DealDamage", 0.5f);

		//Reset thời gian tấn công để tránh spam attack
		lastAttackTime = Time.time;
	}

	void DealDamage()
	{
		if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
		{
			player.GetComponent<PlayerController>().TakeDamage(attackDamage);
		}
	}

	//Nhận diện va chạm với Player
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null && !player.isInvulnerable)
			{
				player.TakeDamage(attackDamage);
			}
		}
	}

	#region Slide quay mặt về hướng nhân vật
	private void Flip()
	{
		if (player != null)
		{
			float direction = player.position.x - transform.position.x;
			if (direction > 0)
			{
				transform.localScale = new Vector3(4.660978f, 4.660978f, 4.660978f); //Hướng sang phải
			}
			else if (direction < 0)
			{
				transform.localScale = new Vector3(-4.660978f, 4.660978f, 4.660978f); //Hướng sang trái
			}
		}
	}
	#endregion

}
