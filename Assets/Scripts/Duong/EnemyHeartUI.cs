using UnityEngine;
using UnityEngine.UI;

public class EnemyHeartUI : MonoBehaviour
{
	public EnemyHealth enemyHealth; //Tham chiếu đến quái vật
	public Transform player;
	public Image[] hearts; //Mảng chứa các trái tim
	public float detectRange = 3f;
	private Canvas canvas;
	private Transform enemy;

	private void Start()
	{
		canvas = GetComponent<Canvas>();

		//Mặc định ẩn thanh máu
		canvas.enabled = false;

		//Lấy quái vật cha (CanvasMonster nằm trong quái vật)
		enemy = transform.parent;
	}

	private void Update()
	{
		if (player == null || enemy == null) return;

		//Kiểm tra khoảng cách giữa quái vật và player
		float distance = Vector2.Distance(player.position, enemy.position);

		if (distance <= detectRange)
		{
			//Hiện thanh máu khi player đến gần
			canvas.enabled = true; 
			UpdateHearts();
		}
		else
		{
			//Ẩn thanh máu khi player đi xa
			canvas.enabled = false; 
		}
	}

	void UpdateHearts()
	{
		for (int i = 0; i < hearts.Length; i++)
		{
			hearts[i].enabled = (i < enemyHealth.health);
		}
	}
}
