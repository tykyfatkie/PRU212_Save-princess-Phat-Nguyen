using UnityEngine;

public class Player : MonoBehaviour
{
	// transform là 1 đối tượng thuộc lớp Transform
	// spriteRenderer là 1 đối tượng thuộc lớp SpriteRenderer
	public float moveSpeed = 5f;
	public Vector3 moveInput;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		moveInput.x = Input.GetAxis("Horizontal");
		moveInput.y = Input.GetAxis("Vertical");
		transform.position += moveInput * moveSpeed * Time.deltaTime * 2.5f;
		if (moveInput.x != 0)
		{
			if (moveInput.x > 0)
			{
				transform.localScale = new Vector3(6, 6, 0);
			}
			else
			{
				transform.localScale = new Vector3(-6, 6, 0);
			}
		}
	}
}
