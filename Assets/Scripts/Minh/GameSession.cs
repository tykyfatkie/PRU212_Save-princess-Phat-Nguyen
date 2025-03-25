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



    void Awake()
    {
        gameOverUi.SetActive(false);
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        playerLives = FindObjectOfType<HealthBar>();
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
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
