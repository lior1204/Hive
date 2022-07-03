using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource _audioSource;
    [SerializeField] private AudioSource _SFXSource;
    [SerializeField] private AudioMixer mixer;
    [Header("Clips")]
    [SerializeField] private AudioClip backgroundLoop;
    [SerializeField] private AudioClip playerConnectClip;
    [SerializeField] private AudioClip playerCaptureClip;
    [SerializeField] private AudioClip enemyConnectClip;
    [SerializeField] private AudioClip enemyCaptureClip;
    [SerializeField] private AudioClip clickPlanetClip;
    [SerializeField] private AudioClip pressButtonClip;
    [Header("Parameters")]
    [SerializeField] private Vector2 scaryClipRandomLength = new Vector2(1f, 5f);
    [SerializeField] private Vector2 scaryClipRandomIntervals = new Vector2(10f, 20f);
    [SerializeField] [Range(0.0001f, 0.9999f)] private float scaryClipMaxVolume = 0.9999f;
    [SerializeField] private float scaryClipStartTime = 1.5f;
    [SerializeField] private float scaryClipIncrementIntervals = 0.01f;
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
                StartCoroutine(PlayClipAtRandom());
        }
        }
    }
    private void SetMusic()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.Play();
        transform.GetChild(0).GetComponent<AudioSource>().Play();
    }
    public void OnPlayerConnect()
    {
        if (playerConnectClip)
        {
            _SFXSource.PlayOneShot(playerConnectClip);
        }
    }
    public void OnPlayerCapture()
    {
        if (playerCaptureClip)
        {
            _SFXSource.PlayOneShot(playerCaptureClip);
        }
    }
    public void OnEnemyConnect()
    {
        if (enemyConnectClip)
        {
            _SFXSource.PlayOneShot(enemyConnectClip);
        }
    }
    public void OnEnemyCapture()
    {
        if (enemyCaptureClip)
        {
            _SFXSource.PlayOneShot(enemyCaptureClip);
        }
    }
    public void OnClickPlanet()
    {
        if (clickPlanetClip)
        {
            _SFXSource.PlayOneShot(clickPlanetClip);
        }
    }
     public void OnPressButton()
    {
        if (pressButtonClip)
        {
            _SFXSource.PlayOneShot(pressButtonClip);
        }
    }

    IEnumerator SwitchBackgroundMusic()
    {
        yield return new WaitWhile(() => _audioSource.isPlaying);
        _audioSource.clip = backgroundLoop;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    IEnumerator PlayClipAtRandom()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(scaryClipRandomIntervals.x, scaryClipRandomIntervals.y));
            float steps = scaryClipStartTime / scaryClipIncrementIntervals;
            float volumeIncrement = scaryClipMaxVolume / steps;
            float volume = 0.0001f;
            for ( int i = 0; i < steps; i++)
            {
                volume+= volumeIncrement;
                mixer.SetFloat(ParamManager.Instance.ScarySoundName, Mathf.Log10(volume) * 20);
                yield return new WaitForSeconds(scaryClipIncrementIntervals);
            }
            yield return new WaitForSeconds(Random.Range(scaryClipRandomLength.x, scaryClipRandomLength.y));
            for (int i = 1; i < steps; i++)
            {
                volume -= volumeIncrement;
                mixer.SetFloat(ParamManager.Instance.ScarySoundName, Mathf.Log10(volume) * 20);
                yield return new WaitForSeconds(scaryClipIncrementIntervals);
            }
        }
    }
    public void OnChangeMusicVolume(float value)
    {
        if (value <= 0)
            value = 0.0001f;
        mixer.SetFloat(ParamManager.Instance.GameVolumeName, Mathf.Log10(value) * 20);
    }
    public void OnChangeSFXVolume(float value)
    {
        if (value <= 0)
            value = 0.0001f;
        mixer.SetFloat(ParamManager.Instance.SfxSoundName, Mathf.Log10(value) * 20);
    }

    public float GetMusicVolume()
    {
        float value;
        mixer.GetFloat(ParamManager.Instance.GameVolumeName, out value);
        //return value;
        return Mathf.Pow(10.0f, value / 20.0f);
    }
    public float GetSFXVolume()
    {
        float value;
        mixer.GetFloat(ParamManager.Instance.SfxSoundName, out value);
        //return value;
        return Mathf.Pow(10.0f, value / 20.0f);
    }

}
