using UnityEngine;

public class JumpingMushroom : MonoBehaviour
{
	public float jumpForceLow = 5f;  //Lực nhảy thấp
	public float jumpForceHigh = 10f; //Lực nhảy cao

	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("Đã chạm vào Mushroom: " + collision.name);

		if (collision.CompareTag("Player")) //Kiểm tra nếu đối tượng chạm là Player
		{
			Debug.Log("Nhân vật chạm vào nấm!");

			Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
			if (playerRb != null)
			{
				//Kiểm tra input để xác định lực nhảy
				float jumpForce = Input.GetKey(KeyCode.Space) ? jumpForceHigh : jumpForceLow;

				//Reset vận tốc trước khi nhảy để đảm bảo nhảy đúng lực
				playerRb.linearVelocity = Vector2.zero;

				//Áp lực nhảy lên Player
				playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpForce);

				Debug.Log("Nhảy lên với lực: " + jumpForce);
			}
		}
	}
}
