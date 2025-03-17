using UnityEngine;

public class BatMovement : MonoBehaviour
{
	public float speed = 2f; //Tốc độ bay
	public float moveDistance = 3f; //Khoảng cách bay từ điểm gốc
	private Vector3 startPosition;
	private int direction = 1; //Hướng di chuyển: 1 là phải, -1 là trái

	void Start()
	{
		//Lưu vị trí ban đầu
		startPosition = transform.position; 
	}

	void Update()
	{
		//Di chuyển qua lại theo trục X
		transform.position += Vector3.right * direction * speed * Time.deltaTime;

		//Nếu vượt quá khoảng cách cho phép thì đổi hướng
		if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
		{
			direction *= -1; //Đảo hướng di chuyển
			Flip();
		}
	}

	//Hàm quay mặt dơi khi đổi hướng
	private void Flip()
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}
}
