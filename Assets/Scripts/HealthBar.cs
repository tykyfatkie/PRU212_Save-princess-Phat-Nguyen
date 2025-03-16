using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject heartPrefab;  //Prefab trái tim
    private List<GameObject> hearts = new List<GameObject>(); //Danh sách trái tim

    void Start()
    {
        currentHealth = maxHealth;
        CreateHearts();
    }

    void CreateHearts()
    {
        //Khoảng cách giữa các trái tim
        float spacing = 100f;

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
            Image heartImage = hearts[i].GetComponent<Image>(); //Lấy UI Image thay vì SpriteRenderer

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

}