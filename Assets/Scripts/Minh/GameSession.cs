using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // private int playerLives = 5;
    [SerializeField] private GameObject gameOverUi;
    public HealthBar playerLives;
    //[SerializeField] int score = 0;
    public bool isInvulnerable = false;
    private static GameSession instance;



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        gameOverUi.SetActive(false);
        playerLives = FindObjectOfType<HealthBar>();

        // Lắng nghe sự kiện khi chuyển màn
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Đã load màn: " + scene.name);

        // Nếu vào Level 3, xóa GameSession
        if (scene.name == "Poison-Swamp")
        {
            Debug.Log("Xóa GameSession khi vào Level 3");
            Destroy(gameObject);
            instance = null;
        }
    }

    public static GameSession GetInstance()
    {
        return instance;
    }




    public void ProcessPlayerDeath()
    {

        StartCoroutine(WaitAndRestart());
    }

    public IEnumerator WaitAndRestart()
    {
        var player = FindAnyObjectByType<PlayerMovement>();
        if (player == null) yield break;

        if (playerLives.currentHealth > 1)
        {
            playerLives.TakeDamage(1);
            if (!isInvulnerable) StartCoroutine(Invulnerability());
            yield return new WaitForSeconds(1f);
            player.isAlive = true;
        }
        else
        {
            player.isAlive = false;
            gameOverUi.SetActive(true);
            Time.timeScale = 0;
            playerLives.SetHealth(0);
        }

    }

    public void PlayAgain()
    {
        gameOverUi.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("Level 1");
        //playerLives = FindObjectOfType<HealthBar>();
        playerLives.SetHealth(3);
    
    }


    private IEnumerator Invulnerability()
    {
        var player = FindObjectOfType<PlayerMovement>();
        if (player == null || !player.isAlive) yield break;

        var playerRenderer = player.GetComponent<SpriteRenderer>();
        if (playerRenderer == null) yield break;

        isInvulnerable = true;

        for (int i = 0; i < 5; i++)
        {
            playerRenderer.enabled = !playerRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        playerRenderer.enabled = true;
        isInvulnerable = false;
    }
}
