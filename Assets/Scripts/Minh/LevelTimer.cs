using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    private float startTime;
    private bool isRunning = true;
    [SerializeField] private TMP_Text levelTimer;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (!isRunning) return;

        float elapsedTime = Time.time - startTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        levelTimer.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
