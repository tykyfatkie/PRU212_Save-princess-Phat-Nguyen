using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoutManager : MonoBehaviour
{
	public Button loginButton;
	public Button replayButton;
	public Button quitButton;
	public TextMeshProUGUI messageText;

	void Start()
	{
		messageText.text = "YOU WIN!";

		//loginButton.onClick.AddListener(GoToLogin);
		replayButton.onClick.AddListener(ReplayGame);
		quitButton.onClick.AddListener(QuitGame);
	}

	//Nút quay về trang Login
	//void GoToLogin()
	//{
	//	SceneManager.LoadScene("LoginScene");
	//}

	//Nút quay lại màn 1: lửa
	void ReplayGame()
	{
		SceneManager.LoadScene("Level 1");
	}

	//Nút thoát Game
	void QuitGame()
	{
		Application.Quit();
	}

}
