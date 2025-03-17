using System.Collections;
using UnityEngine;

public class Stalactite : MonoBehaviour
{
	private Vector3 startPosition;
	private Rigidbody2D rb;
	private AudioSource audioSource;

	//Thời gian trước khi thạch nhũ rơi
	public float fallDelay = 2.0f;

	//Thời gian chờ trước khi quay lại vị trí cũ
	public float resetDelay = 1.0f;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource>();

		startPosition = transform.position;

		//Giữ yên trước khi rơi
		rb.bodyType = RigidbodyType2D.Kinematic;

		//Tự động rơi mà không cần nhân vật
		StartCoroutine(AutoFallLoop());
	}

	IEnumerator AutoFallLoop()
	{
		while (true)
		{
			//Đợi trước khi rơi
			yield return new WaitForSeconds(fallDelay);

			//Phát âm thanh rơi
			if (audioSource != null)
			{
				audioSource.Play();
			}

			rb.bodyType = RigidbodyType2D.Dynamic;
			rb.gravityScale = 2.5f;

			//Chờ cho đến khi chạm đất
			yield return new WaitUntil(() => rb.linearVelocity.y == 0);

			//Chờ trước khi reset
			yield return new WaitForSeconds(resetDelay);

			rb.bodyType = RigidbodyType2D.Kinematic;
			rb.linearVelocity = Vector2.zero;

			//Reset vị trí
			transform.position = startPosition;
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			// Lấy PlayerController của nhân vật
			PlayerController player = collision.GetComponent<PlayerController>();
			if (player != null && !player.isInvulnerable)
			{
				//Nhân vật bị mất 1 máu
				player.TakeDamage(1);

				//Gây Knockback cho nhân vật
				Knockback(player);

				Debug.Log("Nhân vật bị thạch nhũ rơi trúng!");
			}
		}
	}

	private void Knockback(PlayerController player)
	{
		float knockbackForce = 3000f; //Điều chỉnh lực đẩy ngang
		float verticalBoost = 300f; //Lực đẩy lên

		//Xác định hướng đẩy: nếu Player bên trái thì đẩy sang trái, ngược lại thì đẩy sang phải
		float direction = (player.transform.position.x - transform.position.x) >= 0 ? 1 : -1;

		//Reset vận tốc trước khi đẩy
		player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

		//Áp dụng lực đẩy lên Player
		player.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockbackForce * direction, verticalBoost));

		Debug.Log("Nhân vật bị đẩy lùi do thạch nhũ!");
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		//Khi chạm đất
		if (collision.gameObject.CompareTag("Ground")) 
		{
			//Chặn mọi lực tác động
			rb.bodyType = RigidbodyType2D.Kinematic;

			//Đảm bảo đứng yên
			rb.linearVelocity = Vector2.zero; 

			StartCoroutine(ResetPosition());
		}
	}

	IEnumerator ResetPosition()
	{
		yield return new WaitForSeconds(resetDelay);

		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.linearVelocity = Vector2.zero;

		//Reset vị trí
		transform.position = startPosition;
	}
}
