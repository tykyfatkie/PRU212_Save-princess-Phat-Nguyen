using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPS : MonoBehaviour
{
	public int maxHealth = 5;
	public int currentHealth;
	public Sprite fullHeart;
	public Sprite emptyHeart;
	public GameObject heartPrefab;  //Prefab trái tym
	private List<GameObject> hearts = new List<GameObject>(); //Danh sách trái tim

	void Start()
	{
		//currentHealth = maxHealth;
		
		currentHealth = maxHealth;
        CreateHearts();

        //update lại UI máu
        var lastHealth = FindAnyObjectByType<GameSession>().playerLives.currentHealth;
        SetHealth(lastHealth);
		currentHealth = lastHealth;

    }

	void CreateHearts()
	{
		//Khoảng cách giữa các trái tym
		float spacing = 50f; 


		for (int i = 0; i < maxHealth; i++)
		{
			GameObject heart = Instantiate(heartPrefab, transform);
			RectTransform rt = heart.GetComponent<RectTransform>();

			//Sắp xếp hàng ngang
			rt.anchoredPosition = new Vector2(i * spacing, 0);
			hearts.Add(heart);
		}
	}

	public void TakeDamage(int damage)
	{
		currentHealth -= damage;
		if (currentHealth < 0) currentHealth = 0;
		UpdateHearts();
	}

	void UpdateHearts()
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			//Lấy UI Image thay vì SpriteRenderer
			Image heartImage = hearts[i].GetComponent<Image>();

			if (i < currentHealth)
			{
				heartImage.sprite = fullHeart; //Trái tim đầy
			}
			else
			{
				heartImage.sprite = emptyHeart; //Trái tim rỗng
			}
		}
	}

	public void SetHealth(int newHealth)
	{
		currentHealth = newHealth;

		//Đảm bảo không vượt quá giới hạn
		if (currentHealth > maxHealth) currentHealth = maxHealth; 

		UpdateHearts(); //Cập nhật UI
	}

}
