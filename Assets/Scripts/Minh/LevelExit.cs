using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 0.2f;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag== "Player")
        {
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
    }
}
