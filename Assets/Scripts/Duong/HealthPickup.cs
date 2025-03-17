using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	public int healAmount = 1; //Lượng máu hồi phục

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//Kiểm tra xem nhân vật có nhặt không
		if (collision.CompareTag("Player")) 
		{
			PlayerController player = collision.GetComponent<PlayerController>();

			if (player != null)
			{
				//Hồi máu cho nhân vật
				player.Heal(healAmount);

				//Xóa vật phẩm sau khi nhặt
				Destroy(gameObject);
			}
		}
	}
}
