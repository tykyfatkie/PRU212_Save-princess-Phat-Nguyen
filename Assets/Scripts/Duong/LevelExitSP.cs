using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitSP : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 0.2f;
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = FindAnyObjectByType<MainPlayer>();
        if (other.tag == "Player")
        {
            Debug.Log("Azavinha chiwawa");
            //FindObjectOfType<LevelTimer>()?.StopTimer();
            StartCoroutine(LoadNextLevel());
        }
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(levelLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScenceIndex = currentSceneIndex + 1;

        if (nextScenceIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextScenceIndex = 0;
        }
        SceneManager.LoadScene(nextScenceIndex);

        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            FindObjectOfType<LevelTimer>()?.StopTimer();
        }
    }
}

