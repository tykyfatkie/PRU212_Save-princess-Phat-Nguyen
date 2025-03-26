using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	public AudioSource musicSource;
	public bool isSoundOn = true;
	public bool isMusicOn = true;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;

			//Giữ nguyên khi chuyển scene
			DontDestroyOnLoad(gameObject); 
		}
		else
		{
			//Nếu đã có 1 instance rồi thì xóa cái mới
			Destroy(gameObject); 
		}
	}

	public void ToggleSound()
	{
		isSoundOn = !isSoundOn;

		//Cập nhật toàn bộ SFX
		SFXAudio[] sfxList = FindObjectsOfType<SFXAudio>();
		foreach (SFXAudio sfx in sfxList)
		{
			sfx.UpdateMute(); // gọi hàm bên dưới
		}
	}

	public void ToggleMusic()
	{
		isMusicOn = !isMusicOn;
		if (musicSource != null)
		{
			musicSource.mute = !isMusicOn;
		}
	}
}
