using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

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
                DontDestroyOnLoad(this.gameObject);
        }
    }
}
