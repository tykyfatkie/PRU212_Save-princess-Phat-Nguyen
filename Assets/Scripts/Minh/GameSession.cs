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
    public HealthBar playerLives;
    //[SerializeField] int score = 0;
    public bool isInvulnerable = false;



    void Awake()
    {
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

    private IEnumerator WaitAndRestart()
    {
        yield return new WaitForSeconds(0.5f);

        if (playerLives.currentHealth > 1)
        {
            playerLives.TakeDamage(1);
            if (!isInvulnerable){
                StartCoroutine(Invulnerability());
            }
            var player = FindAnyObjectByType<PlayerMovement>();
            player.isAlive = true;
        }
        else
        {
            SceneManager.LoadScene(1);
            //Destroy(gameObject);
            playerLives = FindObjectOfType<HealthBar>();
            playerLives.SetHealth(3);

        }
    }

    private IEnumerator Invulnerability()
    {
        
        SpriteRenderer player = FindObjectOfType<PlayerMovement>().GetComponent<SpriteRenderer>();
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            isInvulnerable = true;
            for (int i = 0; i < 5; i++)
            {
                player.enabled = false;
                yield return new WaitForSeconds(0.1f);
                player.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            isInvulnerable = false;
        yield return new WaitForSeconds(1f);
    }
}
