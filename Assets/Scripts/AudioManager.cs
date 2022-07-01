using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip backgroundLoop;
    [SerializeField] private AudioClip playerConnectClip;
    [SerializeField] private AudioClip playerCaptureClip;
    [SerializeField] private AudioClip enemyConnectClip;
    [SerializeField] private AudioClip enemyCaptureClip;
    [SerializeField] private AudioClip ScaySoundClip;
    private void Awake()
    {
        SetSingelton();
    }
    private void SetSingelton()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            if (Application.isPlaying)
                Destroy(this);
        }
        else
        {
            Instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
                SetMusic();
                StartCoroutine( SwitchBackgroundMusic());
        }
        }
    }
    private void SetMusic()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.Play();
    }
    public void OnPlayerConnect()
    {
        if(playerConnectClip)
        _audioSource.PlayOneShot(playerConnectClip);
    }
    public void OnPlayerCapture()
    {
        if(playerCaptureClip)
        _audioSource.PlayOneShot(playerCaptureClip);
    }
    public void OnEnemyConnect()
    {
        if(enemyConnectClip)
        _audioSource.PlayOneShot(enemyConnectClip);
    }
    public void OnEnemyCapture()
    {
        if(enemyCaptureClip)
        _audioSource.PlayOneShot(enemyCaptureClip);
    }

    IEnumerator SwitchBackgroundMusic()
    {
        yield return new WaitWhile(() => _audioSource.isPlaying);
        _audioSource.clip = backgroundLoop;
        _audioSource.loop = true;
        _audioSource.Play();
    }


}
