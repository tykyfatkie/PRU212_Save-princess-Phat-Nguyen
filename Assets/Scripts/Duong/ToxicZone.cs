using System.Collections;
using UnityEngine;

public class ToxicZone : MonoBehaviour
{
	public GameObject gameOverUI;
	private bool isGameOver = false;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !isGameOver)
		{
			StartCoroutine(DeathSequence(collision.gameObject));
		}
	}

	IEnumerator DeathSequence(GameObject player)
	{
		isGameOver = true;
		PlayerController playerController = player.GetComponent<PlayerController>();
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

		//Vô hiệu hóa điều khiển nhân vật
		if (playerController != null)
		{
			playerController.enabled = false;
		}

		//Tắt trọng lực để không rơi xuyên qua
		if (rb != null)
		{
			rb.gravityScale = 0;
			rb.linearVelocity = Vector2.zero; //Dừng di chuyển
		}

		//Hiển thị Game Over UI
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

		//Chờ cho đến khi người chơi bấm Enter
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

		//--------------------------------
		//Ẩn Game Over UI
		gameOverUI.SetActive(false);

		// Hồi sinh nhân vật với 60% máu
		if (playerController != null)
		{
			playerController.Respawn();

			//Kích hoạt lại điều khiển
			playerController.enabled = true;
		}

		//Khôi phục trọng lực
		if (rb != null)
		{
			rb.gravityScale = 3.5f;
		}

		isGameOver = false;
	}

}