using UnityEngine;

public class AudioManagerAct6 : MonoBehaviour
{
    [SerializeField] AudioSource backgroundAudioSource;  
    [SerializeField] AudioSource FXAudioSource;
    [SerializeField] AudioClip entryClip;
    [SerializeField] AudioClip backgroundClip;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip keyClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip deathScreenClip;
    [SerializeField] AudioClip dashClip;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip laserClip;

    void Start()
    {
        PlayEntryMusic();
        PlayBackGroundMusic();
    }

    
    void Update()
    {
        
    }

    public void PlayEntryMusic()
    {
        FXAudioSource.PlayOneShot(entryClip);
    }
    public void PlayBackGroundMusic()
    {
        backgroundAudioSource.clip = backgroundClip;
        backgroundAudioSource.Play();
        backgroundAudioSource.loop = true;
    }

    public void PlayJumpSound()
    {
        FXAudioSource.PlayOneShot(jumpClip);
    }

    public void PlayAttackSound()
    {
        FXAudioSource.PlayOneShot(attackClip); ;
    }

    public void PlayDashSound()
    {
        FXAudioSource.PlayOneShot(dashClip);
    }

    public void DeathSound()
    {
        FXAudioSource.PlayOneShot(deathClip);
    }

    public void DeathScreenSound()
    {
        FXAudioSource.PlayOneShot(deathScreenClip);    ;
    }

    public void HitSound()
    {
        FXAudioSource.PlayOneShot(hitClip);
    }

    public void LaserSound()
    {
        FXAudioSource.PlayOneShot(laserClip);
    }
}
