using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	public int health = 3;

	public void TakeDamage(int damage)
	{
		health -= damage;
		Debug.Log(gameObject.name + " bị đánh! Máu còn: " + health);

		if (health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		//Xóa quái khi chết
		Debug.Log(gameObject.name + " đã chết!");
		Destroy(gameObject);
	}
}
