using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarPlayer : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth = 2;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject heartPrefab;
    private List<GameObject> hearts = new List<GameObject>();
    private GameManager gameManager;
    private PlayerCollision playerCollision;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        CreateHearts();
    }

    void CreateHearts()
    {
        float spacing = 55f;
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            RectTransform rt = heart.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * spacing, 0);
            hearts.Add(heart);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();

        if (currentHealth == 0)
        {
            gameManager?.GameOver();
        }
    }

    public void IncreaseHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++; 
            UpdateHearts(); 
        }
        else
        {
            Debug.Log("Máu đã đầy, không thể tăng thêm.");
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            Image heartImage = hearts[i].GetComponent<Image>();

            if (i < currentHealth)
            {
                heartImage.sprite = fullHeart;
            }
            else
            {
                heartImage.sprite = emptyHeart;
            }
        }
    }
}
