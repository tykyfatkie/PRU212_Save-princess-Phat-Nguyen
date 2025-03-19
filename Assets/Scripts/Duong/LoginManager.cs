using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
	public TMP_InputField usernameInput;
	public TMP_InputField passwordInput;
	public TextMeshProUGUI messageText;

	public void OnLoginButton()
	{
		string username = usernameInput.text;
		string password = passwordInput.text;

		//Check đăng nhập đơn giản
		if (username == "user" && password == "123")
		{
			messageText.color = Color.green;
			messageText.text = "Login Successful!";

			//Chuyển sang Scene chính
			SceneManager.LoadScene("Poison-Swamp");
		}
		else
		{
			messageText.color = Color.red;
			messageText.text = "Invalid Username or Password!";
		}
	}
}
