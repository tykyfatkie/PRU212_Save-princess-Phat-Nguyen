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

	private void Start()
	{
		UpdateIcons();
	}

	public void ToggleSettingsPanel()
	{
		settingsPanel.SetActive(!settingsPanel.activeSelf);
	}

	public void ToggleSound()
	{
		AudioManager.Instance.ToggleSound();
		UpdateIcons();
	}

	public void ToggleMusic()
	{
		AudioManager.Instance.ToggleMusic();
		UpdateIcons();
	}

	public void CloseSettings()
	{
		settingsPanel.SetActive(false);
	}

	private void UpdateIcons()
	{
		soundImage.sprite = AudioManager.Instance.isSoundOn ? soundOnIcon : soundOffIcon;
		musicImage.sprite = AudioManager.Instance.isMusicOn ? musicOnIcon : musicOffIcon;
	}
}
