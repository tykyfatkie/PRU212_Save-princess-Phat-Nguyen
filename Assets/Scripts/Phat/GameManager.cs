using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverUi;
    private bool isGameOver = false;
    private AudioManagerAct6 audioManager;
    private GameManager gameManager;
    [SerializeField] private GameObject[] healthLayouts;
    public static GameManager Instance;
    void Start()
    {
        gameOverUi.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverUi.SetActive(true);
        Time.timeScale = 0;
        if (audioManager != null)
        {
            audioManager.DeathScreenSound();
        }

    }
    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Boss");
    }
    public bool IsGameOver()
    {
        return isGameOver;
    }

}
