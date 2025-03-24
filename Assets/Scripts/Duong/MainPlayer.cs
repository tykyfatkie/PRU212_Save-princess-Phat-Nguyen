using System.Collections;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
	[SerializeField] public float moveSpeed = 5f;
	[SerializeField] public float runSpeed = 15f;
	[SerializeField] public float jumpForce = 15f;
	[SerializeField] private float moveInput;
	[SerializeField] private float attackRange = 0.5f;
	[SerializeField] private LayerMask enemyLayer; //Layer của quái
	[SerializeField] private LayerMask groundLayer; //Layer của mặt đất
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Transform attackPoint; //Gán GameObject AttackPoint

	public AudioClip jumpSound;
	public AudioClip attackSound;
	public AudioClip hurtSound;
	public AudioClip bgMusic;
	public AudioClip deathSound;
	private AudioSource audioSource;

	public GameObject gameOverUI;
	public HealthBarPS healthBar;
	private Animator animator;
	private Rigidbody2D rb;
	private Vector3 respawnPosition; //Vị trí hồi sinh
	public int health = 5;
	public bool isInvulnerable = false; //Tránh mất máu liên tục
	private bool isGrounded;
	private bool isAttacking = false;
	private bool isGameOver = false;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.loop = true;
		audioSource.clip = bgMusic;

		//Phát nhạc nền khi bắt đầu
		audioSource.Play();

		rb = GetComponent<Rigidbody2D>();

		//Lưu vị trí ban đầu
		respawnPosition = transform.position;
	}

	void Update()
	{
		HandleJump();
		HandleMovement();
		HandleAttack();
		UpdateAnimation();

		if (Input.GetKeyDown(KeyCode.Z))
		{
			moveSpeed = runSpeed;
		}
		if (Input.GetKeyUp(KeyCode.Z))
		{
			moveSpeed = 5f;
		}
	}

	private void HandleMovement()
	{
		float moveInput = Input.GetAxis("Horizontal");
		rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

		if (moveInput > 0)
			transform.localScale = new Vector3(4, 4, 4);
		else if (moveInput < 0)
			transform.localScale = new Vector3(-4, 4, 4);
	}

	private void HandleJump()
	{
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

			//Âm thanh nhảy
			audioSource.PlayOneShot(jumpSound);
		}
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
	}

	private void HandleAttack()
	{
		if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
		{
			StartCoroutine(Attack());
		}
	}

	IEnumerator Attack()
	{
		isAttacking = true;
		animator.SetTrigger("isAttacking");
		Debug.Log("⚔ Nhân vật đang tấn công!");

		//Tạo danh sách các kẻ địch bị tấn công
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
		Debug.Log("👀 Số quái trong vùng đánh: " + hitEnemies.Length);

		//Gây sát thương cho từng kẻ địch
		foreach (Collider2D enemy in hitEnemies)
		{
			Debug.Log("Đánh trúng: " + enemy.name);

			//Gọi hàm TakeDamage() của quái
			EnemyHealthPS enemyHealth = enemy.GetComponent<EnemyHealthPS>();

			if (enemyHealth != null)
			{
				//Gây 1 sát thương
				enemyHealth.TakeDamage(1);
				Debug.Log(enemy.name + " bị trừ máu!");
			}
		}

		//Đợi animation hoàn thành (0.85s)
		yield return new WaitForSeconds(0.85f);
		animator.ResetTrigger("isAttacking");

		//Âm thanh Player tấn công
		audioSource.PlayOneShot(attackSound);

		isAttacking = false;
	}

	private void UpdateAnimation()
	{
		bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
		bool isJumping = !isGrounded;
		animator.SetBool("isRunning", isRunning);
		animator.SetBool("isJumping", isJumping);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("Va chạm với: " + collision.gameObject.name);

		//Check va chạm với dơi (Bat)
		if (collision.CompareTag("Bat") && !isInvulnerable)
		{
			BatMovementPS bat = collision.GetComponent<BatMovementPS>();
			if (bat != null)
			{
				//Trừ 1 máu
				TakeDamage(1);

				//Bị đẩy ra xa
				Knockback(collision.transform);
			}
		}

		//Khi nhân vật chạm vào Checkpoint ở mỗi màn, nhân vật sẽ được lưu tại màn đó
		//if (collision.CompareTag("Checkpoint"))
		//{
		//	respawnPosition = collision.transform.position;
		//	Debug.Log("Checkpoint mới được lưu!");
		//}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		/// Đây là đoạn code check Layer: collision.gameObject.layer == LayerMask.NameToLayer("PoisonFire"))
		//Check va chạm với khói độc (Fire)
		if (collision.CompareTag("Fire") && !isInvulnerable)
		{
			TakeDamage();
		}

		//Check nếu va chạm với Slimer và đang tấn công
		if (collision.CompareTag("Slimer") && !isInvulnerable)
		{
			SlimerAI slimer = collision.GetComponent<SlimerAI>();
			if (slimer != null && slimer.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				TakeDamage(slimer.attackDamage);
			}
		}
	}

	#region Bị khói độc (Fire) gây sát thương
	private void TakeDamage()
	{
		//Chỉ mất máu nếu không trong trạng thái bất tử
		if (!isInvulnerable)
		{
			//Trừ 1 máu
			health -= 1;

			//Cập nhật thanh máu
			healthBar.TakeDamage(1);
			Debug.Log("Dính độc!! Máu còn lại: " + health);

			//Âm thanh bị thương
			audioSource.PlayOneShot(hurtSound);

			//Kích hoạt bất tử tạm thời
			StartCoroutine(Invulnerability());

			//Máu = 0 => Die
			if (health <= 0)
			{
				Die();
			}
		}
	}
	#endregion

	#region Bị Slimer tấn công
	public void TakeDamage(int damage)
	{
		//Chỉ mất máu nếu không trong trạng thái bất tử
		if (!isInvulnerable)
		{
			health -= damage;

			//Cập nhật thanh máu
			healthBar.TakeDamage(damage);
			Debug.Log("Bị Slimer tấn công! Máu còn lại: " + health);

			//Âm thanh bị thương
			audioSource.PlayOneShot(hurtSound);

			//Kích hoạt bất tử tạm thời để tránh mất máu liên tục
			StartCoroutine(Invulnerability());

			//Máu = 0 => Die
			if (health <= 0)
			{
				Die();
			}
		}
	}
	#endregion

	private IEnumerator Invulnerability()
	{
		isInvulnerable = true;

		//Nhấp nháy nhân vật trong 1 giây
		for (int i = 0; i < 5; i++)
		{
			GetComponent<SpriteRenderer>().enabled = false;
			yield return new WaitForSeconds(0.1f);
			GetComponent<SpriteRenderer>().enabled = true;
			yield return new WaitForSeconds(0.1f);
		}

		isInvulnerable = false;
	}

	#region Hồi 1 máu khi ăn bình máu
	public void Heal(int amount)
	{
		//Chỉ hồi máu nếu chưa đầy
		if (health < 5)
		{
			health += amount;

			//Máu tối đa là 5
			if (health > 5)
			{
				health = 5;
			}

			//Cập nhật thanh máu
			healthBar.TakeDamage(-amount);

			Debug.Log("Hồi máu! Máu hiện tại: " + health);
		}
	}
	#endregion

	private void Die()
	{
		//Tránh gọi Die() nhiều lần
		if (isGameOver) return;
		isGameOver = true;

		//Hiển thị UI Game Over
		if (gameOverUI != null)
		{
			gameOverUI.SetActive(true);

			//Gọi hiệu ứng Fade In cho "Game Over"
			GameObject gameOverText = gameOverUI.transform.Find("GameOverText").gameObject;
			if (gameOverText != null)
			{
				gameOverText.GetComponent<Animator>().SetTrigger("FadeIn");
			}
		}

		//Âm thanh Player die
		audioSource.PlayOneShot(deathSound);

		//Vô hiệu hóa di chuyển nhân vật
		this.enabled = false;
		rb.linearVelocity = Vector2.zero;
	}

	#region Nhấn nút "Respawn" để hồi sinh
	public void RespawnButtonClick()
	{
		if (!isGameOver) return;

		//Ẩn UI Game Over
		if (gameOverUI != null)
		{
			gameOverUI.SetActive(false);
		}

		//Hồi sinh nhân vật
		Respawn();

		//Cho phép di chuyển lại
		this.enabled = true;
		isGameOver = false;
	}
	#endregion

	#region Hồi sinh tại vị trí CheckPoint, dc hồi 60% máu
	public void Respawn()
	{
		//Đưa nhân vật về checkpoint
		transform.position = respawnPosition;

		//Hồi sinh với 60% máu
		health = Mathf.RoundToInt(5 * 0.6f);

		if (healthBar != null)
		{
			healthBar.SetHealth(health); //Cập nhật UI thanh máu
		}

		Debug.Log("Nhân vật hồi sinh với " + health + " máu tại vị trí: " + respawnPosition);
	}
	#endregion

	#region Nhân vật bị đẩy ra khi chạm vào quái
	private void Knockback(Transform batTransform)
	{
		//Tăng lực đẩy ngang
		float knockbackForce = 3000f;

		//Tăng lực đẩy lên trên
		float verticalBoost = 500f;

		//Xác định hướng đẩy
		float direction = (transform.position.x - batTransform.position.x) >= 0 ? 1 : -1;

		//Reset vận tốc trước khi đẩy để tránh lỗi bị kẹt
		rb.linearVelocity = Vector2.zero;

		//Áp dụng lực đẩy
		rb.AddForce(new Vector2(knockbackForce * direction, verticalBoost));

		Debug.Log("Nhân vật bị đẩy lùi!");
	}
	#endregion

}