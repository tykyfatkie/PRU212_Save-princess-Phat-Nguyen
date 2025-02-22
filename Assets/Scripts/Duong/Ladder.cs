using UnityEngine;

public class Ladder : MonoBehaviour
{
	private bool isClimbing = false;
	private Rigidbody2D rb;
	public float climbSpeed = 1f;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			isClimbing = true;
			rb = other.GetComponent<Rigidbody2D>();
			rb.gravityScale = 0;  //Tắt trọng lực khi leo
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			isClimbing = false;
			rb.gravityScale = 1;  //Bật lại trọng lực khi rời khỏi thang
		}
	}

	private void Update()
	{
		if (isClimbing && rb != null)
		{
			float vertical = Input.GetAxis("Vertical");  //Dùng phím W/S hoặc ↑/↓
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * climbSpeed);
		}
	}

}
