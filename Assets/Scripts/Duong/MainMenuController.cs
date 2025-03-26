using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public void PlayGame()
	{
		SceneManager.LoadScene("Level 1");
	}

	public void OpenSettings()
	{
		Debug.Log("Cài đặt được mở");
	}
}
