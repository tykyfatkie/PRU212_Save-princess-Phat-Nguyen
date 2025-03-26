using UnityEngine;

public class SFXAudio : MonoBehaviour
{
	private AudioSource audioSource;

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		UpdateMute();
	}

	public void UpdateMute()
	{
		if (AudioManager.Instance != null && audioSource != null)
		{
			audioSource.mute = !AudioManager.Instance.isSoundOn;
		}
	}
}
