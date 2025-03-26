using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public GameObject settingsPanel;
	public Sprite soundOnIcon;
	public Sprite soundOffIcon;
	public Sprite musicOnIcon;
	public Sprite musicOffIcon;

	public Image soundImage;
	public Image musicImage;

	private bool isSoundOn = true;
	private bool isMusicOn = true;

	public void ToggleSettingsPanel()
	{
		settingsPanel.SetActive(!settingsPanel.activeSelf);
	}

	public void ToggleSound()
	{
		isSoundOn = !isSoundOn;
		soundImage.sprite = isSoundOn ? soundOnIcon : soundOffIcon;
		//TODO: Tắt/mở âm thanh thật nếu bạn có AudioManager
	}

	public void ToggleMusic()
	{
		isMusicOn = !isMusicOn;
		musicImage.sprite = isMusicOn ? musicOnIcon : musicOffIcon;
		//TODO: Tắt/mở nhạc nền
	}

	public void CloseSettings()
	{
		settingsPanel.SetActive(false);
	}
}
